﻿namespace TodoList.Requests;

public class CreateTodoRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
}