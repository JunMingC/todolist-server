using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Dto;
using TodoListApi.Models;
using TodoListApi.ViewModels;
using TodoListApi.Helpers;

namespace TodoListApi.Services
{
    public class PriorityService : IPriorityService
    {
        private readonly TodoListContext _context;

        public PriorityService(TodoListContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a Priority item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Priority item to retrieve.</param>
        /// <returns>A list of PriorityViewModels containing the Priority item details, or null if not found.</returns>
        public async Task<IEnumerable<PriorityViewModel>> GetPriorityByIdAsync(int id)
        {
            return await _context.Priorities
                 .Where(priority => priority.Id == id) // Id equals
                 .Select(priority => new PriorityViewModel
                 {
                     Id = priority.Id,
                     Name = priority.Name,
                     Color = priority.Color,
                 })
                 .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Priority items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Priority items by Id.</param>
        /// <returns>A list of PriorityViewModels sorted by Id according to the specified sort order.</returns>
        public async Task<IEnumerable<PriorityViewModel>> GetPrioritiesAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending)
        {
            IQueryable<Priority> query = _context.Priorities;

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(priority => priority.Id);
            }
            else
            {
                query = query.OrderBy(priority => priority.Id);
            }

            return await query
                .Select(priority => new PriorityViewModel
                {
                    Id = priority.Id,
                    Name = priority.Name,
                    Color = priority.Color,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Priority items sorted by Priority.Name based on the specified sort order, and optionally filtered by PriorityId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Priority items by Priority.Name.</param>
        /// <param name="priorityId">Optional. The PriorityId to filter the Priority items.</param>
        /// <returns>A list of PriorityViewModels sorted by Priority.Name according to the specified sort order and filtered by PriorityId.</returns>
        public async Task<IEnumerable<PriorityViewModel>> GetPrioritiesSortedByNameAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? priorityId = null)
        {
            IQueryable<Priority> query = _context.Priorities;

            // Apply filtering by PriorityId if specified
            if (priorityId.HasValue)
            {
                query = query.Where(priority => priority.Id == priorityId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(priority => priority.Name);
            }
            else
            {
                query = query.OrderBy(priority => priority.Name);
            }

            return await query
                .Select(priority => new PriorityViewModel
                {
                    Id = priority.Id,
                    Name = priority.Name,
                    Color = priority.Color,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new Priority item.
        /// </summary>
        /// <param name="priorityCreateDto">The data transfer object containing Priority item details.</param>
        /// <returns>The added Priority item.</returns>
        public async Task<Priority> CreatePriorityAsync(PriorityCreateDto priorityCreateDto)
        {
            // Create a new Priority item
            var priority = new Priority
            {
                Name = priorityCreateDto.Name,
                Color = priorityCreateDto.Color,
                CreatedAt = priorityCreateDto.CreatedAt,
            };

            // Add the new Priority item to the database and save changes
            _context.Priorities.Add(priority);
            await _context.SaveChangesAsync();

            return priority;
        }

        /// <summary>
        /// Updates an existing Priority item with new values.
        /// </summary>
        /// <param name="priorityUpdateDto">The data transfer object containing updated Priority item details.</param>
        /// <returns>The updated Priority item, or null if the Priority item does not exist.</returns>
        public async Task<Priority?> UpdatePriorityAsync(PriorityUpdateDto priorityUpdateDto)
        {
            // Retrieve the Priority item to update
            var priority = await _context.Priorities.FirstOrDefaultAsync(priority => priority.Id == priorityUpdateDto.Id);

            if (priority == null)
            {
                return null; // Return null if the Priority item does not exist
            }


            // Update the Priority item with new values
            priority.Name = priorityUpdateDto.Name;
            priority.Color = priorityUpdateDto.Color;
            priority.UpdatedAt = priorityUpdateDto.UpdatedAt;

            // Save changes to the database
            _context.Priorities.Update(priority);
            await _context.SaveChangesAsync();

            return priority;
        }

        /// <summary>
        /// Deletes a Priority item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Priority item to delete.</param>
        /// <returns>True if the Priority item was successfully deleted, or false if the Priority item does not exist.</returns>
        public async Task<bool> DeletePriorityByIdAsync(int id)
        {
            // Retrieve the Priority item to delete
            var priority = await _context.Priorities.FirstOrDefaultAsync(priority => priority.Id == id);

            if (priority == null)
            {
                return false; // Return false if the Priority item does not exist
            }

            // Remove the Priority item
            _context.Priorities.Remove(priority);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }
    }
}