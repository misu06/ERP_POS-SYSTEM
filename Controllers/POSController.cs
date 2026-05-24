using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class POSController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            new Product {
                ProductId = "#0001",
                ProductName = "Coca Cola 500ml",
                Category = "Beverages",
                Barcode = "8901234567890",
                Price = 2.99m,
                Quantity = 150,
                Status = "Good"
            },
            new Product {
                ProductId = "#0002",
                ProductName = "Lays Chips Classic",
                Category = "Snacks",
                Barcode = "8901234567891",
                Price = 1.99m,
                Quantity = 200,
                Status = "Good"
            },
            new Product {
                ProductId = "#0003",
                ProductName = "Fresh Milk 1L",
                Category = "Dairy",
                Barcode = "8901234567892",
                Price = 3.49m,
                Quantity = 15,
                Status = "Low Stock"
            },
            new Product {
                ProductId = "#0004",
                ProductName = "Whole Wheat Bread",
                Category = "Bakery",
                Barcode = "8901234567893",
                Price = 2.49m,
                Quantity = 0,
                Status = "Out of Stock"
            },
            new Product {
                ProductId = "#0005",
                ProductName = "Organic Eggs (12)",
                Category = "Dairy",
                Barcode = "8901234567894",
                Price = 4.99m,
                Quantity = 80,
                Status = "Good"
            },
            new Product {
                ProductId = "#0006",
                ProductName = "Orange Juice",
                Category = "Beverages",
                Barcode = "8901234567895",
                Price = 4.49m,
                Quantity = 45,
                Status = "Good"
            },
            new Product {
                ProductId = "#0007",
                ProductName = "Potato Chips",
                Category = "Snacks",
                Barcode = "8901234567896",
                Price = 2.49m,
                Quantity = 120,
                Status = "Good"
            },
            new Product {
                ProductId = "#0008",
                ProductName = "Chocolate Bar",
                Category = "Snacks",
                Barcode = "8901234567897",
                Price = 1.49m,
                Quantity = 95,
                Status = "Good"
            },
            new Product {
                ProductId = "#0009",
                ProductName = "Mineral Water",
                Category = "Beverages",
                Barcode = "8901234567898",
                Price = 0.99m,
                Quantity = 300,
                Status = "Good"
            },
            new Product {
                ProductId = "#0010",
                ProductName = "Yogurt",
                Category = "Dairy",
                Barcode = "8901234567899",
                Price = 2.29m,
                Quantity = 40,
                Status = "Good"
            }
        };

        private static List<SaleTransaction> _transactions = new List<SaleTransaction>();
        private static int _transactionCounter = 1001;

        // GET: POS Index
        public IActionResult Index()
        {
            ViewBag.ActivePage = "PointOfSale";
            return View();
        }

        // GET: POS Main View
        public IActionResult PointOfSale()
        {
            var settings = new POSSettings();
            ViewBag.TaxRate = settings.TaxRate;
            return View(_products.Where(p => p.Quantity > 0).ToList());
        }

        // GET: Get All Products
        [HttpGet]
        public IActionResult GetProducts()
        {
            return Json(_products.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.Category,
                p.Barcode,
                p.Price,
                p.Quantity,
                p.Status
            }));
        }

        // GET: Get Product by Barcode
        [HttpGet]
        public IActionResult GetProductByBarcode(string barcode)
        {
            var product = _products.FirstOrDefault(p => p.Barcode == barcode && p.Quantity > 0);
            if (product != null)
            {
                return Json(new
                {
                    success = true,
                    product = new
                    {
                        product.ProductId,
                        product.ProductName,
                        product.Barcode,
                        product.Price,
                        product.Quantity
                    }
                });
            }
            return Json(new { success = false, message = "Product not found or out of stock" });
        }

        // GET: Search Products
        [HttpGet]
        public IActionResult SearchProducts(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(_products.Where(p => p.Quantity > 0).Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Category,
                    p.Barcode,
                    p.Price,
                    p.Quantity
                }));
            }

            var results = _products.Where(p =>
                (p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                 p.Barcode.Contains(searchTerm) ||
                 p.Category.ToLower().Contains(searchTerm.ToLower())) &&
                p.Quantity > 0)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Category,
                    p.Barcode,
                    p.Price,
                    p.Quantity
                });

            return Json(results);
        }

        // POST: Process Sale
        [HttpPost]
        public IActionResult ProcessSale([FromBody] SaleRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return Json(new SaleResponse { Success = false, Message = "Cart is empty" });
            }

            // Validate stock availability
            foreach (var item in request.Items)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product == null)
                {
                    return Json(new SaleResponse { Success = false, Message = $"Product {item.ProductName} not found" });
                }
                if (product.Quantity < item.Quantity)
                {
                    return Json(new SaleResponse { Success = false, Message = $"Insufficient stock for {item.ProductName}. Available: {product.Quantity}" });
                }
            }

            // Calculate totals
            decimal subtotal = request.Items.Sum(i => i.Price * i.Quantity);
            decimal discountAmount = subtotal * (request.DiscountPercentage / 100);
            decimal taxableAmount = subtotal - discountAmount;
            decimal taxAmount = taxableAmount * (request.TaxRate / 100);
            decimal totalAmount = taxableAmount + taxAmount;

            // Update inventory
            foreach (var item in request.Items)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    product.Status = product.Quantity <= 0 ? "Out of Stock" :
                                    (product.Quantity < product.MinimumStock ? "Low Stock" : "Good");
                    product.LastUpdated = DateTime.Now;
                }
            }

            // Create transaction
            var transaction = new SaleTransaction
            {
                TransactionId = _transactionCounter++,
                InvoiceNumber = GenerateInvoiceNumber(),
                TransactionDate = DateTime.Now,
                CustomerName = request.CustomerName ?? "Walk-in Customer",
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                PaymentMethod = request.PaymentMethod,
                Subtotal = subtotal,
                DiscountPercentage = request.DiscountPercentage,
                DiscountAmount = discountAmount,
                TaxPercentage = request.TaxRate,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                AmountPaid = request.AmountPaid,
                ChangeDue = request.AmountPaid - totalAmount,
                Items = request.Items,
                CashierName = "Manager",
                Status = "Completed"
            };

            _transactions.Add(transaction);

            return Json(new SaleResponse
            {
                Success = true,
                Message = "Sale completed successfully",
                InvoiceNumber = transaction.InvoiceNumber,
                ChangeDue = transaction.ChangeDue
            });
        }

        // GET: Get Today's Sales
        [HttpGet]
        public IActionResult GetTodaySales()
        {
            var today = DateTime.Today;
            var todaySales = _transactions.Where(t => t.TransactionDate.Date == today).ToList();

            return Json(new
            {
                totalSales = todaySales.Count,
                totalRevenue = todaySales.Sum(t => t.TotalAmount),
                averageOrderValue = todaySales.Any() ? todaySales.Average(t => t.TotalAmount) : 0,
                transactions = todaySales.Select(t => new
                {
                    t.InvoiceNumber,
                    t.TransactionDate,
                    t.CustomerName,
                    t.TotalAmount,
                    t.PaymentMethod
                })
            });
        }

        // GET: Get Transaction History
        [HttpGet]
        public IActionResult GetTransactionHistory(int days = 7)
        {
            var fromDate = DateTime.Now.AddDays(-days);
            var transactions = _transactions.Where(t => t.TransactionDate >= fromDate)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new
                {
                    t.InvoiceNumber,
                    t.TransactionDate,
                    t.CustomerName,
                    t.TotalAmount,
                    t.PaymentMethod,
                    t.Status
                });

            return Json(transactions);
        }

        // POST: Void Transaction
        [HttpPost]
        public IActionResult VoidTransaction(string invoiceNumber)
        {
            var transaction = _transactions.FirstOrDefault(t => t.InvoiceNumber == invoiceNumber);
            if (transaction != null)
            {
                // Restore inventory
                foreach (var item in transaction.Items)
                {
                    var product = _products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                        product.Status = product.Quantity <= 0 ? "Out of Stock" :
                                        (product.Quantity < product.MinimumStock ? "Low Stock" : "Good");
                    }
                }

                transaction.Status = "Voided";
                return Json(new { success = true, message = "Transaction voided successfully" });
            }
            return Json(new { success = false, message = "Transaction not found" });
        }

        // GET: Print Receipt
        [HttpGet]
        public IActionResult PrintReceipt(string invoiceNumber)
        {
            var transaction = _transactions.FirstOrDefault(t => t.InvoiceNumber == invoiceNumber);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.Now:yyyyMMdd}-{_transactionCounter:D4}";
        }
    }

    public class SaleRequest
    {
        public List<CartItem> Items { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string PaymentMethod { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal TaxRate { get; set; }
        public decimal AmountPaid { get; set; }
    }
}