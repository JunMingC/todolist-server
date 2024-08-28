using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel;
using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Services;
using TodoListApi.Swagger.StatusExample;

namespace TodoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        private readonly IValidator<StatusDto> _validator;

        public StatusController(IStatusService statusService, IValidator<StatusDto> validator)
        {
            _statusService = statusService;
            _validator = validator;
        }

        /// <summary>
        /// Retrieves a Status item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Status item to retrieve.</param>
        /// <returns>A list of StatusViewModels containing the Status item details, or a NotFound result if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatusById([DefaultValue(1)] int id)
        {
            try
            {
                var statuses = await _statusService.GetStatusByIdAsync(id);
                if (statuses == null || !statuses.Any())
                {
                    return NotFound(); // Return 404 Not Found if the Status item doesn't exist
                }
                return Ok(statuses); // Return 200 OK with the Status item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Status items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Status items by Id.</param>
        /// <returns>A list of StatusViewModels sorted by Id according to the specified sort order.</returns>
        [HttpGet("GetStatuses")]
        public async Task<IActionResult> GetStatuses(Utils.SortOrder sortOrder)
        {
            try
            {
                var statuses = await _statusService.GetStatusesAsync(sortOrder);
                return Ok(statuses); // Return 200 OK with the sorted list of Status items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Status items sorted by Status.Name based on the specified sort order, and optionally filtered by StatusId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Status items by Status.Name.</param>
        /// <param name="statusId">Optional. The StatusId to filter the Status items.</param>
        /// <returns>A list of StatusViewModels sorted by Status.Name according to the specified sort order.</returns>
        [HttpGet("GetStatusesSortedByName")]
        public async Task<IActionResult> GetStatusesSortedByName(Utils.SortOrder sortOrder, int? statusId = null)
        {
            try
            {
                var statuses = await _statusService.GetStatusesSortedByNameAsync(sortOrder, statusId);
                return Ok(statuses); // Return 200 OK with the sorted list of Status items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Creates a new Status item.
        /// </summary>
        /// <param name="statusCreateDto">The data transfer object containing Status item details.</param>
        /// <returns>The created Status item with a CreatedAtAction response.</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(StatusCreateDto), typeof(StatusCreateExample))]
        public async Task<IActionResult> CreateStatus([FromBody] StatusCreateDto statusCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(statusCreateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var status = await _statusService.CreateStatusAsync(statusCreateDto);
                return CreatedAtAction(nameof(GetStatusById), new { id = status.Id }, status); // Return 201 Created with location header
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Updates an existing Status item with new values.
        /// </summary>
        /// <param name="statusUpdateDto">The data transfer object containing updated Status item details.</param>
        /// <returns>The updated Status item, or a NotFound result if the Status item does not exist.</returns>
        [HttpPut]
        [SwaggerRequestExample(typeof(StatusUpdateDto), typeof(StatusUpdateExample))]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusUpdateDto statusUpdateDto)
        {
            var validationResult = await _validator.ValidateAsync(statusUpdateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var updatedStatus = await _statusService.UpdateStatusAsync(statusUpdateDto);
                if (updatedStatus == null)
                {
                    return NotFound(); // Return 404 Not Found if the Status item doesn't exist
                }

                return Ok(updatedStatus); // Return 200 OK with the updated Status item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Deletes a Status item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Status item to delete.</param>
        /// <returns>NoContent if successful, or a NotFound result if the Status item does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatusById([DefaultValue(1)] int id)
        {
            try
            {
                var isDeleted = await _statusService.DeleteStatusByIdAsync(id);
                if (!isDeleted)
                {
                    return NotFound(); // Return 404 Not Found if the Status item doesn't exist
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
