using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.ViewModels.Base;
using ToDo.Views;
using System.Windows;


namespace ToDo.ViewModels
{
    public class AddEditTaskViewModel : ViewModel
    {

        public AddEditTaskViewModel()
        {
            GoToBackCommand = new LambdaCommand(OnGoToBackCommandExecuted);
        }
        public ICommand GoToBackCommand { get; }

        private void OnGoToBackCommandExecuted(object p)
        {
            var mainWindow = new RegisterWindow();
            mainWindow.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w != mainWindow && !(w is MainWindow))
                {
                    w.Close();
                }
            }
        }
    }
}
