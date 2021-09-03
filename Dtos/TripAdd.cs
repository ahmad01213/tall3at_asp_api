using System;
namespace Donia.Dtos
{
    public class TripAdd
    {
        public string description { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string notes { get; set; }
        public string orgId { get; set; }
        public int offerId { get; set; }
        public string userId { get; set; }
        public int chips { get; set; }
        public int persons { get; set; }
        public double price { get; set; }
    }
}
