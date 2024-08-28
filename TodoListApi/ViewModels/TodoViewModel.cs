using TodoListApi.Models;

namespace TodoListApi.ViewModels
{
    public class TodoViewModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public PriorityViewModel? Priority { get; set; }
        public StatusViewModel? Status { get; set; }
        public ICollection<TagViewModel>? Tags { get; set; }
    }
}