using Microsoft.AspNetCore.SignalR;
using Moq;
using SignalR.BusinessLayer.Abstract;
using SignalRApi.Hubs;

namespace SignalRApi.Tests;

public class SignalRHubTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IMoneyCaseService> _mockMoneyCaseService;
    private readonly Mock<IMenuTableService> _mockMenuTableService;
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly SignalRHub _hub;

    public SignalRHubTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _mockProductService = new Mock<IProductService>();
        _mockOrderService = new Mock<IOrderService>();
        _mockMoneyCaseService = new Mock<IMoneyCaseService>();
        _mockMenuTableService = new Mock<IMenuTableService>();
        _mockBookingService = new Mock<IBookingService>();
        _mockNotificationService = new Mock<INotificationService>();

        _hub = new SignalRHub(
            _mockCategoryService.Object,
            _mockProductService.Object,
            _mockOrderService.Object,
            _mockMenuTableService.Object,
            _mockMoneyCaseService.Object,
            _mockBookingService.Object,
            _mockNotificationService.Object
        );
    }

    [Fact]
    public async Task SendMessage_ShouldSendMessageToAllClients()
    {
        // Arrange
        var mockClients = new Mock<IHubCallerClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        _hub.Clients = mockClients.Object;

        // Act
        await _hub.SendMessage("user", "message");

        // Assert
        mockClientProxy.Verify(c => c.SendCoreAsync("ReceiveMessage", It.Is<object[]>(o => o.Length == 2 && (string)o[0] == "user" && (string)o[1] == "message"), default), Times.Once);
    }

    [Fact]
    public async Task OnConnectedAsync_ShouldIncrementClientCount()
    {
        // Arrange
        var initialCount = SignalRHub.clientCount;
        var mockClients = new Mock<IHubCallerClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        _hub.Clients = mockClients.Object;

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        Assert.Equal(initialCount + 1, SignalRHub.clientCount);
        mockClientProxy.Verify(c => c.SendCoreAsync("ReceiveClientCount", It.Is<object[]>(o => o.Length == 1 && (int)o[0] == SignalRHub.clientCount), default), Times.Once);
    }

    [Fact]
    public async Task OnDisconnectedAsync_ShouldDecrementClientCount()
    {
        // Arrange
        var initialCount = SignalRHub.clientCount;
        var mockClients = new Mock<IHubCallerClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
        _hub.Clients = mockClients.Object;

        // Act
        await _hub.OnDisconnectedAsync(null);

        // Assert
        Assert.Equal(initialCount - 1, SignalRHub.clientCount);
        mockClientProxy.Verify(c => c.SendCoreAsync("ReceiveClientCount", It.Is<object[]>(o => o.Length == 1 && (int)o[0] == SignalRHub.clientCount), default), Times.Once);
    }
}
