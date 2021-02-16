using ESPNFeed.Enums;
using ESPNFeed.Interfaces;
using ESPNFeed.Logic;
using ESPNFeed.Models.Input;
using ESPNFeed.Models.Outputs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace ESPNFeed.Tests
{
    [TestClass]
    public class FeedLogicFixture
    {
        private FeedLogic _feedLogic;

        private Mock<IFeedData> _feedDataMock;
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger>();

            _feedDataMock = new Mock<IFeedData>();

            _feedDataMock.Setup(fdm => fdm.GetFeedData(It.IsAny<string>(), _loggerMock.Object))
                .Returns(new SyndicationFeed()
                { 
                    Items = new List<SyndicationItem>() 
                    { 
                        new SyndicationItem() 
                        { 
                            Title = new TextSyndicationContent("title"),
                            Summary = new TextSyndicationContent("summary")
                        } 
                    } 
                });

            _feedLogic = new FeedLogic(_feedDataMock.Object);
        }

        [TestMethod]
        public void GetFeedCallsDataMapsSyndicationFeedAndLogsInformation()
        {
            FeedRequest feedRequest = new FeedRequest() 
            { 
                Feed = FeedEnum.MLB, 
                MaxNumberOfResults = 1 
            };

            List<FeedResponse> feedResponses = _feedLogic.GetFeed(feedRequest, _loggerMock.Object);

            Assert.AreEqual(1, feedResponses.Count);
            Assert.AreEqual("title", feedResponses[0].Title);
            Assert.AreEqual("summary", feedResponses[0].Description);

            _feedDataMock.Verify(fdm => fdm.GetFeedData(It.IsAny<string>(), _loggerMock.Object), Times.Once);

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));

            _loggerMock.VerifyNoOtherCalls();

            _feedDataMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void GetFeedURLLogsInformation()
        {
            _feedLogic.GetFeedURL(FeedEnum.NBA, _loggerMock.Object);

            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

            _loggerMock.VerifyNoOtherCalls();

            _feedDataMock.VerifyNoOtherCalls();
        }
    }
}