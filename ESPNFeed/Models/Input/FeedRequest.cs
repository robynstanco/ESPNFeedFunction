using ESPNFeed.Enums;

namespace ESPNFeed.Models.Input
{
    public class FeedRequest
    { 
        public FeedEnum Feed { get; set; }
        public int MaxNumberOfResults { get; set; }
    }
}