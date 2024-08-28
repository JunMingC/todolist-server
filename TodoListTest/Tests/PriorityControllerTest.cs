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
using TodoListTest.Fixtures;
using TodoListApi.Helpers;
using TodoListApi.Models;
using PriorityListTest.Data;

namespace TodoListTest.Controllers
{
    public class PriorityControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public PriorityControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.OK)]
        public async Task GetPriorityById(int priorityId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            PriorityController controller = CreateController(context);

            // Act
            var result = await controller.GetPriorityById(priorityId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Descending)]
        public async Task GetPriorities(Utils.SortOrder sortOrder)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            PriorityController controller = CreateController(context);

            // Act: use controller.GetPriorities to get the Priority items sorted by Id
            var retrievedResult = await controller.GetPriorities(sortOrder);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the PriorityViewModel items
            var retrievedPriority = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedPriority);
            Assert.NotNull(retrievedPriority.Value);
            var retrievedPriorityViewModels = retrievedPriority.Value as List<PriorityViewModel>;
            Assert.NotNull(retrievedPriorityViewModels);

            // Assert: verify that the actual result is sorted by Priority.Id
            if (sortOrder == Utils.SortOrder.Descending)
            {
                retrievedPriorityViewModels.Should().BeInDescendingOrder(priority => priority.Id);
            }
            else
            {
                retrievedPriorityViewModels.Should().BeInAscendingOrder(priority => priority.Id);
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetPrioritiesSortedByName(Utils.SortOrder sortOrder, int? priorityId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            PriorityController controller = CreateController(context);

            // Act: use controller.GetPrioritiesSortedByName to get the Priority items sorted by Priority.Name and filtered by PriorityId
            var retrievedResult = await controller.GetPrioritiesSortedByName(sortOrder, priorityId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the PriorityViewModel items
            var retrievedPriority = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedPriority);
            Assert.NotNull(retrievedPriority.Value);
            var retrievedPriorityViewModels = retrievedPriority.Value as List<PriorityViewModel>;
            Assert.NotNull(retrievedPriorityViewModels);

            // Assert: verify that the actual result is sorted by Priority.Name
            if (sortOrder == Utils.SortOrder.Descending)
            {
                retrievedPriorityViewModels.Should().BeInDescendingOrder(priority => priority.Name);
            }
            else
            {
                retrievedPriorityViewModels.Should().BeInAscendingOrder(priority => priority.Name);
            }

            // Assert: verify that filtering by PriorityId works as expected
            if (priorityId.HasValue)
            {
                var priorityName = await GetPriorityNameByIdAsync(context, priorityId.Value);
                foreach (var priorityViewModel in retrievedPriorityViewModels)
                {
                    priorityViewModel.Name.Should().Be(priorityName);
                }
            }
        }

        [Theory]
        [MemberData(nameof(PriorityTestData.PriorityCreate_Data), MemberType = typeof(PriorityTestData))]
        public async Task CreatePriority(PriorityCreateDto priorityCreateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            PriorityController controller = CreateController(context);

            // Act
            var result = await controller.CreatePriority(priorityCreateDto);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are created correctly
            if (expectedStatusCode == HttpStatusCode.Created)
            {
                var okResult = result as ObjectResult;
                var updatedPriority = okResult?.Value as Priority;
                Assert.NotNull(updatedPriority);

                updatedPriority.Name.Should().Be(priorityCreateDto.Name);
                updatedPriority.Color.Should().Be(priorityCreateDto.Color);
                updatedPriority.CreatedAt.Should().Be(priorityCreateDto.CreatedAt);
            }
        }

        [Theory]
        [MemberData(nameof(PriorityTestData.PriorityUpdate_Data), MemberType = typeof(PriorityTestData))]
        public async Task UpdatePriority(PriorityUpdateDto priorityUpdateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            PriorityController controller = CreateController(context);

            // Act
            var result = await controller.UpdatePriority(priorityUpdateDto);

            // Assert: verify status code matches expected status code
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are updated correctly
            if (expectedStatusCode == HttpStatusCode.OK)
            {
                var okResult = result as ObjectResult;
                var updatedPriority = okResult?.Value as Priority;
                Assert.NotNull(updatedPriority);

                updatedPriority.Id.Should().Be(priorityUpdateDto.Id);
                updatedPriority.Name.Should().Be(priorityUpdateDto.Name);
                updatedPriority.Color.Should().Be(priorityUpdateDto.Color);
                updatedPriority.UpdatedAt.Should().Be(priorityUpdateDto.UpdatedAt);
            }
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.NoContent)]
        public async Task DeletePriorityById(int priorityId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            PriorityController controller = CreateController(context);

            // Act
            var result = await controller.DeletePriorityById(priorityId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        // Helper method to create PriorityController
        private PriorityController CreateController(TodoListContext context)
        {
            // Create the service and validator 
            var service = new PriorityService(context);
            var validator = new PriorityDtoValidator();

            // Inject the service and validator
            PriorityController controller = new PriorityController(service, validator);

            // Return the controller
            return controller;
        }

        // Helper method to get the priority id by its name
        private async Task<string> GetPriorityNameByIdAsync(TodoListContext context, int priorityId)
        {
            var priority = await context.Priorities.FindAsync(priorityId);
            return priority?.Name ?? string.Empty;
        }
    }
}