using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Donia.Models
{
    public class User: IdentityUser
    {
        public string name { get; set; }
        public string knownName { get; set; }
        public string GoogleId { get; set; }
        public string FacebookId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AboutText { get; set; }

        public string BanarImage { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string FullName { get; set; }
        public string Statuse { get; set; }
        public string DeviceToken { get; set; }
        public string bank { get; set; }
        public string code { get; set; }
        public string Role { get; set; }
        public double balance { get; set; }


        public string identityNumber { get; set; }
        public string IbanNumber { get; set; }
        public string ProfileImage { get; set; }
        public string carImage { get; set; }
        public string identityImage { get; set; }


        public User() {
            GoogleId = null;
            FacebookId = null;
           CreatedAt = DateTime.Now;
            balance = 0.0;
        }
    }
}
