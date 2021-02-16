using Microsoft.Extensions.Logging;
using System.ServiceModel.Syndication;

namespace ESPNFeed.Interfaces
{
    public interface IFeedData
    {
        SyndicationFeed GetFeedData(string feedURL, ILogger log);
    }
}