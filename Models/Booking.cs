using System;
namespace Donia.Models
{
    public class Booking
    {


        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string UserId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; }
        public double TotalCost { get; set; }
        public int Sets { get; set; }
        public string Notes { get; set; }
        public Service service { get; set; }
        public User user { get; set; }
        public Booking() {
            CreatedAt = DateTime.Now;
        }
    }
}
