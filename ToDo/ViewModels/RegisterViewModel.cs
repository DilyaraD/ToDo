using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.Models;
using ToDo.Services;
using ToDo.ViewModels.Base;
using ToDo.Views;
using static BCrypt.Net.BCrypt;

namespace ToDo.ViewModels
{
    public class RegisterViewModel : ViewModel
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly NavigationService _navigation = new NavigationService();
        public RegisterViewModel()
        {
            RegisterCommand = new AsyncCommand(RegisterAsync, CanRegister);
            GoToLoginCommand = new LambdaCommand(_ => _navigation.NavigateToLogin());
        }

        #region Команды
        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        #endregion

        private string _email;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        private string _login;
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => Set(ref _confirmPassword, value);
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

        public string RegisterButtonText => IsLoading ? "Register..." : "SIGN UP";
        public bool IsRegisterEnabled => !IsLoading;


        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (Set(ref _isLoading, value))
                {
                    OnPropertyChanged(nameof(RegisterButtonText));
                    OnPropertyChanged(nameof(IsRegisterEnabled));
                }
            }
        }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Login) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !IsLoading;
        }

        private async Task RegisterAsync()
        {
            if (!_loginService.IsValidEmail(Email))
            {
                StatusMessage = "Enter a valid email (for example: user@mail.com).";
                StatusColor = "Red";
                return;
            }

            if (Login.Length < 3 || Login.Length > 25)
            {
                StatusMessage = "The login must contain at least 3 characters and must not be longer than 25.";
                StatusColor = "Red";
                return;
            }

            if (Password.Length < 6 || Password.Length > 100)
            {
                StatusMessage = "The password must contain at least 6 characters and must not be longer than 100.";
                StatusColor = "Red";
                return;
            }

            if (Password != ConfirmPassword)
            {
                StatusMessage = "The passwords do not match.";
                StatusColor = "Red";
                return;
            }

            IsLoading = true;
            StatusMessage = "Data verification...";
            StatusColor = "Blue";

            var error = await _loginService.RegisterAsync(Login, Email, Password);

            if (error != null)
            {
                StatusMessage = error;
                StatusColor = "Red";
                IsLoading = false;
                return;
            }

            StatusMessage = "Registration is successful! Now log in to the system.";
            StatusColor = "Green";

                await Task.Delay(1500);
            _navigation.NavigateToLogin();
        }         
    }
}