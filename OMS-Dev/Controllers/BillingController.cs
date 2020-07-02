using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OMS_Dev.Models;
using OMS_Dev.Models.Billing;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JSM;
using Microsoft.Extensions.Options;

namespace OMS_Dev.Controllers
{
    public class BillingController : Controller
    {
        private ApplicationUserManager _userManager;

        public BillingController()
        {
        }

        public BillingController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
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

        // GET: Billing
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Checkout(CreateSubscription model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var createCardOptions = new CardCreateOptions
            {
                Source = new CardCreateNestedOptions
                {
                    Number = model.CardNumber,
                    ExpMonth = (long?)model.ExpMonth,
                    ExpYear = model.ExpYear,
                    Cvc = model.CVC,
                    Name = model.Cardholder,
                    AddressLine1 = user.Address,
                    AddressLine2 = user.Address2,
                    AddressState = user.Province.ToString(),
                    AddressZip = user.Zip,
                    AddressCity = user.City,
                    AddressCountry = "New Zealand",
                }
            };

            var createCustOptions = new CustomerCreateOptions
            {
                Source = createCardOptions.Source,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Name = user.FullName,
            };

            var customerService = new CustomerService();
            var stripeCustomer = await customerService.CreateAsync(createCustOptions);
            var custSubscription = new SubscriptionCreateOptions
            {
                Customer = stripeCustomer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = "price_1Gz6rrEbjRQUnA5zRvdFKuDT",
                        Quantity = user.UserCount
                    },
                },
            };
            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.CreateAsync(custSubscription);
            user.isSubscribed = true;
            user.SubscriptionId = subscription.Id;
            user.StripeCustomerId = stripeCustomer.Id;
            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return View("Success");
            }
            else
            {
                return View("Failed");
            }
        }

        [HttpGet]
        public ActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Failed()
        {
            return View();
        }
    }
}