using Microsoft.EntityFrameworkCore;

namespace ErpApp.Data;
public class OracleDbContext(DbContextOptions<OracleDbContext> options) : ErpDbContext(options) {}
