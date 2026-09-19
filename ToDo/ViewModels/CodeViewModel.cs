using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDo.Infastructure.Commands;
using ToDo.Services;
using ToDo.ViewModels.Base;

namespace ToDo.ViewModels
{
    public class CodeViewModel : ViewModel
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly NavigationService _navigation = new NavigationService();

        public CodeViewModel()
        {
            GoToBackCommand =  new LambdaCommand(_ => _navigation.NavigateToLogin());
            GoToSendCodeCommand = new AsyncCommand(SendCodeAsync, CanSendCode);
            GoToResetCommand = new AsyncCommand(ResetAsync, CanReset);
        }

        public ICommand GoToBackCommand { get; }
        public ICommand GoToSendCodeCommand { get; }
        public ICommand GoToResetCommand { get; }

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set => Set(ref _statusMessage, value); }

        private string _statusColor = "Red";
        public string StatusColor { get => _statusColor; set => Set(ref _statusColor, value); }

        private string _email;
        public string Email { get => _email; set => Set(ref _email, value); }

        private string _code;
        public string Code { get => _code; set => Set(ref _code, value); }

        private string _newPassword;
        public string NewPassword { get => _newPassword; set => Set(ref _newPassword, value); }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set => Set(ref _confirmPassword, value); }
        private bool _isCodeSent;
        public bool IsCodeSent
        {
            get => _isCodeSent;
            set
            {
                if (Set(ref _isCodeSent, value))
                {
                    OnPropertyChanged(nameof(IsEmailStep));
                    OnPropertyChanged(nameof(IsResetStep));
                }
            }
        }

        public bool IsEmailStep => !IsCodeSent;
        public bool IsResetStep => IsCodeSent;
        public string WindowTitle => IsEmailStep ? "Send code" : "Reset password";

        private bool CanSendCode() => !string.IsNullOrWhiteSpace(Email);

        private bool CanReset()
            => !string.IsNullOrWhiteSpace(Code)
            && !string.IsNullOrWhiteSpace(NewPassword)
            && !string.IsNullOrWhiteSpace(ConfirmPassword);

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        private async Task SendCodeAsync()
        {
            StatusMessage = "Sending code...";
            StatusColor = "Blue";

            var error = await _loginService.SendResetCodeAsync(Email);
            if (error != null)
            {
                StatusMessage = error;
                StatusColor = "Red";
                return;
            }

            StatusMessage = "Code sent! Check your email.";
            StatusColor = "Green";

            IsCodeSent = true;
        }

        private async Task ResetAsync()
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

            StatusMessage = "Checking...";
            StatusColor = "Blue";

            var error = await _loginService.ResetPasswordAsync(Email, Code, NewPassword);
            if (error != null)
            {
                StatusMessage = error;
                StatusColor = "Red";
                return;
            }

            StatusMessage = "Password reset! Now log in.";
            StatusColor = "Green";

            await Task.Delay(1500);
            _navigation.NavigateToLogin();
        }
    }
}
