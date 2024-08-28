using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Models;
using TodoListApi.ViewModels;

namespace TodoListApi.Services
{
    public interface IPriorityService
    {
        Task<IEnumerable<PriorityViewModel>> GetPriorityByIdAsync(int id);
        Task<IEnumerable<PriorityViewModel>> GetPrioritiesAsync(Utils.SortOrder sortOrder);
        Task<IEnumerable<PriorityViewModel>> GetPrioritiesSortedByNameAsync(Utils.SortOrder sortOrder, int? priorityId);
        Task<Priority> CreatePriorityAsync(PriorityCreateDto priorityCreateDto);
        Task<Priority?> UpdatePriorityAsync(PriorityUpdateDto priorityUpdateDto);
        Task<bool> DeletePriorityByIdAsync(int id);
    }
}