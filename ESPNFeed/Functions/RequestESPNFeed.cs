using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ESPNFeed.Models.Input;
using System.Collections.Generic;
using ESPNFeed.Models.Outputs;
using ESPNFeed.Interfaces;

namespace ESPNFeed.Functions
{
    public class RequestESPNFeed
    {
        IFeedData _feedData;
        public RequestESPNFeed(IFeedData feedData)
        {
            _feedData = feedData;
        }

        [FunctionName("RequestESPNFeed")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "api/RequestESPNFeed")] HttpRequest request,
            ILogger log)
        {
            log.LogInformation("Requesting ESPN feed for sport");

            //Deserialize
            string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            FeedRequest feedRequest = JsonConvert.DeserializeObject<FeedRequest>(requestBody);

            //Logic
            List<FeedResponse> feedResponses = _feedData.GetFeedData(feedRequest);

            return new OkObjectResult(feedResponses);
        }
    }
}
