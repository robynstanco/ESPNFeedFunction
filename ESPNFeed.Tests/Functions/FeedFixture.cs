using ESPNFeed.Enums;
using ESPNFeed.Functions;
using ESPNFeed.Interfaces;
using ESPNFeed.Models.Input;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace ESPNFeed.Tests.Functions
{
    //todo finish unit testing error handling
    [TestClass]
    public class FeedFixture
    {
        private Feed _feed;

        private Mock<IFeedLogic> _feedLogicMock;
        private Mock<ILogger> _loggerMock;
        private Mock<HttpRequest> _httpRequestMock;

        [TestInitialize]
        public void Initialize()
        {
            _feedLogicMock = new Mock<IFeedLogic>();

            _loggerMock = new Mock<ILogger>();

            _httpRequestMock = new Mock<HttpRequest>();

            _feed = new Feed(_feedLogicMock.Object);
        }

        [TestMethod]
        public async Task RunningFeedFunctionCallsLogicAndLogsInformation()
        {
            _httpRequestMock.Setup(http => http.Body).Returns(DataGenerator.GetDefaultFeedBody());

            await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);

            _feedLogicMock.Verify(flm => flm.GetFeed(It.Is<FeedRequest>(
                fr => fr.Archive && fr.MaxNumberOfResults == 10 && fr.Feed == FeedEnum.NBA), 
                _loggerMock.Object), Times.Once);

            _feedLogicMock.VerifyNoOtherCalls();

            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry & deserialization logs

            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task RunningFeedFunctionWithInvalidFeedLogsError()
        {
            _httpRequestMock.Setup(http => http.Body).Returns(DataGenerator.GetInvalidFeedBody());

            IActionResult result = await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);
            BadRequestObjectResult badRequest = result as BadRequestObjectResult;

            Assert.AreEqual("Unable to deserialize the request!", badRequest.Value.ToString());

            VerifyLoggerMockLogged(LogLevel.Information, 1);

            VerifyLoggerMockLogged(LogLevel.Error, 1);

            _loggerMock.VerifyNoOtherCalls();

            _feedLogicMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task RunningFeedFunctionWithMalformedRequestLogsError()
        {
            _httpRequestMock.Setup(http => http.Body).Returns(DataGenerator.GetMalformedFeedBody());

            IActionResult result = await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);
            BadRequestObjectResult badRequest = result as BadRequestObjectResult;

            Assert.AreEqual("Unable to read the malformed request!", badRequest.Value.ToString());

            VerifyLoggerMockLogged(LogLevel.Information, 1);

            VerifyLoggerMockLogged(LogLevel.Error, 1);

            _loggerMock.VerifyNoOtherCalls();

            _feedLogicMock.VerifyNoOtherCalls();
        }

        private void VerifyLoggerMockLogged(LogLevel level, int times) 
        {
            _loggerMock.Verify(l => l.Log(level, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(times));
        }
    }
}