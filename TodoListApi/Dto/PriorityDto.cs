namespace TodoListApi.Dto
{
    public class PriorityDto
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
    }

    public class PriorityCreateDto : PriorityDto
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PriorityUpdateDto : PriorityDto
    {
        public required int Id { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}