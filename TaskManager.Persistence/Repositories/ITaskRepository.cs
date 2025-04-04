using TaskManager.Persistence.Models;
using System.Collections.Generic;

namespace TaskManager.Persistence.Repositories
{
    public interface ITaskRepository
    {
        List<TaskItem> GetAllTasks(string userId);
        TaskItem GetTaskById(int id, string userId);
        void SaveTask(TaskItem task);
        void DeleteTask(int id, string userId);
        List<TaskCategory> GetAllCategories(string userId);
        List<TaskPriority> GetAllPriorities(string userId);
        List<TaskState> GetAllStates(string userId);
    }
}