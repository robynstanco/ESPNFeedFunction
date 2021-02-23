using ESPNFeed.Enums;
using ESPNFeed.Interfaces;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;

namespace ESPNFeed.Logic
{
    public class FeedLogic : IFeedLogic
    {
        private IFeedData _feedData;
        public FeedLogic(IFeedData feedData) 
        {
            _feedData = feedData;
        }

        public List<FeedResponse> GetArchiveFeed(int pageSize, int pageNumber, FeedEnum feed, ILogger log)
        {
            int excludeRecords = (pageNumber * pageSize) - pageSize;

            List<FeedResponse> archivedResponses = _feedData.GetArchiveFeed(pageSize, excludeRecords, feed, log);

            return archivedResponses;
        }


        /// <summary>
        /// Get & map the feed responses based on the given feed request. 
        /// Archive feeds only if requested.
        /// </summary>
        /// <param name="feedRequest">the feed request</param>
        /// <param name="log">logger instance</param>
        /// <returns></returns>
        public async Task<List<FeedResponse>> GetFeed(FeedRequest feedRequest, ILogger log)
        {
            string feedURL = GetFeedURL(feedRequest.Feed, log);

            SyndicationFeed feed = _feedData.GetFeedData(feedURL, log);

            List<FeedResponse> feedResponses = MapSyndicationFeedToFeedResponses(feed, feedRequest, log);

            if (feedRequest.Archive && feedResponses.Count > 0)
            {
                await _feedData.ArchiveFeedData(feedResponses, log);
            }

            return feedResponses;
        }

        /// <summary>
        /// Grab the feed url based on the feed enum from configuration.
        /// </summary>
        /// <param name="feed">feed enum</param>
        /// <param name="log">logger instance</param>
        /// <returns></returns>
        public string GetFeedURL(FeedEnum feed, ILogger log)
        {
            string feedURL = Environment.GetEnvironmentVariable(feed.ToString());

            log.LogInformation("Grabbed FeedURL: " + feedURL);

            return feedURL;
        }

        /// <summary>
        /// Map the SyndicationFeed Items to a list of FeedResponses. Only map the maximum requested.
        /// </summary>
        /// <param name="feed">syndication feed to map</param>
        /// <param name="maxResults">requested max results to map</param>
        /// <param name="log">logger instance</param>
        /// <returns>mapped feed responses</returns>
        public List<FeedResponse> MapSyndicationFeedToFeedResponses(SyndicationFeed feed, FeedRequest feedRequest, ILogger log)
        {
            List<FeedResponse> feedResponses = new List<FeedResponse>();

            foreach (SyndicationItem item in feed.Items.Take(feedRequest.MaxNumberOfResults))
            {
                feedResponses.Add(new FeedResponse()
                {
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    Link = item.Links.Count == 0 ? string.Empty : item.Links[0].Uri.AbsoluteUri, //only first url needed (often only one entry anyway)
                    id = item.Title.Text,
                    Feed = feedRequest.Feed
                });
            }

            log.LogInformation("Mapped {0} to {1} {2}s.", nameof(SyndicationFeed), feedResponses.Count, nameof(FeedResponse));

            return feedResponses;
        }
    }
}