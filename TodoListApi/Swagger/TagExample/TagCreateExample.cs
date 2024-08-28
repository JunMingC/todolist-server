using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.TagExample
{
    public class TagCreateExample : IExamplesProvider<TagCreateDto>
    {
        public TagCreateDto GetExamples()
        {
            return new TagCreateDto
            {
                Name = "Tag 1",
                Color = "#73496f",
            };
        }
    }
}