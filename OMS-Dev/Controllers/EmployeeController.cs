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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OMS_Dev.Entities;
using OMS_Dev.Models;

namespace OMS_Dev.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        protected ApplicationDbContext db { get; set; }
        protected UserManager<Employee> EmployeeManager { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }
        private EmployeeSignInManager _signInManager;

        public EmployeeController()
        {
            db = new ApplicationDbContext();
            EmployeeManager = new UserManager<Employee>(new UserStore<Employee>(db));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public EmployeeController(EmployeeSignInManager signInManager)
        {
            SignInManager = signInManager;
        }

        public EmployeeSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<EmployeeSignInManager>(); }
            set { _signInManager = value; }
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: /Employee/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Employee/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            var employee = await EmployeeManager.FindByNameAsync(model.Email);

            switch (result)
            {
                case SignInStatus.Success:
                    employee.LastLogin = DateTime.Now;
                    return RedirectToLocal(returnUrl);

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(BulkRegister employees, Business business)
        {
            if (ModelState.IsValid)
            {
                var userId = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                business = userId.Business;

                foreach (var e in employees.UserToRegister)
                {
                    var employee = new Employee { UserName = e.Email, Email = e.Email, Business = business, User = userId, RegisteredOn = DateTime.Now };
                    var result = await EmployeeManager.CreateAsync(employee, e.Password);

                    if (!result.Succeeded)
                    {
                        break;
                    }

                    AddErrors(result);
                }
                return RedirectToAction("Index", "Home");
            }
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
                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
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