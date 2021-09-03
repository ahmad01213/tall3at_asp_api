using System;
using Donia.Models;

namespace Donia.Dtos
{
    public class ServiceResponse
    {

        public bool Expired { get; set; }
        public User user { get; set; }
        public Service service { get; set; }

    }
}
