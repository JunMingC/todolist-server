using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.PriorityExample
{
    public class PriorityUpdateExample : IExamplesProvider<PriorityUpdateDto>
    {
        public PriorityUpdateDto GetExamples()
        {
            return new PriorityUpdateDto
            {
                Id = 1,
                Name = "Update Task 1",
                Color = "#3baec8",
            };
        }
    }
}