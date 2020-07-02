using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OMS_Dev.Entities;
using OMS_Dev.Helpers;
using OMS_Dev.Models;
using Owin;
using Stripe;
using System;

[assembly: OwinStartupAttribute(typeof(OMS_Dev.Startup))]

namespace OMS_Dev
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateDefaultAdmin();
        }

        private async void CreateDefaultAdmin()
        {
            try
            {
                ApplicationDbContext dbContext = new ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));
                var email = userManager.FindByEmail("AdminEmail");

                if (roleManager.RoleExists("Admin").Equals(false) || email.Equals(null))
                {
                    var roles = roleManager.RoleExists("NotSubscribed");

                    if (!roles.Equals(true))
                    {
                        await roleManager.CreateAsync(new IdentityRole("NotSubscribed"));
                    }
                    var options = new CustomerCreateOptions
                    {
                        Name = "AdminStripeName",
                        Email = "AdminStripeEmail",
                        Source = "tok_visa",
                        Phone = "AdminStripePhone"
                    };

                    var custService = new CustomerService();
                    var stripe = await custService.CreateAsync(options);

                    var user = new ApplicationUser
                    {
                        FirstName = "firstname",
                        LastName = "lastname",
                        UserName = "username",
                        Email = "email",
                        EmailConfirmed = true,
                        StripeCustomerId = stripe.Id,
                        Address = "address",
                        Address2 = "addressline2",
                        Zip = "postcode",
                        City = "city",
                        Province = Enums.Province.MWT,
                        CardNumber = "4242424242424242",
                        ExpMonth = Enums.Month.October,
                        ExpYear = 2024,
                        CardCvC = "232",
                        PhoneNumber = "0226547685",
                        PhoneNumberConfirmed = true,
                        RegisteredOn = DateTime.Now,
                    };

                    var result = await userManager.CreateAsync(user, "Password123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user.Id, "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                var result = ex.Message + " " + ex.InnerException;
                // log error in future
            }
        }
    }
}