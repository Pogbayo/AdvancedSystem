﻿namespace StockApi.Domain.Entities
{
    public class Product
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImgUrl { get; set; }
        public required string CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
