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
        // GET: Dashboard
        public ActionResult Index()
        {
            var posts = this.db.Posts
                .Where(p => p.isPublic)
                .OrderByDescending(p => p.PostedOn)
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
                    Author = e.Author
                });
            return View(posts);
        }
    }
}