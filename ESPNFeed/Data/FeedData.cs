using ESPNFeed.Interfaces;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

namespace ESPNFeed.Data
{
    public class FeedData : IFeedData
    {
        public List<FeedResponse> GetFeedData(FeedRequest feedRequest)
        {
            string envVarName = feedRequest.Feed.ToString() + "RSSFeedURL";

            string feedURL = Environment.GetEnvironmentVariable(envVarName);

            XmlReader reader = XmlReader.Create(feedURL);
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            reader.Close();

            List<FeedResponse> response = new List<FeedResponse>();

            foreach (SyndicationItem item in feed.Items.Take(feedRequest.MaxNumberOfResults))
            {
                response.Add(new FeedResponse()
                {
                    Title = item.Title.Text,
                    Description = item.Summary.Text,
                    Link = item.Links[0].Uri.AbsoluteUri
                });           
            }

            return response;
        }
    }
}
