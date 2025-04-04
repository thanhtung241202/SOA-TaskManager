using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Business.Services;
using TaskManager.Persistence.Models;
using System.Security.Claims;

namespace TaskManager.Presentation.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        public IActionResult ListTasks(string? categoryFilter = null, string? priorityFilter = null, string? statusFilter = null, string? sortBy = null)
        {
            string userId = GetCurrentUserId();
            var tasks = _taskService.GetAllTasks(userId);

            // Thêm thông báo deadline
            int urgentCount = _taskService.GetUrgentDeadlinesCount(userId);
            if (urgentCount > 0)
            {
                TempData["Message"] = $"Bạn có {urgentCount} công việc cần hoàn thành trong vòng 3 ngày tới!";
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                var category = _taskService.GetAllCategories(userId).FirstOrDefault(c => c.Name == categoryFilter);
                int categoryId = category?.Id ?? 0;
                tasks = tasks.Where(t => t.CategoryId == categoryId).ToList();
            }
            if (!string.IsNullOrEmpty(priorityFilter))
            {
                var priority = _taskService.GetAllPriorities(userId).FirstOrDefault(p => p.Name == priorityFilter);
                int priorityId = priority?.Id ?? 0;
                tasks = tasks.Where(t => t.PriorityId == priorityId).ToList();
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var state = _taskService.GetAllStates(userId).FirstOrDefault(s => s.Name == statusFilter);
                int statusId = state?.Id ?? 0;
                tasks = tasks.Where(t => t.StatusId == statusId).ToList();
            }

            // Thêm sắp xếp
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (sortBy == "Priority")
                {
                    tasks = tasks.OrderBy(t => t.PriorityId ?? int.MaxValue).ToList();
                }
                else if (sortBy == "Date")
                {
                    tasks = tasks.OrderBy(t => t.Date).ToList();
                }
            }

            ViewBag.Categories = _taskService.GetAllCategories(userId);
            ViewBag.Priorities = _taskService.GetAllPriorities(userId);
            ViewBag.States = _taskService.GetAllStates(userId);
            ViewBag.CurrentCategoryFilter = categoryFilter;
            ViewBag.CurrentPriorityFilter = priorityFilter;
            ViewBag.CurrentStatusFilter = statusFilter;
            ViewBag.CurrentSortBy = sortBy;

            return View(tasks);
        }

        public IActionResult CreateTask()
        {
            string userId = GetCurrentUserId();
            ViewBag.Categories = _taskService.GetAllCategories(userId);
            ViewBag.Priorities = _taskService.GetAllPriorities(userId);
            ViewBag.States = _taskService.GetAllStates(userId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTask(TaskItem task)
        {
            string userId = GetCurrentUserId();
            if (ModelState.IsValid)
            {
                _taskService.SaveTask(task, userId);
                TempData["Message"] = "Công việc đã được thêm thành công!";
                return RedirectToAction("ListTasks");
            }
            ViewBag.Categories = _taskService.GetAllCategories(userId);
            ViewBag.Priorities = _taskService.GetAllPriorities(userId);
            ViewBag.States = _taskService.GetAllStates(userId);
            return View(task);
        }

        public IActionResult EditTask(int id)
        {
            string userId = GetCurrentUserId();
            var task = _taskService.GetTaskById(id, userId);
            if (task == null)
            {
                return NotFound();
            }
            ViewBag.Categories = _taskService.GetAllCategories(userId);
            ViewBag.Priorities = _taskService.GetAllPriorities(userId);
            ViewBag.States = _taskService.GetAllStates(userId);
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTask(TaskItem task)
        {
            string userId = GetCurrentUserId();
            if (ModelState.IsValid)
            {
                _taskService.SaveTask(task, userId);
                TempData["Message"] = "Công việc đã được cập nhật thành công!";
                return RedirectToAction("ListTasks");
            }
            ViewBag.Categories = _taskService.GetAllCategories(userId);
            ViewBag.Priorities = _taskService.GetAllPriorities(userId);
            ViewBag.States = _taskService.GetAllStates(userId);
            return View(task);
        }

        public IActionResult DetailsTask(int id)
        {
            string userId = GetCurrentUserId();
            var task = _taskService.GetTaskById(id, userId);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        public IActionResult DeleteTask(int id)
        {
            string userId = GetCurrentUserId();
            var task = _taskService.GetTaskById(id, userId);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost, ActionName("DeleteTask")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTaskConfirmed(int id)
        {
            string userId = GetCurrentUserId();
            _taskService.DeleteTask(id, userId);
            TempData["Message"] = "Công việc đã được xóa thành công!";
            return RedirectToAction("ListTasks");
        }
    }
}