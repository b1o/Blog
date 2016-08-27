using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    [Authorize]
    public class PostsController : BaseController
    {
        // GET: Delete
        public ActionResult Delete(int id)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                var model = new PostViewModel()
                {
                    Id = post.Id,
                    Author = post.Author,
                    PostedOn = post.PostedOn,
                    Content = post.Content,
                    IsPublic = post.isPublic,
                    Tags = post.Tags,
                    Description = post.Description,
                    Title = post.Title,
                    AuthorDisplayName = post.Author.DisplayName
                };
                return this.PartialView("Delete", model);
            }

            return this.HttpNotFound("Post not found");
        }

        // POST: Delete
        [HttpPost]
        public ActionResult Delete(PostViewModel model)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == model.Id);
            this.db.Posts.Remove(post);
            this.db.SaveChanges();

            return this.RedirectToAction("My", "Home");
        }

        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            return this.PartialView("Create");
        }

        // POST: Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(PostViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var currentUser = this.User.Identity.GetUserId();
                var post = new Post()
                {
                    Author = this.db.Users.FirstOrDefault(u => u.Id == currentUser),
                    AuthorId = currentUser,
                    Content = model.Content,
                    isPublic = model.IsPublic,
                    Tags = model.Tags,
                    Description = model.Description,
                    PostedOn = DateTime.Now,
                    ScheduledOn = DateTime.Now,
                    Title = model.Title,
                    Id = model.Id
                };

                this.db.Posts.Add(post);
                this.db.SaveChanges();
                if (model.IsPublic)
                {
                    return this.RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return this.RedirectToAction("My", "Home");
                }
            }

            return this.PartialView("Create", model);
        }

        public ActionResult GetUserPosts(string name)
        {
            var model = this.db.Users
            .Where(u => u.DisplayName == name)
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

            return this.PartialView("_UserPostsPartial", model);
        }
    }
}