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
using StatusListTest.Data;

namespace TodoListTest.Controllers
{
    public class StatusControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public StatusControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.OK)]
        public async Task GetStatusById(int statusId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            StatusController controller = CreateController(context);

            // Act
            var result = await controller.GetStatusById(statusId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Descending)]
        public async Task GetStatus(Utils.SortOrder sortOrder)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            StatusController controller = CreateController(context);

            // Act: use controller.GetStatuses to get the Status items sorted by Id
            var retrievedResult = await controller.GetStatuses(sortOrder);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the StatusViewModel items
            var retrievedStatus = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedStatus);
            Assert.NotNull(retrievedStatus.Value);
            var retrievedStatusViewModels = retrievedStatus.Value as List<StatusViewModel>;
            Assert.NotNull(retrievedStatusViewModels);

            // Assert: verify that the actual result is sorted by Status.Id
            if (sortOrder == Utils.SortOrder.Descending)
            {
                retrievedStatusViewModels.Should().BeInDescendingOrder(status => status.Id);
            }
            else
            {
                retrievedStatusViewModels.Should().BeInAscendingOrder(status => status.Id);
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetStatusesSortedByName(Utils.SortOrder sortOrder, int? statusId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            StatusController controller = CreateController(context);

            // Act: use controller.GetStatusesSortedByName to get the Status items sorted by Status.Name and filtered by StatusId
            var retrievedResult = await controller.GetStatusesSortedByName(sortOrder, statusId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the StatusViewModel items
            var retrievedStatus = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedStatus);
            Assert.NotNull(retrievedStatus.Value);
            var retrievedStatusViewModels = retrievedStatus.Value as List<StatusViewModel>;
            Assert.NotNull(retrievedStatusViewModels);

            // Assert: verify that the actual result is sorted by Status.Name
            if (sortOrder == Utils.SortOrder.Descending)
            {
                retrievedStatusViewModels.Should().BeInDescendingOrder(status => status.Name);
            }
            else
            {
                retrievedStatusViewModels.Should().BeInAscendingOrder(status => status.Name);
            }

            // Assert: verify that filtering by StatusId works as expected
            if (statusId.HasValue)
            {
                var statusName = await GetStatusNameByIdAsync(context, statusId.Value);
                foreach (var statusViewModel in retrievedStatusViewModels)
                {
                    statusViewModel.Name.Should().Be(statusName);
                }
            }
        }

        [Theory]
        [MemberData(nameof(StatusTestData.StatusCreate_Data), MemberType = typeof(StatusTestData))]
        public async Task CreateStatus(StatusCreateDto statusCreateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            StatusController controller = CreateController(context);

            // Act
            var result = await controller.CreateStatus(statusCreateDto);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are created correctly
            if (expectedStatusCode == HttpStatusCode.Created)
            {
                var okResult = result as ObjectResult;
                var updatedStatus = okResult?.Value as Status;
                Assert.NotNull(updatedStatus);

                updatedStatus.Name.Should().Be(statusCreateDto.Name);
                updatedStatus.Color.Should().Be(statusCreateDto.Color);
                updatedStatus.CreatedAt.Should().Be(statusCreateDto.CreatedAt);
            }
        }

        [Theory]
        [MemberData(nameof(StatusTestData.StatusUpdate_Data), MemberType = typeof(StatusTestData))]
        public async Task UpdateStatus(StatusUpdateDto statusUpdateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            StatusController controller = CreateController(context);

            // Act
            var result = await controller.UpdateStatus(statusUpdateDto);

            // Assert: verify status code matches expected status code
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are updated correctly
            if (expectedStatusCode == HttpStatusCode.OK)
            {
                var okResult = result as ObjectResult;
                var updatedStatus = okResult?.Value as Status;
                Assert.NotNull(updatedStatus);

                updatedStatus.Id.Should().Be(statusUpdateDto.Id);
                updatedStatus.Name.Should().Be(statusUpdateDto.Name);
                updatedStatus.Color.Should().Be(statusUpdateDto.Color);
                updatedStatus.UpdatedAt.Should().Be(statusUpdateDto.UpdatedAt);
            }
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.NoContent)]
        public async Task DeleteStatusById(int statusId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            StatusController controller = CreateController(context);

            // Act
            var result = await controller.DeleteStatusById(statusId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        // Helper method to create StatusController
        private StatusController CreateController(TodoListContext context)
        {
            // Create the service and validator 
            var service = new StatusService(context);
            var validator = new StatusDtoValidator();

            // Inject the service and validator
            StatusController controller = new StatusController(service, validator);

            // Return the controller
            return controller;
        }

        // Helper method to get the status id by its name
        private async Task<string> GetStatusNameByIdAsync(TodoListContext context, int statusId)
        {
            var status = await context.Statuses.FindAsync(statusId);
            return status?.Name ?? string.Empty;
        }
    }
}