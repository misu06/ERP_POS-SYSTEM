namespace WebApplication1.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
