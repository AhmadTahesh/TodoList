using TodoList.DTOs;
using TodoList.Requests;

namespace TodoList.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Entities;

public interface ITodoService
{
    Task<IEnumerable<TodoDto>> GetAllAsync(string userId);
    Task<TodoDto> GetByIdAsync(int id, string userId);
    Task<TodoDto> CreateAsync(CreateTodoRequest request, string userId);
    Task UpdateAsync(int id, UpdateTodoRequest request, string userId);
    Task DeleteAsync(int id, string userId);
}