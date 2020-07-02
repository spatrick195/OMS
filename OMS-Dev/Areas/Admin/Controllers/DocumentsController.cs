using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using OMS_Dev.Areas.Admin.Models;
using System.IO;
using System.Drawing;

namespace OMS_Dev.Areas.Admin.Controllers
{
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Documents
        public async Task<ActionResult> Index()
        {
            return View(await db.Documents.ToListAsync());
        }

        // GET: Admin/Documents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Admin/Documents/Create
        public ActionResult Create()
        {
            ViewBag.Industry = new SelectList(db.Industries, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Thumbnail,ModalSelector,Contents,Colour,Country,Created,Updated")] DocumentViewModels model, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(DateTime.Now + " " + Image.FileName);
                    var folderPath = HttpContext.Server.MapPath("~\\Content\\Thumbnails\\");
                    var filePath = Path.Combine(folderPath, fileName);
                    Image.SaveAs(filePath);
                }

                var document = new Document
                {
                    Title = model.Title,
                    Thumbnail = Image.FileName,
                    Colour = model.Colour,
                    Contents = model.Contents,
                    Country = model.Country,
                    ModalSelector = model.ModalSelector,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };

                try
                {
                    db.Documents.Add(document);
                    await db.SaveChangesAsync();
                }
                catch (DBConcurrencyException DbException)
                {
                    ViewBag.ErrorMessage = "The document has not been created. If this continues to persist, please contact the administrator. Technical Details: \r\n" + DbException.Message + "\r\n" + DbException.InnerException;
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Admin/Documents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Admin/Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Thumbnail,Contents,Colour,Country,Created,Updated")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(document).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(document);
        }

        // GET: Admin/Documents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Admin/Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Document document = await db.Documents.FindAsync(id);
            db.Documents.Remove(document);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

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