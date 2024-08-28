using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.PriorityExample
{
    public class PriorityCreateExample : IExamplesProvider<PriorityCreateDto>
    {
        public PriorityCreateDto GetExamples()
        {
            return new PriorityCreateDto
            {
                Name = "Priority 1",
                Color = "#73496f",
            };
        }
    }
}