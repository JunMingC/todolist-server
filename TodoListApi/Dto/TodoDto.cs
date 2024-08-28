namespace TodoListApi.Dto
{
    public abstract class TodoDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public ICollection<int>? TagIds { get; set; }
    }

    public class TodoCreateDto : TodoDto
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TodoUpdateDto : TodoDto
    {
        public required int Id { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}