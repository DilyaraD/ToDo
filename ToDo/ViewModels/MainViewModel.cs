using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDo.Data;
using ToDo.Infastructure.Commands;
using ToDo.Models;
using ToDo.Services;
using ToDo.ViewModels.Base;
using ToDo.Views;

namespace ToDo.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly LoginService _loginService = new LoginService();
        private readonly NavigationService _navigation = new NavigationService();
        private readonly TaskService _tasks = new TaskService();
        private readonly List<UserTask> _allTasks = new List<UserTask>();

        public MainViewModel()
        {
            GoToProfileCommand = new LambdaCommand(_=> _navigation.NavigateToProfile());
            GoToAddTaskCommand = new LambdaCommand(_ => _navigation.NavigateToAddTask());
            LogoutCommand = new LambdaCommand(_ => Logout());
            ResetFilterCommand = new LambdaCommand(_ => ResetFilter());
            GoToEditCommand = new LambdaCommand(EditTask);
            DeleteTaskCommand = new AsyncCommandT<object>(DeleteTaskAsync);
            GoToUpdateCommand = new AsyncCommandT<UserTask>(ToggleCompletedAsync);

            _ = LoadTasksAsync();
        }
        public string TasksSummary => $"{_allTasks.Count(t => t.IsCompleted)}/{_allTasks.Count} completed";
        public ObservableCollection<UserTask> Tasks { get; } = new ObservableCollection<UserTask>();
        public ICommand GoToProfileCommand { get; }
        public ICommand GoToAddTaskCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand GoToUpdateCommand { get; }
        public ICommand GoToEditCommand { get; }
        public ICommand ResetFilterCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { if (Set(ref _searchText, value)) ApplyFilter(); }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set { if (Set(ref _selectedCategory, value)) ApplyFilter(); }
        }

        private string _selectedPriority;
        public string SelectedPriority
        {
            get => _selectedPriority;
            set { if (Set(ref _selectedPriority, value)) ApplyFilter(); }
        }
        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>
            {"Work", "Personal", "Study", "Home", "Shopping", "Health", "Other" };
        public ObservableCollection<string> Priorities { get; } = new ObservableCollection<string> { "Low", "Medium", "High" };

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set => Set(ref _statusMessage, value); }

        private void Logout()
        {
            _loginService.Logout();
            _navigation.NavigateToLogin();
        }

        public async Task LoadTasksAsync()
        {
            if (LoginService.CurrentProfile == null) return;

            var list = await _tasks.GetTasksAsync(LoginService.CurrentProfile.IdProfile);

            _allTasks.Clear();
            _allTasks.AddRange(list);

            Categories.Clear();
            foreach (var c in _allTasks
                        .Select(t => t.Category)
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Distinct())
            {
                Categories.Add(c);
            }

            ApplyFilter();
        }
        private void ApplyFilter()
        {
            Tasks.Clear();

            var query = _allTasks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(t => t.Title != null
                                      && t.Title.ToLower().Contains(SearchText.ToLower()));

            if (!string.IsNullOrWhiteSpace(SelectedCategory))
                query = query.Where(t => t.Category == SelectedCategory);

            if (!string.IsNullOrWhiteSpace(SelectedPriority))
            {
                int p = SelectedPriority == "Low" ? 1
                      : SelectedPriority == "Medium" ? 2
                      : 3;
                query = query.Where(t => t.Property == p);
            }

            foreach (var t in query)
                Tasks.Add(t);
        }

        private void ResetFilter()
        {
            SearchText = string.Empty;
            SelectedCategory = null;
            SelectedPriority = null;
        }

        private async Task ToggleCompletedAsync(UserTask task)
        {
            if (task == null) return;
            await _tasks.SetCompletedAsync(task.IdTask, task.IsCompleted);
            OnPropertyChanged(nameof(TasksSummary));
        }
        private void EditTask(object param)
        {
            if (!(param is UserTask task)) return;
            AddEditTaskViewModel.TaskToEdit = task;
            _navigation.NavigateToAddTask();
        }

        private async Task DeleteTaskAsync(object param)
        {
            if (!(param is UserTask task)) return;
            await _tasks.DeleteTaskAsync(task.IdTask);
            _allTasks.Remove(task);
            Tasks.Remove(task);
        }
    }
}
