using ESPNFeed.Interfaces;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ESPNFeed.Functions
{
    public class ESPNFeed
    {
        IFeedLogic _feedLogic;
        public ESPNFeed(IFeedLogic feedLogic)
        {
            _feedLogic = feedLogic;
        }

        [FunctionName(nameof(ESPNFeed))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = nameof(ESPNFeed))] HttpRequest request, ILogger log)
        {
            try
            {
                log.LogInformation("Requesting ESPN Feed!");

                //Deserialize request to hard typed FeedRequest
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                FeedRequest feedRequest = JsonConvert.DeserializeObject<FeedRequest>(requestBody);

                if(feedRequest.Feed == 0)
                {
                    throw new ArgumentNullException();
                }

                log.LogInformation("Deserialized " + nameof(FeedRequest) + ".");

                List<FeedResponse> feedResponses = await _feedLogic.GetFeed(feedRequest, log);

                return new OkObjectResult(feedResponses);
            }
            catch(ArgumentNullException argsNullEx)
            {
                log.LogError(argsNullEx, argsNullEx.Message);

                return new BadRequestObjectResult("Please enter a valid Feed!");
            }
            catch(CosmosException cosmosEx)
            {
                log.LogError(cosmosEx, cosmosEx.Message);

                return new BadRequestObjectResult("Unable to archive data: " + cosmosEx.Message);
            }
            catch(JsonReaderException jsonReaderEx)
            {
                log.LogError(jsonReaderEx, jsonReaderEx.Message);

                return new BadRequestObjectResult("Unable to read the malformed request!");
            }
            catch(JsonSerializationException jsonSerializationEx)
            {
                log.LogError(jsonSerializationEx, jsonSerializationEx.Message);

                return new BadRequestObjectResult("Unable to deserialize the request!");
            }
            catch(Exception ex) //unhandled exception, return 400 with message instead of vague 500 server error.
            {
                log.LogError(ex, ex.Message);

                return new BadRequestObjectResult("An unexpected error occured: " + ex.Message);
            }
        }
    }
}