using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.ViewModels.Base;
using ToDo.Views;

namespace ToDo.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly AppDbContext _context;
        public MainViewModel()
        {
            _context = new AppDbContext();
            {
                GoToProfileCommand = new LambdaCommand(OnGoToProfileCommandExecuted);
                GoToAddTaskCommand = new LambdaCommand(OnGoToAddTaskCommandExecuted);
            }
        }

        public ICommand GoToProfileCommand { get; }
        public ICommand GoToAddTaskCommand { get; }

        private void OnGoToProfileCommandExecuted(object p)
        {
            var prof = new SettingWindow();
            prof.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w != prof && !(w is MainWindow))
                {
                    w.Close();
                }
            }
        }

        private void OnGoToAddTaskCommandExecuted(object p)
        { 
            var addT = new AddEditTaskWindow();
            addT.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w != addT && !(w is MainWindow))
                {
                    w.Close();
                }
            }
        }
    }
}
