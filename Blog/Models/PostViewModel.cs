using Blog.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Blog.Models
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Автор")]
        public string AuthorDisplayName { get; set; }

        public ApplicationUser Author { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public DateTime PostedOn { get; set; }

        public ISet<Tag> Tags { get; set; }

        public bool IsPublic { get; set; }

        public bool IsLiked { get; set; }

        public int Likes { get; set; }

        [MaxLength(300)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [MaxLength(300)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public static Expression<Func<Post, PostViewModel>> ViewModel
        {
            get
            {
                return e => new PostViewModel()
                {
                    Id = e.Id,
                    Author = e.Author,
                    Content = e.Content,
                    PostedOn = e.PostedOn,
                    Likes = e.Likes,
                    Tags = e.Tags,
                    Description = e.Description,
                    Title = e.Title,
                    AuthorDisplayName = e.Author.DisplayName,
                    IsPublic = e.isPublic,
                    IsLiked = false
                };
            }
        }
    }
}