using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.ServerSentEvents;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ManagerController : Controller
    {
        // Mock data - In real application, this would come from database
        private static List<Product> _products = new List<Product>
        {
            new Product { ProductId = "#0001", ProductName = "Coca-Cola", Category = "Beverages", Barcode = "8901234567890", Price = 2.99m, Quantity = 150, Status = "In Stock" },
            new Product { ProductId = "#0002", ProductName = "Lays Chips Classic", Category = "Snacks", Barcode = "8901234567891", Price = 1.99m, Quantity = 200, Status = "In Stock" },
            new Product { ProductId = "#0003", ProductName = "Fresh Milk", Category = "Dairy", Barcode = "8901234567892", Price = 3.49m, Quantity = 15, Status = "Low Stock" },
            new Product { ProductId = "#0004", ProductName = "Whole Wheat Bread", Category = "Bakery", Barcode = "8901234567893", Price = 2.49m, Quantity = 0, Status = "Out of Stock" },
            new Product { ProductId = "#0005", ProductName = "Organic Eggs (12)", Category = "Dairy", Barcode = "8901234567894", Price = 4.99m, Quantity = 80, Status = "In Stock" },
            new Product { ProductId = "#0006", ProductName = "French Fries", Category = "Snacks", Barcode = "8901234567895", Price = 3.99m, Quantity = 45, Status = "In Stock" },
            new Product { ProductId = "#0007", ProductName = "Orange Juice", Category = "Beverages", Barcode = "8901234567896", Price = 4.49m, Quantity = 12, Status = "Low Stock" },
            new Product { ProductId = "#0008", ProductName = "Butter", Category = "Dairy", Barcode = "8901234567897", Price = 5.99m, Quantity = 60, Status = "In Stock" },
        };

        private static List<Employee> _employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Smith", Position = "Cashier", Email = "john@pos.com", Phone = "555-0101", Salary = 32000, IsActive = true },
            new Employee { Id = 2, Name = "Sarah Johnson", Position = "Manager", Email = "sarah@pos.com", Phone = "555-0102", Salary = 55000, IsActive = true },
            new Employee { Id = 3, Name = "Mike Brown", Position = "Stock Clerk", Email = "mike@pos.com", Phone = "555-0103", Salary = 28000, IsActive = true },
            new Employee { Id = 4, Name = "Lisa Davis", Position = "Cashier", Email = "lisa@pos.com", Phone = "555-0104", Salary = 32000, IsActive = false },
        };

        private static List<Customer> _customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Alice Wonder", Email = "alice@email.com", Phone = "555-1001", TotalSpent = 1250.50m, JoinDate = DateTime.Now.AddMonths(-6) },
            new Customer { Id = 2, Name = "Bob Martin", Email = "bob@email.com", Phone = "555-1002", TotalSpent = 890.75m, JoinDate = DateTime.Now.AddMonths(-3) },
            new Customer { Id = 3, Name = "Carol White", Email = "carol@email.com", Phone = "555-1003", TotalSpent = 2100.00m, JoinDate = DateTime.Now.AddMonths(-12) },
        };

        private static List<Supplier> _suppliers = new List<Supplier>
        {
            new Supplier { Id = 1, Name = "Global Beverages Inc.", Contact = "David Lee", Phone = "555-2001", Email = "orders@globalbeverages.com", Address = "123 Industrial Ave" },
            new Supplier { Id = 2, Name = "Fresh Foods Distributors", Contact = "Emma Watson", Phone = "555-2002", Email = "sales@freshfoods.com", Address = "456 Market Street" },
            new Supplier { Id = 3, Name = "Dairy Best Suppliers", Contact = "Oliver Chen", Phone = "555-2003", Email = "contact@dairybest.com", Address = "789 Farm Road" },
        };

        private static List<Purchase> _purchases = new List<Purchase>
        {
            new Purchase { Id = 1, SupplierName = "Global Beverages Inc.", ProductName = "Coca-Cola", Quantity = 100, UnitPrice = 1.50m, TotalPrice = 150.00m, PurchaseDate = DateTime.Now.AddDays(-5) },
            new Purchase { Id = 2, SupplierName = "Fresh Foods Distributors", ProductName = "Lays Chips", Quantity = 200, UnitPrice = 0.99m, TotalPrice = 198.00m, PurchaseDate = DateTime.Now.AddDays(-3) },
            new Purchase { Id = 3, SupplierName = "Dairy Best Suppliers", ProductName = "Fresh Milk", Quantity = 50, UnitPrice = 2.50m, TotalPrice = 125.00m, PurchaseDate = DateTime.Now.AddDays(-1) },
        };

        // ==================== DASHBOARD ====================
        public IActionResult Dashboard()
        {
            ViewBag.TotalProducts = _products.Count;
            ViewBag.LowStockCount = _products.Count(p => p.Status == "Low Stock");
            ViewBag.OutOfStockCount = _products.Count(p => p.Status == "Out of Stock");
            ViewBag.TotalEmployees = _employees.Count(e => e.IsActive);
            ViewBag.TotalRevenue = 29550; // Mock value
            return View();
        }

        // ==================== PRODUCTS ====================
        public IActionResult Products()
        {
            return View(_products);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                // Generate new ID
                var lastId = _products.LastOrDefault()?.ProductId;
                int newNum = 1;
                if (lastId != null && lastId.StartsWith("#"))
                {
                    int.TryParse(lastId.Substring(1), out newNum);
                    newNum++;
                }
                product.ProductId = $"#{newNum:D4}";

                // Update status based on quantity
                product.Status = product.Quantity <= 0 ? "Out of Stock" : (product.Quantity < 20 ? "Low Stock" : "In Stock");

                _products.Add(product);
                return Json(new { success = true, message = "Product added successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existing != null)
            {
                existing.ProductName = product.ProductName;
                existing.Category = product.Category;
                existing.Barcode = product.Barcode;
                existing.Price = product.Price;
                existing.Quantity = product.Quantity;
                existing.Status = product.Quantity <= 0 ? "Out of Stock" : (product.Quantity < 20 ? "Low Stock" : "In Stock");
                return Json(new { success = true, message = "Product updated successfully" });
            }
            return Json(new { success = false, message = "Product not found" });
        }

        [HttpPost]
        public IActionResult DeleteProduct(string id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                _products.Remove(product);
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            return Json(new { success = false, message = "Product not found" });
        }

        public IActionResult GetProduct(string id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            return Json(product);
        }

        // ==================== INVENTORY ====================
        public IActionResult Inventory()
        {
            var lowStockItems = _products.Where(p => p.Status == "Low Stock" || p.Status == "Out of Stock").ToList();
            ViewBag.TotalValue = _products.Sum(p => p.Price * p.Quantity);
            ViewBag.TotalItems = _products.Sum(p => p.Quantity);
            return View(_products);
        }

        // ==================== POINT OF SALE ====================
        public IActionResult PointOfSale()
        {
            return View(_products);
        }

        [HttpPost]
        public IActionResult ProcessSale(List<SaleItem> items, decimal totalAmount)
        {
            // Process sale logic here
            foreach (var item in items)
            {
                var product = _products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null && product.Quantity >= item.Quantity)
                {
                    product.Quantity -= item.Quantity;
                    // Update status
                    product.Status = product.Quantity <= 0 ? "Out of Stock" : (product.Quantity < 20 ? "Low Stock" : "In Stock");
                }
            }
            return Json(new { success = true, message = "Sale completed successfully" });
        }

        // ==================== CUSTOMERS ====================
        public IActionResult Customers()
        {
            return View(_customers);
        }

        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            customer.Id = _customers.Max(c => c.Id) + 1;
            customer.JoinDate = DateTime.Now;
            _customers.Add(customer);
            return Json(new { success = true, message = "Customer added successfully" });
        }

        [HttpPost]
        public IActionResult UpdateCustomer(Customer customer)
        {
            var existing = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existing != null)
            {
                existing.Name = customer.Name;
                existing.Email = customer.Email;
                existing.Phone = customer.Phone;
                return Json(new { success = true, message = "Customer updated successfully" });
            }
            return Json(new { success = false, message = "Customer not found" });
        }

        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return Json(new { success = true, message = "Customer deleted successfully" });
            }
            return Json(new { success = false, message = "Customer not found" });
        }

        // ==================== SUPPLIERS ====================
        public IActionResult Suppliers()
        {
            return View(_suppliers);
        }

        [HttpPost]
        public IActionResult AddSupplier(Supplier supplier)
        {
            supplier.Id = _suppliers.Max(s => s.Id) + 1;
            _suppliers.Add(supplier);
            return Json(new { success = true, message = "Supplier added successfully" });
        }

        [HttpPost]
        public IActionResult UpdateSupplier(Supplier supplier)
        {
            var existing = _suppliers.FirstOrDefault(s => s.Id == supplier.Id);
            if (existing != null)
            {
                existing.Name = supplier.Name;
                existing.Contact = supplier.Contact;
                existing.Phone = supplier.Phone;
                existing.Email = supplier.Email;
                existing.Address = supplier.Address;
                return Json(new { success = true, message = "Supplier updated successfully" });
            }
            return Json(new { success = false, message = "Supplier not found" });
        }

        [HttpPost]
        public IActionResult DeleteSupplier(int id)
        {
            var supplier = _suppliers.FirstOrDefault(s => s.Id == id);
            if (supplier != null)
            {
                _suppliers.Remove(supplier);
                return Json(new { success = true, message = "Supplier deleted successfully" });
            }
            return Json(new { success = false, message = "Supplier not found" });
        }

        // ==================== PURCHASES ====================
        public IActionResult Purchases()
        {
            return View(_purchases);
        }

        [HttpPost]
        public IActionResult CreatePurchase(Purchase purchase)
        {
            purchase.Id = _purchases.Max(p => p.Id) + 1;
            purchase.PurchaseDate = DateTime.Now;
            purchase.TotalPrice = purchase.Quantity * purchase.UnitPrice;
            _purchases.Add(purchase);

            // Update inventory
            var product = _products.FirstOrDefault(p => p.ProductName == purchase.ProductName);
            if (product != null)
            {
                product.Quantity += purchase.Quantity;
                product.Status = product.Quantity <= 0 ? "Out of Stock" : (product.Quantity < 20 ? "Low Stock" : "In Stock");
            }
            return Json(new { success = true, message = "Purchase recorded successfully" });
        }

        // ==================== EMPLOYEES ====================
        public IActionResult Employees()
        {
            return View(_employees);
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            employee.Id = _employees.Max(e => e.Id) + 1;
            _employees.Add(employee);
            return Json(new { success = true, message = "Employee added successfully" });
        }

        [HttpPost]
        public IActionResult UpdateEmployee(Employee employee)
        {
            var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existing != null)
            {
                existing.Name = employee.Name;
                existing.Position = employee.Position;
                existing.Email = employee.Email;
                existing.Phone = employee.Phone;
                existing.Salary = employee.Salary;
                existing.IsActive = employee.IsActive;
                return Json(new { success = true, message = "Employee updated successfully" });
            }
            return Json(new { success = false, message = "Employee not found" });
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                _employees.Remove(employee);
                return Json(new { success = true, message = "Employee deleted successfully" });
            }
            return Json(new { success = false, message = "Employee not found" });
        }

        // ==================== PAYROLL & ATTENDANCE ====================
        public IActionResult PayrollAttendance()
        {
            ViewBag.TotalEmployees = _employees.Count;
            ViewBag.ActiveEmployees = _employees.Count(e => e.IsActive);
            ViewBag.MonthlyPayroll = _employees.Where(e => e.IsActive).Sum(e => e.Salary);
            return View(_employees);
        }

        [HttpPost]
        public IActionResult RecordAttendance(int employeeId, bool isPresent, DateTime date)
        {
            // Record attendance logic
            return Json(new { success = true, message = "Attendance recorded" });
        }

        // ==================== REPORTS ====================
        public IActionResult Reports()
        {
            ViewBag.TotalRevenue = 29550;
            ViewBag.TotalProducts = _products.Count;
            ViewBag.LowStockCount = _products.Count(p => p.Status == "Low Stock");
            ViewBag.TotalCustomers = _customers.Count;
            return View();
        }

        public IActionResult GetSalesReport(DateTime startDate, DateTime endDate)
        {
            // Generate sales report
            return Json(new { success = true, data = new { total = 15000, items = new List<object>() } });
        }

        // ==================== FRAUD DETECTION ====================
        public IActionResult FraudDetection()
        {
            var suspiciousActivities = new List<object>
            {
                new { Date = DateTime.Now.AddDays(-2), Description = "Unusual refund amount", Amount = 500.00m, Status = "Flagged" },
                new { Date = DateTime.Now.AddDays(-5), Description = "Multiple voided transactions", Amount = 0m, Status = "Review" }
            };
            return View(suspiciousActivities);
        }

        // ==================== INVENTORY PREDICTION ====================
        public IActionResult InventoryPrediction()
        {
            var predictions = _products.Select(p => new
            {
                Product = p.ProductName,
                CurrentStock = p.Quantity,
                PredictedDemand = p.Quantity * 0.3m,
                ReorderPoint = p.Status == "Low Stock" ? "Order Now" : "OK"
            }).ToList();
            return View(predictions);
        }

        // ==================== HELPER METHODS ====================
        public IActionResult GetDashboardStats()
        {
            var stats = new
            {
                totalProducts = _products.Count,
                lowStock = _products.Count(p => p.Status == "Low Stock"),
                outOfStock = _products.Count(p => p.Status == "Out of Stock"),
                totalEmployees = _employees.Count(e => e.IsActive),
                totalRevenue = 29550
            };
            return Json(stats);
        }
    }
}