using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OMS_Dev;
using OMS_Dev.Models;
using OMS_Dev.Models.Subscriptions;
using Stripe;

namespace MvcStripeExample.Controllers
{
    public class SubscriptionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

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

        // GET: Subscriptions
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user.SubscriptionId == null || user.SubscriptionId == string.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var subscription = await db.Subscriptions.FindAsync(user.SubscriptionId);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            var customerService = new CustomerService();
            var stripeCustomer = await customerService.GetAsync(subscription.StripeCustomerId);
            var customerSubscriptionList = new SubscriptionListOptions { Customer = stripeCustomer.Id };

            var subscriptionview = new SubscriptionDetailsViewModel
            {
                AdminEmail = stripeCustomer.Email,
                CardExpiration = new DateTime(),
                CardLastFour = "n/a",
                MonthlyPrice = "n/a",
                SubscribedPlan = "n/a"
            };

            var subscriptionService = new SubscriptionService();
            IEnumerable<Subscription> stripeSubscriptions = await subscriptionService.ListAsync(customerSubscriptionList);

            if (stripeSubscriptions.Any())
            {
                subscriptionview.SubscribedPlan = stripeSubscriptions.FirstOrDefault().Plan.Product.Name;
                subscriptionview.MonthlyPrice = stripeSubscriptions.FirstOrDefault().Plan.Amount.ToString();
            }

            var cardService = new CardService();
            IEnumerable<Card> stripeCards = await cardService.ListAsync(subscription.StripeCustomerId);
            if (stripeCards.Any())
            {
                var dateString = string.Format("{1}/1/{0}", stripeCards.FirstOrDefault().ExpYear,
                    stripeCards.FirstOrDefault().ExpMonth);

                subscriptionview.CardExpiration = DateTime.Parse(dateString);
                subscriptionview.CardLastFour = "XXXX XXXX XXXX " + stripeCards.FirstOrDefault().Last4;
            }

            return View(subscriptionview);
        }

        public ActionResult AddCard()
        {
            return View();
        }

        //
        // POST: /Subscription/AddCard
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddCard(string stripeToken)
        {
            return null;
        }
    }
}