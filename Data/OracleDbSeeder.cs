using Bogus;
using ErpApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Data;

public static class OracleDbSeeder
{
    public static async Task SeedAsync(OracleDbContext db)
    {
        var customerCount = await db.Customers.CountAsync();
        if (customerCount > 0) return;

        Console.WriteLine("[Oracle] Seeding Customers...");
        var customerFaker = new Faker<Customer>("pl")
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.Address, f => f.Address.FullAddress());

        var customers = customerFaker.Generate(2000);
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();
        Console.WriteLine("[Oracle] Customers seeded.");

        Console.WriteLine("[Oracle] Seeding Products...");
        var products = GetProductList();
        db.Products.AddRange(products);
        await db.SaveChangesAsync();
        Console.WriteLine("[Oracle] Products seeded.");

        Console.WriteLine("[Oracle] Seeding Orders + OrderItems...");
        var rnd = new Random();
        var orders = new List<Order>();
        var orderItems = new List<OrderItem>();

        foreach (var customer in customers)
        {
            var orderCount = rnd.Next(1, 4);
            for (int i = 0; i < orderCount; i++)
            {
                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderDate = DateTime.Today.AddDays(-rnd.Next(0, 365))
                };

                var itemsInOrder = rnd.Next(1, 4);
                for (int j = 0; j < itemsInOrder; j++)
                {
                    var product = products[rnd.Next(products.Count)];
                    var quantity = rnd.Next(1, 4);
                    orderItems.Add(new OrderItem
                    {
                        Order = order,
                        ProductId = product.Id,
                        Quantity = quantity,
                        Price = product.Price
                    });
                }

                orders.Add(order);
            }
        }

        db.Orders.AddRange(orders);
        db.OrderItems.AddRange(orderItems);
        await db.SaveChangesAsync();
        Console.WriteLine("[Oracle] Orders + OrderItems seeded.");
    }

    private static List<Product> GetProductList()
    {
        return [
            new() { Name = "Cyberpunk 2077", Category = "PC", Price = 192.5m },
            new() { Name = "Baldur's Gate 3", Category = "PC", Price = 237.24m },
            new() { Name = "The Sims 4", Category = "PC", Price = 250.06m },
            new() { Name = "Civilization VI", Category = "PC", Price = 274.42m },
            new() { Name = "Counter-Strike 2", Category = "PC", Price = 125.4m },
            new() { Name = "God of War", Category = "PS4", Price = 162.45m },
            new() { Name = "Uncharted 4", Category = "PS4", Price = 266.64m },
            new() { Name = "Halo 5: Guardians", Category = "Xbox One", Price = 186.31m },
            new() { Name = "Starfield", Category = "Xbox Series X", Price = 345.3m },
            new() { Name = "Zelda: Breath of the Wild", Category = "Nintendo Switch", Price = 185.41m }
        ];
    }
}
