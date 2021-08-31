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
        /// <summary>
        /// CosmosDB Connection String
        /// </summary>
        public string ESPNFeedDBConnectionString 
        { 
            get
            {
                return Environment.GetEnvironmentVariable(nameof(ESPNFeedDBConnectionString));
            }
        }

        /// <summary>
        /// Configure services via the builder.
        /// </summary>
        /// <param name="builder">builder to configure</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Register custom service dependencies
            builder.Services.AddSingleton<IFeedData, FeedData>();
            builder.Services.AddSingleton<IFeedLogic, FeedLogic>();

            //Register dependency for CosmosDB instance
            builder.Services.AddSingleton((s) =>
            {
                CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(ESPNFeedDBConnectionString);

                return cosmosClientBuilder.WithConnectionModeDirect().Build();
            });
        }
    }
}
