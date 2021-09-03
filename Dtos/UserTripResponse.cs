﻿using System;
using Donia.Models;

namespace Donia.Dtos
{
    public class UserTripResponse
    {

        public int id { get; set; }
        public string description { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string notes { get; set; }
        public int status { get; set; }
        public string orgId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime date { get; set; }
        public int offerId { get; set; }
        public string userId { get; set; }
        public int chips { get; set; }
        public int persons { get; set; }
        public double price { get; set; }
        public string address { get; set; }
        public int offered { get; set; }
        public User user { get; set; }
    }
}
