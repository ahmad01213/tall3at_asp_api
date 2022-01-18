using System;
using Microsoft.AspNetCore.Http;

namespace Donia.Dtos
{
    public class UserForRegister
    {
        public string FullName { get; set; }
        public string knownName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string identityNumber { get; set; }
        public string IbanNumber { get; set; }
        public string ProfileImage { get; set; }
        public string identityImage { get; set; }
    }
}
