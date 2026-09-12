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
    public class LoginViewModel : ViewModel
    {

        public LoginViewModel()
        {
            GoToRegisterCommand = new LambdaCommand(OnGoToRegisterCommandExecuted);
            GoToCodeCommand = new LambdaCommand(OnGoToCodeCommandExecuted);
            LoginCommand = new LambdaCommand(OnLoginCommandExecuted, CanLoginCommandExecute);
        }
        public ICommand GoToRegisterCommand { get; }
        public ICommand GoToCodeCommand { get; }
        public ICommand LoginCommand { get; }
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

        private string _email;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => Set(ref _statusMessage, value);
        }

        private string _statusColor = "Red";
        public string StatusColor
        {
            get => _statusColor;
            set => Set(ref _statusColor, value);
        }

        private bool CanLoginCommandExecute(object p) => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

        private void OnLoginCommandExecuted(object p)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Profiles.FirstOrDefault(u => u.Email == Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.Password))
                {
                    StatusMessage = "Incorrect email or password!";
                    StatusColor = "Red";
                    return;
                }

                CurrentUser.Profile = user;
                var main = new MainWindow();
                main.Show();
                var log = new Login();
                log.Close();
            }
        }
    }
}