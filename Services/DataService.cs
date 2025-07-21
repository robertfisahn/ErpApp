using ErpApp.Interfaces;
using ErpApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Services;
public class DataService(IDbContextResolver resolver)
{
    private readonly IDbContextResolver _resolver = resolver;

    public async Task<(List<Product> Products, string Sql, string Linq)> GetAllProductsWithSqlAsync()
    {
        var context = _resolver.GetContext();
        var query = context.Products.AsNoTracking().OrderBy(p => p.Name);
        string sql = query.ToQueryString();
        string linq = "context.Products.AsNoTracking.OrderBy(p => p.Name)";
        var list = await query.ToListAsync();
        return (list, sql, linq);
    }

    public async Task<(List<Product> Products, string Sql, string Linq)> GetTop10ByDemandAsync()
    {
        var context = _resolver.GetContext();
        var groupedQuery = context.OrderItems
            .AsNoTracking()
            .GroupBy(oi => oi.ProductId)
            .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(oi => oi.Quantity) })
            .OrderByDescending(x => x.TotalSold)
            .Take(10);

        var joinedQuery = groupedQuery
            .Join(
                context.Products.AsNoTracking(),
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
                .Join(
                    context.Products.AsNoTracking(),
                    x => x.ProductId,
                    p => p.Id,
                    (x, p) => p
                );
            ".Trim();
        var result = await joinedQuery.ToListAsync();
        return (result, sql, linq);
    }

    public async Task<(List<Product> Products, string Sql, string Linq)> GetTop10ByPriceAsync()
    {
        var context = _resolver.GetContext();
        var query = context.Products.AsNoTracking().OrderByDescending(p => p.Price).Take(10);
        var sql = query.ToQueryString();
        var linq = "context.Products.AsNoTracking().OrderByDescending(p => p.Price).Take(10);";
        var result = await query.ToListAsync();
        return (result, sql, linq);
    }
}
