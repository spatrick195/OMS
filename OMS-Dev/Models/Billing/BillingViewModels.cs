using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static OMS_Dev.Helpers.Enums;

namespace OMS_Dev.Models.Billing
{
    public class CreateSubscription
    {
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Display(Name = "Expiry Year")]
        public int ExpYear { get; set; }

        [Display(Name = "Expiry Month")]
        public Month ExpMonth { get; set; }

        [Display(Name = "Security Code")]
        public string CVC { get; set; }

        [Display(Name = "Card Holder")]
        public string Cardholder { get; set; }
    }

    public class RetryInvoice
    {
        [JsonProperty("customerId")]
        public string Customer { get; set; }

        [JsonProperty("paymentMethodId")]
        public string PaymentMethod { get; set; }

        [JsonProperty("invoiceId")]
        public string Invoice { get; set; }
    }

    public class CancelSubscriptionRequest
    {
        [JsonProperty("subscriptionId")]
        public string Subscription { get; set; }
    }
}