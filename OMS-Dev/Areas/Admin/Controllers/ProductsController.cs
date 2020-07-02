using Microsoft.AspNet.Identity.Owin;
using OMS_Dev.Areas.Admin.Models;
using OMS_Dev.Entities;
using OMS_Dev.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OMS_Dev.Helpers;

namespace OMS_Dev.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/Products
        public ActionResult Index()
        {
            var productService = new ProductService();

            var products = productService.List().Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Active = p.Active,
                Description = p.Description,
                StatementDescriptor = p.StatementDescriptor
            });

            return View(products);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var options = new ProductCreateOptions
                {
                    Name = model.Name,
                    Active = model.Active,
                    Description = model.Description,
                    StatementDescriptor = model.StatementDescriptor
                };
                var productService = new ProductService();
                await productService.CreateAsync(options);
                return View("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) // if there is no id return a 400 http error.
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productService = new ProductService(); // get instance of productservice object from stripe api
            var p = await productService.GetAsync(id); // retrieve the id passed into the paramter
            var product = new ProductDetailViewModel // fill the data from stripe api into our view model
            {
                Id = p.Id,
                Name = p.Name,
                Active = p.Active,
                Description = p.Description,
                StatementDescriptor = p.StatementDescriptor,
                Created = p.Created,
                Updated = p.Updated
            };
            return View(product); // return the view model with the populated data
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productService = new ProductService();
            var p = await productService.GetAsync(id);

            var product = new ProductEditViewModel
            {
                Name = p.Name,
                Active = p.Active,
                Description = p.Description,
                StatementDescriptor = p.StatementDescriptor
            };

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var productService = new ProductService();

                var options = new ProductUpdateOptions
                {
                    Name = model.Name,
                    Active = model.Active,
                    Description = model.Description,
                    StatementDescriptor = model.StatementDescriptor
                };

                try
                {
                    await productService.UpdateAsync(model.Id, options);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
                return RedirectToAction("Details", "Products", new { id = model.Id });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productService = new ProductService();
            var p = await productService.GetAsync(id);
            var product = new ProductDetailViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Active = p.Active,
                Description = p.Description,
                StatementDescriptor = p.StatementDescriptor,
                Created = p.Created,
                Updated = p.Updated
            };
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productService = new ProductService();
            try
            {
                await productService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Delete");
            }
        }
    }
}