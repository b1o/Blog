using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Blog.Data;

namespace Blog.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Display(Name="Display Name")]
        public string Name { get; set; }

        [Display(Name = "Header Image")]
        public byte[] HeaderImage { get; set; }

        [Display(Name = "Profile Image")]
        public byte[] ProfileImage { get; set; }

        public bool IsFollowing { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<PostViewModel> Likes { get; set; }

        public ICollection<ApplicationUser> Followers { get; set; }

        public ICollection<ApplicationUser> Following { get; set; }
    }
}