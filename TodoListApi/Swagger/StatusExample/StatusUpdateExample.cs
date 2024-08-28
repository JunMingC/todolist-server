using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.StatusExample
{
    public class StatusUpdateExample : IExamplesProvider<StatusUpdateDto>
    {
        public StatusUpdateDto GetExamples()
        {
            return new StatusUpdateDto
            {
                Id = 1,
                Name = "Update Task 1",
                Color = "#3baec8",
            };
        }
    }
}