using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToDo.Infastructure.Commands
{
    public class AsyncCommandT<T> : ICommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isExecuting;

        public AsyncCommandT(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
            => !_isExecuting && (_canExecute?.Invoke((T)parameter) ?? true);

        public async void Execute(object parameter)
        {
            _isExecuting = true;
            try { await _execute((T)parameter); }
            finally { _isExecuting = false; }
        }
    }
}
