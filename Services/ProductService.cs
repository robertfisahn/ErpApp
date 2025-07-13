using ErpApp.Data;
using ErpApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Services;

public class ProductService(ErpDbContext context)
{
    private readonly ErpDbContext _context = context;

    public async Task<(List<Product> Products, string Sql, string Linq)> GetAllProductsWithSqlAsync()
    {
        var query = _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name);

        string sql = query.ToQueryString();
        string linq = "context.Products.AsNoTracking.OrderBy(p => p.Name)";

        var list = await query.ToListAsync();
        return (list, sql, linq);
    }


    public async Task<(List<Product> Products, string Sql, string Linq)> GetTop10ByDemandAsync()
    {
        var groupedQuery = _context.OrderItems
            .AsNoTracking()
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(10);

        var joinedQuery = groupedQuery
            .Join(
                _context.Products.AsNoTracking(),
                x => x.ProductId,
                p => p.Id,
                (x, p) => p
            );

        var sql = joinedQuery.ToQueryString();
        var linq = @"
            context.OrderItems
                .AsNoTracking()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .Join(context.Products.AsNoTracking(), x => x.ProductId, p => p.Id, (x, p) => p);
            ".Trim();
        var result = await joinedQuery.ToListAsync();
        return (result, sql, linq.Trim());
    }


    public async Task<(List<Product> Products, string Sql, string Linq)> GetTop10ByPriceAsync()
    {
        var query = _context.Products
            .AsNoTracking()
            .OrderByDescending(p => p.Price)
            .Take(10);

        var sql = query.ToQueryString();
        var linq = "context.Products.AsNoTracking().OrderByDescending(p => p.Price).Take(10);";

        var result = await query.ToListAsync();
        return (result, sql, linq);
    }

}
