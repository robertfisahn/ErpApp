using ErpApp.Data;
using ErpApp.DTO;
using ErpApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Services
{
    public class ReportService
    {
        private readonly ErpDbContext _context;
        public ReportService(IDbContextResolver resolver)
        {
            _context = resolver.GetContext();
        }
        public async Task<(List<ProductReportDto> Data, string Sql, string Linq)> GetSalesReportAsync(
            string? category,
            DateTime? from,
            DateTime? to,
            int minSold = 0)
        {
            var query = _context.OrderItems
                .AsNoTracking()
                .Where(oi => oi.Order != null);

            if (from.HasValue)
            {
                query = query.Where(oi => oi.Order.OrderDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(oi => oi.Order.OrderDate <= to.Value);
            }

            var grouped = query
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name, oi.Product.Category, oi.Product.Price })
                .Select(g => new ProductReportDto
                {
                    Name = g.Key.Name,
                    Category = g.Key.Category,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * g.Key.Price)
                });

            if (!string.IsNullOrWhiteSpace(category))
            {
                grouped = grouped.Where(p => p.Category == category);
            }

            if (minSold > 0)
            {
                grouped = grouped.Where(p => p.TotalSold >= minSold);
            }

            var linq = @"
                query = OrderItems
                    .Where(oi => oi.Order.OrderDate between ... )
                    .GroupBy(ProductId, Name, Category, Price)
                    .Select(ProductReportDto with TotalSold, Revenue)
                    .Where(optional conditions...)
                ".Trim();

            var sql = grouped.ToQueryString();
            var result = await grouped.ToListAsync();

            return (result, sql, linq);
        }


        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

    }
}
