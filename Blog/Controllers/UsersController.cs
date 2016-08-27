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
            var userViewModel = new UserViewModel
            {
                Name = user.DisplayName,
                Followers = user.Followers,
                Following = user.Following,
                Posts = user.Posts,
                Id = user.Id
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
                    HeaderImage = currentUser.HeaderImage
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
                var userViewModel = new UserViewModel
                {
                    Name = user.DisplayName,
                    Followers = user.Followers,
                    Posts = user.Posts,
                    Id = user.Id
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

        // POST: /Users/Edit
        [Authorize]
        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "ProfileImage, HeaderImage")]UserEditModel model)
        {
            if (ModelState.IsValid && Request.IsAuthenticated)
            {
                var user = this.db.Users
                    .FirstOrDefault(u => u.Id == model.Id);
                user.DisplayName = model.DisplayName;

                
                if (Request.Files.Count > 0)
                {
                    byte[] profileImgData = null;
                    byte[] headerImgData = null;

                    HttpPostedFileBase profileImage = Request.Files["ProfileImage"];
                    HttpPostedFileBase headerImage = Request.Files["HeaderImage"];

                    using (BinaryReader binary = new BinaryReader(profileImage.InputStream), binary2 = new BinaryReader(headerImage.InputStream))
                    {
                        profileImgData = binary.ReadBytes(profileImage.ContentLength);
                        headerImgData = binary2.ReadBytes(headerImage.ContentLength);
                    }

                    if (headerImgData.Length > 0)
                    {
                        user.HeaderImage = headerImgData;
                    }

                    if (profileImgData.Length > 0)
                    {
                        user.ProfileImage = profileImgData;
                    }
                }

                
                this.db.SaveChanges();
                return this.RedirectToAction("Profile", "Users");
            }
            return this.View(model);
        }

        public ActionResult Follow(string name)
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

            return this.RedirectToAction("Show", "Users", new {name = targetUser.DisplayName});
        }

        public ActionResult Unfollow(string name)
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

            return this.RedirectToAction("Show", "Users", new { name = targetUser.DisplayName });
        }
    }
}