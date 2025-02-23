namespace TodoList.Tests.Controllers;

public class AuthControllerTests : TestBase
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task SignUp_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new SignUpRequest { Email = "test@example.com", Password = "password123" };
        var session = new Session
        {
            AccessToken = "test-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600
        };
        _mockAuthService.Setup(s => s.SignUp(request))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.SignUp(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSession = Assert.IsType<Session>(okResult.Value);
        Assert.Equal(session.AccessToken, returnedSession.AccessToken);
    }

    [Fact]
    public async Task SignUp_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new SignUpRequest { Email = "", Password = "" };

        // Act
        var result = await _controller.SignUp(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task SignUp_WithExistingEmail_ReturnsConflict()
    {
        // Arrange
        var request = new SignUpRequest 
        { 
            Email = "existing@example.com", 
            Password = "password123",
            DisplayName = "Test User",
            PhoneNumber = "1234567890"
        };
        _mockAuthService.Setup(s => s.SignUp(request))
            .ThrowsAsync(new Exception("User already registered"));

        // Act
        var result = await _controller.SignUp(request);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("User already exists", conflictResult.Value);
    }

    [Fact]
    public async Task SignIn_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new SignInRequest { Email = "test@example.com", Password = "password123" };
        var session = new Session
        {
            AccessToken = "test-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600
        };
        _mockAuthService.Setup(s => s.SignIn(request))
            .ReturnsAsync(session);

        // Act
        var result = await _controller.SignIn(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedSession = Assert.IsType<Session>(okResult.Value);
        Assert.Equal(session.AccessToken, returnedSession.AccessToken);
    }

    [Fact]
    public async Task SignIn_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new SignInRequest { Email = "test@example.com", Password = "wrong" };
        _mockAuthService.Setup(s => s.SignIn(request))
            .ThrowsAsync(new Exception("Invalid credentials"));

        // Act
        var result = await _controller.SignIn(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task SignOut_WithValidToken_ReturnsNoContent()
    {
        // Arrange
        var token = CreateTestBearerToken();
        _controller.ControllerContext = CreateControllerContext(CreateTestUser());
        _mockAuthService.Setup(s => s.SignOut(token))
            .ReturnsAsync(true);

        // Set the Authorization header
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] =
            $"Bearer {token}";

        // Act
        var result = await _controller.SignOut();

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task SignOut_WithError_ReturnsInternalServerError()
    {
        // Arrange
        var token = CreateTestBearerToken();
        _controller.ControllerContext = CreateControllerContext(CreateTestUser());
        _mockAuthService.Setup(s => s.SignOut(token))
            .ThrowsAsync(new Exception("SignOut failed"));

        // Set the Authorization header
        _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] =
            $"Bearer {token}";

        // Act
        var result = await _controller.SignOut();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
}