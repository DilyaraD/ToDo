using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using ToDo.Models;

namespace ToDo.Data
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }
    }
}
