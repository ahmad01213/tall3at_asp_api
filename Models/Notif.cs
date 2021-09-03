using System;
namespace Donia.Models
{
    public class Notif
    {


        public int Id { get; set; }

        public string Title { get; set; }
        public string Image { get; set; }
        public string Body { get; set; }
        public string Modle { get; set; }
        public int IsRead { get; set; }
        public DateTime Date { get; set; }
        public string ModleId { get; set; }
        public string UserId { get; set; }

        public Notif()
        {
            Date = DateTime.Now;
        }
    }
}
