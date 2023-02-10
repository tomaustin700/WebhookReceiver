using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebHookPD
{
    public static class Function
    {


        [FunctionName("index")]
        public static IActionResult GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "content", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "serverless")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

      
        [FunctionName(nameof(WebHookRecieve))]
        public static async Task<IActionResult> WebHookRecieve(
       [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Webhooks/WebHookRecieve/{unique}")] HttpRequest req, string unique,
       [SignalR(HubName = "serverless")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            await signalRMessages.AddAsync(
             new SignalRMessage
             {
                 Target = unique,
                 Arguments = new[] { await new StreamReader(req.Body).ReadToEndAsync() }
             });

            return new OkResult();
        }

       
    }
}
