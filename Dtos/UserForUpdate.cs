using System;
using Microsoft.AspNetCore.Http;

namespace Donia.Dtos
{
    public class UserForUpdate
    {
        public string name { get; set; }
        public string userId { get; set; }
        public string aboutText { get; set; }
        public IFormFile profileImage { get; set; }
        public IFormFile banarImage { get; set; }

    }
}
