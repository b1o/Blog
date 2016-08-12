﻿using Blog.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Author")]
        public string AuthorDisplayName { get; set; }

        public ApplicationUser Author { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime PostedOn { get; set; }

        public ISet<Tag> Tags { get; set; }

        public bool IsPublic { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}