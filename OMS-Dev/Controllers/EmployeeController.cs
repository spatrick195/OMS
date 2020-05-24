using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace OMS_Dev.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        protected ApplicationDbContext db { get; set; }
        protected UserManager<Employee> EmployeeManager { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public EmployeeController()
        {
            db = new ApplicationDbContext();
            EmployeeManager = new UserManager<Employee>(new UserStore<Employee>(db));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegisterEmployees()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterEmployees(EmployeeListViewModel employees, Business business)
        {
            if (ModelState.IsValid)
            {
                var successful = new List<string>();
                var failed = new List<string>();
                var errors = new List<string>();

                var userId = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                business = userId.Business;

                foreach (var employee in employees.UsersToRegister.ToList())
                {
                    var emp = new Employee { UserName = employee.Email, Email = employee.Email, Business = business, User = userId };
                    var result = await EmployeeManager.CreateAsync(emp, employee.Password);
                    if (!result.Succeeded)
                    {
                        errors.AddRange(result.Errors);
                        break;
                    }
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    // we would only reach here if something unusual occured
                    AddErrors(result);
                }
            }
            // ModelState.IsValid was not true, something failed before it was reached
            return View(employees);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                {
                    UserManager.Dispose();
                    UserManager = null;
                }
                if (EmployeeManager != null)
                {
                    EmployeeManager.Dispose();
                    EmployeeManager = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}