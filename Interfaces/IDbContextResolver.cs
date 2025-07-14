using ErpApp.Data;

namespace ErpApp.Interfaces;

public interface IDbContextResolver
{
    ErpDbContext GetContext();
}
