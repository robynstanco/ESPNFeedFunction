using ESPNFeed.Interfaces;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;

namespace ESPNFeed.Logic
{
    public class FeedLogic : IFeedLogic
    {
        private IFeedData _feedData;
        public FeedLogic(IFeedData feedData) 
        {
            _feedData = feedData;
        }

        public List<FeedResponse> GetFeed(FeedRequest feedRequest, ILogger log)
        {
            string feedURL = GetFeedURL(feedRequest, log);

            SyndicationFeed feed = _feedData.GetFeedData(feedURL, log);

            List<FeedResponse> feedResponses = MapSyndicationFeedToFeedResponses(feed, feedRequest.MaxNumberOfResults, log);

            return feedResponses;
        }

        public string GetFeedURL(FeedRequest feedRequest, ILogger log)
        {
            string feedURL = Environment.GetEnvironmentVariable(feedRequest.Feed.ToString());

            log.LogInformation("Grabbed FeedURL: " + feedURL);

            return feedURL;
        }

        public List<FeedResponse> MapSyndicationFeedToFeedResponses(SyndicationFeed feed, int maxResults, ILogger log)
        {
            List<FeedResponse> feedResponses = new List<FeedResponse>();

            foreach (SyndicationItem item in feed.Items.Take(maxResults))
            {
                feedResponses.Add(new FeedResponse()
                {
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    Link = item.Links[0].Uri.AbsoluteUri
                });
            }

            log.LogInformation("Mapped {0} to {1} {2}s.", nameof(SyndicationFeed), feedResponses.Count, nameof(FeedResponse));

            return feedResponses;
        }
    }
}