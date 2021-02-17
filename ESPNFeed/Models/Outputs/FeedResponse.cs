namespace ESPNFeed.Models.Outputs
{
    public class FeedResponse
    {
        public string Description { get; set; }
        public string id { get; set; } //note: case sensitive
        public string Link { get; set; }
        public string Title { get; set; }
    }
}