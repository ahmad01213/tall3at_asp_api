using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donia.Dtos
{
    public class UserDetailResponse
    {


        public string Id { get; set; }
        public string knownName { get; set; }
        public string Email { get; set; }
        public string userName { get; set; }
        public DateTime CreatedAt { get; set; }


        public string City { get; set; }
        public string Country { get; set; }
        public string FullName { get; set; }
        public string Statuse { get; set; }
        public string Role { get; set; }
        public double balance { get; set; }

        public string identityNumber { get; set; }
        public string IbanNumber { get; set; }
        public string ProfileImage { get; set; }
        public string carImage { get; set; }
        public string identityImage { get; set; }

    }
}
