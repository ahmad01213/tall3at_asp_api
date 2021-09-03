using System;
using Microsoft.AspNetCore.Http;

namespace Donia.Dtos
{
    public class AddPhoto
    {
        public string userId { get; set; }
        public IFormFile image { get; set; }
    }
}
