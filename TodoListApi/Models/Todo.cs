using System;
using System.Collections.Generic;

namespace TodoListApi.Models;

public partial class Todo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? PriorityId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Priority? Priority { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
