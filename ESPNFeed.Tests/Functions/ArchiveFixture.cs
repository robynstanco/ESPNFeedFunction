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
            _feedLogicMock = new Mock<IFeedLogic>();

            _httpRequestMock = new Mock<HttpRequest>();

            _loggerMock = new Mock<ILogger>();

            _archive = new Archive(_feedLogicMock.Object);
        }

        [TestMethod]
        public void RunningArchiveFunctionCallsLogicAndLogsInformation()
        {
            _httpRequestMock.Setup(http => http.Query["feed"]).Returns("MLB");
            _httpRequestMock.Setup(http => http.Query["pageNumber"]).Returns("2");
            _httpRequestMock.Setup(http => http.Query["pageSize"]).Returns("1");

            _archive.Run(_httpRequestMock.Object, _loggerMock.Object);

            _feedLogicMock.Verify(flm => flm.GetArchiveFeed(1, 2, FeedEnum.MLB, _loggerMock.Object), Times.Once);

            _feedLogicMock.VerifyNoOtherCalls();

            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry and query string parse

            _loggerMock.VerifyNoOtherCalls();
        }

        private void VerifyLoggerMockLogged(LogLevel level, int times)
        {
            _loggerMock.Verify(l => l.Log(level, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(times));
        }
    }
}