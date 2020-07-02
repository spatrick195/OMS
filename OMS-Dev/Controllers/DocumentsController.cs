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
using OMS_Dev.Helpers;
using IronPdf;

namespace OMS_Dev.Controllers
{
    [Authorize(Roles = "Admin, Subscribed, Employee")]
    [BlackList(Roles = "NotSubscribed")]
    public class DocumentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Documents
        public async Task<ActionResult> Index()
        {
            return View(await db.Documents.ToListAsync());
        }

        public FileResult DownloadPdf(int? id)
        {
            Document document = db.Documents.Find(id);

            // bool checkImg = document.Body.Contains("<img id='UserLogo'>");
            // todo: check index position of image
            // replace img src with users logo
            // change font of IronPdf

            //if (checkImg)
            //{
            //    document.Body.Insert(checkImg, "<img id='UserLogo'>")
            //}

            document.Contents = document.Contents.ToString().Replace("BusinessName", User.Identity.FirstName().ToString());

            //Create a PDF Document
            HtmlToPdf htmlToPdf = new HtmlToPdf();
            htmlToPdf.PrintOptions.EnableJavaScript = true;
            htmlToPdf.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
            htmlToPdf.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Portrait;
            var PDF = htmlToPdf.RenderHtmlAsPdf(document.Contents);

            //return a pdf document from a view
            var contentLength = PDF.BinaryData.Length;
            Response.AppendHeader("Content-Length", contentLength.ToString());
            Response.AppendHeader("Content-Disposition", "inline; filename=" + document.Title + " " + DateTime.Now + ".pdf");

            return File(PDF.BinaryData, "application/pdf;");
        }

        // GET: Documents/Details/5
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