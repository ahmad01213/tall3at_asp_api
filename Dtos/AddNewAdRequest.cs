using System;
using Microsoft.AspNetCore.Http;

namespace Donia.Dtos
{
    public class AddNewAdRequest
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string phone { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string category { get; set; }
        public string subcategory { get; set; }
        public IFormFile[] images { get; set; }
    }
}
