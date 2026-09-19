using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDo.Infastructure.Commands;
using ToDo.Models;
using ToDo.Services;
using ToDo.ViewModels.Base;


namespace ToDo.ViewModels
{
    public class AddEditTaskViewModel : ViewModel
    {
        private readonly NavigationService _navigation = new NavigationService();
        private readonly TaskService _tasks = new TaskService();
        public static UserTask TaskToEdit { get; set; }
        public AddEditTaskViewModel()
        {
            GoToBackCommand = new LambdaCommand(_ => { TaskToEdit = null; _navigation.NavigateToMain(); });
            GoToSaveCommand = new AsyncCommand(SaveAsync, CanSave);
            if (TaskToEdit != null)
                LoadFromTask(TaskToEdit);
        }
        public ICommand GoToBackCommand { get; }
        public ICommand GoToSaveCommand { get; }


        public ObservableCollection<string> Priorities { get; } = new ObservableCollection<string> { "Low", "Medium", "High" };
        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>
            {"Work", "Personal", "Study", "Home", "Shopping", "Health", "Other" };

        private string _title;
        public string Title { get => _title; set => Set(ref _title, value); }

        private string _category;
        public string Category { get => _category; set => Set(ref _category, value); }

        private string _selectedPriority = "Medium";
        public string SelectedPriority { get => _selectedPriority; set => Set(ref _selectedPriority, value); }
        private string _selectedCategory = "Other";
        public string SelectedCategory { get => _selectedCategory; set => Set(ref _selectedCategory, value); }

        private DateTime? _dueDate;
        public DateTime? DueDate { get => _dueDate; set => Set(ref _dueDate, value); }

        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set => Set(ref _statusMessage, value); }

        private string _statusColor = "Red";
        public string StatusColor { get => _statusColor; set => Set(ref _statusColor, value); }

        public string WindowTitle => TaskToEdit == null ? "Add task" : "Edit task";
        private bool CanSave() => !string.IsNullOrWhiteSpace(Title);

        private async Task SaveAsync()
        {
            if (LoginService.CurrentProfile == null)
            {
                StatusMessage = "You are not logged in.";
                StatusColor = "Red";
                return;
            }

            int priority = SelectedPriority == "Low" ? 1
                         : SelectedPriority == "High" ? 3
                         : 2;

                if (DueDate == null || DueDate.Value.Date < DateTime.Today)
                {
                    StatusMessage = "Due date cannot be in the past or empty.";
                    StatusColor = "Red";
                    return;
                }

            if (TaskToEdit == null)
            {
                var task = new UserTask
                {
                    Title = Title,
                    Category = SelectedCategory,
                    Property = priority,
                    DueDate = DueDate,
                    IsCompleted = false,
                    ProfileId = LoginService.CurrentProfile.IdProfile
                };

                await _tasks.AddTaskAsync(task);
            }
            else
            {
                TaskToEdit.Title = Title;
                TaskToEdit.Category = SelectedCategory;
                TaskToEdit.Property = priority;
                TaskToEdit.DueDate = DueDate;

                await _tasks.UpdateTaskAsync(TaskToEdit);
            }

            TaskToEdit = null;
            _navigation.NavigateToMain();
        }

        private void LoadFromTask(UserTask task)
        {
            Title = task.Title;
            SelectedCategory = task.Category;
            DueDate = task.DueDate;
            SelectedPriority = task.Property == 1 ? "Low"
                             : task.Property == 3 ? "High"
                             : "Medium";
        }
    }
}
