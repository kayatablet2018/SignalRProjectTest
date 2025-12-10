using Xunit; 
using Moq; 
using Microsoft.AspNetCore.Mvc; 
using SignalR.BusinessLayer.Abstract; 
using SignalR.DtoLayer.MessageDto; 
using SignalRApi.Controllers; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
 
namespace SignalRApi.Tests.Controllers 
{ 
    public class MessageControllerTests 
    { 
        private readonly Mock<IMessageService> _mockMessageService; 
        private readonly MessageController _controller; 
 
        public MessageControllerTests() 
        { 
            _mockMessageService = new Mock<IMessageService>(); 
            _controller = new MessageController(_mockMessageService.Object); 
        } 
 
        [Fact] 
        public async Task GetMessage_ReturnsOkResult_WithListOfMessages() 
        { 
            // Arrange 
            var messages = new List<ResultMessageDto> 
            { 
                new ResultMessageDto { MessageID = 1, NameSurname = "Test 1", Mail = "test1@test.com" }, 
                new ResultMessageDto { MessageID = 2, NameSurname = "Test 2", Mail = "test2@test.com" } 
            }; 
            _mockMessageService.Setup(s => s.TGetListAll()).ReturnsAsync(messages); 
 
            // Act 
            var result = await _controller.GetMessage(); 
 
            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<List<ResultMessageDto>>(okResult.Value); 
            Assert.Equal(2, returnValue.Count); 
        } 
 
        [Fact] 
        public async Task CreateMessage_ReturnsOkResult_WithCreatedMessage() 
        { 
            // Arrange 
            var createMessageDto = new CreateMessageDto { NameSurname = "New Message", Mail = "new@test.com" }; 
            _mockMessageService.Setup(s => s.TAdd(It.IsAny<CreateMessageDto>())).Returns(Task.CompletedTask); 
 
            // Act 
            var result = await _controller.CreateMessage(createMessageDto); 
 
            // Assert 
            Assert.IsType<OkObjectResult>(result); 
            _mockMessageService.Verify(s => s.TAdd(createMessageDto), Times.Once); 
        } 
 
        [Fact] 
        public async Task UpdateMessage_ReturnsNoContentResult() 
        { 
            // Arrange 
            var updateMessageDto = new UpdateMessageDto { MessageID = 1, NameSurname = "Updated Message", Mail = "updated@test.com" }; 
            _mockMessageService.Setup(s => s.TUpdate(It.IsAny<UpdateMessageDto>())).Returns(Task.CompletedTask); 
 
            // Act 
            var result = await _controller.UpdateMessage(updateMessageDto); 
 
            // Assert 
            Assert.IsType<NoContentResult>(result); 
            _mockMessageService.Verify(s => s.TUpdate(updateMessageDto), Times.Once); 
        } 
 
        [Fact] 
        public async Task DeleteMessage_ReturnsNoContentResult() 
        { 
            // Arrange 
            var messageId = 1; 
            _mockMessageService.Setup(s => s.TDelete(messageId)).Returns(Task.CompletedTask); 
 
            // Act 
            var result = await _controller.DeleteMessage(messageId); 
 
            // Assert 
            Assert.IsType<NoContentResult>(result); 
            _mockMessageService.Verify(s => s.TDelete(messageId), Times.Once); 
        } 
 
        [Fact] 
        public async Task GetMessageById_ReturnsOkResult_WithSingleMessage() 
        { 
            // Arrange 
            var messageId = 1; 
            var message = new ResultMessageDto { MessageID = messageId, NameSurname = "Test 1", Mail = "test1@test.com" }; 
            _mockMessageService.Setup(s => s.TGetByID(messageId)).ReturnsAsync(message); 
 
            // Act 
            var result = await _controller.GetMessage(messageId); 
 
            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<ResultMessageDto>(okResult.Value); 
            Assert.Equal(messageId, returnValue.MessageID); 
        } 
 
        [Fact] 
        public async Task GetTotalMessageCount_ReturnsOkResult_WithMessageCount() 
        { 
            // Arrange 
            var count = 5; 
            _mockMessageService.Setup(s => s.TGetTotalMessageCount()).ReturnsAsync(count); 
 
            // Act 
            var result = await _controller.GetTotalMessageCount(); 
 
            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result); 
            Assert.Equal(count, okResult.Value); 
            _mockMessageService.Verify(s => s.TGetTotalMessageCount(), Times.Once); 
        } 
 
        [Fact] 
        public async Task GetActiveMessageCount_ReturnsOkResult_WithActiveMessageCount() 
        { 
            // Arrange 
            var count = 3; 
            _mockMessageService.Setup(s => s.TGetActiveMessageCount()).ReturnsAsync(count); 
 
            // Act 
            var result = await _controller.GetActiveMessageCount(); 
 
            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result); 
            Assert.Equal(count, okResult.Value); 
            _mockMessageService.Verify(s => s.TGetActiveMessageCount(), Times.Once); 
        } 
 
        [Fact] 
        public async Task GetMessageListByTrue_ReturnsOkResult_WithTrueMessages() 
        { 
            // Arrange 
            var messages = new List<ResultMessageDto> 
            { 
                new ResultMessageDto { MessageID = 1, Status = true }, 
                new ResultMessageDto { MessageID = 2, Status = true } 
            }; 
            _mockMessageService.Setup(s => s.TGetMessageListByTrue()).ReturnsAsync(messages); 
 
            // Act 
            var result = await _controller.GetMessageListByTrue(); 
 
            // Assert 
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<List<ResultMessageDto>>(okResult.Value); 
            Assert.Equal(2, returnValue.Count); 
            Assert.True(returnValue.All(m => m.Status == true)); 
            _mockMessageService.Verify(s => s.TGetMessageListByTrue(), Times.Once); 
        } 
    } 
}