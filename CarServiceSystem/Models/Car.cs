
namespace CarServiceSystem.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? LicensePlate { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public List<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
