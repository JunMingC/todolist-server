using TodoListApi.Dto;
using System.Net;

namespace TagListTest.Data
{
    public static class TagTestData
    {
        public static IEnumerable<object[]> TagCreate_Data()
        {
            // Valid case
            yield return new object[]
            {
                new TagCreateDto
                {
                    Name = "Create Tag 1",
                    Color = "#8e9148",
                },
                HttpStatusCode.Created
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new TagCreateDto
                {
                    Name = "", // Invalid Name
                    Color = "#8e9148",
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: empty color
            yield return new object[]
            {
                new TagCreateDto
                {
                    Name = "Create Tag 2",
                    Color = "", // Invalid Color
                },
                HttpStatusCode.BadRequest
            };
        }

        public static IEnumerable<object[]> TagUpdate_Data()
        {
            // Valid case: all new
            yield return new object[]
            {
                new TagUpdateDto
                {
                    Id = 1,
                    Name = "Update Tag 1",
                    Color = "#0c9f60",
                },
                HttpStatusCode.OK
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new TagUpdateDto
                {
                    Id = 1,
                    Name = "", // Invalid Name
                    Color = "#8e9148",
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: empty color
            yield return new object[]
            {
                new TagUpdateDto
                {
                    Id = 1,
                    Name = "Update Tag 2",
                    Color = "", // Invalid Color
                },
                HttpStatusCode.BadRequest
            };
        }
    }
}