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
using System.Threading.Tasks;

namespace ESPNFeed.Tests.Logic
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
            Environment.SetEnvironmentVariable("MLB", "someRssFeedUrl");

            _loggerMock = new Mock<ILogger>();

            _feedDataMock = new Mock<IFeedData>();

            _feedDataMock.Setup(fdm => fdm.GetFeedData(It.IsAny<string>(), _loggerMock.Object)).Returns(DataGenerator.GetSyndicationFeed());

            _feedLogic = new FeedLogic(_feedDataMock.Object);
        }

        [TestMethod]
        public void GettingArchiveFeedCallsData()
        {
            _feedLogic.GetArchiveFeed(1, 2, FeedEnum.MLB, _loggerMock.Object);

            _feedDataMock.Verify(fdm => fdm.GetArchiveFeed(1, 1, FeedEnum.MLB, _loggerMock.Object), Times.Once);//skip 1 record

            _feedDataMock.VerifyNoOtherCalls();

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GettingFeedCallsDataMapsSyndicationFeedAndLogsInformation()
        {
            FeedRequest feedRequest = new FeedRequest() 
            { 
                Feed = FeedEnum.MLB, 
                MaxNumberOfResults = 1 
            };

            List<FeedResponse> feedResponses = await _feedLogic.GetFeed(feedRequest, _loggerMock.Object);

            Assert.AreEqual(1, feedResponses.Count);
            Assert.AreEqual("title", feedResponses[0].id);
            Assert.AreEqual("title", feedResponses[0].Title);
            Assert.AreEqual("summary", feedResponses[0].Description);
            Assert.AreEqual(FeedEnum.MLB, feedResponses[0].Feed);

            VerifyLoggerMockLoggedInformation(2);

            _feedDataMock.Verify(fdm => fdm.GetFeedData("someRssFeedUrl", _loggerMock.Object), Times.Once);

            _feedDataMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task GettingFeedWithArchiveCallsDataAndLogsInformation()
        {
            FeedRequest feedRequest = new FeedRequest()
            {
                Feed = FeedEnum.MLB,
                MaxNumberOfResults = 1,
                Archive = true
            };

            List<FeedResponse> feedResponses = await _feedLogic.GetFeed(feedRequest, _loggerMock.Object);

            VerifyLoggerMockLoggedInformation(2);

            _feedDataMock.Verify(fdm => fdm.GetFeedData(It.IsAny<string>(), _loggerMock.Object), Times.Once);

            _feedDataMock.Verify(fdm => fdm.ArchiveFeedData(feedResponses, _loggerMock.Object), Times.Once);

            _feedDataMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void GettingFeedURLIsSuccessfulAndLogsInformation()
        {
            string feedUrl = _feedLogic.GetFeedURL(FeedEnum.MLB, _loggerMock.Object);

            Assert.AreEqual("someRssFeedUrl", feedUrl);

            VerifyLoggerMockLoggedInformation(1);

            _feedDataMock.VerifyNoOtherCalls();
        }

        private void VerifyLoggerMockLoggedInformation(int times)
        {
            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(times));

            _loggerMock.VerifyNoOtherCalls();
        }
    }
}