using ESPNFeed.Interfaces;
using ESPNFeed.Models.Outputs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace ESPNFeed.Data
{
    public class FeedData : IFeedData
    {
        private string _espnFeedContainerName;
        public string ESPNFeedContainerName
        {
            get
            {
                if (_espnFeedContainerName == null)
                {
                    _espnFeedContainerName = Environment.GetEnvironmentVariable(nameof(ESPNFeedContainerName));
                }

                return _espnFeedContainerName;
            }
        }

        private string _espnFeedDBName;
        public string ESPNFeedDBName
        {
            get
            {
                if(_espnFeedDBName == null)
                {
                    _espnFeedDBName = Environment.GetEnvironmentVariable(nameof(ESPNFeedDBName));
                }

                return _espnFeedDBName;
            }
        }

        private CosmosClient _cosmosClient;
        private Database _cosmosDb;
        private Container _cosmosContainer;
        public FeedData(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;

            _cosmosDb = _cosmosClient.GetDatabase(ESPNFeedDBName);
            _cosmosContainer = _cosmosDb.GetContainer(ESPNFeedContainerName);
        }

        /// <summary>
        /// Archive (upsert) the feed responses to the CosmosClient via the CosmosContainer.
        /// </summary>
        /// <param name="feedResponses">responses to archive</param>
        /// <param name="log">logger instance</param>
        public async Task ArchiveFeedData(List<FeedResponse> feedResponses, ILogger log)
        {
            foreach (FeedResponse feedResponse in feedResponses)
            {
                await _cosmosContainer.UpsertItemAsync(feedResponse);
            }

            log.LogInformation("Archived {0} " + nameof(FeedResponse), feedResponses.Count);
        }

        /// <summary>
        /// Read & load in the RSS feed via the given URL.
        /// </summary>
        /// <param name="feedURL">url to read from</param>
        /// <param name="log">logger instance</param>
        /// <returns>loaded syndication (RSS) feed</returns>
        public SyndicationFeed GetFeedData(string feedURL, ILogger log)
        {
            XmlReader reader = XmlReader.Create(feedURL);

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            reader.Close();

            log.LogInformation("Loaded the " + nameof(SyndicationFeed) + ".");

            return feed;
        }
    }
}