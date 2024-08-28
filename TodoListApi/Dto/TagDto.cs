namespace TodoListApi.Dto
{
    public class TagDto
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
    }

    public class TagCreateDto : TagDto
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class TagUpdateDto : TagDto
    {
        public required int Id { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}