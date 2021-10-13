using ESPNFeed.Enums;
using ESPNFeed.Functions;
using ESPNFeed.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace ESPNFeed.Tests.Functions
{
    [TestClass]
    public class ArchiveFixture//todo finish error handling tests
    {
        private Archive _archive;

        private Mock<IFeedLogic> _feedLogicMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            _feedLogicMock = new Mock<IFeedLogic>();

            _httpRequestMock = new Mock<HttpRequest>();
            _httpRequestMock.Setup(http => http.Query["feed"]).Returns("MLB");
            _httpRequestMock.Setup(http => http.Query["pageNumber"]).Returns("2");
            _httpRequestMock.Setup(http => http.Query["pageSize"]).Returns("1");

            _loggerMock = new Mock<ILogger>();

            _archive = new Archive(_feedLogicMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            VerifyNoOtherLogicOrLogCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public void RunningArchiveFunctionCallsLogicAndLogsInformation()
        {
            //Arrange
            _httpRequestMock.Setup(http => http.Query.ContainsKey("pageNumber")).Returns(true);
            _httpRequestMock.Setup(http => http.Query.ContainsKey("pageSize")).Returns(true);

            //Act
            _archive.Run(_httpRequestMock.Object, _loggerMock.Object);

            //Assert
            _feedLogicMock.Verify(flm => flm.GetArchiveFeed(1, 2, FeedEnum.MLB, _loggerMock.Object), Times.Once);

            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry and query string parse
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public void RunningArchiveFunctionSetsDefaultPagingIfNotProvided()
        {
            //Act
            _archive.Run(_httpRequestMock.Object, _loggerMock.Object);

            //Assert [1, 10 defaults]
            _feedLogicMock.Verify(flm => flm.GetArchiveFeed(10, 1, FeedEnum.MLB, _loggerMock.Object), Times.Once);

            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry and query string parse
        }

        private void VerifyLoggerMockLogged(LogLevel level, int times)
        {
            _loggerMock.Verify(l => l.Log(level, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(times));
        }

        private void VerifyNoOtherLogicOrLogCalls()
        {
            _feedLogicMock.VerifyNoOtherCalls();
            _loggerMock.VerifyNoOtherCalls();
        }
    }
}