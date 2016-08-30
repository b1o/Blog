using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class Post
    {
        public Post()
        {
            this.isPublic = true;
            this.Tags = new HashSet<Tag>();
            this.PostedOn = DateTime.Now;
            this.ScheduledOn = PostedOn;
            this.Likes = 0;
        }

        public int Id { get; set; }

        public string AuthorId { get; set; }

        public virtual ApplicationUser Author { get; set; }

        public bool isPublic { get; set; }

        public virtual ISet<Tag> Tags { get; set; }

        public DateTime PostedOn { get; set; }

        public DateTime ScheduledOn { get; set; }

        public int Likes { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public string Description { get; set; }
    }
}
