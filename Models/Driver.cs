using System;
namespace Donia.Models
{
    public class Driver
    {

        public int Id { get; set; }
        public string knownName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string AboutText { get; set; }

        public string BanarImage { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string user_id { get; set; }
        public string ProfileImage { get; set; }
        public string FullName { get; set; }
        public string Statuse { get; set; }
        public string DeviceToken { get; set; }
        public string bank { get; set; }
        public string IbanNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int rate { get; set; }
        public int reviews { get; set; }
        public int review_sum { get; set; }
        public int order_count { get; set; }

    }
}
