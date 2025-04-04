using TaskManager.Persistence.Models;
using TaskManager.Persistence.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace TaskManager.Business.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public List<TaskItem> GetAllTasks(string userId)
        {
            return _taskRepository.GetAllTasks(userId);
        }

        public TaskItem GetTaskById(int id, string userId)
        {
            return _taskRepository.GetTaskById(id, userId);
        }

        public void SaveTask(TaskItem task, string userId)
        {
            task.UserId = userId;
            EnsureDefaults(task, userId);
            _taskRepository.SaveTask(task);
        }

        public void DeleteTask(int id, string userId)
        {
            _taskRepository.DeleteTask(id, userId);
        }

        public List<TaskCategory> GetAllCategories(string userId)
        {
            EnsureDefaultCategories(userId);
            return _taskRepository.GetAllCategories(userId);
        }

        public List<TaskPriority> GetAllPriorities(string userId)
        {
            EnsureDefaultPriorities(userId);
            return _taskRepository.GetAllPriorities(userId);
        }

        public List<TaskState> GetAllStates(string userId)
        {
            EnsureDefaultStates(userId);
            return _taskRepository.GetAllStates(userId);
        }

        public int GetUrgentDeadlinesCount(string userId)
        {
            var tasks = _taskRepository.GetAllTasks(userId);
            return tasks.Count(t => t.Date <= DateTime.Now.AddDays(3) && t.Status?.Name != "Xong");
        }

        private void EnsureDefaults(TaskItem task, string userId)
        {
            if (task.CategoryId == null)
            {
                var defaultCategory = GetAllCategories(userId).FirstOrDefault(c => c.Name == "Công việc");
                if (defaultCategory != null) task.CategoryId = defaultCategory.Id;
            }
            if (task.PriorityId == null)
            {
                var defaultPriority = GetAllPriorities(userId).FirstOrDefault(p => p.Name == "Thấp");
                if (defaultPriority != null) task.PriorityId = defaultPriority.Id;
            }
            if (task.StatusId == null)
            {
                var defaultState = GetAllStates(userId).FirstOrDefault(s => s.Name == "Chưa làm");
                if (defaultState != null) task.StatusId = defaultState.Id;
            }
        }

        private void EnsureDefaultCategories(string userId)
        {
            var categories = _taskRepository.GetAllCategories(userId);
            if (!categories.Any(c => c.Name == "Công việc"))
                _taskRepository.SaveTask(new TaskItem { Category = new TaskCategory { Name = "Công việc", UserId = userId } });
            if (!categories.Any(c => c.Name == "Cá nhân"))
                _taskRepository.SaveTask(new TaskItem { Category = new TaskCategory { Name = "Cá nhân", UserId = userId } });
        }

        private void EnsureDefaultPriorities(string userId)
        {
            var priorities = _taskRepository.GetAllPriorities(userId);
            if (!priorities.Any(p => p.Name == "Thấp"))
                _taskRepository.SaveTask(new TaskItem { Priority = new TaskPriority { Name = "Thấp", Color = "green", UserId = userId } });
            if (!priorities.Any(p => p.Name == "Cao"))
                _taskRepository.SaveTask(new TaskItem { Priority = new TaskPriority { Name = "Cao", Color = "red", UserId = userId } });
        }

        private void EnsureDefaultStates(string userId)
        {
            var states = _taskRepository.GetAllStates(userId);
            if (!states.Any(s => s.Name == "Chưa làm"))
                _taskRepository.SaveTask(new TaskItem { Status = new TaskState { Name = "Chưa làm", UserId = userId } });
            if (!states.Any(s => s.Name == "Đang làm"))
                _taskRepository.SaveTask(new TaskItem { Status = new TaskState { Name = "Đang làm", UserId = userId } });
            if (!states.Any(s => s.Name == "Xong"))
                _taskRepository.SaveTask(new TaskItem { Status = new TaskState { Name = "Xong", UserId = userId } });
        }
    }
}