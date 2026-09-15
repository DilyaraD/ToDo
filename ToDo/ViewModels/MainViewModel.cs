using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.Services;
using ToDo.ViewModels.Base;
using ToDo.Views;

namespace ToDo.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly NavigationService _navigation = new NavigationService();
        public MainViewModel()
        {
            GoToProfileCommand = new LambdaCommand(_=> _navigation.NavigateToProfile());
            GoToAddTaskCommand = new LambdaCommand(_ => _navigation.NavigateToAddTask());
            LogoutCommand = new LambdaCommand(_ => Logout());
        }

        public ICommand GoToProfileCommand { get; }
        public ICommand GoToAddTaskCommand { get; }
        public ICommand LogoutCommand { get; }

        private void Logout()
        {
            _loginService.Logout();
            _navigation.NavigateToLogin();
        }
    }
}
