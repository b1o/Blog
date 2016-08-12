using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class Tag
    {
        public Tag()
        {
            this.AssociatedPosts = new HashSet<Post>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ISet<Post> AssociatedPosts { get; set; }
    }
}
