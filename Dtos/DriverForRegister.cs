using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Dtos
{
    public class DriverForRegister
    {
        public string FullName { get; set; }
        public string knownName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IdentityNumber { get; set; }
        public string address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
