using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;

namespace ESPNFeed.Tests
{
    public class DataGenerator
    {
        public static MemoryStream GetDefaultFeedBody()
        {
            return GetBody("{ \"MaxNumberOfResults\": 10, \"Feed\": \"NBA\", \"Archive\": true }");
        }

        public static MemoryStream GetInvalidFeedBody()
        {
            return GetBody("{\"Feed\": \"BAD\" }");
        }

        public static MemoryStream GetMalformedFeedBody()
        {
            return GetBody("{\"Feed\" \"BAD\" }");//bad json
        }

        private static MemoryStream GetBody(string body)
        {
            MemoryStream defaultBody = new MemoryStream();

            StreamWriter streamWriter = new StreamWriter(defaultBody);

            streamWriter.Write(body);
            streamWriter.Flush();

            defaultBody.Position = 0;

            return defaultBody;
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
    }
}