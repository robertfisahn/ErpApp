using ErpApp.Data;
using ErpApp.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ErpApp.Services;

public class DbContextResolver(IServiceProvider sp, DbContextSelectorService selector) : IDbContextResolver
{
    private readonly IServiceProvider _sp = sp;
    private readonly DbContextSelectorService _selector = selector;

    public ErpDbContext GetContext() => _selector.Current switch
    {
        "mssql" => _sp.GetRequiredService<SqlServerDbContext>(),
        "oracle" => _sp.GetRequiredService<OracleDbContext>(),
        "postgres" => _sp.GetRequiredService<PostgresDbContext>(),
        _ => throw new InvalidOperationException("Unsupported DB context.")
    };
}
