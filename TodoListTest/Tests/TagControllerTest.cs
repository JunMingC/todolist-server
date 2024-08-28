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
using TagListTest.Data;

namespace TodoListTest.Controllers
{
    public class TagControllerTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public TagControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.OK)]
        public async Task GetTagById(int tagId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TagController controller = CreateController(context);

            // Act
            var result = await controller.GetTagById(tagId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Descending)]
        public async Task GetTags(Utils.SortOrder sortOrder)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TagController controller = CreateController(context);

            // Act: use controller.GetTags to get the Tag items sorted by Id
            var retrievedResult = await controller.GetTags(sortOrder);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the TagViewModel items
            var retrievedTag = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTag);
            Assert.NotNull(retrievedTag.Value);
            var retrievedTagViewModels = retrievedTag.Value as List<TagViewModel>;
            Assert.NotNull(retrievedTagViewModels);

            // Assert: verify that the actual result is sorted by Tag.Id
            if (sortOrder == Utils.SortOrder.Descending)
            {
                retrievedTagViewModels.Should().BeInDescendingOrder(tag => tag.Id);
            }
            else
            {
                retrievedTagViewModels.Should().BeInAscendingOrder(tag => tag.Id);
            }
        }

        [Theory]
        [InlineData(Utils.SortOrder.Ascending)]
        [InlineData(Utils.SortOrder.Ascending, 0)]
        [InlineData(Utils.SortOrder.Ascending, 1)]
        [InlineData(Utils.SortOrder.Descending)]
        [InlineData(Utils.SortOrder.Descending, 0)]
        [InlineData(Utils.SortOrder.Descending, 1)]
        public async Task GetTagsSortedByName(Utils.SortOrder sortOrder, int? tagId = null)
        {
            // Act: create context and controller
            using TodoListContext context = Fixture.CreateContext();
            TagController controller = CreateController(context);

            // Act: use controller.GetTagsSortedByName to get the Tag items sorted by Tag.Name and filtered by TagId
            var retrievedResult = await controller.GetTagsSortedByName(sortOrder, tagId);

            // Assert: verify that the status code is HttpStatusCode.OK
            var retrievedStatusCodeResult = retrievedResult as IStatusCodeActionResult;
            Assert.NotNull(retrievedStatusCodeResult);
            retrievedStatusCodeResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // Act: get the TagViewModel items
            var retrievedTag = retrievedResult as OkObjectResult;
            Assert.NotNull(retrievedTag);
            Assert.NotNull(retrievedTag.Value);
            var retrievedTagViewModels = retrievedTag.Value as List<TagViewModel>;
            Assert.NotNull(retrievedTagViewModels);

            // Assert: verify that the actual result is sorted by Tag.Name
            if (sortOrder == Utils.SortOrder.Descending)
            {
                retrievedTagViewModels.Should().BeInDescendingOrder(tag => tag.Name);
            }
            else
            {
                retrievedTagViewModels.Should().BeInAscendingOrder(tag => tag.Name);
            }

            // Assert: verify that filtering by TagId works as expected
            if (tagId.HasValue)
            {
                var tagName = await GetTagNameByIdAsync(context, tagId.Value);
                foreach (var tagViewModel in retrievedTagViewModels)
                {
                    tagViewModel.Name.Should().Be(tagName);
                }
            }
        }

        [Theory]
        [MemberData(nameof(TagTestData.TagCreate_Data), MemberType = typeof(TagTestData))]
        public async Task CreateTag(TagCreateDto tagCreateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            TagController controller = CreateController(context);

            // Act
            var result = await controller.CreateTag(tagCreateDto);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are created correctly
            if (expectedStatusCode == HttpStatusCode.Created)
            {
                var okResult = result as ObjectResult;
                var updatedTag = okResult?.Value as Tag;
                Assert.NotNull(updatedTag);

                updatedTag.Name.Should().Be(tagCreateDto.Name);
                updatedTag.Color.Should().Be(tagCreateDto.Color);
                updatedTag.CreatedAt.Should().Be(tagCreateDto.CreatedAt);
            }
        }

        [Theory]
        [MemberData(nameof(TagTestData.TagUpdate_Data), MemberType = typeof(TagTestData))]
        public async Task UpdateTag(TagUpdateDto tagUpdateDto, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            TagController controller = CreateController(context);

            // Act
            var result = await controller.UpdateTag(tagUpdateDto);

            // Assert: verify status code matches expected status code
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);

            // Assert: verify content are updated correctly
            if (expectedStatusCode == HttpStatusCode.OK)
            {
                var okResult = result as ObjectResult;
                var updatedTag = okResult?.Value as Tag;
                Assert.NotNull(updatedTag);

                updatedTag.Id.Should().Be(tagUpdateDto.Id);
                updatedTag.Name.Should().Be(tagUpdateDto.Name);
                updatedTag.Color.Should().Be(tagUpdateDto.Color);
                updatedTag.UpdatedAt.Should().Be(tagUpdateDto.UpdatedAt);
            }
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.NoContent)]
        public async Task DeleteTagById(int tagId, HttpStatusCode expectedStatusCode)
        {
            // Act: create context and controller (with transaction)
            using TodoListContext context = Fixture.CreateContext();
            context.Database.BeginTransaction();
            TagController controller = CreateController(context);

            // Act
            var result = await controller.DeleteTagById(tagId);

            // Assert
            var statusCodeResult = result as IStatusCodeActionResult;
            Assert.NotNull(statusCodeResult);
            statusCodeResult.StatusCode.Should().Be((int)expectedStatusCode);
        }

        // Helper method to create TagController
        private TagController CreateController(TodoListContext context)
        {
            // Create the service and validator 
            var service = new TagService(context);
            var validator = new TagDtoValidator();

            // Inject the service and validator
            TagController controller = new TagController(service, validator);

            // Return the controller
            return controller;
        }

        // Helper method to get the tag id by its name
        private async Task<string> GetTagNameByIdAsync(TodoListContext context, int tagId)
        {
            var tag = await context.Tags.FindAsync(tagId);
            return tag?.Name ?? string.Empty;
        }
    }
}