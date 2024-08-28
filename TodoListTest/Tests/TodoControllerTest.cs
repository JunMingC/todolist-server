using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using TodoListApi.Controllers;
using TodoListApi.Data;
using TodoListApi.Dto;
using TodoListApi.Services;
using TodoListApi.Validators;
using TodoListApi.ViewModels;
using TodoListTest.Data;
using TodoListTest.Fixtures;
using TodoListApi.Helpers;
using TodoListApi.Models;
using System.Data;
using Sprache;

namespace TodoListTest.Controllers
{
    public class TodoControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public TodoControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.OK)]
        public async Task GetTodoById(int todoId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act
            var result = await controller.GetTodoById(todoId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Descending)]
        public async Task GetTodos(Utils.SortOrder sortOrder)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodos to get the Todo items sorted by Id
            var retrievedResult = await controller.GetTodos(sortOrder);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Assert: verify that items are sorted by Id
            var itemIdList = retrievedTodoViewModels.Select(todo => todo.Id).ToList();
            if (sortOrder == Utils.SortOrder.Descending)
            {
                itemIdList.Should().BeInDescendingOrder();
            }
            else
            {
                itemIdList.Should().BeInAscendingOrder();
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, Utils.Period.Today)]
        [InlineData(Utils.SortOrder.Ascending, Utils.Period.ThisWeek)]
        [InlineData(Utils.SortOrder.Ascending, Utils.Period.ThisMonth)]
        [InlineData(Utils.SortOrder.Ascending, Utils.Period.ThisYear)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, Utils.Period.Today)]
        [InlineData(Utils.SortOrder.Descending, Utils.Period.ThisWeek)]
        [InlineData(Utils.SortOrder.Descending, Utils.Period.ThisMonth)]
        [InlineData(Utils.SortOrder.Descending, Utils.Period.ThisYear)]
        public async Task GetTodosSortedByDueDate(Utils.SortOrder sortOrder, Utils.Period? period = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodosSortedByDueDate to get the Todo items sorted by DueDate and filtered by period
            var retrievedResult = await controller.GetTodosSortedByDueDate(sortOrder, period);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Assert: verify that items with null DueDate are at the bottom
            var nullIndicatorList = retrievedTodoViewModels.Select(todo => todo.DueDate.HasValue ? 0 : 1).ToList();
            nullIndicatorList.Should().BeInAscendingOrder();

            // Assert: verify that items are sorted by DueDate
            var itemsWithDueDate = retrievedTodoViewModels.Where(todo => todo.DueDate.HasValue).ToList();
            if (sortOrder == Utils.SortOrder.Descending)
            {
                itemsWithDueDate.Should().BeInDescendingOrder(todo => todo.DueDate);
            }
            else
            {
                itemsWithDueDate.Should().BeInAscendingOrder(todo => todo.DueDate);
            }

            // Assert: verify that items with null DueDate are sorted by Todo.Id
            var itemsWithNullDueDate = retrievedTodoViewModels.Where(todo => !todo.DueDate.HasValue).ToList();
            if (itemsWithNullDueDate.Count > 1)
            {
                itemsWithNullDueDate.Should().BeInAscendingOrder(todo => todo.Id);
            }

            // Assert: verify that filtering by Period works as expected
            if (period.HasValue)
            {
                var (startDate, endDate) = GetDateRangeByPeriod(period.Value);

                foreach (var todoViewModel in retrievedTodoViewModels)
                {
                    todoViewModel.DueDate.Should().HaveValue();
                    todoViewModel.DueDate.Should().BeOnOrAfter(startDate).And.BeOnOrBefore(endDate);
                }
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetTodosSortedByPriorityId(Utils.SortOrder sortOrder, int? priorityId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodosSortedByPriorityId to get the Todo items sorted by Priority.Id and filtered by PriorityId
            var retrievedResult = await controller.GetTodosSortedByPriorityId(sortOrder, priorityId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Assert: verify that items with null Priority are at the bottom
            var nullIndicatorList = retrievedTodoViewModels.Select(todo => todo.Priority != null ? 0 : 1).ToList();
            nullIndicatorList.Should().BeInAscendingOrder();

            // Assert: verify that items with Priority are sorted by Priority.Id
            var itemsWithPriority = retrievedTodoViewModels.Where(todo => todo.Priority != null).Select(todo => todo?.Priority?.Id).ToList();
            if (sortOrder == Utils.SortOrder.Descending)
            {
                itemsWithPriority.Should().BeInDescendingOrder();
            }
            else
            {
                itemsWithPriority.Should().BeInAscendingOrder();
            }

            // Assert: verify that items with null Priority.Id are sorted by Todo.Id
            var itemsWithNullPriority = retrievedTodoViewModels.Where(todo => todo.Priority == null).ToList();
            if (itemsWithNullPriority.Count > 1)
            {
                itemsWithNullPriority.Should().BeInAscendingOrder(todo => todo.Id);
            }

            // Assert: verify that filtering by PriorityId works as expected
            if (priorityId.HasValue)
            {
                var priority = await GetPriorityByIdAsync(context, priorityId.Value);
                foreach (var todoViewModel in retrievedTodoViewModels)
                {
                    Assert.NotNull(todoViewModel.Priority);
                    Assert.NotNull(priority);
                    todoViewModel.Priority.Id.Should().Be(priority.Id);
                }
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetTodosSortedByPriorityName(Utils.SortOrder sortOrder, int? priorityId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodosSortedByPriorityName to get the Todo items sorted by Priority.Name and filtered by PriorityId
            var retrievedResult = await controller.GetTodosSortedByPriorityName(sortOrder, priorityId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Assert: verify that items with null Priority are at the bottom
            var nullIndicatorList = retrievedTodoViewModels.Select(todo => todo.Priority != null ? 0 : 1).ToList();
            nullIndicatorList.Should().BeInAscendingOrder();

            // Assert: verify that items with Priority are sorted by Priority.Name
            var itemsWithPriority = retrievedTodoViewModels.Where(todo => todo.Priority != null).Select(todo => todo?.Priority?.Name).ToList();
            if (sortOrder == Utils.SortOrder.Descending)
            {
                itemsWithPriority.Should().BeInDescendingOrder();
            }
            else
            {
                itemsWithPriority.Should().BeInAscendingOrder();
            }

            // Assert: verify that items with null Priority.Name are sorted by Todo.Id
            var itemsWithNullPriority = retrievedTodoViewModels.Where(todo => todo.Priority == null).ToList();
            if (itemsWithNullPriority.Count > 1)
            {
                itemsWithNullPriority.Should().BeInAscendingOrder(todo => todo.Id);
            }

            // Assert: verify that filtering by PriorityId works as expected
            if (priorityId.HasValue)
            {
                var priority = await GetPriorityByIdAsync(context, priorityId.Value);
                foreach (var todoViewModel in retrievedTodoViewModels)
                {
                    Assert.NotNull(todoViewModel.Priority);
                    Assert.NotNull(priority);
                    todoViewModel.Priority.Id.Should().Be(priority.Id);
                }
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetTodosSortedByStatusId(Utils.SortOrder sortOrder, int? statusId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodosSortedByStatusId to get the Todo item sorted by Status.Id and filtered by StatusId
            var retrievedResult = await controller.GetTodosSortedByStatusId(sortOrder, statusId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Assert: verify that items with null Status are at the bottom
            var nullIndicatorList = retrievedTodoViewModels.Select(todo => todo.Status != null ? 0 : 1).ToList();
            nullIndicatorList.Should().BeInAscendingOrder();

            // Assert: verify that items with Status are sorted by Status.Id
            var itemsWithStatus = retrievedTodoViewModels.Where(todo => todo.Status != null).Select(todo => todo?.Status?.Id).ToList();
            if (sortOrder == Utils.SortOrder.Descending)
            {
                itemsWithStatus.Should().BeInDescendingOrder();
            }
            else
            {
                itemsWithStatus.Should().BeInAscendingOrder();
            }

            // Assert: verify that items with null Status.Id are sorted by Todo.Id
            var itemsWithNullStatus = retrievedTodoViewModels.Where(todo => todo.Status == null).ToList();
            if (itemsWithNullStatus.Count > 1)
            {
                itemsWithNullStatus.Should().BeInAscendingOrder(todo => todo.Id);
            }

            // Assert: verify that filtering by StatusId works as expected
            if (statusId.HasValue)
            {
                var status = await GetStatusByIdAsync(context, statusId.Value);
                foreach (var todoViewModel in retrievedTodoViewModels)
                {
                    Assert.NotNull(todoViewModel.Status);
                    Assert.NotNull(status);
                    todoViewModel.Status.Id.Should().Be(status.Id);
                }
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetTodosSortedByStatusName(Utils.SortOrder sortOrder, int? statusId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodosSortedByStatusName to get the Todo item sorted by Status.Name and filtered by StatusId
            var retrievedResult = await controller.GetTodosSortedByStatusName(sortOrder, statusId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Assert: verify that items with null Status are at the bottom
            var nullIndicatorList = retrievedTodoViewModels.Select(todo => todo.Status != null ? 0 : 1).ToList();
            nullIndicatorList.Should().BeInAscendingOrder();

            // Assert: verify that items with Status are sorted by Status.Name
            var itemsWithStatus = retrievedTodoViewModels.Where(todo => todo.Status != null).Select(todo => todo?.Status?.Name).ToList();
            if (sortOrder == Utils.SortOrder.Descending)
            {
                itemsWithStatus.Should().BeInDescendingOrder();
            }
            else
            {
                itemsWithStatus.Should().BeInAscendingOrder();
            }

            // Assert: verify that items with null Status.Name are sorted by Todo.Id
            var itemsWithNullStatus = retrievedTodoViewModels.Where(todo => todo.Status == null).ToList();
            if (itemsWithNullStatus.Count > 1)
            {
                itemsWithNullStatus.Should().BeInAscendingOrder(todo => todo.Id);
            }

            // Assert: verify that filtering by StatusId works as expected
            if (statusId.HasValue)
            {
                var status = await GetStatusByIdAsync(context, statusId.Value);
                foreach (var todoViewModel in retrievedTodoViewModels)
                {
                    Assert.NotNull(todoViewModel.Status);
                    Assert.NotNull(status);
                    todoViewModel.Status.Id.Should().Be(status.Id);
                }
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetTodosSortedByTagsCount(Utils.SortOrder sortOrder, int? tagId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TodoController controller = CreateController(context);

            // Act: use controller.GetTodosSortedByTagsCount to get the Todo item sorted by the number of Tags and filtered by TagId
            var retrievedResult = await controller.GetTodosSortedByTagsCount(sortOrder, tagId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the ToDoViewModel items
            var retrievedTodo = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTodo);
            Assert.NotNull(retrievedTodo.Value);
            var retrievedTodoViewModels = retrievedTodo.Value as List<TodoViewModel>;
            Assert.NotNull(retrievedTodoViewModels);

            // Act: manually sort the result based on the TagNames count or Name (for null TagNames)
            List<TodoViewModel> expectedSortedTodos;
            if (sortOrder == Utils.SortOrder.Descending)
            {
                expectedSortedTodos = retrievedTodoViewModels
                    .OrderBy(todo => todo?.Tags?.Count == 0 ? 1 : 0)
                    .ThenByDescending(todo => todo.Tags?.Count ?? 0)
                    .ThenBy(todo => todo.Name)
                    .ToList();
            }
            else
            {
                expectedSortedTodos = retrievedTodoViewModels
                    .OrderBy(todo => todo?.Tags?.Count == 0 ? 1 : 0)
                    .ThenBy(todo => todo.Tags?.Count ?? 0)
                    .ThenBy(todo => todo.Name)
                    .ToList();
            }

            // Assert: verify that the retrieved results are correctly sorted
            retrievedTodoViewModels.Should().Equal(expectedSortedTodos);

            // Assert: verify that filtering by TagId works as expected
            if (tagId.HasValue)
            {
                var tag = await GetTagByIdAsync(context, tagId.Value);
                foreach (var todoViewModel in retrievedTodoViewModels)
                {
                    Assert.NotNull(todoViewModel.Tags);
                    Assert.NotNull(tag);
                    var tagIds = todoViewModel.Tags.Select(todo => todo.Id).ToList();
                    tagIds.Should().Contain(tag.Id);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TodoTestData.TodoCreate_Data), MemberType = typeof(TodoTestData))]
        public async Task CreateTodo(TodoCreateDto todoCreateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            TodoController controller = CreateController(context);

            // Act
            var result = await controller.CreateTodo(todoCreateDto);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are created correctly
            if (expectedStatusCode == HttpStatusCode.Created)
            {
                var okResult = result as ObjectResult;
                var updatedTodo = okResult?.Value as Todo;
                Assert.NotNull(updatedTodo);

                updatedTodo.Name.Should().Be(todoCreateDto.Name);
                updatedTodo.Description.Should().Be(todoCreateDto.Description);
                updatedTodo.DueDate.Should().Be(todoCreateDto.DueDate);
                updatedTodo.CreatedAt.Should().Be(todoCreateDto.CreatedAt);
                updatedTodo.PriorityId.Should().Be(todoCreateDto.PriorityId);
                updatedTodo.Priority?.Id.Should().Be(todoCreateDto.PriorityId);
                updatedTodo.StatusId.Should().Be(todoCreateDto.StatusId);
                updatedTodo.Status?.Id.Should().Be(todoCreateDto.StatusId);
                updatedTodo.Tags.Count.Should().Be(todoCreateDto?.TagIds?.Count ?? 0);

                // Assert: verify tags content
                var updatedTagIds = updatedTodo.Tags.Select(todo => todo.Id).ToList();
                updatedTagIds.Should().BeEquivalentTo(todoCreateDto?.TagIds ?? new List<int>());
            }
        }

        [Theory]
        [MemberData(nameof(TodoTestData.TodoUpdate_Data), MemberType = typeof(TodoTestData))]
        public async Task UpdateTodo(TodoUpdateDto todoUpdateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            TodoController controller = CreateController(context);

            // Act
            var result = await controller.UpdateTodo(todoUpdateDto);

            // Assert: verify status code matches expected status code
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are updated correctly
            if (expectedStatusCode == HttpStatusCode.OK)
            {
                var okResult = result as ObjectResult;
                var updatedTodo = okResult?.Value as Todo;
                Assert.NotNull(updatedTodo);

                updatedTodo.Id.Should().Be(todoUpdateDto.Id);
                updatedTodo.Name.Should().Be(todoUpdateDto.Name);
                updatedTodo.Description.Should().Be(todoUpdateDto.Description);
                updatedTodo.DueDate.Should().Be(todoUpdateDto.DueDate);
                updatedTodo.UpdatedAt.Should().Be(todoUpdateDto.UpdatedAt);
                updatedTodo.PriorityId.Should().Be(todoUpdateDto.PriorityId);
                updatedTodo.Priority?.Id.Should().Be(todoUpdateDto.PriorityId);
                updatedTodo.StatusId.Should().Be(todoUpdateDto.StatusId);
                updatedTodo.Status?.Id.Should().Be(todoUpdateDto.StatusId);
                updatedTodo.Tags.Count.Should().Be(todoUpdateDto?.TagIds?.Count ?? 0);

                // Assert: verify tags content
                var updatedTagIds = updatedTodo.Tags.Select(todo => todo.Id).ToList();
                updatedTagIds.Should().BeEquivalentTo(todoUpdateDto?.TagIds ?? new List<int>());
            }
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.NoContent)]
        public async Task DeleteTodoById(int todoId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            TodoController controller = CreateController(context);

            // Act
            var result = await controller.DeleteTodoById(todoId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        // Helper method to create TodoController
        private TodoController CreateController(TodoListContext context)
        {
            // Create the service and validator 
            var service = new TodoService(context);
            var validator = new TodoDtoValidator(context);

            // Inject the service and validator
            TodoController controller = new TodoController(service, validator);

            // Return the controller
            return controller;
        }

        // Helper method to get the date range by its Period
        private (DateTime startDate, DateTime endDate) GetDateRangeByPeriod(Utils.Period period)
        {
            var now = DateTime.UtcNow;

            DateTime startDate = now;
            DateTime endDate = now;

            if (period == Utils.Period.Today)
            {
                startDate = now.Date;
                endDate = startDate.AddDays(1).AddTicks(-1);
            }
            else if (period == Utils.Period.ThisWeek)
            {
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
                var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
                startDate = startOfWeek;
                endDate = endOfWeek;
            }
            else if (period == Utils.Period.ThisMonth)
            {
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
                startDate = startOfMonth;
                endDate = endOfMonth;
            }
            else if (period == Utils.Period.ThisYear)
            {
                var startOfYear = new DateTime(now.Year, 1, 1);
                var endOfYear = startOfYear.AddYears(1).AddTicks(-1);
                startDate = startOfYear;
                endDate = endOfYear;
            }

            return (startDate, endDate);
        }

        // Helper method to get the priority id by its name
        private async Task<Priority?> GetPriorityByIdAsync(TodoListContext context, int priorityId)
        {
            return await context.Priorities.FindAsync(priorityId);
        }

        // Helper method to get the status id by its name
        private async Task<Status?> GetStatusByIdAsync(TodoListContext context, int statusId)
        {
            return await context.Statuses.FindAsync(statusId);
        }

        // Helper method to get the tag name by its Id
        private async Task<Tag?> GetTagByIdAsync(TodoListContext context, int tagId)
        {
            return await context.Tags.FindAsync(tagId);
        }
    }
}