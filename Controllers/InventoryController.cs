using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class InventoryController : Controller
    {
        // Mock data - In production, this would come from database
        private static List<Product> _products = new List<Product>
        {
            new Product {
                ProductId = "#0001",
                ProductName = "Coca Cola 500ml",
                Category = "Beverages",
                Barcode = "8901234567890",
                Price = 2.99m,
                Quantity = 150,
                Status = "Good",
                MinimumStock = 50,
                MaximumStock = 300,
                LastUpdated = new DateTime(2026, 5, 17, 10, 30, 0)
            },
            new Product {
                ProductId = "#0002",
                ProductName = "Lays Chips Classic",
                Category = "Snacks",
                Barcode = "8901234567891",
                Price = 1.99m,
                Quantity = 200,
                Status = "Good",
                MinimumStock = 100,
                MaximumStock = 400,
                LastUpdated = new DateTime(2026, 5, 17, 9, 15, 0)
            },
            new Product {
                ProductId = "#0003",
                ProductName = "Fresh Milk 1L",
                Category = "Dairy",
                Barcode = "8901234567892",
                Price = 3.49m,
                Quantity = 15,
                Status = "Low Stock",
                MinimumStock = 30,
                MaximumStock = 100,
                LastUpdated = new DateTime(2026, 5, 17, 11, 0, 0)
            },
            new Product {
                ProductId = "#0004",
                ProductName = "Whole Wheat Bread",
                Category = "Bakery",
                Barcode = "8901234567893",
                Price = 2.49m,
                Quantity = 0,
                Status = "Out of Stock",
                MinimumStock = 20,
                MaximumStock = 80,
                LastUpdated = new DateTime(2026, 5, 16, 18, 30, 0)
            },
            new Product {
                ProductId = "#0005",
                ProductName = "Organic Eggs (12)",
                Category = "Dairy",
                Barcode = "8901234567894",
                Price = 4.99m,
                Quantity = 80,
                Status = "Good",
                MinimumStock = 40,
                MaximumStock = 200,
                LastUpdated = new DateTime(2026, 5, 17, 8, 0, 0)
            },
            new Product {
                ProductId = "#0006",
                ProductName = "Orange Juice",
                Category = "Beverages",
                Barcode = "8901234567895",
                Price = 4.49m,
                Quantity = 45,
                Status = "Good",
                MinimumStock = 30,
                MaximumStock = 150,
                LastUpdated = new DateTime(2026, 5, 16, 14, 20, 0)
            },
            new Product {
                ProductId = "#0007",
                ProductName = "Potato Chips",
                Category = "Snacks",
                Barcode = "8901234567896",
                Price = 2.49m,
                Quantity = 120,
                Status = "Good",
                MinimumStock = 50,
                MaximumStock = 250,
                LastUpdated = new DateTime(2026, 5, 17, 12, 45, 0)
            }
        };

        // GET: Inventory Dashboard
        public IActionResult Index()
        {
            ViewBag.ActivePage = "Inventory";
            return View();
        }

        // GET: Inventory Management View
        public IActionResult InventoryManagement()
        {
            var inventoryItems = _products.Select(p => new Inventory
            {
                Id = int.Parse(p.ProductId.Substring(1)),
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Category = p.Category,
                CurrentStock = p.Quantity,
                MinimumStock = p.MinimumStock,
                MaximumStock = p.MaximumStock,
                StockPercentage = (decimal)p.Quantity / p.MaximumStock * 100,
                StockLevel = GetStockLevel(p.Quantity, p.MinimumStock),
                LastUpdated = p.LastUpdated,
                Status = GetStatus(p.Quantity, p.MinimumStock)
            }).ToList();

            return View(inventoryItems);
        }

        // GET: Get Inventory Statistics
        [HttpGet]
        public IActionResult GetInventoryStats()
        {
            var stats = new InventoryStats
            {
                TotalItems = _products.Count,
                GoodStock = _products.Count(p => p.Quantity >= p.MinimumStock),
                LowStock = _products.Count(p => p.Quantity > 0 && p.Quantity < p.MinimumStock),
                OutOfStock = _products.Count(p => p.Quantity == 0),
                TotalInventoryValue = _products.Sum(p => p.Price * p.Quantity)
            };

            return Json(stats);
        }

        // GET: Get Stock Trends for Chart
        [HttpGet]
        public IActionResult GetStockTrends()
        {
            var trends = new List<StockTrend>
            {
                new StockTrend { Month = "Jan", StockValue = 5200 },
                new StockTrend { Month = "Feb", StockValue = 4800 },
                new StockTrend { Month = "Mar", StockValue = 6000 },
                new StockTrend { Month = "Apr", StockValue = 4500 },
                new StockTrend { Month = "May", StockValue = 3800 },
                new StockTrend { Month = "Jun", StockValue = 4200 }
            };

            return Json(trends);
        }

        // GET: Get Stock Alerts
        [HttpGet]
        public IActionResult GetStockAlerts()
        {
            var alerts = new StockAlert
            {
                LowStockCount = _products.Count(p => p.Quantity > 0 && p.Quantity < p.MinimumStock),
                OutOfStockCount = _products.Count(p => p.Quantity == 0),
                LowStockItems = _products.Where(p => p.Quantity > 0 && p.Quantity < p.MinimumStock)
                    .Select(p => new Inventory
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Category = p.Category,
                        CurrentStock = p.Quantity,
                        MinimumStock = p.MinimumStock,
                        Status = "Low Stock"
                    }).ToList(),
                OutOfStockItems = _products.Where(p => p.Quantity == 0)
                    .Select(p => new Inventory
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Category = p.Category,
                        CurrentStock = p.Quantity,
                        MinimumStock = p.MinimumStock,
                        Status = "Out of Stock"
                    }).ToList()
            };

            return Json(alerts);
        }

        // POST: Adjust Stock
        [HttpPost]
        public IActionResult AdjustStock(string productId, int newQuantity, string reason)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Quantity = newQuantity;
                product.LastUpdated = DateTime.Now;
                product.Status = GetStatus(newQuantity, product.MinimumStock);

                return Json(new
                {
                    success = true,
                    message = $"Stock updated successfully. New quantity: {newQuantity}",
                    newStatus = product.Status
                });
            }

            return Json(new { success = false, message = "Product not found" });
        }

        // POST: Bulk Stock Update
        [HttpPost]
        public IActionResult BulkStockUpdate(List<StockUpdateItem> updates)
        {
            int updatedCount = 0;
            foreach (var update in updates)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == update.ProductId);
                if (product != null)
                {
                    product.Quantity = update.NewQuantity;
                    product.LastUpdated = DateTime.Now;
                    product.Status = GetStatus(update.NewQuantity, product.MinimumStock);
                    updatedCount++;
                }
            }

            return Json(new { success = true, message = $"{updatedCount} products updated successfully" });
        }

        // GET: Get Stock History
        [HttpGet]
        public IActionResult GetStockHistory(string productId, int days = 30)
        {
            // Mock history data - In production, get from database
            var history = new List<object>
            {
                new { Date = DateTime.Now.AddDays(-7), Quantity = 150 },
                new { Date = DateTime.Now.AddDays(-14), Quantity = 145 },
                new { Date = DateTime.Now.AddDays(-21), Quantity = 160 },
                new { Date = DateTime.Now.AddDays(-28), Quantity = 155 }
            };

            return Json(history);
        }

        // GET: Export Inventory Report
        [HttpGet]
        public IActionResult ExportInventoryReport()
        {
            var inventory = _products.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.Category,
                p.Quantity,
                p.MinimumStock,
                p.MaximumStock,
                StockPercentage = ((decimal)p.Quantity / p.MaximumStock * 100).ToString("F2") + "%",
                p.Status,
                p.LastUpdated
            }).ToList();

            return Json(inventory);
        }

        // Helper Methods
        private string GetStockLevel(int quantity, int minimumStock)
        {
            if (quantity == 0) return "Critical";
            if (quantity < minimumStock) return "Low";
            if (quantity >= minimumStock && quantity <= minimumStock * 2) return "Medium";
            return "High";
        }

        private string GetStatus(int quantity, int minimumStock)
        {
            if (quantity == 0) return "Out of Stock";
            if (quantity < minimumStock) return "Low Stock";
            return "Good";
        }
    }

    public class StockUpdateItem
    {
        public string ProductId { get; set; }
        public int NewQuantity { get; set; }
        public string Reason { get; set; }
    }
}