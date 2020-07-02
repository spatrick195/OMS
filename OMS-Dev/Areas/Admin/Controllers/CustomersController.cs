using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OMS_Dev.Areas.Admin.Models;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OMS_Dev.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext _dbContext;
        private ApplicationUserManager _userManager;

        public CustomersController(ApplicationDbContext dbContext, ApplicationUserManager userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public CustomersController()
        {

        }
        public ApplicationDbContext ApplicationDbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Customers
        public ActionResult Index()
        {
            var customerService = new CustomerService();
            var customers = customerService.List().Select(c => new CustomerViewModels
            {
                Id = c.Id,
                FirstName = UserManager.FindByName(c.Email).FirstName,
                LastName = UserManager.FindByName(c.Email).LastName,
                PhoneNumber = UserManager.FindByName(c.Email).PhoneNumber,
                Email = c.Email,
                AccountBalance = (int)c.Balance,
                Created = c.Created,
                Deleted = c.Deleted,
                Delinquent = c.Delinquent,
            });

            return View(customers);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = new CustomerCreateOptions
                {
                    Email = model.Email,
                    Description = model.Description
                };

                var customerService = new CustomerService();
                var stripeCustomer = await customerService.CreateAsync(customer);

                var subscription = new Entities.Subscription
                {
                    AdminEmail = model.Email,
                    Status = Helpers.Enums.SubscriptionStatus.TrialWithoutCard,
                    StatusDetail = "Admin created with no card",
                    StripeCustomerId = stripeCustomer.Id
                };

                _dbContext.Subscriptions.Add(subscription);
                await _dbContext.SaveChangesAsync();

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Address2 = model.Address2,
                    PhoneNumber = model.PhoneNumber,
                    City = model.City,
                    Province = model.Province,
                    Zip = model.Zip,
                    SubscriptionId = subscription.Id
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Ban(string id)
        {
            return View();
        }

        public async Task<ActionResult> BanUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            user.LockoutEnabled = true;
            user.LockoutEndDateUtc = DateTime.Now.AddYears(9999);
            await UserManager.UpdateAsync(user);
            return RedirectToAction("Details");
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            var model = new CustomerDeleteViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeletePermanently(string id)
        {
            var userId = await UserManager.FindByIdAsync(id);
            await UserManager.DeleteAsync(userId);
            return RedirectToAction("Index");
        }
    }
}