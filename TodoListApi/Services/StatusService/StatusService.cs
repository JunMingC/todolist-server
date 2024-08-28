using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Dto;
using TodoListApi.Models;
using TodoListApi.ViewModels;
using TodoListApi.Helpers;

namespace TodoListApi.Services
{
    public class StatusService : IStatusService
    {
        private readonly TodoListContext _context;

        public StatusService(TodoListContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a Status item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Status item to retrieve.</param>
        /// <returns>A list of StatusViewModels containing the Status item details, or null if not found.</returns>
        public async Task<IEnumerable<StatusViewModel>> GetStatusByIdAsync(int id)
        {
            return await _context.Statuses
                 .Where(status => status.Id == id) // Id equals
                 .Select(status => new StatusViewModel
                 {
                     Id = status.Id,
                     Name = status.Name,
                     Color = status.Color,
                 })
                 .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Status items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Status items by Id.</param>
        /// <returns>A list of StatusViewModels sorted by Id according to the specified sort order.</returns>
        public async Task<IEnumerable<StatusViewModel>> GetStatusesAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending)
        {
            IQueryable<Status> query = _context.Statuses;

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(status => status.Id);
            }
            else
            {
                query = query.OrderBy(status => status.Id);
            }

            return await query
                .Select(status => new StatusViewModel
                {
                    Id = status.Id,
                    Name = status.Name,
                    Color = status.Color,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Status items sorted by Status.Name based on the specified sort order, and optionally filtered by StatusId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Status items by Status.Name.</param>
        /// <param name="statusId">Optional. The StatusId to filter the Status items.</param>
        /// <returns>A list of StatusViewModels sorted by Status.Name according to the specified sort order and filtered by StatusId.</returns>
        public async Task<IEnumerable<StatusViewModel>> GetStatusesSortedByNameAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? statusId = null)
        {
            IQueryable<Status> query = _context.Statuses;

            // Apply filtering by StatusId if specified
            if (statusId.HasValue)
            {
                query = query.Where(status => status.Id == statusId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(status => status.Name);
            }
            else
            {
                query = query.OrderBy(status => status.Name);
            }

            return await query
                .Select(status => new StatusViewModel
                {
                    Id = status.Id,
                    Name = status.Name,
                    Color = status.Color,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new Status item.
        /// </summary>
        /// <param name="statusCreateDto">The data transfer object containing Status item details.</param>
        /// <returns>The added Status item.</returns>
        public async Task<Status> CreateStatusAsync(StatusCreateDto statusCreateDto)
        {
            // Create a new Status item
            var status = new Status
            {
                Name = statusCreateDto.Name,
                Color = statusCreateDto.Color,
                CreatedAt = statusCreateDto.CreatedAt,
            };

            // Add the new Status item to the database and save changes
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();

            return status;
        }

        /// <summary>
        /// Updates an existing Status item with new values.
        /// </summary>
        /// <param name="statusUpdateDto">The data transfer object containing updated Status item details.</param>
        /// <returns>The updated Status item, or null if the Status item does not exist.</returns>
        public async Task<Status?> UpdateStatusAsync(StatusUpdateDto statusUpdateDto)
        {
            // Retrieve the Status item to update
            var status = await _context.Statuses.FirstOrDefaultAsync(status => status.Id == statusUpdateDto.Id);

            if (status == null)
            {
                return null; // Return null if the Status item does not exist
            }


            // Update the Status item with new values
            status.Name = statusUpdateDto.Name;
            status.Color = statusUpdateDto.Color;
            status.UpdatedAt = statusUpdateDto.UpdatedAt;

            // Save changes to the database
            _context.Statuses.Update(status);
            await _context.SaveChangesAsync();

            return status;
        }

        /// <summary>
        /// Deletes a Status item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Status item to delete.</param>
        /// <returns>True if the Status item was successfully deleted, or false if the Status item does not exist.</returns>
        public async Task<bool> DeleteStatusByIdAsync(int id)
        {
            // Retrieve the Status item to delete
            var status = await _context.Statuses.FirstOrDefaultAsync(status => status.Id == id);

            if (status == null)
            {
                return false; // Return false if the Status item does not exist
            }

            // Remove the Status item
            _context.Statuses.Remove(status);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }
    }
}