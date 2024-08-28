namespace TodoListApi.Dto
{
    public class StatusDto
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
    }

    public class StatusCreateDto : StatusDto
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class StatusUpdateDto : StatusDto
    {
        public required int Id { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}