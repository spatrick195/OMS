using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal;
using PayPal.Api;

namespace OMS_Dev.Controllers
{
    public class BillingPlansController : Controller
    {
        // GET: BillingPlans
        public ActionResult Index() // result for index action view
        {
            var apiContext = GetApiContext(); // get API context 
            var list = Plan.List(apiContext, status: "ACTIVE"); // get active plans 

            // if we dont find any plans we create the plans
            if (list == null || !list.plans.Any())
            {
                SeedBillingPlans(apiContext);
                list = Plan.List(apiContext, status: "ACTIVE");
            }
            return View(list);
        }

        public ActionResult Delete(string id)
        {
            var apiContext = GetApiContext();
            var plan = new Plan() { id = id };
            plan.Delete(apiContext);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            var apiContext = GetApiContext();
            var list = Plan.List(apiContext, status: "ACTIVE");
            foreach (var plan in list.plans)
            {
                var deletePlan = new Plan()
                {
                    id = plan.id
                };
                deletePlan.Delete(apiContext);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create the billing plans
        /// </summary>
        private void SeedBillingPlans(APIContext apiContext)
        {
            var starterPlan = new Plan()
            {
                name = "Starter Subscription",
                description = "Access to the website for a monthly payment",
                type = "FIXED",
                payment_definitions = new List<PaymentDefinition>()
                {
                    new PaymentDefinition()
                    {
                        
                        name = "Starting Out Subscription",
                        type = "REGULAR",
                        frequency = "MONTH",
                        frequency_interval = "1",
                        amount = new Currency()
                        {
                            currency = "NZD",
                            value = "30.00"
                        },
                        cycles = "0"
                    }
                },
                merchant_preferences = new MerchantPreferences()
                {
                    // The initial payment
                    setup_fee = new Currency()
                    {
                        currency = "NZD",
                        value = "0.00"
                    },
                    return_url = Url.Action("Return", "Subscription", null, Request.Url.Scheme),
                    cancel_url = Url.Action("Cancel", "Subscription", null, Request.Url.Scheme)
                }
            };
            // create plans
            starterPlan = Plan.Create(apiContext, starterPlan);
        
            // When plans are created initially, they are in a state called 'CREATED', we need to change this to 'ACTIVE'. Therefore, we do an HTTP Patch request to replace the state of the plans. 
            var updateStatePatchRequest = new PatchRequest()
            {
                new Patch()
                {
                    op = "replace",
                    path = "/",
                    value = new Plan { state = "ACTIVE" }
                }
            };

            // Call the patch request
            starterPlan.Update(apiContext, updateStatePatchRequest);
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