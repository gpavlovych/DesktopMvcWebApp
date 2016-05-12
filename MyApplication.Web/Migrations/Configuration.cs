using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics.CodeAnalysis;
using MyApplication.Web.Models;
using Microsoft.AspNet.Identity;

namespace MyApplication.Web.Migrations
{
    [ExcludeFromCodeCoverage]

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var passwordHash = new PasswordHasher();
            var password = passwordHash.HashPassword("Password@123");
            var publisher = new ApplicationUser
                                {
                                    UserName = "publisher@example.com", 
                                    Email = "publisher@example.com", 
                                    EmailConfirmed = true, 
                                    SecurityStamp = "someStamp1", 
                                    PasswordHash = password
                                };
            var subscriber = new ApplicationUser
                                 {
                                     UserName = "subscriber@example.com", 
                                     Email = "subscriber@example.com", 
                                     EmailConfirmed = true, 
                                     SecurityStamp = "someStamp2", 
                                     PasswordHash = password
                                 };
            context.Users.AddOrUpdate(u => u.UserName, publisher, subscriber);
        }
    }
}
