using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Models
{
    public class FieldMarket
    {
        public int Id { get; set; }
        public int market_id { get; set; }
        public int field_id { get; set; }

        public double lat { get; set; }
        public double lng { get; set; }

    }
}
