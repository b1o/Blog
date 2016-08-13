using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Blog.Controllers;
using Blog.Data;

namespace Blog.Helpers
{
    public static class Utils
    {
        public static byte[] ImgToBytes(HttpPostedFileBase img)
        {
            byte[] imgData = null;
            using (var binary = new BinaryReader(img.InputStream))
            {
                imgData = binary.ReadBytes(img.ContentLength);
            }

            return imgData;
        }

        public static bool GetUser(string userId, ApplicationDbContext dbContext, out ApplicationUser user)
        {
            user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                return true;
            }
            return false;
        }
    }
}