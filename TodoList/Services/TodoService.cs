using TodoList.Requests;

namespace TodoList.Services;

using TodoList.DTOs;
using TodoList.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Entities;

public class TodoService : ITodoService
{
    private readonly ISupabaseService _supabaseService;

    public TodoService(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<IEnumerable<TodoDto>> GetAllAsync(string userId)
    {
        var client = _supabaseService.GetClient();
        var response = await client
            .From<Todo>()
            .Select("id, title, description, is_completed, created_at, user_id")
            .Filter("user_id", Postgrest.Constants.Operator.Equals, userId)
            .Get();

        return response.Models.Select(todo => new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            UserId = todo.UserId
        });
    }

    public async Task<TodoDto> GetByIdAsync(int id, string userId)
    {
        var client = _supabaseService.GetClient();
        var todo = await client.From<Todo>()
            .Select("id, title, description, is_completed, created_at, user_id")
            .Where(t => t.Id == id)
            .Single();

        if (todo == null)
            return null;

        if (todo.UserId != userId)
            throw new UnauthorizedAccessException("User is not authorized to access this todo");

        return new TodoDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            UserId = todo.UserId
        };
    }

    public async Task<TodoDto> CreateAsync(CreateTodoRequest request, string userId)
    {
        var client = _supabaseService.GetClient();
    
        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var response = await client.From<Todo>().Insert(todo);
        var createdTodo = response.Models.FirstOrDefault();

        return new TodoDto
        {
            Id = createdTodo.Id,
            Title = createdTodo.Title,
            Description = createdTodo.Description,
            IsCompleted = createdTodo.IsCompleted,
            CreatedAt = createdTodo.CreatedAt,
            UserId = createdTodo.UserId
        };
    }

    public async Task UpdateAsync(int id, UpdateTodoRequest request, string userId)
    {
        var client = _supabaseService.GetClient();
        
        var existingTodo = await client.From<Todo>()
            .Where(t => t.Id == id)
            .Single();
        
        if (existingTodo == null)
            return;
        
        if (existingTodo.UserId != userId)
            throw new UnauthorizedAccessException("User is not authorized to update this todo");

        var todo = new Todo
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted,
            UserId = userId
        };

        await client.From<Todo>()
            .Where(t => t.Id == id)
            .Update(todo);
    }

    public async Task DeleteAsync(int id, string userId)
    {
        var client = _supabaseService.GetClient();
    
        // Check if todo exists and belongs to user
        var existingTodo = await client.From<Todo>()
            .Where(t => t.Id == id)
            .Single();
        
        if (existingTodo == null)
            return;
        
        if (existingTodo.UserId != userId)
            throw new UnauthorizedAccessException("User is not authorized to delete this todo");

        await client.From<Todo>()
            .Where(t => t.Id == id)
            .Delete();
    }
}