using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Models
{
    public class FoodForAdd
    {

        public int Id { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string attatchments { get; set; }
        public string serve_way { get; set; }
        public string notes { get; set; }
        public int price { get; set; }
        public int persons { get; set; }
        public double preparation_time { get; set; }
        public int category_id { get; set; }
        public int market_id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int field_id { get; set; }
        public string images { get; set; }

        public string marketName { get; set; }
    }
}
