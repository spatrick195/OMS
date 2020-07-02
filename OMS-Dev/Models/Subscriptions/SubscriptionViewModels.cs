using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OMS_Dev.Models.Subscriptions
{
    public class SubscriptionDetailsViewModel
    {
        [Display(Name = "Administrator Email")]
        public string AdminEmail { get; set; }

        [Display(Name = "Current Plan")]
        public string SubscribedPlan { get; set; }

        [Display(Name = "Monthly Price")]
        public string MonthlyPrice { get; set; }

        [Display(Name = "Credit Card")]
        public string CardLastFour { get; set; }

        [Display(Name = "Card Expiration")]
        public DateTime CardExpiration { get; set; }
    }
}