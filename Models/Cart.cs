using System;
namespace Donia.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int price { get; set; }
        public string user_id { get; set; }
        public int quantity { get; set; }
        public int food_id { get; set; }
        public int order_id { get; set; }
        public int market_id { get; set; }
        public int address_id { get; set; }
    }
}
