using System;
using Donia.Models;

namespace Donia.Dtos
{
    public class MessageResponse
    {
        public Message message { get; set; }
        public User sender { get; set; }
        public User reciever { get; set; }
    }
}
