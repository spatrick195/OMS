using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static OMS_Dev.Helpers.Enums;

namespace OMS_Dev.Entities
{
    public class Subscription
    {
        [Key]
        public string Id { get; set; }

        public string AdminEmail { get; set; }

        public string StripeCustomerId { get; set; }
        public SubscriptionStatus Status { get; set; }

        public string StatusDetail { get; set; }
    }
}