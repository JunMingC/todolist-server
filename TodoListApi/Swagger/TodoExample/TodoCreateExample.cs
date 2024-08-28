using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.TodoExample
{
    public class TodoCreateExample : IExamplesProvider<TodoCreateDto>
    {
        public TodoCreateDto GetExamples()
        {
            return new TodoCreateDto
            {
                Name = "Test Task 1",
                Description = "This is a test task",
                DueDate = DateTime.UtcNow.AddDays(1),
                PriorityId = 1,
                StatusId = 1,
                TagIds = new List<int> {1}
            };
        }
    }
}