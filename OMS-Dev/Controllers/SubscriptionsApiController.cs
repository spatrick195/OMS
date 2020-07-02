using OMS_Dev.Entities;
using OMS_Dev.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace OMS_Dev.Controllers
{
    public class SubscriptionsApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// List subscriptions
        /// </summary>
        /// <returns></returns>
        // GET: api/SubscriptionsApi
        public IQueryable<Subscription> GetSubscriptions()
        {
            return db.Subscriptions;
        }

        /// <summary>
        /// Details of subscription matching the ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/SubscriptionsApi/5
        [ResponseType(typeof(Subscription))]
        public async Task<IHttpActionResult> GetSubscription(string id)
        {
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        /// <summary>
        /// Update Subscription
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSubscription(string id, Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscription.Id)
            {
                return BadRequest();
            }

            db.Entry(subscription).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Create subscription
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        [ResponseType(typeof(Subscription))]
        public async Task<IHttpActionResult> PostSubscription(Subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Subscriptions.Add(subscription);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = subscription.Id }, subscription);
        }

        /// <summary>
        /// Delete the subscription matching the ID
        /// </summary>
        /// <param name="id">Id of the subscription</param>
        /// <returns></returns>
        [ResponseType(typeof(Subscription))]
        public async Task<IHttpActionResult> DeleteSubscription(string id)
        {
            Subscription subscription = await db.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            db.Subscriptions.Remove(subscription);
            await db.SaveChangesAsync();

            return Ok(subscription);
        }

        /// <summary>
        /// Check if the subscription exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the subscription exists, false if it does not</returns>
        private bool SubscriptionExists(string id)
        {
            return db.Subscriptions.Count(e => e.Id == id) > 0;
        }

        /// <summary>
        /// Dispose DB
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}