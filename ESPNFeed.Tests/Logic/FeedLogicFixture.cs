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
            //Arrange
            Environment.SetEnvironmentVariable("MLB", "someRssFeedUrl");

            _loggerMock = new Mock<ILogger>();

            _feedDataMock = new Mock<IFeedData>();

            _feedDataMock.Setup(fdm => fdm.GetFeedData(It.IsAny<string>(), _loggerMock.Object))
                .Returns(DataGenerator.GetSyndicationFeed());

            _feedDataMock.Setup(fdm => fdm.GetArchiveFeed(1, 1, FeedEnum.MLB, _loggerMock.Object))
                .Returns(DataGenerator.GetFeedResponses(FeedEnum.MLB, 1));

            _feedLogic = new FeedLogic(_feedDataMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _feedDataMock.VerifyNoOtherCalls();
            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public void GettingArchiveFeedCallsData()
        {
            //Act
            List<FeedResponse> responses = _feedLogic.GetArchiveFeed(1, 2, FeedEnum.MLB, _loggerMock.Object);

            //Assert & Verify
            Assert.AreEqual(1, responses.Count);
            Assert.AreEqual("desc0", responses[0].Description);
            Assert.AreEqual(FeedEnum.MLB, responses[0].Feed);
            Assert.AreEqual("0", responses[0].id);
            Assert.AreEqual("http://0.com", responses[0].Link);
            Assert.AreEqual("title0", responses[0].Title);

            _feedDataMock.Verify(fdm => fdm.GetArchiveFeed(1, 1, FeedEnum.MLB, _loggerMock.Object), Times.Once);//skip 1 record
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task GettingFeedCallsDataMapsSyndicationFeedAndLogsInformation()
        {
            //Arrange
            FeedRequest feedRequest = new FeedRequest() 
            { 
                Feed = FeedEnum.MLB, 
                MaxNumberOfResults = 1 
            };

            //Act
            List<FeedResponse> feedResponses = await _feedLogic.GetFeed(feedRequest, _loggerMock.Object);

            //Assert & Verify
            Assert.AreEqual(1, feedResponses.Count);
            Assert.AreEqual("title", feedResponses[0].id);
            Assert.AreEqual("title", feedResponses[0].Title);
            Assert.AreEqual("summary", feedResponses[0].Description);
            Assert.AreEqual(FeedEnum.MLB, feedResponses[0].Feed);

            VerifyLoggerMockLoggedInformation(2);

            _feedDataMock.Verify(fdm => fdm.GetFeedData("someRssFeedUrl", _loggerMock.Object), Times.Once);
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task GettingFeedWithArchiveCallsDataAndLogsInformation()
        {
            //Arrange
            FeedRequest feedRequest = new FeedRequest()
            {
                Feed = FeedEnum.MLB,
                MaxNumberOfResults = 1,
                Archive = true
            };

            //Act
            List<FeedResponse> feedResponses = await _feedLogic.GetFeed(feedRequest, _loggerMock.Object);

            //Assert & Verify
            VerifyLoggerMockLoggedInformation(2);

            _feedDataMock.Verify(fdm => fdm.GetFeedData(It.IsAny<string>(), _loggerMock.Object), Times.Once);

            _feedDataMock.Verify(fdm => fdm.ArchiveFeedData(feedResponses, _loggerMock.Object), Times.Once);
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public void GettingFeedURLIsSuccessfulAndLogsInformation()
        {
            //Act
            string feedUrl = _feedLogic.GetFeedURL(FeedEnum.MLB, _loggerMock.Object);

            //Assert & Verify
            Assert.AreEqual("someRssFeedUrl", feedUrl);

            VerifyLoggerMockLoggedInformation(1);
        }

        private void VerifyLoggerMockLoggedInformation(int times)
        {
            _loggerMock.Verify(l => l.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(times));
        }
    }
}