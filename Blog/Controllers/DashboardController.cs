using Blog.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        //TODO: exctract the select as expression in the PostViewModel for less code duplication
        // GET: Dashboard
        public ActionResult Index()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);

            var posts = currentUser.Following
                .SelectMany(u => u.Posts)
                .Where(p => p.isPublic)
                .Select( e => new PostViewModel()
                {
                    Id = e.Id,
                    AuthorDisplayName = e.Author.DisplayName,
                    Content = e.Content,
                    IsPublic = e.isPublic,
                    Description = e.Description,
                    PostedOn = e.PostedOn,
                    Tags = e.Tags,
                    Title = e.Title,
                    Author = e.Author,
                    Likes = this.db.Likes.Count(l => l.LikedPost.Id == e.Id),
                    IsLiked = currentUser.LikesPosts.Any(l => l.LikedPost.Id == e.Id)
                });

            var myPosts = currentUser.Posts.Select(e => new PostViewModel()
            {
                Id = e.Id,
                AuthorDisplayName = e.Author.DisplayName,
                Content = e.Content,
                IsPublic = e.isPublic,
                Description = e.Description,
                PostedOn = e.PostedOn,
                Tags = e.Tags,
                Title = e.Title,
                Author = e.Author,
                Likes = this.db.Likes.Count(l => l.LikedPost.Id == e.Id),
                IsLiked = currentUser.LikesPosts.Any(l => l.LikedPost.Id == e.Id)
            });

            posts = posts.Concat(myPosts).OrderByDescending(p => p.PostedOn);

            return this.View(posts);
        }
    }
}