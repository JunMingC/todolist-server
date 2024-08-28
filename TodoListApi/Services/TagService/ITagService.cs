using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Models;
using TodoListApi.ViewModels;

namespace TodoListApi.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagViewModel>> GetTagByIdAsync(int id);
        Task<IEnumerable<TagViewModel>> GetTagsAsync(Utils.SortOrder sortOrder);
        Task<IEnumerable<TagViewModel>> GetTagsSortedByNameAsync(Utils.SortOrder sortOrder, int? tagId);
        Task<Tag> CreateTagAsync(TagCreateDto tagCreateDto);
        Task<Tag?> UpdateTagAsync(TagUpdateDto tagUpdateDto);
        Task<bool> DeleteTagByIdAsync(int id);
    }
}