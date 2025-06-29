namespace CarServiceSystem.Models
{
    public class Repair
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime RepairDate { get; set; }
        public decimal Cost { get; set; }
        public int CarId { get; set; }
        public Car? Car { get; set; }
    }
}
