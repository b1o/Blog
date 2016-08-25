using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class UserEditModel
    {
        public string Id { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public byte[] ProfileImage { get; set; }

        public byte[] HeaderImage { get; set; }
    }
}