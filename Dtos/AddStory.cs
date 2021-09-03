using System;
using Microsoft.AspNetCore.Http;

namespace Donia.Dtos
{
    public class AddStory
    {
        public string userId { get; set; }
        public IFormFile file { get; set; }
        public IFormFile image { get; set; }
    }
}
