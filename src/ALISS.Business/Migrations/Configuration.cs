namespace ALISS.Business.Migrations
{
    using ALISS.Business.Enums;
    using ALISS.Models.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ALISS.Business.ALISSContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ALISS.Business.ALISSContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            //AddRoleIfNotExists(RolesEnum.SuperAdmin.ToString(), context);
            //AddRoleIfNotExists(RolesEnum.ALISSAdmin.ToString(), context);
            //AddRoleIfNotExists(RolesEnum.Editor.ToString(), context);
            //AddRoleIfNotExists(RolesEnum.ClaimedUser.ToString(), context);
            //AddRoleIfNotExists(RolesEnum.BaseUser.ToString(), context);

            //AddUserIfNotExists("kennysteen", "ksteen@tactuum.com", "Abcd1234!", "Kenny Steen", context);
            //AddUserIfNotExists("paulstewart", "pstewart@tactuum.com", "Abcd1234!", "Paul Stewart", context);
            //AddUserIfNotExists("stefanolabia", "slabia@tactuum.com", "Abcd1234!", "Stefano Labia", context);
            //AddUserIfNotExists("jasonsharp", "jsharp@tactuum.com", "Abcd1234!", "Jason Sharp", context);
            //AddUserIfNotExists("chrismackie", "Chris.Mackie@alliance-scotland.org.uk", "Alli4nce!", "Chris Mackie", context);
            //AddUserIfNotExists("lornaprentice", "Lorna.Prentice@alliance-scotland.org.uk", "Alli4nce!", "Lorna Prentice", context);
            //AddUserIfNotExists("cameronmacfarlane", "Cameron.MacFarlane@alliance-scotland.org.uk", "Alli4nce!", "Cameron MacFarlane", context);
            //AddUserIfNotExists("angelafulton", "angela.fulton@alliance-scotland.org.uk", "Alli4nce!", "Angela Fulton", context);
            //AddUserIfNotExists("katherinelong", "Katherine.Long@alliance-scotland.org.uk", "Alli4nce!", "Katherine Long", context);
        }

        void AddRoleIfNotExists(string role, ALISSContext context)
        {
            IdentityResult identityResult;
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!roleManager.RoleExists(role))
            {
                identityResult = roleManager.Create(new IdentityRole(role)); 
            }
        }

        void AddUserIfNotExists(string username, string email, string password, string name, ALISSContext context)
        {
            var passwordHasher = new PasswordHasher();
            string passwordHash = passwordHasher.HashPassword(password);
            //context.Users.AddOrUpdate(u => u.UserName,
            //    new ApplicationUser()
            //    {
            //        UserName = username,
            //        PasswordHash = passwordHash,
            //        Email = email,
            //        SecurityStamp = new Guid().ToString()
            //    });

            context.UserProfiles.AddOrUpdate(up => up.Email,
                new UserProfile()
                {
                    Username = username,
                    Email = email,
                    Name = name,
                    AcceptPrivacyPolicy = true,
                    AcceptTermsAndConditions = true,
                    Active = true,
                    DateJoined = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                });
        }
    }
}
