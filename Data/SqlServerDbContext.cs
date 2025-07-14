using Microsoft.EntityFrameworkCore;

namespace ErpApp.Data;
public class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : ErpDbContext(options) {}
