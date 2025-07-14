using ErpApp.Data;
using ErpApp.Interfaces;

namespace ErpApp.Services;

public class DbContextResolver(SqlServerDbContext sql, OracleDbContext oracle, DbContextSelectorService selector) : IDbContextResolver
{
    private readonly SqlServerDbContext _sql = sql;
    private readonly OracleDbContext _oracle = oracle;
    private readonly DbContextSelectorService _selector = selector;

    public ErpDbContext GetContext() => _selector.Current switch
    {
        "sql" => _sql,
        "oracle" => _oracle,
        _ => throw new InvalidOperationException("Unsupported DB context.")
    };
}
