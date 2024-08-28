using TodoListApi.Dto;
using System.Net;

namespace StatusListTest.Data
{
    public static class StatusTestData
    {
        public static IEnumerable<object[]> StatusCreate_Data()
        {
            // Valid case
            yield return new object[]
            {
                new StatusCreateDto
                {
                    Name = "Create Status 1",
                    Color = "#8e9148",
                },
                HttpStatusCode.Created
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new StatusCreateDto
                {
                    Name = "", // Invalid Name
                    Color = "#8e9148",
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: empty color
            yield return new object[]
            {
                new StatusCreateDto
                {
                    Name = "Create Status 2",
                    Color = "", // Invalid Color
                },
                HttpStatusCode.BadRequest
            };
        }

        public static IEnumerable<object[]> StatusUpdate_Data()
        {
            // Valid case: all new
            yield return new object[]
            {
                new StatusUpdateDto
                {
                    Id = 1,
                    Name = "Update Status 1",
                    Color = "#0c9f60",
                },
                HttpStatusCode.OK
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new StatusUpdateDto
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
                new StatusUpdateDto
                {
                    Id = 1,
                    Name = "Update Status 2",
                    Color = "", // Invalid Color
                },
                HttpStatusCode.BadRequest
            };
        }
    }
}