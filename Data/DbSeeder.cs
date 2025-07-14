using Bogus;
using ErpApp.Models;

namespace ErpApp.Data;

public static class DbSeeder
{
    public static async Task SeedAllAsync(ErpDbContext context)
    {
        if (context is not ErpDbContext db) return;
        if (db.Customers.Any()) return;

        Console.WriteLine("Seeding Customers...");
        var customerFaker = new Faker<Customer>("pl")
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .RuleFor(c => c.Address, f => f.Address.FullAddress());

        var customers = customerFaker.Generate(50000);
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync();
        Console.WriteLine("Customers seeded.");

        var products = GetProductList();
        db.Products.AddRange(products);
        await db.SaveChangesAsync();
        Console.WriteLine("Products seeded.");

        Console.WriteLine("Seeding Orders + OrderItems...");
        var rnd = new Random();
        var orders = new List<Order>();
        var orderItems = new List<OrderItem>();

        foreach (var customer in customers)
        {
            var orderCount = rnd.Next(2, 6);
            for (int i = 0; i < orderCount; i++)
            {
                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderDate = DateTime.Today.AddDays(-rnd.Next(0, 365))
                };

                var itemsInOrder = rnd.Next(1, 6);
                for (int j = 0; j < itemsInOrder; j++)
                {
                    var product = products[rnd.Next(products.Count)];
                    var quantity = rnd.Next(1, 5);
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
        Console.WriteLine("Orders + OrderItems seeded.");
    }
    private static List<Product> GetProductList()
    {
        return [
            new() { Name = "Cyberpunk 2077", Category = "PC", Price = 192.5m },
            new() { Name = "Baldur's Gate 3", Category = "PC", Price = 237.24m },
            new() { Name = "The Sims 4", Category = "PC", Price = 250.06m },
            new Product { Name = "Civilization VI", Category = "PC", Price = 274.42m },
            new Product { Name = "Counter-Strike 2", Category = "PC", Price = 125.4m },
            new Product { Name = "League of Legends", Category = "PC", Price = 239.13m },
            new Product { Name = "Valorant", Category = "PC", Price = 233.24m },
            new Product { Name = "World of Warcraft", Category = "PC", Price = 240.94m },
            new Product { Name = "Minecraft", Category = "PC", Price = 133.33m },
            new Product { Name = "Stardew Valley", Category = "PC", Price = 284.8m },
            new Product { Name = "God of War", Category = "PS4", Price = 162.45m },
            new Product { Name = "Uncharted 4", Category = "PS4", Price = 266.64m },
            new Product { Name = "Horizon Zero Dawn", Category = "PS4", Price = 93.58m },
            new Product { Name = "Bloodborne", Category = "PS4", Price = 220.44m },
            new Product { Name = "The Last of Us Remastered", Category = "PS4", Price = 243.41m },
            new Product { Name = "Spider-Man", Category = "PS4", Price = 296.17m },
            new Product { Name = "Days Gone", Category = "PS4", Price = 283.84m },
            new Product { Name = "Persona 5", Category = "PS4", Price = 251.48m },
            new Product { Name = "Gran Turismo Sport", Category = "PS4", Price = 98.85m },
            new Product { Name = "Detroit: Become Human", Category = "PS4", Price = 292.12m },
            new Product { Name = "Hogwarts Legacy", Category = "PS5", Price = 323.7m },
            new Product { Name = "Demon's Souls", Category = "PS5", Price = 173.51m },
            new Product { Name = "Returnal", Category = "PS5", Price = 334.99m },
            new Product { Name = "Ratchet & Clank: Rift Apart", Category = "PS5", Price = 213.09m },
            new Product { Name = "Spider-Man: Miles Morales", Category = "PS5", Price = 314.86m },
            new Product { Name = "Final Fantasy XVI", Category = "PS5", Price = 173.59m },
            new Product { Name = "Gran Turismo 7", Category = "PS5", Price = 191.94m },
            new Product { Name = "Resident Evil Village", Category = "PS5", Price = 209.01m },
            new Product { Name = "Forspoken", Category = "PS5", Price = 158.91m },
            new Product { Name = "Call of Duty: MW3", Category = "PS5", Price = 348.28m },
            new Product { Name = "Halo 5: Guardians", Category = "Xbox One", Price = 186.31m },
            new Product { Name = "Gears 5", Category = "Xbox One", Price = 268.46m },
            new Product { Name = "Forza Horizon 4", Category = "Xbox One", Price = 296.8m },
            new Product { Name = "Sea of Thieves", Category = "Xbox One", Price = 258.52m },
            new Product { Name = "Ori and the Blind Forest", Category = "Xbox One", Price = 333.18m },
            new Product { Name = "Sunset Overdrive", Category = "Xbox One", Price = 172.82m },
            new Product { Name = "Quantum Break", Category = "Xbox One", Price = 313.74m },
            new Product { Name = "ReCore", Category = "Xbox One", Price = 104.92m },
            new Product { Name = "Crackdown 3", Category = "Xbox One", Price = 142.18m },
            new Product { Name = "Ryse: Son of Rome", Category = "Xbox One", Price = 90.89m },
            new Product { Name = "Starfield", Category = "Xbox Series X", Price = 345.3m },
            new Product { Name = "Forza Horizon 5", Category = "Xbox Series X", Price = 183.19m },
            new Product { Name = "Halo Infinite", Category = "Xbox Series X", Price = 295.45m },
            new Product { Name = "Microsoft Flight Simulator", Category = "Xbox Series X", Price = 210.93m },
            new Product { Name = "The Medium", Category = "Xbox Series X", Price = 144.84m },
            new Product { Name = "Redfall", Category = "Xbox Series X", Price = 106.99m },
            new Product { Name = "Scorn", Category = "Xbox Series X", Price = 282.35m },
            new Product { Name = "Grounded", Category = "Xbox Series X", Price = 148.2m },
            new Product { Name = "High on Life", Category = "Xbox Series X", Price = 306.96m },
            new Product { Name = "Warhammer 40,000: Darktide", Category = "Xbox Series X", Price = 198.13m },
            new Product { Name = "Zelda: Breath of the Wild", Category = "Nintendo Switch", Price = 185.41m },
            new Product { Name = "Animal Crossing", Category = "Nintendo Switch", Price = 171.35m },
            new Product { Name = "Super Mario Odyssey", Category = "Nintendo Switch", Price = 155.95m },
            new Product { Name = "Metroid Dread", Category = "Nintendo Switch", Price = 292.37m },
            new Product { Name = "Splatoon 3", Category = "Nintendo Switch", Price = 179.44m },
            new Product { Name = "Mario Kart 8 Deluxe", Category = "Nintendo Switch", Price = 178.9m },
            new Product { Name = "Pokémon Scarlet", Category = "Nintendo Switch", Price = 184.35m },
            new Product { Name = "Luigi's Mansion 3", Category = "Nintendo Switch", Price = 343.18m },
            new Product { Name = "Fire Emblem Engage", Category = "Nintendo Switch", Price = 160.9m },
            new Product { Name = "Bayonetta 3", Category = "Nintendo Switch", Price = 195.61m },
        ];
    }
}