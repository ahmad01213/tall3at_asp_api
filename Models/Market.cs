

using System.Text.Json.Serialization;

namespace Donia.Models
{
    public class Market
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string user_id { get; set; }
        public int balance { get; set; }
        public string image { get; set; }
        public string bannarImage { get; set; }
        public double lat { get; set; }
        public int isClosed { get; set; }
        public double lng { get; set; }
        public string phone { get; set; }
        public int status { get; set; }
        public string summary { get; set; }
        public int rate { get; set; }
        public int reviews { get; set; }
        public int review_sum { get; set; }
        public int order_count { get; set; }


        

        public Market() {
            status = 1;
            rate = 0;
            order_count = 0;
            isClosed = 0;
            balance = 0;
        }
    }
}
