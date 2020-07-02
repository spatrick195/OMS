using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMS_Dev.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int CustomerCount { get; set; }

        public int MonthlyCustomerValue { get; set; }

        public int DisputedChargeCount { get; set; }

        public int AccountAvailableBalance { get; set; }

        public int AccountPendingBalance { get; set; }

        public int TrialCustomerCount { get; set; }

        public int ActiveCustomerCount { get; set; }

        public int PastDueCustomerCount { get; set; }

        public int CanceledCustomerCount { get; set; }

        public int UnpaidCustomerCount { get; set; }
    }
}