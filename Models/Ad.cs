using System;
namespace Donia.Models
{
    public class Ad
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string phone { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string category { get; set; }
        public string user_id  { get; set; }
        public string subcategory { get; set; }
    }
}
