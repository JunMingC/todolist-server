using TodoListApi.Dto;
using System.Net;

namespace PriorityListTest.Data
{
    public static class PriorityTestData
    {
        public static IEnumerable<object[]> PriorityCreate_Data()
        {
            // Valid case
            yield return new object[]
            {
                new PriorityCreateDto
                {
                    Name = "Create Priority 1",
                    Color = "#8e9148",
                },
                HttpStatusCode.Created
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new PriorityCreateDto
                {
                    Name = "", // Invalid Name
                    Color = "#8e9148",
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: empty color
            yield return new object[]
            {
                new PriorityCreateDto
                {
                    Name = "Create Priority 2",
                    Color = "", // Invalid Color
                },
                HttpStatusCode.BadRequest
            };
        }

        public static IEnumerable<object[]> PriorityUpdate_Data()
        {
            // Valid case: all new
            yield return new object[]
            {
                new PriorityUpdateDto
                {
                    Id = 1,
                    Name = "Update Priority 1",
                    Color = "#0c9f60",
                },
                HttpStatusCode.OK
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new PriorityUpdateDto
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
                new PriorityUpdateDto
                {
                    Id = 1,
                    Name = "Update Priority 2",
                    Color = "", // Invalid Color
                },
                HttpStatusCode.BadRequest
            };
        }
    }
}