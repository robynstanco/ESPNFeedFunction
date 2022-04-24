using ESPNFeed.Enums;
using ESPNFeed.Interfaces;
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

namespace ESPNFeed.Functions
{
    /// <summary>
    /// The Archive Function.
    /// </summary>
    public class Archive
    {
        IFeedLogic _feedLogic;
        public Archive(IFeedLogic feedLogic)
        {
            _feedLogic = feedLogic;
        }

        /// <summary>
        /// Grab query string parameters and execute logic to get archived feed responses.
        /// </summary>
        /// <param name="request">The generic http request.</param>
        /// <param name="log">The logger instance.</param>
        /// <returns>The archived feed responses.</returns>
        [FunctionName(nameof(Archive))]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = nameof(Archive))] HttpRequest request, ILogger log)
        {
            try
            {
                log.LogInformation("Requesting Archived ESPN Feed!");

                //Parse query string parameters, default paging size if not provided.
                FeedEnum feed =  Enum.Parse<FeedEnum>(request.Query[nameof(feed)]);
                int pageNumber = request.Query.ContainsKey(nameof(pageNumber)) ? int.Parse(request.Query[nameof(pageNumber)]) : 1;
                int pageSize = request.Query.ContainsKey(nameof(pageSize)) ? int.Parse(request.Query[nameof(pageSize)]) : 10;

                if (feed == 0)
                {
                    throw new ArgumentNullException();
                }

                log.LogInformation($"Grabbed query string from {nameof(HttpRequest)}.");

                List<FeedResponse> archivedResponses = _feedLogic.GetArchiveFeed(pageSize, pageNumber, feed, log);

                return new OkObjectResult(archivedResponses);
            }
            catch (ArgumentNullException argsNullEx)
            {
                log.LogError(argsNullEx, argsNullEx.Message);

                return new BadRequestObjectResult("Please enter a valid Feed!");
            }
            catch (CosmosException cosmosEx)
            {
                log.LogError(cosmosEx, cosmosEx.Message);

                return new BadRequestObjectResult($"Unable to get archived data: {cosmosEx.Message}");
            }
            catch (JsonReaderException jsonReaderEx)
            {
                log.LogError(jsonReaderEx, jsonReaderEx.Message);

                return new BadRequestObjectResult("Unable to read the malformed request!");
            }
            catch (JsonSerializationException jsonSerializationEx)
            {
                log.LogError(jsonSerializationEx, jsonSerializationEx.Message);

                return new BadRequestObjectResult("Unable to deserialize the request!");
            }
            catch (Exception ex)//unhandled exception, return 400 with message instead of vague 500 server error.
            {
                log.LogError(ex, ex.Message);

                return new BadRequestObjectResult($"An unexpected error occured: {ex.Message}");
            }
        }
    }
}