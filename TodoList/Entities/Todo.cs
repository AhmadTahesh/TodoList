using System.Text.Json.Serialization;
using Postgrest.Attributes;
using Postgrest.Models;

namespace TodoList.Entities;

[Table("todos")] 
public class Todo : BaseModel
{
    [PrimaryKey("id")]
    [JsonIgnore]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("user_id")]
    public string UserId { get; set; }
}