using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Dtos
{
    public class MarketForAddDto
    {
        public string title { get; set; }
        public string user_id { get; set; }
        public string image { get; set; }
        public string bannarImage { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string phone { get; set; }
        public string summary { get; set; }
        public string fields { get; set; }
        
    }
}
