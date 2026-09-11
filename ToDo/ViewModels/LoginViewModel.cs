using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDo.Infastructure.Commands;
using ToDo.ViewModels.Base;
using ToDo.Views;

namespace ToDo.ViewModels
{
    public class LoginViewModel : ViewModel
    {

        public LoginViewModel()
        {
            GoToRegisterCommand = new LambdaCommand(OnGoToRegisterCommandExecuted);
            GoToCodeCommand = new LambdaCommand(OnGoToCodeCommandExecuted);
        }
        public ICommand GoToRegisterCommand { get; }
        public ICommand GoToCodeCommand { get; }
        private void OnGoToRegisterCommandExecuted(object p)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        private void OnGoToCodeCommandExecuted(object p)
        {
            var codeWindow = new CodeWindow();
            codeWindow.ShowDialog();
        }
    }
}