namespace Blog.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class DbMigrationConfig : DbMigrationsConfiguration<Blog.Data.ApplicationDbContext>
    {
        public DbMigrationConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Blog.Data.ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var adminEmail = "admin@admin.bg";
                var adminDisplayName = "Admin";
                var adminPassword = "Admin";
                string adminRole = "Administrator";

                var userMail = "user@mail.bg";
                var userName = "User";
                var userPass = "user";

                CreateAdminUser(context, adminDisplayName, adminEmail, adminPassword, adminRole);
                this.CreateUser(context, userName, userMail, userPass);
                CreateSeveralPosts(context);
            }
        }

        private void CreateSeveralPosts(ApplicationDbContext context)
        {
            context.Posts.Add(new Post()
            {
                Title = "Example post TItle",
                PostedOn = DateTime.Now,
                Content = "Example Content\r\n with multiLines",
                Author = context.Users.First()
            });

            context.Posts.Add(new Post()
            {
                Title = "Example Post with Desc Title",
                Author = context.Users.First(),
                PostedOn = DateTime.Now,
                Content = "Some useless Content",
                Description = "Example Description",
                isPublic = true,
                Tags = new HashSet<Tag>()
                {
                    new Tag() { Name = "Test tag 1" },
                    new Tag() { Name = "Test tag 2" }
                }
            });

            context.Posts.Add(new Post()
            {
                Title = "Radnom Post",
                Author = context.Users.First(),
                PostedOn = DateTime.Now,
                Content = "Some useless Cdfsdfasdfasdfjásdfas'dfasdfásj\r\n asdfasdfsdfsfsontent",
                Description = "Example Description",
                isPublic = false,
                Tags = new HashSet<Tag>()
                {
                    new Tag() { Name = "Test tag 1" },
                    new Tag() { Name = "Test tag 2" },
                    new Tag() { Name = "Test tag 2" },
                    new Tag() { Name = "Test 2" }
                }
            });
        }

        private void CreateAdminUser(ApplicationDbContext context, string adminDisplayName, string adminEmail, string adminPassword, string adminRole)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                DisplayName = adminDisplayName,
                Email = adminEmail
            };

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole("Administrator"));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            var addAdminRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!addAdminRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
            }
        }

        private void CreateUser(ApplicationDbContext context, string adminDisplayName, string adminEmail, string adminPassword)
        {
            var user = new ApplicationUser
            {
                UserName = adminEmail,
                DisplayName = adminDisplayName,
                Email = adminEmail
            };

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            var userCreateResult = userManager.Create(user, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }
        }
    }
}
