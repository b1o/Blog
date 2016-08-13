using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }

        public virtual ISet<ApplicationUser> Followers { get; set; }

        public virtual ISet<ApplicationUser> Following { get; set; }

        public virtual ISet<Post> Posts { get; set; }

        public byte[] ProfileImage { get; set; }

        public byte[] HeaderImage { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("DisplayName", this.DisplayName));
            return userIdentity;
        }
    }

}
