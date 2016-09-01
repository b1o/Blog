using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Data;
using Blog.Helpers;
using Blog.Models;
using Microsoft.AspNet.Identity;
using Blog.Extensions;
namespace Blog.Controllers
{
    public class UsersController : BaseController
    {
        // GET: Users/Show
        public ActionResult Show(string name)
        {
            var user = this.db.Users.FirstOrDefault(u => u.DisplayName == name);
            var userLikes = user.LikesPosts.Select(l => l.LikedPost)
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
                 IsLiked = user.LikesPosts.Any(l => l.LikedPost.Id == p.Id)
             }).ToList();

            var userViewModel = new UserViewModel
            {
                Name = user.DisplayName,
                Followers = user.Followers,
                Following = user.Following,
                Posts = user.Posts,
                Id = user.Id,
                Likes = userLikes
            };

            var isOwner = user.Id == this.User.Identity.GetUserId();
            this.ViewBag.CanEdit = isOwner && this.Request.IsAuthenticated;

            ApplicationUser currentUser;
            if (Utils.GetUser(this.User.Identity.GetUserId(), this.db, out currentUser))
            {
                this.ViewBag.IsFollowing = currentUser.Following.Contains(user) && currentUser != user;

                this.ViewBag.IsCurrentUser = currentUser.DisplayName == name;
            }
            return this.View(userViewModel);
        }

        public ActionResult Profile()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var userLikes = currentUser.LikesPosts.Select(l => l.LikedPost)
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
                IsLiked = currentUser.LikesPosts.Any(l => l.LikedPost.Id == p.Id)
            }).ToList();

            if (currentUser != null)
            {
                var model = new UserViewModel()
                {
                    Id = currentUser.Id,
                    Name = currentUser.DisplayName,
                    ProfileImage = currentUser.ProfileImage,
                    Posts = currentUser.Posts,
                    Following = currentUser.Following,
                    Followers = currentUser.Followers,
                    HeaderImage = currentUser.HeaderImage,
                    Likes = userLikes
                };

                return this.View(model);
            }

            return this.HttpNotFound();
        }

        public ActionResult PopulateHoverCard(string name)
        {
            var user = this.db.Users.FirstOrDefault(u => u.DisplayName == name);
            if (user != null)
            {
                var userLikes = user.LikesPosts.Select(l => l.LikedPost)
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
                        IsLiked = user.LikesPosts.Any(l => l.LikedPost.Id == p.Id)
                    }).ToList();

                var userViewModel = new UserViewModel
                {
                    Name = user.DisplayName,
                    Followers = user.Followers,
                    Posts = user.Posts,
                    Id = user.Id,
                    Likes = userLikes
                };

                return this.PartialView("_HoverCard", userViewModel);
            }
            return this.PartialView("_HoverCard", new UserViewModel() {Name = "error"});
        }

        public FileContentResult GetUserProfileImage(string id)
        {
            var user = this.db.Users.FirstOrDefault(u => u.Id == id);

            return new FileContentResult(user.ProfileImage, "image/jpg");
        }

        public FileContentResult GetUserHeaderImage(string id)
        {
            var user = this.db.Users.FirstOrDefault(u => u.Id == id);

            return new FileContentResult(user.HeaderImage, "image/jpg");
        }

        // GET: /Users/Edit
        [Authorize]
        [HttpGet]
        public ActionResult Edit()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var user = this.db.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => new UserEditModel()
                {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    ProfileImage = u.ProfileImage,
                    HeaderImage = u.HeaderImage
                }).FirstOrDefault();
            return this.View(user);
         
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditProfileImage()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);

            var image = this.Request.Files["ProfileImage"];

            if (image.ContentLength > 3500 * 1024)
            {
                this.AddNotification("Image size should not be larger than 3.5mb", NotificationType.ERROR);
                return this.RedirectToAction("Profile", "Users");
            }

            byte[] imageData = null;
            using (BinaryReader binary = new BinaryReader(image.InputStream))
            {
                imageData = binary.ReadBytes(image.ContentLength);
            }

            if (imageData.Length > 0)
            {
                currentUser.ProfileImage = imageData;
                this.db.SaveChanges();
            }
            else
            {
                this.AddNotification("Something went wrong", NotificationType.ERROR);
            }

            return this.RedirectToAction("Profile", "Users");
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditHeaderImage()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);

            var image = this.Request.Files["HeaderImage"];
            byte[] imageData = null;
            using (BinaryReader binary = new BinaryReader(image.InputStream))
            {
                imageData = binary.ReadBytes(image.ContentLength);
            }

            if (imageData.Length > 0)
            {
                currentUser.HeaderImage = imageData;
                this.db.SaveChanges();
            }
            return this.RedirectToAction("Profile", "Users");
        }

        // POST: /Users/Edit
        //[Authorize]
        //[HttpPost]
        //public ActionResult Edit([Bind(Exclude = "ProfileImage, HeaderImage")]UserEditModel model)
        //{
        //    if (ModelState.IsValid && Request.IsAuthenticated)
        //    {
        //        var user = this.db.Users
        //            .FirstOrDefault(u => u.Id == model.Id);
        //        user.DisplayName = model.DisplayName;

                
        //        if (Request.Files.Count > 0)
        //        {
        //            byte[] profileImgData = null;
        //            byte[] headerImgData = null;

        //            HttpPostedFileBase profileImage = Request.Files["ProfileImage"];
        //            HttpPostedFileBase headerImage = Request.Files["HeaderImage"];

        //            using (BinaryReader binary = new BinaryReader(profileImage.InputStream), binary2 = new BinaryReader(headerImage.InputStream))
        //            {
        //                profileImgData = binary.ReadBytes(profileImage.ContentLength);
        //                headerImgData = binary2.ReadBytes(headerImage.ContentLength);
        //            }

        //            if (headerImgData.Length > 0)
        //            {
        //                user.HeaderImage = headerImgData;
        //            }

        //            if (profileImgData.Length > 0)
        //            {
        //                user.ProfileImage = profileImgData;
        //            }
        //        }

                
        //        this.db.SaveChanges();
        //        return this.RedirectToAction("Profile", "Users");
        //    }
        //    return this.View(model);
        //}

        public void Follow(string name)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var targetUser = this.db.Users.FirstOrDefault(u => u.DisplayName == name);

            if (targetUser != null && currentUser != null && !currentUser.Following.Contains(targetUser) && targetUser != currentUser)
            {
                currentUser.Following.Add(targetUser);
                targetUser.Followers.Add(currentUser);
                this.db.SaveChanges();
            }
        }

        public void Unfollow(string name)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);
            var targetUser = this.db.Users.FirstOrDefault(u => u.DisplayName == name);

            if (targetUser != null && currentUser != null && currentUser.Following.Contains(targetUser) && targetUser != currentUser)
            {
                currentUser.Following.Remove(targetUser);
                targetUser.Followers.Remove(currentUser);
                this.db.SaveChanges();
            }
        }

        //Really Slow TODO: find a way to optimize
        public ActionResult GetUsers(int limit, int offset)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var currentUser = this.db.Users.FirstOrDefault(u => u.Id == currentUserId);

            var users = this.db.Users
                .Where(u => currentUserId != u.Id)
                .OrderBy(u => u.DisplayName)
                .Skip(offset)
                .Take(limit);

            var model = new List<UserViewModel>();
            foreach (var user in users)
            {
                var isFollowing = currentUser.Following.Contains(user);
                var userLikes = user.LikesPosts.Select(l => l.LikedPost).Select(p => new PostViewModel()
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
                    IsLiked = user.LikesPosts.Any(l => l.LikedPost.Id == p.Id)
                }).ToList();
                model.Add(new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.DisplayName,
                    ProfileImage = user.ProfileImage,
                    Following = user.Following,
                    Posts = user.Posts,
                    HeaderImage = user.HeaderImage,
                    Followers = user.Followers,
                    IsFollowing = isFollowing,
                    Likes = userLikes
                });
            }

            this.ViewBag.Offset = model.Count + offset;

            return this.PartialView("_UserListPartial", model);
        }
    }
}