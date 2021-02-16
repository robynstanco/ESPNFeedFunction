using ESPNFeed.Logic;
using ESPNFeed.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ESPNFeed.Data;

[assembly: FunctionsStartup(typeof(ESPNFeed.Startup))]

namespace ESPNFeed
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IFeedLogic, FeedLogic>();
            builder.Services.AddSingleton<IFeedData, FeedData>();
        }
    }
}