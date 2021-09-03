using System;
using Microsoft.AspNetCore.Http;

namespace Donia.Dtos
{
    public class AddService
    {
        public string Country { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string Desc { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Cost { get; set; }
        public bool Status { get; set; }
        public string Location { get; set; }
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public int MaxBookings { get; set; }
        public int MinPersons { get; set; }
        public string Lnk { get; set; }
        public string Meta { get; set; }
        public DateTime StartTime { get; set; }
        public string Payments { get; set; }
        public IFormFile[] images { get; set; }

    }
}
