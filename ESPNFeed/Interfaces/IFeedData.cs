using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESPNFeed.Interfaces
{
    public interface IFeedData
    {
        List<FeedResponse> GetFeedData(FeedRequest feedRequest);
    }
}
