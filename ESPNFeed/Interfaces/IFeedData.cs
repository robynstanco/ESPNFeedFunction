using ESPNFeed.Enums;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace ESPNFeed.Interfaces
{
    public interface IFeedData
    {
        /// <summary>
        /// Archive (upsert) the feed responses to the CosmosClient via the Cosmos Container.
        /// </summary>
        /// <param name="feedResponses">responses to archive</param>
        /// <param name="log">logger instance</param>
        Task ArchiveFeedData(List<FeedResponse> feedResponses, ILogger log);

        /// <summary>
        /// Get the archived feed responses for given feed parameters. Pagination applied to query.
        /// </summary>
        /// <param name="pageSize">entries per page</param>
        /// <param name="excludeRecords">records to exclude from pagination</param>
        /// <param name="feed">feed to find</param>
        /// <param name="log">logger instance</param>
        /// <returns>archived feed responses</returns>
        List<FeedResponse> GetArchiveFeed(int pageSize, int excludeRecords, FeedEnum feed, ILogger log);

        /// <summary>
        /// Get the archived feed responses for given feed parameters. Pagination applied to query.
        /// </summary>
        /// <param name="pageSize">entries per page</param>
        /// <param name="excludeRecords">records to exclude from pagination</param>
        /// <param name="feed">feed to find</param>
        /// <param name="log">logger instance</param>
        /// <returns>archived feed responses</returns>
        SyndicationFeed GetFeedData(string feedURL, ILogger log);
    }
}