using Donia.Models;
using System;
using System.Collections.Generic;

namespace Donia.Dtos
{
    public class FoodDetailResponse
    {
        public Food food { get; set; }
        public List<Photo> photos { get; set; }
         
    }
}
