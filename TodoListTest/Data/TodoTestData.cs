using System.Net;
using TodoListApi.Dto;

namespace TodoListTest.Data
{
    public static class TodoTestData
    {
        public static IEnumerable<object[]> TodoCreate_Data()
        {
            // Valid case: with tags
            yield return new object[]
            {
                new TodoCreateDto
                {
                    Name = "Create Task 1",
                    Description = "This is a create task",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    PriorityId = 1,
                    StatusId = 1,
                    TagIds = new List<int>{1,3,6}
                },
                HttpStatusCode.Created
            };

            // Valid case: empty tags
            yield return new object[]
            {
                new TodoCreateDto
                {
                    Name = "Create Task 2",
                    Description = "This is a create task with no tags",
                    DueDate = DateTime.UtcNow.AddDays(2),
                    PriorityId = 2,
                    StatusId = 2,
                    TagIds = new List<int>()
                },
                HttpStatusCode.Created
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new TodoCreateDto
                {
                    Name = "", // Invalid name
                    Description = "This task has an invalid name",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    PriorityId = 1,
                    StatusId = 1,
                    TagIds = new List<int>{1}
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: invalid priority Id
            yield return new object[]
            {
                new TodoCreateDto
                {
                    Name = "Create Task 3",
                    Description = "This task has an invalid priority",
                    DueDate = DateTime.UtcNow.AddDays(3),
                    PriorityId = 0, // Invalid priority Id
                    StatusId = 1,
                    TagIds = new List<int>{1}
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: invalid status Id
            yield return new object[]
            {
                new TodoCreateDto
                {
                    Name = "Create Task 4",
                    Description = "This task has an invalid status",
                    DueDate = DateTime.UtcNow.AddDays(4),
                    PriorityId = 1,
                    StatusId = 0, // Invalid status Id
                    TagIds = new List<int>{1}
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: invalid tag Id
            yield return new object[]
            {
                new TodoCreateDto
                {
                    Name = "Create Task 5",
                    Description = "This task has an invalid tag",
                    DueDate = DateTime.UtcNow.AddDays(5),
                    PriorityId = 1,
                    StatusId = 1,
                    TagIds = new List<int>{0}
                },
                HttpStatusCode.BadRequest
            };
        }

        public static IEnumerable<object[]> TodoUpdate_Data()
        {
            // Valid case: all new
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 1,
                    Name = "Update Task 1",
                    Description = "This is a update task",
                    DueDate = DateTime.UtcNow.AddYears(999),
                    PriorityId = 1,
                    StatusId = 1,
                    TagIds = new List<int>{1, 2, 3, 4}
                },
                HttpStatusCode.OK
            };

            // Valid case: all empty (except required)
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 1,
                    Name = "Update Task 2",
                    Description = null,
                    DueDate = null,
                    PriorityId = null,
                    StatusId = null,
                    TagIds = null
                },
                HttpStatusCode.OK
            };

            // Invalid case: invalid Id
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 0,
                    Name = "Update Task 3",
                    Description = "This task has an invalid id",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    PriorityId = 2,
                    StatusId = 2,
                    TagIds = new List<int>{2}
                },
                HttpStatusCode.NotFound
            };

            // Invalid case: empty name
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 1,
                    Name = "", // Invalid name
                    Description = "This task has an invalid name",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    PriorityId = 1,
                    StatusId = 1,
                    TagIds = new List<int>{1}
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: invalid priority Id
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 1,
                    Name = "Update Task 4",
                    Description = "This task has an invalid priority",
                    DueDate = DateTime.UtcNow.AddDays(3),
                    PriorityId = 0, // Invalid priority Id
                    StatusId = 1,
                    TagIds = new List<int>{1}
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: invalid status Id
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 1,
                    Name = "Update Task 5",
                    Description = "This task has an invalid status",
                    DueDate = DateTime.UtcNow.AddDays(4),
                    PriorityId = 1,
                    StatusId = 0, // Invalid status Id
                    TagIds = new List<int>{1}
                },
                HttpStatusCode.BadRequest
            };

            // Invalid case: invalid tag Id
            yield return new object[]
            {
                new TodoUpdateDto
                {
                    Id = 1,
                    Name = "Update Task 6",
                    Description = "This task has an invalid tag",
                    DueDate = DateTime.UtcNow.AddDays(5),
                    PriorityId = 1,
                    StatusId = 1,
                    TagIds = new List<int>{0}
                },
                HttpStatusCode.BadRequest
            };
        }
    }
}