namespace WebApplication1.Models
{
    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public int MinimumStock { get; set; } = 20;
        public int MaximumStock { get; set; } = 500;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public string ImageUrl { get; set; }
    }
}