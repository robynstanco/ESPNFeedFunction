using ESPNFeed.Enums;

namespace ESPNFeed.Models.Outputs
{
    /// <summary>
    /// The Feed Response.
    /// </summary>
    public class FeedResponse
    {
        /// <summary>
        /// The Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Feed enumeration.
        /// </summary>
        public FeedEnum Feed { get; set; }

        /// <summary>
        /// The id. Note: case sensitive from cosmos.
        /// </summary>
        public string id { get; set; } 

        /// <summary>
        /// The Link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// The Title.
        /// </summary>
        public string Title { get; set; }
    }
}