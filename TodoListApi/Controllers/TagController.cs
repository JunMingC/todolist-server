using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel;
using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Services;
using TodoListApi.Swagger.TagExample;

namespace TodoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IValidator<TagDto> _validator;

        public TagController(ITagService tagService, IValidator<TagDto> validator)
        {
            _tagService = tagService;
            _validator = validator;
        }

        /// <summary>
        /// Retrieves a Tag item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Tag item to retrieve.</param>
        /// <returns>A list of TagViewModels containing the Tag item details, or a NotFound result if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById([DefaultValue(1)] int id)
        {
            try
            {
                var tags = await _tagService.GetTagByIdAsync(id);
                if (tags == null || !tags.Any())
                {
                    return NotFound(); // Return 404 Not Found if the Tag item doesn't exist
                }
                return Ok(tags); // Return 200 OK with the Tag item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Tag items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Tag items by Tag.Name.</param>
        /// <returns>A list of TagViewModels sorted by Id according to the specified sort order.</returns>
        [HttpGet("GetTags")]
        public async Task<IActionResult> GetTags(Utils.SortOrder sortOrder)
        {
            try
            {
                var tags = await _tagService.GetTagsAsync(sortOrder);
                return Ok(tags); // Return 200 OK with the sorted list of Tag items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Tag items sorted by Tag.Name based on the specified sort order, and optionally filtered by TagId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Tag items by Tag.Name.</param>
        /// <param name="tagId">Optional. The TagId to filter the Tag items.</param>
        /// <returns>A list of TagViewModels sorted by Tag.Name according to the specified sort order.</returns>
        [HttpGet("GetTagsSortedByName")]
        public async Task<IActionResult> GetTagsSortedByName(Utils.SortOrder sortOrder, int? tagId = null)
        {
            try
            {
                var tags = await _tagService.GetTagsSortedByNameAsync(sortOrder, tagId);
                return Ok(tags); // Return 200 OK with the sorted list of Tag items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Creates a new Tag item.
        /// </summary>
        /// <param name="tagCreateDto">The data transfer object containing Tag item details.</param>
        /// <returns>The created Tag item with a CreatedAtAction response.</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(TagCreateDto), typeof(TagCreateExample))]
        public async Task<IActionResult> CreateTag([FromBody] TagCreateDto tagCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(tagCreateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var tag = await _tagService.CreateTagAsync(tagCreateDto);
                return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag); // Return 201 Created with location header
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Updates an existing Tag item with new values.
        /// </summary>
        /// <param name="tagUpdateDto">The data transfer object containing updated Tag item details.</param>
        /// <returns>The updated Tag item, or a NotFound result if the Tag item does not exist.</returns>
        [HttpPut]
        [SwaggerRequestExample(typeof(TagUpdateDto), typeof(TagUpdateExample))]
        public async Task<IActionResult> UpdateTag([FromBody] TagUpdateDto tagUpdateDto)
        {
            var validationResult = await _validator.ValidateAsync(tagUpdateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var updatedTag = await _tagService.UpdateTagAsync(tagUpdateDto);
                if (updatedTag == null)
                {
                    return NotFound(); // Return 404 Not Found if the Tag item doesn't exist
                }

                return Ok(updatedTag); // Return 200 OK with the updated Tag item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Deletes a Tag item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Tag item to delete.</param>
        /// <returns>NoContent if successful, or a NotFound result if the Tag item does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTagById([DefaultValue(1)] int id)
        {
            try
            {
                var isDeleted = await _tagService.DeleteTagByIdAsync(id);
                if (!isDeleted)
                {
                    return NotFound(); // Return 404 Not Found if the Tag item doesn't exist
                }
                return NoContent(); // Return 204 No Content on successful delete
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }
    }
}
