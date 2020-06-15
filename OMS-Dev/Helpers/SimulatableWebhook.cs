using Newtonsoft.Json;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OMS_Dev.Helpers
{
    public class SimulatableWebhook : Webhook
    {
        public WebhookEvent SimulateEvent(string eventType, string webHookId)
        {
            var apiContext = GetApiContext();

            if (apiContext.HTTPHeaders == null)
            {
                apiContext.HTTPHeaders = new Dictionary<string, string>();
            }
            apiContext.HTTPHeaders["Content-Type"] = "application/json";
            apiContext.SdkVersion = new SDKVersion();

            string resource = "v1/notifications/simulate-event";

            var data = new
            {
                webhook_id = webHookId,
                event_type = eventType
            };

            var webhookEvent = ConfigureAndExecute<WebhookEvent>(apiContext, HttpMethod.POST, resource, JsonConvert.SerializeObject(data), "", true);

            return webhookEvent;
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