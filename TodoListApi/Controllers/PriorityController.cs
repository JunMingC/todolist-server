using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel;
using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Services;
using TodoListApi.Swagger.PriorityExample;

namespace TodoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriorityController : ControllerBase
    {
        private readonly IPriorityService _priorityService;
        private readonly IValidator<PriorityDto> _validator;

        public PriorityController(IPriorityService priorityService, IValidator<PriorityDto> validator)
        {
            _priorityService = priorityService;
            _validator = validator;
        }

        /// <summary>
        /// Retrieves a Priority item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Priority item to retrieve.</param>
        /// <returns>A list of PriorityViewModels containing the Priority item details, or a NotFound result if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriorityById([DefaultValue(1)] int id)
        {
            try
            {
                var priorities = await _priorityService.GetPriorityByIdAsync(id);
                if (priorities == null || !priorities.Any())
                {
                    return NotFound(); // Return 404 Not Found if the Priority item doesn't exist
                }
                return Ok(priorities); // Return 200 OK with the Priority item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Priority items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Priority items by Id.</param>
        /// <returns>A list of PriorityViewModels sorted by Id according to the specified sort order.</returns>
        [HttpGet("GetPriorities")]
        public async Task<IActionResult> GetPriorities(Utils.SortOrder sortOrder)
        {
            try
            {
                var priorities = await _priorityService.GetPrioritiesAsync(sortOrder);
                return Ok(priorities); // Return 200 OK with the sorted list of Priority items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Priority items sorted by Priority.Name based on the specified sort order, and optionally filtered by PriorityId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Priority items by Priority.Name.</param>
        /// <param name="priorityId">Optional. The PriorityId to filter the Priority items.</param>
        /// <returns>A list of PriorityViewModels sorted by Priority.Name according to the specified sort order.</returns>
        [HttpGet("GetPrioritiesSortedByName")]
        public async Task<IActionResult> GetPrioritiesSortedByName(Utils.SortOrder sortOrder, int? priorityId = null)
        {
            try
            {
                var priorities = await _priorityService.GetPrioritiesSortedByNameAsync(sortOrder, priorityId);
                return Ok(priorities); // Return 200 OK with the sorted list of Priority items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Creates a new Priority item.
        /// </summary>
        /// <param name="priorityCreateDto">The data transfer object containing Priority item details.</param>
        /// <returns>The created Priority item with a CreatedAtAction response.</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(PriorityCreateDto), typeof(PriorityCreateExample))]
        public async Task<IActionResult> CreatePriority([FromBody] PriorityCreateDto priorityCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(priorityCreateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var priority = await _priorityService.CreatePriorityAsync(priorityCreateDto);
                return CreatedAtAction(nameof(GetPriorityById), new { id = priority.Id }, priority); // Return 201 Created with location header
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Updates an existing Priority item with new values.
        /// </summary>
        /// <param name="priorityUpdateDto">The data transfer object containing updated Priority item details.</param>
        /// <returns>The updated Priority item, or a NotFound result if the Priority item does not exist.</returns>
        [HttpPut]
        [SwaggerRequestExample(typeof(PriorityUpdateDto), typeof(PriorityUpdateExample))]
        public async Task<IActionResult> UpdatePriority([FromBody] PriorityUpdateDto priorityUpdateDto)
        {
            var validationResult = await _validator.ValidateAsync(priorityUpdateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var updatedPriority = await _priorityService.UpdatePriorityAsync(priorityUpdateDto);
                if (updatedPriority == null)
                {
                    return NotFound(); // Return 404 Not Found if the Priority item doesn't exist
                }

                return Ok(updatedPriority); // Return 200 OK with the updated Priority item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Deletes a Priority item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Priority item to delete.</param>
        /// <returns>NoContent if successful, or a NotFound result if the Priority item does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriorityById([DefaultValue(1)] int id)
        {
            try
            {
                var isDeleted = await _priorityService.DeletePriorityByIdAsync(id);
                if (!isDeleted)
                {
                    return NotFound(); // Return 404 Not Found if the Priority item doesn't exist
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
