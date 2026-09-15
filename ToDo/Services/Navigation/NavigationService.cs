using System.Linq;
using System.Windows;
using ToDo.Views;

namespace ToDo.Services
{
    public class NavigationService
    {
        public void NavigateToLogin()
        {
            var window = new Login();
            window.Show();
            CloseOthers(window);
        }

        public void NavigateToRegister()
        {
            var window = new RegisterWindow();
            window.Show();
            CloseOthers(window);
        }

        public void NavigateToCode()
        {
            var window = new CodeWindow();
            window.Show();
            CloseOthers(window);
        }

        public void NavigateToMain()
        {
            var window = new MainWindow();
            window.Show();
            CloseOthers(window);
        }

        public void NavigateToProfile()
        {
            var window = new SettingWindow();
            window.Show();
            CloseOthers(window);
        }

        public void NavigateToAddTask()
        {
            var window = new AddEditTaskWindow();
            window.Show();
            CloseOthers(window);
        }

        private static void CloseOthers(Window current)
        {
            foreach (var w in Application.Current.Windows.Cast<Window>().ToList())
            {
                if (w != current) w.Close();
            }
        }
    }
}