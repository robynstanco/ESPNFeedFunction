using ESPNFeed.Enums;
using ESPNFeed.Functions;
using ESPNFeed.Interfaces;
using ESPNFeed.Models.Input;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ESPNFeed.Tests.Functions
{
    [TestClass]
    public class FeedFixture
    {
        private Mock<IFeedLogic> _feedLogicMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<ILogger> _loggerMock;

        private Feed _feed;

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            _feedLogicMock = new Mock<IFeedLogic>();

            _httpRequestMock = new Mock<HttpRequest>();
            _httpRequestMock.Setup(http => http.Body).Returns(DataGenerator.GetDefaultFeedBody());

            _loggerMock = new Mock<ILogger>();

            _feed = new Feed(_feedLogicMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Assert
            _feedLogicMock.VerifyNoOtherCalls();
            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        [TestCategory("Happy Path")]
        public async Task RunningFeedFunctionCallsLogicAndLogsInformation()
        {
            //Act
            await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);

            //Assert
            VerifyFeedLogicMockGetFeedWithDefaultRequest();
            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry & deserialization logs
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task RunningFeedFunctionWithInvalidFeedLogsError()
        {
            //Arrange
            _httpRequestMock.Setup(http => http.Body).Returns(DataGenerator.GetInvalidFeedBody());

            //Act
            IActionResult result = await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);
            var badRequest = result as BadRequestObjectResult;

            //Assert
            Assert.AreEqual("Unable to deserialize the request!", badRequest.Value.ToString());

            VerifyLoggerMockLogged(LogLevel.Information, 1);//entry 
            VerifyLoggerMockLogged(LogLevel.Error, 1);//json exception
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task RunningFeedFunctionWithMalformedRequestLogsError()
        {
            //Arrange
            _httpRequestMock.Setup(http => http.Body).Returns(DataGenerator.GetMalformedFeedBody());
            
            //Act
            IActionResult result = await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);
            var badRequest = result as BadRequestObjectResult;

            //Assert
            Assert.AreEqual("Unable to read the malformed request!", badRequest.Value.ToString());

            VerifyLoggerMockLogged(LogLevel.Information, 1);//entry
            VerifyLoggerMockLogged(LogLevel.Error, 1);//json exception
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task RunningFeedFunctionWithCosmosErrorLogsError()
        {
            //Arrange
            _feedLogicMock.Setup(flm => flm.GetFeed(It.IsAny<FeedRequest>(), _loggerMock.Object))
                .ThrowsAsync(new CosmosException("some cosmos exception", HttpStatusCode.BadRequest, 1, "1", 1));

            //Act
            IActionResult result = await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);
            var badRequest = result as BadRequestObjectResult;

            //Assert
            Assert.AreEqual("Unable to archive data: some cosmos exception", badRequest.Value.ToString());

            VerifyFeedLogicMockGetFeedWithDefaultRequest();
            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry & deserialization logs
            VerifyLoggerMockLogged(LogLevel.Error, 1); //cosmos exception
        }

        [TestMethod]
        [TestCategory("Error Handling")]
        public async Task RunningFeedFunctionWithUnexpectedErrorLogsError()
        {
            //Arrange
            _feedLogicMock.Setup(flm => flm.GetFeed(It.IsAny<FeedRequest>(), _loggerMock.Object))
                .ThrowsAsync(new Exception("some unexpected exception"));

            //Act
            IActionResult result = await _feed.Run(_httpRequestMock.Object, _loggerMock.Object);
            var badRequest = result as BadRequestObjectResult;

            //Assert
            Assert.AreEqual("An unexpected error occured: some unexpected exception", badRequest.Value.ToString());

            VerifyFeedLogicMockGetFeedWithDefaultRequest();
            VerifyLoggerMockLogged(LogLevel.Information, 2); //entry & deserialization logs
            VerifyLoggerMockLogged(LogLevel.Error, 1); //unexpected exception
        }

        private void VerifyFeedLogicMockGetFeedWithDefaultRequest()
        {
            _feedLogicMock.Verify(flm => flm.GetFeed(It.Is<FeedRequest>(
                fr => fr.Archive && fr.MaxNumberOfResults == 10 && fr.Feed == FeedEnum.NBA), _loggerMock.Object), Times.Once);
        }

        private void VerifyLoggerMockLogged(LogLevel level, int times)
        {
            _loggerMock.Verify(l => l.Log(level, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(times));
        }
    }
}