using Donia.Models;
using System;
using System.Collections.Generic;

namespace Donia.Dtos
{
    public class MarketDetailResponse
    {
        public Market market { get; set; }
        public List<Field> fields { get; set; }
        public List<FoodDetailResponse> foods { get; set; }
    }
}
