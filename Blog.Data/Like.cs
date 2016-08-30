using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class Like
    {
        public int Id { get; set; } 

        public virtual ApplicationUser User { get; set; }

        public virtual Post LikedPost { get; set; }
    }
}
