using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using TodoListApi.Dto;
using TodoListApi.Models;
using TodoListApi.ViewModels;
using TodoListApi.Helpers;

namespace TodoListApi.Services
{
    public class TagService : ITagService
    {
        private readonly TodoListContext _context;

        public TagService(TodoListContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a Tag item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Tag item to retrieve.</param>
        /// <returns>A list of TagViewModels containing the Tag item details, or null if not found.</returns>
        public async Task<IEnumerable<TagViewModel>> GetTagByIdAsync(int id)
        {
            return await _context.Tags
                 .Where(tag => tag.Id == id) // Id equals
                 .Select(tag => new TagViewModel
                 {
                     Id = tag.Id,
                     Name = tag.Name,
                     Color = tag.Color,
                 })
                 .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Tag items sorted by Id based on the specified sort order.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Tag items by Id.</param>
        /// <returns>A list of TagViewModels sorted by Id according to the specified sort order.</returns>
        public async Task<IEnumerable<TagViewModel>> GetTagsAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending)
        {
            IQueryable<Tag> query = _context.Tags;

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(tag => tag.Id);
            }
            else
            {
                query = query.OrderBy(tag => tag.Id);
            }

            return await query
                .Select(tag => new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all Tag items sorted by Tag.Name based on the specified sort order, and optionally filtered by TagId.
        /// </summary>
        /// <param name="sortOrder">The order in which to sort the Tag items by Tag.Name.</param>
        /// <param name="tagId">Optional. The TagId to filter the Tag items.</param>
        /// <returns>A list of TagViewModels sorted by Tag.Name according to the specified sort order and filtered by TagId.</returns>
        public async Task<IEnumerable<TagViewModel>> GetTagsSortedByNameAsync(Utils.SortOrder sortOrder = Utils.SortOrder.Ascending, int? tagId = null)
        {
            IQueryable<Tag> query = _context.Tags;

            // Apply filtering by TagId if specified
            if (tagId.HasValue)
            {
                query = query.Where(tag => tag.Id == tagId.Value);
            }

            // Apply sorting based on the sortOrder parameter
            if (sortOrder == Utils.SortOrder.Descending)
            {
                query = query.OrderByDescending(tag => tag.Name);
            }
            else
            {
                query = query.OrderBy(tag => tag.Name);
            }

            return await query
                .Select(tag => new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new Tag item.
        /// </summary>
        /// <param name="tagCreateDto">The data transfer object containing Tag item details.</param>
        /// <returns>The added Tag item.</returns>
        public async Task<Tag> CreateTagAsync(TagCreateDto tagCreateDto)
        {
            // Create a new Tag item
            var tag = new Tag
            {
                Name = tagCreateDto.Name,
                Color = tagCreateDto.Color,
                CreatedAt = tagCreateDto.CreatedAt,
            };

            // Add the new Tag item to the database and save changes
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return tag;
        }

        /// <summary>
        /// Updates an existing Tag item with new values.
        /// </summary>
        /// <param name="tagUpdateDto">The data transfer object containing updated Tag item details.</param>
        /// <returns>The updated Tag item, or null if the Tag item does not exist.</returns>
        public async Task<Tag?> UpdateTagAsync(TagUpdateDto tagUpdateDto)
        {
            // Retrieve the Tag item to update
            var tag = await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == tagUpdateDto.Id);

            if (tag == null)
            {
                return null; // Return null if the Tag item does not exist
            }


            // Update the Tag item with new values
            tag.Name = tagUpdateDto.Name;
            tag.Color = tagUpdateDto.Color;
            tag.UpdatedAt = tagUpdateDto.UpdatedAt;

            // Save changes to the database
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();

            return tag;
        }

        /// <summary>
        /// Deletes a Tag item by its Id.
        /// </summary>
        /// <param name="id">The Id of the Tag item to delete.</param>
        /// <returns>True if the Tag item was successfully deleted, or false if the Tag item does not exist.</returns>
        public async Task<bool> DeleteTagByIdAsync(int id)
        {
            // Retrieve the Tag item to delete
            var tag = await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == id);

            if (tag == null)
            {
                return false; // Return false if the Tag item does not exist
            }

            // Remove the Tag item
            _context.Tags.Remove(tag);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }
    }
}