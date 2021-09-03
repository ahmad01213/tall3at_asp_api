using System;
namespace Donia.Dtos
{
    public class AddBooking
    {


        public int ServiceId { get; set; }
        public string UserId { get; set; }
        public string PaymentMethod { get; set; }
        public string TotalCost { get; set; }
        public string Sets { get; set; }
    }
}