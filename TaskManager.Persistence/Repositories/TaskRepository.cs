using TaskManager.Persistence.Data;
using TaskManager.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TaskManager.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TaskItem> GetAllTasks(string userId)
        {
            return _context.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToList();
        }

        public TaskItem GetTaskById(int id, string userId)
        {
            return _context.TaskItems
                .Include(t => t.Category)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .FirstOrDefault(t => t.Id == id && t.UserId == userId);
        }

        public void SaveTask(TaskItem task)
        {
            if (task.Id == 0)
                _context.TaskItems.Add(task);
            else
                _context.TaskItems.Update(task);
            _context.SaveChanges();
        }

        public void DeleteTask(int id, string userId)
        {
            var task = GetTaskById(id, userId);
            if (task != null)
            {
                _context.TaskItems.Remove(task);
                _context.SaveChanges();
            }
        }

        public List<TaskCategory> GetAllCategories(string userId)
        {
            return _context.TaskCategories
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public List<TaskPriority> GetAllPriorities(string userId)
        {
            return _context.TaskPriorities
                .Where(p => p.UserId == userId)
                .ToList();
        }

        public List<TaskState> GetAllStates(string userId)
        {
            return _context.TaskStatuses
                .Where(s => s.UserId == userId)
                .ToList();
        }
    }
}