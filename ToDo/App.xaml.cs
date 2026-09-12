using System;
using System.IO;
using System.Windows;
using ToDo.Data;

namespace ToDo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SQLitePCL.Batteries_V2.Init();

            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ToDo.db");
            if (File.Exists(dbPath) && new FileInfo(dbPath).Length == 0)
            {
                File.Delete(dbPath);
            }

            using (var db = new AppDbContext())
            {
                db.Database.Initialize(force: true);
            }
        }
    }
}