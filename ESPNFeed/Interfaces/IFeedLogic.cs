using ESPNFeed.Enums;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace ESPNFeed.Interfaces
{
    public interface IFeedLogic
    {
        Task<List<FeedResponse>> GetFeed(FeedRequest feedRequest, ILogger log);
        string GetFeedURL(FeedEnum feed, ILogger log);
        List<FeedResponse> MapSyndicationFeedToFeedResponses(SyndicationFeed feed, int maxResults, ILogger log);
    }
}