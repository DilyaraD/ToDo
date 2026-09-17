using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Data;
using ToDo.Models;

namespace ToDo.Services
{
    public class TaskService
    {
        public async Task<List<UserTask>> GetTasksAsync(int profileId)
        {
            using (var db = new AppDbContext())
            {
                return await db.UserTasks
                    .Where(t => t.ProfileId == profileId)
                    .OrderBy(t => t.IsCompleted)
                    .ThenBy(t => t.DueDate)
                    .ToListAsync();
            }
        }

        public async Task AddTaskAsync(UserTask task)
        {
            using (var db = new AppDbContext())
            {
                db.UserTasks.Add(task);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            using (var db = new AppDbContext())
            {
                db.Entry(task).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            using (var db = new AppDbContext())
            {
                var task = await db.UserTasks.FindAsync(taskId);
                if (task != null)
                {
                    db.UserTasks.Remove(task);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task SetCompletedAsync(int taskId, bool isCompleted)
        {
            using (var db = new AppDbContext())
            {
                var task = await db.UserTasks.FindAsync(taskId);
                if (task != null)
                {
                    task.IsCompleted = isCompleted;
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
