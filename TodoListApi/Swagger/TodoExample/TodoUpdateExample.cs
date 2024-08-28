using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.TodoExample
{
    public class TodoUpdateExample : IExamplesProvider<TodoUpdateDto>
    {
        public TodoUpdateDto GetExamples()
        {
            return new TodoUpdateDto
            {
                Id = 1,
                Name = "Update Task 1",
                Description = "This is a update task",
                DueDate = DateTime.UtcNow.AddDays(1),
                PriorityId = 1,
                StatusId = 1,
                TagIds = new List<int> {1}
            };
        }
    }
}