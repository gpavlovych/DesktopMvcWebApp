// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="">
//   
// </copyright>
// <summary>
//   TODO The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using MyApplication.Web.Models;
using Microsoft.AspNet.Identity;

namespace MyApplication.Web.Migrations
{
    /// <summary>TODO The configuration.</summary>
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        /// <summary>Initializes a new instance of the <see cref="Configuration"/> class.</summary>
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
        }

        /// <summary>TODO The seed.</summary>
        /// <param name="context">TODO The context.</param>
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
