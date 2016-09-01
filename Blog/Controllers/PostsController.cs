using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Data;
using Blog.Models;
using Blog.Extensions;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    [Authorize]
    public class PostsController : BaseController
    {
        // GET: Edit
        public ActionResult Edit(int id)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == id);

            if (post != null)
            {
                var model = new PostViewModel()
                {
                    Id = post.Id,
                    Content = post.Content,
                    Title = post.Title,
                    Description = post.Description,
                    IsPublic = post.isPublic
                };

                return this.PartialView("Edit", model);
            }

            this.AddNotification("Post not found", NotificationType.ERROR);
            return this.RedirectToAction("Index", "DashBoard");
        }

        // Post: Edit
        [HttpPost]
        public ActionResult Edit(PostViewModel model)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == model.Id);
            if (this.User.Identity.IsAuthenticated)
            {
                if (this.ModelState.IsValid)
                {
                    post.Content = model.Content;
                    post.Description = model.Description;
                    post.Title = model.Title;
                    model.IsPublic = model.IsPublic;

                    this.db.SaveChanges();
                    this.AddNotification($"Post #{post.Id} Edited", NotificationType.SUCCESS);
                    return this.RedirectToAction("Profile", "Users");
                }

                return this.PartialView("Edit", model);
            }

            return this.RedirectToAction("Login", "Account");
        }

        // GET: Delete
        public ActionResult Delete(int id)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                if (this.User.Identity.GetUserId() == post.AuthorId)
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
                    Likes = this.db.Likes.Count(l => l.LikedPost.Id == model.Id),
                    Id = model.Id
                };

                this.db.Posts.Add(post);
                this.db.SaveChanges();
                if (model.IsPublic)
                {
                    this.AddNotification("Public Post Created", NotificationType.SUCCESS);
                    return this.RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    this.AddNotification("Private Post Created", NotificationType.SUCCESS);
                    return this.RedirectToAction("Profile", "Users");
                }
            }

            return this.PartialView("Create", model);
        }

        public ActionResult GetUserLikedPosts(string name)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var user = this.db.Users.FirstOrDefault(u => u.DisplayName == name);

            var model = user.LikesPosts.Select(l => l.LikedPost).Select(p => new PostViewModel()
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
                IsLiked = currentUser.LikesPosts.Any(l => l.LikedPost.Id == p.Id)
            });

            return this.PartialView("_UserPostsPartial", model);
        }

        public ActionResult GetUserPosts(string name)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);

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
                Likes = this.db.Likes.Count(l => l.LikedPost.Id == e.Id),
                IsPublic = e.isPublic,
                IsLiked = currentUser.LikesPosts.Any(l => l.LikedPost.Id == e.Id )
            });

            return this.PartialView("_UserPostsPartial", model);
        }

        public ActionResult Like(int id)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == id);
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var like = new Like()
            {
                LikedPost = post,
                User = currentUser
            };

            if (post != null && !currentUser.LikesPosts.Any(l => l.LikedPost == like.LikedPost))
            {
                currentUser.LikesPosts.Add(like);
                this.db.SaveChanges();
            }

            return this.Json(new {postLikesCount = this.db.Likes.Count(l => l.LikedPost.Id == id)}, "application/json");
        }

        public ActionResult Unlike(int id)
        {
            var post = this.db.Posts.FirstOrDefault(p => p.Id == id);
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var like = new Like()
            {
                LikedPost = post,
                User = currentUser
            };

            if (post != null && currentUser.LikesPosts.Any(l => l.LikedPost == like.LikedPost))
            {
                var result = currentUser.LikesPosts.FirstOrDefault(l => l.LikedPost == post);
                this.db.Likes.Remove(result);
                currentUser.LikesPosts.Remove(result);
                this.db.SaveChanges();
            }

            return this.Json(new { postLikesCount = this.db.Likes.Count(l => l.LikedPost.Id == id) }, "application/json");
        }
    }
}