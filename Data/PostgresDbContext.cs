using Microsoft.EntityFrameworkCore;

namespace ErpApp.Data;
public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : ErpDbContext(options) {}
