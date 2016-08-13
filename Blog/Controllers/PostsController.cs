﻿using System;
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
        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            return this.View();
        }

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

            return this.View();
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