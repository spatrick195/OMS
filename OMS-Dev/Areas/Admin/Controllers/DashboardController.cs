using OMS_Dev.Areas.Admin.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMS_Dev.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            var customerService = new CustomerService();
            var customers = customerService.List();

            var balanceService = new BalanceService();
            var balance = balanceService.Get();

            var chargeService = new ChargeService();
            var charges = chargeService.List().Where(c => c.Dispute != null);

            var dashboard = new DashboardViewModel
            {
                CustomerCount = customers.Count(),
                AccountAvailableBalance = (int)balance.Available.Sum(b => b.Amount),
                AccountPendingBalance = (int)balance.Pending.Sum(b => b.Amount),
                MonthlyCustomerValue = (int)customers.Sum(c => c.Subscriptions.Data.Sum(s => s.Plan.Amount)),
                DisputedChargeCount = (int)charges.Sum(c => c.Dispute.Amount.GetValueOrDefault()),
                TrialCustomerCount = (int)customers.Count(c => c.Subscriptions.Data.Any(s => s.Status.Equals("trialing"))),
                ActiveCustomerCount = customers.Count(c => c.Subscriptions.Data.Any(s => s.Status.Equals("active"))),
                PastDueCustomerCount = customers.Count(c => c.Subscriptions.Data.Any(s => s.Status.Equals("past_due"))),
                CanceledCustomerCount = customers.Count(c => c.Subscriptions.Data.Any(s => s.Status.Equals("cancelled"))),
                UnpaidCustomerCount = customers.Count(c => c.Subscriptions.Data.Any(s => s.Status.Equals("unpaid")))
            };

            return View(dashboard);
        }
    }
}