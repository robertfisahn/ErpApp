﻿namespace ErpApp.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
