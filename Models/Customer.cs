namespace WebApplication1.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
