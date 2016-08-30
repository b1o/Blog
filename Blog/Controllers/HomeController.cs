using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Dashboard");
            }

            return this.RedirectToAction("Login", "Account");
        }

        [Authorize]
        public ActionResult Explore()
        {
            return this.View();
        }

        [Authorize]
        public ActionResult My()
        {
            var currentUserid = this.User.Identity.GetUserId();
            var myPosts = this.db.Users
                .Where(u => u.Id == currentUserid)
                .Select(u => u.Posts)
                .FirstOrDefault()
                .OrderByDescending(e => e.PostedOn)
                .Select(e => new PostViewModel()
                {
                    AuthorDisplayName = e.Author.DisplayName,
                    Id = e.Id,
                    Author = e.Author,
                    PostedOn = e.PostedOn,
                    Title = e.Title,
                    Tags = e.Tags,
                    Description = e.Description,
                    Content = e.Content,
                    IsPublic = e.isPublic
                });

            return this.View(myPosts);
        }
    }
}