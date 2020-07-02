using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using OMS_Dev.Models.Employees;
using Stripe;

namespace OMS_Dev.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private ApplicationDbContext _dbContext;
        private ApplicationUserManager _userManager;
        private ApplicationEmployeeManager _employeeManager;
        private EmployeeSignInManager _signInManager;
        private RoleManager<IdentityRole> RoleManager;

        public EmployeeController()
        {
            _dbContext = new ApplicationDbContext();
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext));
        }

        public EmployeeController(ApplicationDbContext dbContext, ApplicationEmployeeManager employeeManager, EmployeeSignInManager signInManager, ApplicationUserManager userManager)
        {
            ApplicationDbContext = dbContext;
            EmployeeManager = employeeManager;
            SignInManager = signInManager;
        }

        public ApplicationDbContext ApplicationDbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            set
            {
                _dbContext = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationEmployeeManager EmployeeManager
        {
            get
            {
                return _employeeManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationEmployeeManager>();
            }
            set
            {
                _employeeManager = value;
            }
        }

        public EmployeeSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<EmployeeSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
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

        public ActionResult ManageEmployees()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var count = _dbContext.Employees.Where(x => x.OriginalUser == user.Id);

            var employees = count.ToList().Select(p => new EmployeeListViewModel
            {
                Id = p.Id,
                UserName = p.UserName
            });

            return View(employees);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(BulkRegister employees)
        {
            if (ModelState.IsValid)
            {
                var count = employees.UserToRegister.Count();
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

                var userBusiness = user.Business;

                foreach (var e in employees.UserToRegister)
                {
                    var employee = new Employee
                    {
                        User = user,
                        UserName = e.UserName,
                        OriginalUser = user.Id,
                        RegisteredOn = DateTime.Now,
                        Business = userBusiness
                    };
                    count++;
                    user.UserCount = count;
                    await UserManager.UpdateAsync(user);

                    var result = await EmployeeManager.CreateAsync(employee, e.Password);

                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                    }
                }
                return RedirectToAction("Checkout", "Billing");
            }
            return View(employees);
        }

        [HttpGet]
        public ActionResult Edit(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = EmployeeManager.FindById(Id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, EditEmployeeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var employee = EmployeeManager.FindById(Id);
                    employee.UserName = model.UserName;
                    var password = EmployeeManager.ChangePassword(employee.Id, employee.PasswordHash, model.Password);
                    EmployeeManager.Update(employee);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message + " " + ex.InnerException);
                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ApplicationDbContext != null)
                {
                    ApplicationDbContext.Dispose();
                    ApplicationDbContext = null;
                }

                if (UserManager != null)
                {
                    UserManager.Dispose();
                    UserManager = null;
                }

                if (RoleManager != null)
                {
                    RoleManager.Dispose();
                    RoleManager = null;
                }

                if (EmployeeManager != null)
                {
                    EmployeeManager.Dispose();
                    EmployeeManager = null;
                }

                if (SignInManager != null)
                {
                    SignInManager.Dispose();
                    SignInManager = null;
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