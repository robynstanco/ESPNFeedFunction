using ESPNFeed.Enums;

namespace ESPNFeed.Models.Input
{
    public class FeedRequest
    {
        public bool Archive { get; set; }
        public FeedEnum Feed { get; set; }
        public int MaxNumberOfResults { get; set; }
    }
}