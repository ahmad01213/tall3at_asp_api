using System;
namespace Donia.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecieverId { get; set; }
        public string message { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public int ServiceId { get; set; }
        public string OrgId { get; set; }
        public int Readed { get; set; }
 
        public Message()
        {
            Date = DateTime.Now;
        }
    }
}
