using System;
namespace Donia.Dtos
{
    public class AddMessage
    {
        public string senderId { get; set; }
        public string recieverId { get; set; }
        public string orgId { get; set; }
        public string serviceId { get; set; }
        public string message { get; set; }
    }
}
