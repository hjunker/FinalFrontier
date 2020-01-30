using System;
using System.Windows.Input;


namespace FinalFrontier
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> executeHandler;
        private readonly Predicate<object> canExecuteHandler;

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("Execute could not be null.");
            executeHandler = execute;
            canExecuteHandler = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute (object parameter)
        {
            executeHandler(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteHandler == null)
                return true;
            return canExecuteHandler(parameter);
        }
    }
}
