using ESPNFeed.Enums;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace ESPNFeed.Interfaces
{
    public interface IFeedLogic
    {
        List<FeedResponse> GetFeed(FeedRequest feedRequest, ILogger log);
        string GetFeedURL(FeedEnum feed, ILogger log);
        List<FeedResponse> MapSyndicationFeedToFeedResponses(SyndicationFeed feed, int maxResults, ILogger log);
    }
}