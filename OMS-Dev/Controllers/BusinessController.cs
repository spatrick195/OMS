using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OMS_Dev.Controllers
{
    [Authorize]
    public class BusinessController : Controller
    {
        protected ApplicationDbContext db { get; set; }

        protected UserManager<ApplicationUser> UserManager { get; set; }

        public BusinessController()
        {
            db = new ApplicationDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Business
        public ActionResult Index()
        {
            var businesses = db.Businesses.Include(i => i.Industry);
            return View(businesses.ToList());
        }

        // GET: Business/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Business business = db.Businesses.Find(id);
            if (business == null)
            {
                return HttpNotFound();
            }
            return View(business);
        }

        // GET: Business/Create
        public ActionResult Create()
        {
            ViewBag.IndustryId = new SelectList(db.Industries, "Id", "Title");
            return View();
        }

        // POST: Business/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id, Title, Phone, Address, Description, Incorporation, Industry, User")] Business Business)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = UserManager.FindById(User.Identity.GetUserId());
                    ViewBag.IndustryId = new SelectList(db.Industries, "Id", "Title");
                    Business.User = currentUser;
                    Business.User.UserName = currentUser.UserName;
                    currentUser.Business = Business;
                    db.Businesses.Add(Business);
                    db.SaveChanges();
                    return RedirectToAction("Register", "Employee");
                }
                return View();
            }
            catch (Exception)
            {
                return View();
            }
            // TODO: Add insert logic here
        }

        // GET: Business/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Business business = db.Businesses.Find(id);
            if (business == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndustryId = new SelectList(db.Industries, "Id", "Title", business.Industry);
            return View(business);
        }

        // POST: Business/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Business business)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(business).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.IndustryId = new SelectList(db.Industries, "Id", "Title", business.Industry);
                return View(business);
            }
            catch
            {
                return View();
            }
        }

        // GET: Business/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Business business = db.Businesses.Find(id);
            if (business == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndustryId = new SelectList(db.Industries, "Id", "Title", business.Industry);
            return View(business);
        }

        // POST: Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Business business = db.Businesses.Find(id);
            db.Businesses.Remove(business);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // dispose db
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}