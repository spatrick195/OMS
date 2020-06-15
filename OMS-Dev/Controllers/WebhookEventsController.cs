using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using OMS_Dev.Helpers;
using OMS_Dev.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OMS_Dev.Controllers
{
    public class WebhookEventsController : Controller
    {
        private ApplicationDbContext db => HttpContext.GetOwinContext().Get<ApplicationDbContext>();

        public ActionResult Index()
        {
            var webhookEvents = db.WebhookEvents.OrderByDescending(x => x.DateReceived).Take(50).ToList();

            return View(webhookEvents);
        }

        [HttpPost]
        public ActionResult SimulateEvent(string eventType)
        {
            var simlulatableWebhook = new SimulatableWebhook();
            var webhookEvent = simlulatableWebhook.SimulateEvent(eventType, "3EN71282VG238364N");

            return RedirectToAction("Index");
        }   

        [HttpPost]
        public ActionResult Receive()
        {
            var apiContext = GetApiContext();

            var postBody = new StreamReader(Request.InputStream).ReadToEnd();
            var webhookEvent = JsonConvert.DeserializeObject<WebhookEvent>(postBody);

            // Ensure this event was genuinely sent by PayPal
            var isValid = WebhookEvent.ValidateReceivedEvent(apiContext, Request.Headers, postBody, "3EN71282VG238364N"); // Last parameter is your Webhook ID

            if (!isValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var localEvent = new Entities.WebhookEvent()
            {
                EventType = webhookEvent.event_type,
                PayPalWebHookEventId = webhookEvent.id,
                ResourceType = webhookEvent.resource_type,
                Summary = webhookEvent.summary,
                ResourceJson = JsonConvert.SerializeObject(webhookEvent.resource),
                DateCreated = DateTime.Parse(webhookEvent.create_time),
                DateReceived = DateTime.UtcNow
            };
            db.WebhookEvents.Add(localEvent);
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK); 
        }

        private APIContext GetApiContext()
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }
    }
}