using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Dto;
using TodoListApi.Models;
using TodoListApi.ViewModels;
using TodoListApi.Helpers;

namespace TodoListApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoListContext _context;

        public TodoService(TodoListContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a Todo item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Todo item to retrieve.</param>
        /// <returns>A list of TodoViewModels containing the Todo item details, or null if not found.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodoByIdAsync(int id)
        {
            return await _context.Todos
                 .Include(todo => todo.Priority)
                 .Include(todo => todo.Status)
                 .Include(todo => todo.Tags)
                 .Where(todo => todo.Id == id) // Id equals
                 .Select(todo => new TodoViewModel
                 {
                     Id = todo.Id,
                     Name = todo.Name,
                     Description = todo.Description,
                     DueDate = todo.DueDate,
                     Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                     Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                     Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                 })
                 .ToListAsync();
        }


        /// <summary>
        /// Retrieves all Todo items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Priority.Name.</param>
        /// <returns>A list of TodoViewModels sorted by Id according to the specified sort order.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(todo => todo.Id);
            }
            else
            {
                query = query.OrderBy(todo => todo.Id);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Due Date based on the specified sort order and filtered by the specified period.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Due Date.</param>
        /// <param name="period">The period to filter the Todo items by.</param>
        /// <returns>A list of TodoViewModels sorted by Due Date according to the specified sort order and filtered by the specified period.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosSortedByDueDateAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, Utils.Period? period = null)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply period-based filtering if specified
            if (period.HasValue)
            {
                var now = DateTime.UtcNow;

                if (period.Value == Utils.Period.Today)
                {
                    var startOfDay = now.Date;
                    var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
                    query = query.Where(todo => todo.DueDate >= startOfDay && todo.DueDate <= endOfDay);
                }
                else if (period.Value == Utils.Period.ThisWeek)
                {
                    var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
                    var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
                    query = query.Where(todo => todo.DueDate >= startOfWeek && todo.DueDate <= endOfWeek);
                }
                else if (period.Value == Utils.Period.ThisMonth)
                {
                    var startOfMonth = new DateTime(now.Year, now.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
                    query = query.Where(todo => todo.DueDate >= startOfMonth && todo.DueDate <= endOfMonth);
                }
                else if (period.Value == Utils.Period.ThisYear)
                {
                    var startOfYear = new DateTime(now.Year, 1, 1);
                    var endOfYear = startOfYear.AddYears(1).AddTicks(-1);
                    query = query.Where(todo => todo.DueDate >= startOfYear && todo.DueDate <= endOfYear);
                }
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query
                    .OrderBy(todo => todo.DueDate != null ? 0 : 1)
                    .ThenByDescending(todo => todo.DueDate)
                    .ThenBy(todo => todo.Id);
            }
            else
            {
                query = query
                    .OrderBy(todo => todo.DueDate != null ? 0 : 1)
                    .ThenBy(todo => todo.DueDate)
                    .ThenBy(todo => todo.Id);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Priority.Id based on the specified sort order, and optionally filtered by PriorityId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Priority.Id.</param>
        /// <param name="priorityId">Optional. The PriorityId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Priority.Id according to the specified sort order and filtered by PriorityId.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosSortedByPriorityIdAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? priorityId = null)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply filtering by PriorityId if specified
            if (priorityId.HasValue)
            {
                query = query.Where(todo => todo.PriorityId == priorityId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query
                    .OrderBy(todo => todo.Priority != null ? 0 : 1)
                    .ThenByDescending(todo => todo.Priority != null ? todo.Priority.Id : 0)
                    .ThenBy(todo => todo.Id);
            }
            else
            {
                query = query
                    .OrderBy(todo => todo.Priority != null ? 0 : 1)
                    .ThenBy(todo => todo.Priority != null ? todo.Priority.Id : 0)
                    .ThenBy(todo => todo.Id);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Priority.Name based on the specified sort order, and optionally filtered by PriorityId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Priority.Name.</param>
        /// <param name="priorityId">Optional. The PriorityId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Priority.Name according to the specified sort order and filtered by PriorityId.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosSortedByPriorityNameAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? priorityId = null)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply filtering by PriorityId if specified
            if (priorityId.HasValue)
            {
                query = query.Where(todo => todo.PriorityId == priorityId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query
                    .OrderBy(todo => todo.Priority != null ? 0 : 1)
                    .ThenByDescending(todo => todo.Priority != null ? todo.Priority.Name : string.Empty)
                    .ThenBy(todo => todo.Id);
            }
            else
            {
                query = query
                    .OrderBy(todo => todo.Priority != null ? 0 : 1)
                    .ThenBy(todo => todo.Priority != null ? todo.Priority.Name : string.Empty)
                    .ThenBy(todo => todo.Id);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Status.Id based on the specified sort order, and optionally filtered by StatusId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Status.Id.</param>
        /// <param name="statusId">Optional. The StatusId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Status.Id according to the specified sort order and filtered by StatusId.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosSortedByStatusIdAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? statusId = null)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply filtering by StatusId if specified
            if (statusId.HasValue)
            {
                query = query.Where(todo => todo.StatusId == statusId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query
                    .OrderBy(todo => todo.Status != null ? 0 : 1)
                    .ThenByDescending(todo => todo.Status != null ? todo.Status.Id : 0)
                    .ThenBy(todo => todo.Id);
            }
            else
            {
                query = query
                    .OrderBy(todo => todo.Status != null ? 0 : 1)
                    .ThenBy(todo => todo.Status != null ? todo.Status.Id : 0)
                    .ThenBy(todo => todo.Id);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Todo items sorted by Status.Name based on the specified sort order, and optionally filtered by StatusId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by Status.Name.</param>
        /// <param name="statusId">Optional. The StatusId to filter the Todo items.</param>
        /// <returns>A list of TodoViewModels sorted by Status.Name according to the specified sort order and filtered by StatusId.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosSortedByStatusNameAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? statusId = null)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply filtering by StatusId if specified
            if (statusId.HasValue)
            {
                query = query.Where(todo => todo.StatusId == statusId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query
                    .OrderBy(todo => todo.Status != null ? 0 : 1)
                    .ThenByDescending(todo => todo.Status != null ? todo.Status.Name : string.Empty)
                    .ThenBy(todo => todo.Id);
            }
            else
            {
                query = query
                    .OrderBy(todo => todo.Status != null ? 0 : 1)
                    .ThenBy(todo => todo.Status != null ? todo.Status.Name : string.Empty)
                    .ThenBy(todo => todo.Id);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Todo items sorted by the number of Tags based on the specified sort order, and optionally filtered by TagId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Todo items by the number of Tags.</param>
        /// <param name="tagId">Optional. The TagId to filter the Todo items by.</param>
        /// <returns>A list of TodoViewModels sorted by the number of Tags according to the specified sort order and filtered by TagId.</returns>
        public async Task<IEnumerable<TodoViewModel>> GetTodosSortedByTagsCountAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Descending, int? tagId = null)
        {
            IQueryable<Todo> query = _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags);

            // Apply filtering by TagId if specified
            if (tagId.HasValue)
            {
                query = query.Where(todo => todo.Tags.Any(tag => tag.Id == tagId.Value));
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query
                    .OrderBy(todo => todo.Tags.Count == 0 ? 1 : 0)
                    .ThenByDescending(todo => todo.Tags.Count)
                    .ThenBy(todo => todo.Name);
            }
            else
            {
                query = query
                    .OrderBy(todo => todo.Tags.Count == 0 ? 1 : 0)
                    .ThenBy(todo => todo.Tags.Count)
                    .ThenBy(todo => todo.Name);
            }

            return await query
                .Select(todo => new TodoViewModel
                {
                    Id = todo.Id,
                    Name = todo.Name,
                    Description = todo.Description,
                    DueDate = todo.DueDate,
                    Priority = todo.Priority != null ? new PriorityViewModel { Id = todo.Priority.Id, Name = todo.Priority.Name, Color = todo.Priority.Color } : null,
                    Status = todo.Status != null ? new StatusViewModel { Id = todo.Status.Id, Name = todo.Status.Name, Color = todo.Status.Color } : null,
                    Tags = todo.Tags.Select(tag => new TagViewModel { Id = tag.Id, Name = tag.Name, Color = tag.Color }).ToList(),
                })
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new Todo item.
        /// </summary>
        /// <param name="todoCreateDto">The data transfer object containing Todo item details.</param>
        /// <returns>The added Todo item.</returns>
        public async Task<Todo> CreateTodoAsync(TodoCreateDto todoCreateDto)
        {
            // Process Tags: Retrieve existing tags based on provided TagIds
            var tags = new List<Tag>();

            if (todoCreateDto.TagIds != null && todoCreateDto.TagIds.Count > 0)
            {
                var existingTags = await _context.Tags
                    .Where(todo => todoCreateDto.TagIds.Contains(todo.Id))
                    .ToListAsync();

                tags.AddRange(existingTags);
            }

            // Create a new Todo item
            var todo = new Todo
            {
                Name = todoCreateDto.Name,
                Description = todoCreateDto.Description,
                DueDate = todoCreateDto.DueDate,
                PriorityId = todoCreateDto.PriorityId,
                StatusId = todoCreateDto.StatusId,
                CreatedAt = todoCreateDto.CreatedAt,
                Tags = tags
            };

            // Add the new Todo item to the database and save changes
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return todo;
        }

        /// <summary>
        /// Updates an existing Todo item with new values.
        /// </summary>
        /// <param name="todoUpdateDto">The data transfer object containing updated Todo item details.</param>
        /// <returns>The updated Todo item, or null if the Todo item does not exist.</returns>
        public async Task<Todo?> UpdateTodoAsync(TodoUpdateDto todoUpdateDto)
        {
            // Retrieve the Todo item to update
            var todo = await _context.Todos
                .Include(todo => todo.Priority)
                .Include(todo => todo.Status)
                .Include(todo => todo.Tags)
                .FirstOrDefaultAsync(todo => todo.Id == todoUpdateDto.Id);

            if (todo == null)
            {
                return null; // Return null if the Todo item does not exist
            }

            // Clear existing tags
            todo.Tags.Clear();

            // Update the Todo item with new values
            todo.Name = todoUpdateDto.Name;
            todo.Description = todoUpdateDto.Description;
            todo.DueDate = todoUpdateDto.DueDate;
            todo.PriorityId = todoUpdateDto.PriorityId;
            todo.StatusId = todoUpdateDto.StatusId;
            todo.UpdatedAt = todoUpdateDto.UpdatedAt;

            // Update tags
            if (todoUpdateDto.TagIds != null)
            {
                // Get new tags
                var tags = await _context.Tags
                    .Where(tag => todoUpdateDto.TagIds.Contains(tag.Id))
                    .ToListAsync();

                // Add new tags
                foreach (var tag in tags)
                {
                    todo.Tags.Add(tag);
                }
            }

            // Save changes to the database
            _context.Todos.Update(todo);
            await _context.SaveChangesAsync();

            return todo;
        }

        /// <summary>
        /// Deletes a Todo item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Todo item to delete.</param>
        /// <returns>True if the Todo item was successfully deleted, or false if the Todo item does not exist.</returns>
        public async Task<bool> DeleteTodoByIdAsync(int id)
        {
            // Retrieve the Todo item to delete
            var todo = await _context.Todos
                .Include(todo => todo.Tags) // Include related tags for cascade delete
                .FirstOrDefaultAsync(todo => todo.Id == id);

            if (todo == null)
            {
                return false; // Return false if the Todo item does not exist
            }

            // Remove the Todo item
            _context.Todos.Remove(todo);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }
    }
}