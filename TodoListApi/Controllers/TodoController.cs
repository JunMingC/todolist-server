using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.ComponentModel;
using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Services;
using TodoListApi.Swagger.TodoExample;

namespace TodoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IValidator<TodoDto> _validator;

        public TodoController(ITodoService todoService, IValidator<TodoDto> validator)
        {
            _todoService = todoService;
            _validator = validator;
        }

        /// <summary>
        /// Retrieves a Todo item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Todo item to retrieve.</param>
        /// <returns>A list of TodoViewModels containing the Todo item details, or a NotFound result if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoById([DefaultValue(1)] int id)
        {
            try
            {
                var todos = await _todoService.GetTodoByIdAsync(id);
                if (todos == null || !todos.Any())
                {
                    return NotFound(); // Return 404 Not Found if the Todo item doesn't exist
                }
                return Ok(todos); // Return 200 OK with the Todo item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Due Date.</param>
        /// <returns>A list of TodoViewModels sorted by Id according to the specified sort order.</returns>
        [HttpGet("GetTodos")]
        public async Task<IActionResult> GetTodos(Utils.SortOrder sortOrder)
        {
            try
            {
                var todos = await _todoService.GetTodosAsync(sortOrder);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Due Date based on the specified sort order and filtered by the specified period.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Due Date.</param>
        /// <param name="period">The period to filter the Todo items by.</param>
        /// <returns>A list of TodoViewModels sorted by Due Date according to the specified sort order.</returns>
        [HttpGet("GetTodosSortedByDueDate")]
        public async Task<IActionResult> GetTodosSortedByDueDate(Utils.SortOrder sortOrder, Utils.Period? period = null)
        {
            try
            {
                var todos = await _todoService.GetTodosSortedByDueDateAsync(sortOrder, period);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Priority.Id based on the specified sort order, and optionally filtered by PriorityId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Priority.Id.</param>
        /// <param name="priorityId">Optional. The PriorityId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Priority.Id according to the specified sort order.</returns>
        [HttpGet("GetTodosSortedByPriorityId")]
        public async Task<IActionResult> GetTodosSortedByPriorityId(Utils.SortOrder sortOrder, int? priorityId = null)
        {
            try
            {
                var todos = await _todoService.GetTodosSortedByPriorityIdAsync(sortOrder, priorityId);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Priority.Name based on the specified sort order, and optionally filtered by PriorityId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Priority.Name.</param>
        /// <param name="priorityId">Optional. The PriorityId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Priority.Name according to the specified sort order.</returns>
        [HttpGet("GetTodosSortedByPriorityName")]
        public async Task<IActionResult> GetTodosSortedByPriorityName(Utils.SortOrder sortOrder, int? priorityId = null)
        {
            try
            {
                var todos = await _todoService.GetTodosSortedByPriorityNameAsync(sortOrder, priorityId);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Status.Id based on the specified sort order, and optionally filtered by StatusId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Status.Id.</param>
        /// <param name="statusId">Optional. The StatusId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Status.Id according to the specified sort order.</returns>
        [HttpGet("GetTodosSortedByStatusId")]
        public async Task<IActionResult> GetTodosSortedByStatusId(Utils.SortOrder sortOrder, int? statusId = null)
        {
            try
            {
                var todos = await _todoService.GetTodosSortedByStatusIdAsync(sortOrder, statusId);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Status.Name based on the specified sort order, and optionally filtered by StatusId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Status.Name.</param>
        /// <param name="statusId">Optional. The StatusId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Status.Name according to the specified sort order.</returns>
        [HttpGet("GetTodosSortedByStatusName")]
        public async Task<IActionResult> GetTodosSortedByStatusName(Utils.SortOrder sortOrder, int? statusId = null)
        {
            try
            {
                var todos = await _todoService.GetTodosSortedByStatusNameAsync(sortOrder, statusId);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Retrieves all Todo items sorted by the number of Tags based on the specified sort order, and optionally filtered by TagId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by the number of Tags.</param>
        /// <param name="tagId">Optional. The TagId to filter the Todo items by.</param>
        /// <returns>A list of TodoViewModels sorted by the number of Tags according to the specified sort order.</returns>
        [HttpGet("GetTodosSortedByTagsCount")]
        public async Task<IActionResult> GetTodosSortedByTagsCount(Utils.SortOrder sortOrder = Utils.SortOrder.Descending, int? tagId = null)
        {
            try
            {
                var todos = await _todoService.GetTodosSortedByTagsCountAsync(sortOrder, tagId);
                return Ok(todos); // Return 200 OK with the sorted list of Todo items
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Return 500 Internal Server Error if an error occurs
            }
        }

        /// <summary>
        /// Creates a new Todo item.
        /// </summary>
        /// <param name="todoCreateDto">The data transfer object containing Todo item details.</param>
        /// <returns>The created Todo item with a CreatedAtAction response.</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(TodoCreateDto), typeof(TodoCreateExample))]
        public async Task<IActionResult> CreateTodo([FromBody] TodoCreateDto todoCreateDto)
        {
            var validationResult = await _validator.ValidateAsync(todoCreateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var todo = await _todoService.CreateTodoAsync(todoCreateDto);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo); // Return 201 Created with location header
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Updates an existing Todo item with new values.
        /// </summary>
        /// <param name="todoUpdateDto">The data transfer object containing updated Todo item details.</param>
        /// <returns>The updated Todo item, or a NotFound result if the Todo item does not exist.</returns>
        [HttpPut]
        [SwaggerRequestExample(typeof(TodoUpdateDto), typeof(TodoUpdateExample))]
        public async Task<IActionResult> UpdateTodo([FromBody] TodoUpdateDto todoUpdateDto)
        {
            var validationResult = await _validator.ValidateAsync(todoUpdateDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState); // Return 400 Bad Request if validation fails
            }

            try
            {
                var updatedTodo = await _todoService.UpdateTodoAsync(todoUpdateDto);
                if (updatedTodo == null)
                {
                    return NotFound(); // Return 404 Not Found if the Todo item doesn't exist
                }

                return Ok(updatedTodo); // Return 200 OK with the updated Todo item details
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return 400 Bad Request if an error occurs
            }
        }

        /// <summary>
        /// Deletes a Todo item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Todo item to delete.</param>
        /// <returns>NoContent if successful, or a NotFound result if the Todo item does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoById([DefaultValue(1)] int id)
        {
            try
            {
                var isDeleted = await _todoService.DeleteTodoByIdAsync(id);
                if (!isDeleted)
                {
                    return NotFound(); // Return 404 Not Found if the Todo item doesn't exist
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
