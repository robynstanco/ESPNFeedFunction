using ESPNFeed.Enums;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace ESPNFeed.Interfaces
{
    /// <summary>
    /// The Feed Logic interface.
    /// </summary>
    public interface IFeedLogic
    {
        /// <summary>
        /// Calculate records to exclude for pagination. Get archived responses from data.
        /// </summary>
        /// <param name="pageSize">The entries per page.</param>
        /// <param name="pageNumber">The current page.</param>
        /// <param name="feed">The feed selection.</param>
        /// <param name="log">The logger instance.</param>
        /// <returns>The paginated archived feed responses.</returns>
        List<FeedResponse> GetArchiveFeed(int pageSize, int pageNumber, FeedEnum feed, ILogger log);

        /// <summary>
        /// Get and map the feed responses based on the given feed request. Archive feeds only if requested.
        /// </summary>
        /// <param name="feedRequest">The feed request.</param>
        /// <param name="log">The logger instance.</param>
        /// <returns>The feed responses for given feed request.</returns>
        Task<List<FeedResponse>> GetFeed(FeedRequest feedRequest, ILogger log);

        /// <summary>
        /// Grab the feed url based on the feed enum from configuration.
        /// </summary>
        /// <param name="feed">The feed enum.</param>
        /// <param name="log">The logger instance.</param>
        /// <returns>The feed url.</returns>
        string GetFeedURL(FeedEnum feed, ILogger log);

        /// <summary>
        /// Map the SyndicationFeed Items to a list of FeedResponses. Only map the maximum requested.
        /// </summary>
        /// <param name="feed">The syndication feed to map.</param>
        /// <param name="feedRequest">The feed request.</param>
        /// <param name="log">The logger instance.</param>
        /// <returns>The mapped feed responses.</returns>
        List<FeedResponse> MapSyndicationFeedToFeedResponses(SyndicationFeed feed, FeedRequest feedRequest, ILogger log);
    }
}