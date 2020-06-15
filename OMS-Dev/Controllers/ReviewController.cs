using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OMS_Dev.Controllers
{
    public class ReviewController : Controller
    {
        protected ApplicationDbContext db { get; set; }

        protected UserManager<ApplicationUser> UserManager { get; set; }
        protected UserManager<Employee> EmployeeManager { get; set; }

        public ReviewController()
        {
            db = new ApplicationDbContext();
            EmployeeManager = new UserManager<Employee>(new UserStore<Employee>(db));
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Review
        [Authorize]
        public ActionResult Index()
        {
            var userId = UserManager.FindByIdAsync(User.Identity.GetUserId());

            //ReviewModel rm = new ReviewModel();
            // count number of employees with same user id
            //var employees = rm.employees.Where(i => i.User.Equals(userId)).Count();
            // get business that matches userid
            //var business = rm.businesses.Where(i => i.User.Equals(userId)).FirstOrDefault();

            return View();
        }
    }
}