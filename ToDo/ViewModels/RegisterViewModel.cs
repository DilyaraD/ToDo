using System;
using System.Collections.Generic;
using static BCrypt.Net.BCrypt;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.ViewModels.Base;
using ToDo.Models;
using ToDo.Views;

namespace ToDo.ViewModels
{
    public class RegisterViewModel : ViewModel
    {
        private readonly AppDbContext _context;
        public RegisterViewModel()
        {
            _context = new AppDbContext();
            _context.Database.CreateIfNotExists();

            RegisterCommand = new LambdaCommand(OnRegisterCommandExecuted, CanRegisterCommandExecute);
            GoToLoginCommand = new LambdaCommand(OnGoToLoginCommandExecuted);
        }

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

        #region Команды
        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        #endregion

        private bool CanRegisterCommandExecute(object p)
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Login) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !IsLoading;
        }

        private async void OnRegisterCommandExecuted(object p)
        {
            if (!IsValidEmail(Email))
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

            try
            {
                var userExistingEmail = _context.Profiles.FirstOrDefault(u => u.Email == Email);
                if (userExistingEmail != null)
                {
                    StatusMessage = "A user with this email address is already registered.";
                    StatusColor = "Red";
                    return;
                }

                var userExistingLogin = _context.Profiles.FirstOrDefault(u => u.Login == Login);
                if (userExistingLogin != null)
                {
                    StatusMessage = "A user with this login has already been registered.";
                    StatusColor = "Red";
                    return;
                }

                StatusMessage = "Account creation...";
                StatusColor = "Blue";

                var newProfile = new Profile
                {
                    Login = Login,
                    Email = Email,
                    Password = HashPassword(Password)
                };
                _context.Profiles.Add(newProfile);
                await _context.SaveChangesAsync();

                StatusMessage = "Registration is successful! Now log in to the system.";
                StatusColor = "Green";

                await Task.Delay(1500);

                if (p is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                StatusColor = "Red";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        private void OnGoToLoginCommandExecuted(object p)
        {
            var loginWindow = new Login();
            loginWindow.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w != loginWindow && !(w is MainWindow))
                {
                    w.Close();
                }
            }
        }
    }
}