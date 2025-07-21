using ErpApp.Data;
using ErpApp.Interfaces;
using ErpApp.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerDbConnection")));
builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDbConnection")));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<DbContextSelectorService>();
builder.Services.AddScoped<IDbContextResolver, DbContextResolver>();
builder.Services.AddMudServices();

builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddSingleton<ReportPdfGenerator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var sqlServerDbContext = scope.ServiceProvider.GetRequiredService<SqlServerDbContext>();
    await DbSeeder.SeedAllAsync(sqlServerDbContext);

    var oracleDbContext = scope.ServiceProvider.GetRequiredService<OracleDbContext>();
    await OracleDbSeeder.SeedAsync(oracleDbContext);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
