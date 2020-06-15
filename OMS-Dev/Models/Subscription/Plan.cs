using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMS_Dev.Models.Subscription
{
    public class Plan
    {

        public string PayPalPlanId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int NumberOfEmployees { get; set; }
        public string Description { get; set; }
        public static List<Plan> Plans => new List<Plan>()
        {
            new Plan()
            {
                Name = "Starter Subscription",
                Price = 30,
                PayPalPlanId = "P-27H34871BG666983A2CPYWUI", // Created in the BillingPlansController.
                NumberOfEmployees = 1,
                Description = "Access to the website for a monthly payment"
            }
        };
    }
}