using Bogus;
using ErpApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Data;

public static class PostgresDbSeeder
{
    public static async Task SeedAsync(PostgresDbContext db)
    {
        var products = GetProductList();
        if (await db.Products.CountAsync() == products.Count)
        {
            return;
        }
        else if (await db.Products.AnyAsync())
        {
            Console.WriteLine("[Postgres] Product data mismatch. Clearing related data...");
            db.OrderItems.RemoveRange(db.OrderItems);
            db.Orders.RemoveRange(db.Orders);
            db.Products.RemoveRange(db.Products);
            db.Customers.RemoveRange(db.Customers);
            await db.SaveChangesAsync();
        }

        Console.WriteLine("[Postgres] Seeding Customers...");
        var customerFaker = new Faker<Customer>("pl")
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.Address, f => f.Address.FullAddress());

        var customers = customerFaker.Generate(3000);
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();
        Console.WriteLine("[Postgres] Customers seeded.");

        Console.WriteLine("[Postgres] Seeding Products...");
        db.Products.AddRange(products);
        await db.SaveChangesAsync();
        Console.WriteLine("[Postgres] Products seeded.");

        Console.WriteLine("[Postgres] Seeding Orders + OrderItems...");
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
                    OrderDate = DateTime.UtcNow.AddDays(-rnd.Next(0, 365))
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
        Console.WriteLine("[Postgres] Orders + OrderItems seeded.");
    }

    private static List<Product> GetProductList()
    {
        return
        [
        new() { Name = "Elden Ring", Category = "PC", Price = 289.99m },
        new() { Name = "Hades II", Category = "PC", Price = 139.50m },
        new() { Name = "The Witcher 3: Wild Hunt", Category = "PC", Price = 199.00m },
        new() { Name = "Spider-Man 2", Category = "PS5", Price = 319.99m },
        new() { Name = "Returnal", Category = "PS5", Price = 289.50m },
        new() { Name = "Gran Turismo 7", Category = "PS5", Price = 249.00m },
        new() { Name = "Red Dead Redemption 2", Category = "Xbox One", Price = 209.00m },
        new() { Name = "Forza Horizon 5", Category = "Xbox Series X", Price = 269.99m },
        new() { Name = "Hi-Fi Rush", Category = "Xbox Series X", Price = 149.90m },
        new() { Name = "Grounded", Category = "Xbox Series X", Price = 139.00m },
        new() { Name = "Fire Emblem Engage", Category = "Nintendo Switch", Price = 199.99m },
        new() { Name = "Bayonetta 3", Category = "Nintendo Switch", Price = 179.90m },
        new() { Name = "Luigi's Mansion 3", Category = "Nintendo Switch", Price = 159.00m },
        new() { Name = "Cyberpunk 2077", Category = "PC", Price = 179.99m },
        new() { Name = "Baldur's Gate 3", Category = "PC", Price = 239.99m },
        new() { Name = "God of War Ragnarök", Category = "PS5", Price = 299.99m },
        new() { Name = "Sea of Thieves", Category = "Xbox One", Price = 199.00m },
        new() { Name = "Ori and the Will of the Wisps", Category = "Xbox One", Price = 119.99m },
        new() { Name = "Mario Kart 8 Deluxe", Category = "Nintendo Switch", Price = 189.00m },
        new() { Name = "Pikmin 4", Category = "Nintendo Switch", Price = 179.50m },
        new() { Name = "Tibia", Category = "PC", Price = 30.50m },
        ];
    }
}
