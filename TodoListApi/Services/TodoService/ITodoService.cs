using TodoListApi.Dto;
using TodoListApi.Helpers;
using TodoListApi.Models;
using TodoListApi.ViewModels;

namespace TodoListApi.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoViewModel>> GetTodoByIdAsync(int id);
        Task<IEnumerable<TodoViewModel>> GetTodosAsync(Utils.SortOrder sortOrder);
        Task<IEnumerable<TodoViewModel>> GetTodosSortedByDueDateAsync(Utils.SortOrder sortOrder, Utils.Period? period);
        Task<IEnumerable<TodoViewModel>> GetTodosSortedByPriorityIdAsync(Utils.SortOrder sortOrder, int? priorityId);
        Task<IEnumerable<TodoViewModel>> GetTodosSortedByPriorityNameAsync(Utils.SortOrder sortOrder, int? priorityId);
        Task<IEnumerable<TodoViewModel>> GetTodosSortedByStatusIdAsync(Utils.SortOrder sortOrder, int? statusId);
        Task<IEnumerable<TodoViewModel>> GetTodosSortedByStatusNameAsync(Utils.SortOrder sortOrder, int? statusId);
        Task<IEnumerable<TodoViewModel>> GetTodosSortedByTagsCountAsync(Utils.SortOrder sortOrder, int? tagId);
        Task<Todo> CreateTodoAsync(TodoCreateDto todoCreateDto);
        Task<Todo?> UpdateTodoAsync(TodoUpdateDto todoUpdateDto);
        Task<bool> DeleteTodoByIdAsync(int id);
    }
}