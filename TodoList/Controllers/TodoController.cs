using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Interfaces;
using TodoList.Requests;

namespace TodoList.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var todos = await _todoService.GetAllAsync(userId);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get todos");
            return StatusCode(500, "An error occurred while fetching todos");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodo(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var todo = await _todoService.GetByIdAsync(id, userId);
            if (todo == null) return NotFound();

            return Ok(todo);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get todo");
            return StatusCode(500, "An error occurred while fetching the todo");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo(CreateTodoRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required");

            var todo = await _todoService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create todo");
            return StatusCode(500, "An error occurred while creating the todo");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, UpdateTodoRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required");

            await _todoService.UpdateAsync(id, request, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update todo");
            return StatusCode(500, "An error occurred while updating the todo");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _todoService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete todo");
            return StatusCode(500, "An error occurred while deleting the todo");
        }
    }
}