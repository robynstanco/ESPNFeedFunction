using ESPNFeed.Interfaces;
using Microsoft.Extensions.Logging;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ESPNFeed.Data
{
    public class FeedData : IFeedData
    {
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