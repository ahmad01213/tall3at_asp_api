using System;
namespace Donia.Models
{
    public class Comment
    {



        public int Id { get; set; }

        public int Rate { get; set; }
        public string CommentText { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string OrgId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Comment()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
