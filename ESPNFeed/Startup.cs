using ESPNFeed.Data;
using ESPNFeed.Interfaces;
using ESPNFeed.Logic;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(ESPNFeed.Startup))]

namespace ESPNFeed
{
    public class Startup : FunctionsStartup
    {
        public string ESPNFeedDBConnectionString 
        { 
            get
            {
                return Environment.GetEnvironmentVariable(nameof(ESPNFeedDBConnectionString));
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Add dependency injection for data/logic layers
            builder.Services.AddSingleton<IFeedData, FeedData>();
            builder.Services.AddSingleton<IFeedLogic, FeedLogic>();

            //Add DI for CosmosDB instance
            builder.Services.AddSingleton((s) =>
            {
                CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(ESPNFeedDBConnectionString);

                return cosmosClientBuilder.WithConnectionModeDirect().Build();
            });
        }
    }
}