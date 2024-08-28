using Swashbuckle.AspNetCore.Filters;
using TodoListApi.Dto;

namespace TodoListApi.Swagger.TagExample
{
    public class TagUpdateExample : IExamplesProvider<TagUpdateDto>
    {
        public TagUpdateDto GetExamples()
        {
            return new TagUpdateDto
            {
                Id = 1,
                Name = "Update Task 1",
                Color = "#3baec8",
            };
        }
    }
}