using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Dtos
{
    public class CartListResponse
    {
        public int Id { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public FoodDetailResponse food { get; set; }
    }
}
