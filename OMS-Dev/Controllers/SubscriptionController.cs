using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using OMS_Dev.Models.Subscription;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plan = OMS_Dev.Models.Subscription.Plan;

namespace OMS_Dev.Controllers
{
    public class SubscriptionController : Controller

    {
        private ApplicationDbContext _dbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
        private UserManager<ApplicationUser> UserManager => new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_dbContext));

        // GET: Subscription
        public ActionResult Index()
        {
            var model = new IndexVm()
            {
                Plans = Plan.Plans
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Purchase(string id)
        {
            var model = new PurchaseVm()
            {

                Plan = Plan.Plans.FirstOrDefault(x => x.PayPalPlanId == id)
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Purchase(PurchaseVm model)
        {
            // find plan
            var plan = Plan.Plans.FirstOrDefault(x => x.PayPalPlanId == model.Plan.PayPalPlanId);
            if (ModelState.IsValid)
            {
                // Since we take an Initial Payment (instant payment), the start date of the recurring payments will be next month.
                var startDate = DateTime.UtcNow.AddMonths(1);
                // Get the PayPal API
                var apiContext = GetApiContext();

                // Get the logged in user.
                var subscription = new Subscription()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    StartDate = startDate,
                    NumberOfEmployees = plan.NumberOfEmployees,
                    PayPalPlanId = plan.PayPalPlanId
                };

                _dbContext.Subscriptions.Add(subscription);
                _dbContext.SaveChanges();

                var agreement = new Agreement()
                {
                    name = plan.Name,
                    description = $"Access to doc-u-ment for a monthly payment - {plan.Description}",
                    start_date = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    create_time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    plan = new PayPal.Api.Plan()
                    {
                        id = plan.PayPalPlanId,
                       
                    },
                    payer = new Payer()
                    {
                        payment_method = "paypal",
                        payer_info = new PayerInfo()
                        {
                            first_name = model.FirstName,
                            last_name = model.LastName, 
                            email = model.Email
                        }
                    },
                    
                };
                // variable that sends the agreement to PayPal
                var createdAgreement = agreement.Create(apiContext);


                subscription.PayPalAgreementToken = createdAgreement.token;
                _dbContext.SaveChanges();

                var approvalUrl = createdAgreement.links.FirstOrDefault(x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));
                return Redirect(approvalUrl.href);
            }
            model.Plan = plan;
            return View(model);
        }

        public ActionResult Return(string token)
        {
            var subscription = _dbContext.Subscriptions.FirstOrDefault(x => x.PayPalAgreementToken == token);
            var apiContext = GetApiContext();

            var agreement = new Agreement()
            {
                token = token
            };

            // Save the PayPal agreement in our subscription so we can look it up later.
            var executedAgreement = agreement.Execute(apiContext);
            _dbContext.SaveChanges();

            return RedirectToAction("ThankYou");
        }

        public ActionResult Cancel()
        {
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        private APIContext GetApiContext()
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }
    }
}