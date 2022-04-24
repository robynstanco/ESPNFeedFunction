using ESPNFeed.Enums;
using ESPNFeed.Interfaces;
using ESPNFeed.Models.Outputs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace ESPNFeed.Data
{
    /// <summary>
    /// The Feed Data class.
    /// </summary>
    public class FeedData : IFeedData
    {
        /// <summary>
        /// Cosmos DB Container name.
        /// </summary>
        private string ESPNFeedContainerName
        {
            get
            {
                return Environment.GetEnvironmentVariable(nameof(ESPNFeedContainerName));
            }
        }

        /// <summary>
        /// Cosmos DB name.
        /// </summary>
        private string ESPNFeedDBName
        {
            get
            {
                return Environment.GetEnvironmentVariable(nameof(ESPNFeedDBName));
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
        /// Archive (upsert) the feed responses to the CosmosClient via the Cosmos Container.
        /// </summary>
        /// <param name="feedResponses">The responses to archive.</param>
        /// <param name="log">The logger instance.</param>
        public async Task ArchiveFeedData(List<FeedResponse> feedResponses, ILogger log)
        {
            foreach (var feedResponse in feedResponses)
            {
                await _cosmosContainer.UpsertItemAsync(feedResponse);
            }

            log.LogInformation("Archived {0} feed.", feedResponses.Count);
        }

        /// <summary>
        /// Get the archived feed responses for given feed parameters. Pagination applied to query.
        /// </summary>
        /// <param name="pageSize">The entries per page.</param>
        /// <param name="excludeRecords">The records to exclude from pagination.</param>
        /// <param name="feed">The feed to find.</param>
        /// <param name="log">The logger instance/</param>
        /// <returns>The archived feed responses.</returns>
        public List<FeedResponse> GetArchiveFeed(int pageSize, int excludeRecords, FeedEnum feed, ILogger log)
        {
            var archivedFeedResponses = _cosmosContainer.GetItemLinqQueryable<FeedResponse>(true)
                .Where(fr => fr.Feed == feed)
                .Skip(excludeRecords)
                .Take(pageSize).ToList();

            log.LogInformation("Grabbed {0} archived feeds.", archivedFeedResponses.Count);

            return archivedFeedResponses;
        }

        /// <summary>
        /// Read and load in the RSS feed via the given URL.
        /// </summary>
        /// <param name="feedURL">The Url to read from.</param>
        /// <param name="log">The Logger instance.</param>
        /// <returns>The loaded syndication (RSS) feed.</returns>
        public SyndicationFeed GetFeedData(string feedURL, ILogger log)
        {
            var reader = XmlReader.Create(feedURL);

            var feed = SyndicationFeed.Load(reader);

            reader.Close();

            log.LogInformation("Loaded the feed.");

            return feed;
        }
    }
}