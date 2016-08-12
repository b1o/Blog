using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Data;

namespace Blog.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<ApplicationUser> Followers { get; set; }
    }
}