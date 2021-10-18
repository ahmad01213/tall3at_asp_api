using System;
namespace Donia.Dtos
{
    public class SearchMarketRequest
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public string searchText { get; set; }
    }
}

