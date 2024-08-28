using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoListApi.Models;

public partial class Priority
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
