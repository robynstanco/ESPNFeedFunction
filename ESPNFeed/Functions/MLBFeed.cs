using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ESPNFeed.Functions
{
    public static class MLBFeed
    {
        [FunctionName(nameof(MLBFeed))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = nameof(MLBFeed))] HttpRequest request, ILogger logger)
        {
            throw new NotImplementedException();
        }
    }
}