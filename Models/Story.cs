using System;
namespace Donia.Models
{
    public class Story
    {


        public int Id { get; set; }

        public string UserId { get; set; }
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public Story()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
