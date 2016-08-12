using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class UsersController : BaseController
    {
        // GET: Users
        public ActionResult Show(string name)
        {
            var user = this.db.Users.Where(u => u.DisplayName == name).Select(e => new UserViewModel
            {
                Name = e.DisplayName,
                Followers = e.Followers,
                Posts = e.Posts,
                Id = e.Id
            }).FirstOrDefault();

            return this.View(user);
        }

        public FileContentResult GetUserProfileImage(string id)
        {
            var user = this.db.Users.FirstOrDefault(u => u.Id == id);

            return new FileContentResult(user.ProfileImage, "image/jpg");
        }
    }
}