using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.Services;
using ToDo.ViewModels.Base;
using ToDo.Views;

namespace ToDo.ViewModels
{
    public class SettingViewModel : ViewModel
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly NavigationService _navigation = new NavigationService();
        public SettingViewModel() 
        {
            GoToMainCommand = new LambdaCommand(_ => _navigation.NavigateToMain());
            GoToSaveCommand = new AsyncCommand(SaveAsync, CanSave);
            LogoutCommand = new LambdaCommand(_ => Logout());
        }
        public ICommand GoToMainCommand { get; }
        public ICommand GoToSaveCommand { get; }
        public ICommand LogoutCommand { get; }

        public string Login => LoginService.CurrentProfile?.Login ?? "—";

        private string _currentPassword;
        public string CurrentPassword { get => _currentPassword; set => Set(ref _currentPassword, value); }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set => Set(ref _newPassword, value); }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set => Set(ref _confirmPassword, value); }

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set => Set(ref _statusMessage, value); }

        private string _statusColor = "Red";
        public string StatusColor { get => _statusColor; set => Set(ref _statusColor, value); }

        private void Logout()
        {
            _loginService.Logout();
            _navigation.NavigateToLogin();
        }

        private bool CanSave()
            => !string.IsNullOrWhiteSpace(CurrentPassword) && !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmPassword);

        private async Task SaveAsync()
        {
            if (NewPassword.Length < 6 || NewPassword.Length > 100)
            {
                StatusMessage = "The password must contain at least 6 characters and must not be longer than 100.";
                StatusColor = "Red";
                return;
            }
            if (NewPassword != ConfirmPassword)
            {
                StatusMessage = "The passwords do not match.";
                StatusColor = "Red";
                return;
            }

            StatusMessage = "Saving...";
            StatusColor = "Blue";

            var error = await _loginService.ChangePasswordAsync(CurrentPassword, NewPassword);
            if (error != null)
            {
                StatusMessage = error;
                StatusColor = "Red";
                return;
            }

            StatusMessage = "Password changed successfully!";
            StatusColor = "Green";

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

    }
}
