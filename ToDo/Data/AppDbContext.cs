using System.Data.Entity;
using SQLite.CodeFirst;
using ToDo.Models;

namespace ToDo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DefaultConnection") { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder mb)
        {
            mb.Entity<Profile>().HasKey(p => p.IdProfile);
            mb.Entity<UserTask>().HasKey(t => t.IdTask);

            mb.Entity<Profile>()
              .HasMany(p => p.UserTasks)
              .WithRequired(t => t.Profile)
              .HasForeignKey(t => t.ProfileId);

            Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<AppDbContext>(mb));
        }
    }
}