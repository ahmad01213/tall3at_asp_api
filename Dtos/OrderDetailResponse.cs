using System;
using System.Collections.Generic;
using Donia.Models;

namespace Donia.Dtos
{
    public class OrderDetailResponse
    {



        public Order order { get; set; }
        public Driver driver { get; set; }

        public Market market { get; set; }
        public List<CartListResponse> carts { get; set; }

    }
}
