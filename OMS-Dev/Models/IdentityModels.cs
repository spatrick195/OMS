using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using OMS_Dev.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using static OMS_Dev.Helpers.Enums;

namespace OMS_Dev.Models
{
    // ASP.Net Identity 'Users' Table, add custom user fields here e.g. public string FirstName { get; set; }
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return LastName + "," + FirstName;
            }
        }

        public string Address { get; set; }
        public string Address2 { get; set; }
        public Province Province { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string CardNumber { get; set; }
        public Month ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string CardCvC { get; set; }
        public string CardName { get; set; }
        public int UserCount { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? RegisteredOn { get; set; }
        public Business Business { get; set; }

        public virtual ICollection<Business> Businesses { get; set; }
        public string StripeCustomerId { get; set; }

        public string SubscriptionId { get; set; }
        public bool isSubscribed { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    // The Entity Framework Core DbContext class represents a session with a database and provides an API for communicating with the database
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // Rename ASP.NET Identity tables from the naming convention given to them by asp.net identity
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        }

        // same as doing ApplicationDbContext appContext = new ApplicationDbContext();
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}