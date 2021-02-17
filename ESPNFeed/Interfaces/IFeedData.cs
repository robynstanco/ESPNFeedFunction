using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace ESPNFeed.Interfaces
{
    public interface IFeedData
    {
        Task ArchiveFeedData(List<FeedResponse> feedResponses, ILogger log);
        SyndicationFeed GetFeedData(string feedURL, ILogger log);
    }
}