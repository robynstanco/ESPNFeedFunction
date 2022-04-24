using ESPNFeed.Enums;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace ESPNFeed.Interfaces
{
    /// <summary>
    /// The Feed Data interface.
    /// </summary>
    public interface IFeedData
    {
        /// <summary>
        /// Archive (upsert) the feed responses to the CosmosClient via the Cosmos Container.
        /// </summary>
        /// <param name="feedResponses">The responses to archive.</param>
        /// <param name="log">The logger instance.</param>
        Task ArchiveFeedData(List<FeedResponse> feedResponses, ILogger log);

        /// <summary>
        /// Get the archived feed responses for given feed parameters. Pagination applied to query.
        /// </summary>
        /// <param name="pageSize">The entries per page.</param>
        /// <param name="excludeRecords">The records to exclude from pagination.</param>
        /// <param name="feed">The feed to find.</param>
        /// <param name="log">The logger instance/</param>
        /// <returns>The archived feed responses.</returns>
        List<FeedResponse> GetArchiveFeed(int pageSize, int excludeRecords, FeedEnum feed, ILogger log);

        /// <summary>
        /// Read and load in the RSS feed via the given URL.
        /// </summary>
        /// <param name="feedURL">The Url to read from.</param>
        /// <param name="log">The Logger instance.</param>
        /// <returns>The loaded syndication (RSS) feed.</returns>
        SyndicationFeed GetFeedData(string feedURL, ILogger log);
    }
}