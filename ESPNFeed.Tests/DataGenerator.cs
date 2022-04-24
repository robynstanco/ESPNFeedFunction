using ESPNFeed.Enums;
using ESPNFeed.Models.Outputs;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;

namespace ESPNFeed.Tests
{
    /// <summary>
    /// The Mock Data Generator used for test fixtures.
    /// </summary>
    public class DataGenerator
    {
        public static MemoryStream GetDefaultFeedBody()
        {
            return GetBody("{ \"MaxNumberOfResults\": 10, \"Feed\": \"NBA\", \"Archive\": true }");
        }

        public static List<FeedResponse> GetFeedResponses(FeedEnum feed, int num)
        {
            var responses = new List<FeedResponse>();

            for (int i = 0; i < num; i++)
            {
                responses.Add(new FeedResponse()
                {
                    Description = $"desc{i}",
                    Feed = feed,
                    id = i.ToString(),
                    Link = $"http://{i}.com",
                    Title = $"title{i}"
                });
            }

            return responses;
        }

        public static MemoryStream GetInvalidFeedBody()
        {
            return GetBody("{\"Feed\": \"BAD\" }");
        }

        public static MemoryStream GetMalformedFeedBody()
        {
            return GetBody("{\"Feed\" \"BAD\" }");//malformed json
        }

        public static SyndicationFeed GetSyndicationFeed()
        {
            return new SyndicationFeed()
            {
                Items = new List<SyndicationItem>()
                    {
                        new SyndicationItem()
                        {
                            Title = new TextSyndicationContent("title"),
                            Summary = new TextSyndicationContent("summary")
                        }
                    }
            };
        }

        /// <summary>
        /// Create memory stream with the given body.
        /// </summary>
        /// <param name="body">The body to write to stream.</param>
        /// <returns>The filled memory stream.</returns>
        private static MemoryStream GetBody(string body)
        {
            var defaultBody = new MemoryStream();

            var streamWriter = new StreamWriter(defaultBody);

            streamWriter.Write(body);
            streamWriter.Flush();

            defaultBody.Position = 0;

            return defaultBody;
        }
    }
}