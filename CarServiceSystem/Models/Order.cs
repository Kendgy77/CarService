namespace CarServiceSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? CarBrand { get; set; }
        public string? CarLicensePlate { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string Status { get; set; } = "Nowe";
        public int ClientId { get; set; }
        public Client? Client { get; set; }
    }
}