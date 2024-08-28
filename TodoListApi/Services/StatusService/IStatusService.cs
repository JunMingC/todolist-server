using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Models;
using TodoListApi.ViewModels;

namespace TodoListApi.Services
{
    public interface IStatusService
    {
        Task<IEnumerable<StatusViewModel>> GetStatusByIdAsync(int id);
        Task<IEnumerable<StatusViewModel>> GetStatusesAsync(Utils.SortOrder sortOrder);
        Task<IEnumerable<StatusViewModel>> GetStatusesSortedByNameAsync(Utils.SortOrder sortOrder, int? statusId);
        Task<Status> CreateStatusAsync(StatusCreateDto statusCreateDto);
        Task<Status?> UpdateStatusAsync(StatusUpdateDto statusUpdateDto);
        Task<bool> DeleteStatusByIdAsync(int id);
    }
}