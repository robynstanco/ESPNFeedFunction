using ESPNFeed.Enums;

namespace ESPNFeed.Models.Input
{
    /// <summary>
    /// The Feed Request
    /// </summary>
    public class FeedRequest
    {
        /// <summary>
        /// True to archive.
        /// </summary>
        public bool Archive { get; set; }

        /// <summary>
        /// The Feed enumeration.
        /// </summary>
        public FeedEnum Feed { get; set; }

        /// <summary>
        /// The max number of results.
        /// </summary>
        public int MaxNumberOfResults { get; set; }
    }
}