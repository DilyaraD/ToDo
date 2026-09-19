using System.Threading.Tasks;
using System.Windows.Input;
using ToDo.Infastructure.Commands;
using ToDo.Services;
using ToDo.ViewModels.Base;

namespace ToDo.ViewModels
{
    public class LoginViewModel : ViewModel
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly NavigationService _navigation = new NavigationService();
        public LoginViewModel()
        {
            GoToRegisterCommand = new LambdaCommand(_ => _navigation.NavigateToRegister());
            GoToCodeCommand = new LambdaCommand(_ => _navigation.NavigateToCode());
            LoginCommand = new AsyncCommand(LoginAsync, CanLogin);
        }

        public ICommand GoToRegisterCommand { get; }
        public ICommand GoToCodeCommand { get; }
        public ICommand LoginCommand { get; }

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

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

        private async Task LoginAsync()
        {
            StatusMessage = "Checking...";
            StatusColor = "Blue";

            var error = await _loginService.LoginAsync(Email, Password);

            if (error != null)
            {
                StatusMessage = error;
                StatusColor = "Red";
                return;
            }

            _navigation.NavigateToMain();
        }
    }
}