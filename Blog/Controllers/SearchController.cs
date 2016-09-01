using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    public class SearchController : BaseController
    {
        // GET: Search
        public ActionResult Index(string query)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var queryTokens = query.Trim().Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            IDictionary<int, IEnumerable<PostViewModel>> result = new Dictionary<int, IEnumerable<PostViewModel>>();
            var parsedQuery = string.Join(" ", queryTokens).ToLower();
            var userLikes = currentUser.LikesPosts;

            var posts = this.db.Posts.Where(p => queryTokens.Any(q => p.Title.ToLower().Contains(parsedQuery) && p.isPublic))
                .Select(p => new PostViewModel()
                {
                    Id = p.Id,
                    Author = p.Author,
                    Content = p.Content,
                    PostedOn = p.PostedOn,
                    Tags = p.Tags,
                    Description = p.Description,
                    Title = p.Title,
                    IsPublic = p.isPublic,
                    AuthorDisplayName = p.Author.DisplayName,
                    Likes = this.db.Likes.Count(l => l.LikedPost.Id == p.Id),
                    IsLiked = this.db.Likes.Any(l => l.LikedPost.Id == p.Id && l.User.Id == p.Author.Id)
                });


            var model = result.OrderByDescending(r => r.Key).SelectMany(r => r.Value);

            return this.View(posts);
        }
    }
}