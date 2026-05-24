using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public string StockLevel { get; set; }
        public decimal StockPercentage { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Status { get; set; }
    }

    public class InventoryStats
    {
        public int TotalItems { get; set; }
        public int GoodStock { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
    }

    public class StockTrend
    {
        public string Month { get; set; }
        public int StockValue { get; set; }
    }

    public class StockAlert
    {
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<Inventory> LowStockItems { get; set; }
        public List<Inventory> OutOfStockItems { get; set; }
    }
}