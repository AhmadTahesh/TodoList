namespace TodoList.Tests.Controllers;

public class TodoControllerTests : TestBase
{
    private readonly Mock<ITodoService> _mockTodoService;
    private readonly Mock<ILogger<TodoController>> _mockLogger;
    private readonly TodoController _controller;

    public TodoControllerTests()
    {
        _mockTodoService = new Mock<ITodoService>();
        _mockLogger = new Mock<ILogger<TodoController>>();
        _controller = new TodoController(_mockTodoService.Object, _mockLogger.Object);
        
        var user = CreateTestUser();
        _controller.ControllerContext = CreateControllerContext(user);
    }

    [Fact]
    public async Task GetTodos_ReturnsOkResult_WithTodosList()
    {
        // Arrange
        var expectedTodos = new List<TodoDto>
        {
            new() { Id = 1, Title = "Test Todo", UserId = "test-user-id" }
        };
        _mockTodoService.Setup(s => s.GetAllAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedTodos);

        // Act
        var result = await _controller.GetTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var todos = Assert.IsType<List<TodoDto>>(okResult.Value);
        Assert.Single(todos);
        Assert.Equal(expectedTodos[0].Title, todos[0].Title);
    }

    [Fact]
    public async Task GetTodos_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = CreateControllerContext();

        // Act
        var result = await _controller.GetTodos();

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetTodo_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var expectedTodo = new TodoDto { Id = 1, Title = "Test Todo", UserId = "test-user-id" };
        _mockTodoService.Setup(s => s.GetByIdAsync(1, "test-user-id"))
            .ReturnsAsync(expectedTodo);

        // Act
        var result = await _controller.GetTodo(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var todo = Assert.IsType<TodoDto>(okResult.Value);
        Assert.Equal(expectedTodo.Title, todo.Title);
    }

    [Fact]
    public async Task GetTodo_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockTodoService.Setup(s => s.GetByIdAsync(1, "test-user-id"))
            .ReturnsAsync((TodoDto)null);

        // Act
        var result = await _controller.GetTodo(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task GetTodo_WithDifferentUserId_ReturnsUnauthorized()
    {
        // Arrange
        var anotherUsersTodo = new TodoDto 
        { 
            Id = 1, 
            Title = "Test Todo", 
            UserId = "different-user-id" // Different from "test-user-id"
        };
    
        _mockTodoService.Setup(s => s.GetByIdAsync(1, "test-user-id"))
            .ThrowsAsync(new UnauthorizedAccessException("User is not authorized to access this todo"));

        // Act
        var result = await _controller.GetTodo(1);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CreateTodo_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "New Todo" };
        var createdTodo = new TodoDto { Id = 1, Title = "New Todo", UserId = "test-user-id" };
        _mockTodoService.Setup(s => s.CreateAsync(request, "test-user-id"))
            .ReturnsAsync(createdTodo);

        // Act
        var result = await _controller.CreateTodo(request);

        // Assert
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
        var todo = Assert.IsType<TodoDto>(createdAtResult.Value);
        Assert.Equal(createdTodo.Title, todo.Title);
    }

    [Fact]
    public async Task CreateTodo_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "" };

        // Act
        var result = await _controller.CreateTodo(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateTodo_WithValidRequest_ReturnsNoContent()
    {
        // Arrange
        var request = new UpdateTodoRequest { Title = "Updated Todo" };
        _mockTodoService.Setup(s => s.UpdateAsync(1, request, "test-user-id"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTodo(1, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTodo_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateTodoRequest { Title = "" };

        // Act
        var result = await _controller.UpdateTodo(1, request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateTodo_WithDifferentUserId_ReturnsUnauthorized()
    {
        // Arrange
        var request = new UpdateTodoRequest { Title = "Updated Todo" };
        _mockTodoService.Setup(s => s.UpdateAsync(1, request, "test-user-id"))
            .ThrowsAsync(new UnauthorizedAccessException("User is not authorized to update this todo"));

        // Act
        var result = await _controller.UpdateTodo(1, request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task DeleteTodo_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockTodoService.Setup(s => s.DeleteAsync(1, "test-user-id"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteTodo(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTodo_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _mockTodoService.Setup(s => s.DeleteAsync(1, "test-user-id"))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.DeleteTodo(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }
    
    [Fact]
    public async Task DeleteTodo_WithDifferentUserId_ReturnsUnauthorized()
    {
        // Arrange
        _mockTodoService.Setup(s => s.DeleteAsync(1, "test-user-id"))
            .ThrowsAsync(new UnauthorizedAccessException("User is not authorized to delete this todo"));

        // Act
        var result = await _controller.DeleteTodo(1);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
}