using System;
namespace Donia.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string card_number { get; set; }
        public string expire_date { get; set; }
        public string user_id { get; set; }
        public int cvv { get; set; }

    }
}
