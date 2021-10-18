using System;
namespace Donia.Models
{
    public class Order
    {

        public int Id { get; set; }
        public int address_id { get; set; }
        public int market_id { get; set; }
        public int market_rate { get; set; }
        public int rated { get; set; }
        public string market_image { get; set; }
        public string market_name { get; set; }
        public string user_id { get; set; }

        public int delivery_id { get; set; }
        public double price { get; set; }

        public double user_lat { get; set; }
        public double user_lng { get; set; }
        public double market_lat { get; set; }
        public double market_lng { get; set; }
        public string user_address { get; set; }
        public string market_address { get; set; }


        public int status { get; set; }
        public DateTime date { get; set; }

    }
}
