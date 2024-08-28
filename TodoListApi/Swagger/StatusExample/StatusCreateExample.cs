using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.StatusExample
{
    public class StatusCreateExample : IExamplesProvider<StatusCreateDto>
    {
        public StatusCreateDto GetExamples()
        {
            return new StatusCreateDto
            {
                Name = "Status 1",
                Color = "#73496f",
            };
        }
    }
}