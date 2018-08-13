using System;
using System.Windows.Input;

namespace ContestMeter.UI
{
    public delegate void ExecuteCommandDelegate<T>(T arg);
    public delegate bool CanExecuteCommandDelegate<T>(T arg);

    public class DelegateCommand<T> : ICommand
    {
        ExecuteCommandDelegate<T> _executeCommand;
        CanExecuteCommandDelegate<T> _canExecuteCommand;

        public DelegateCommand(ExecuteCommandDelegate<T> executeCommand) : this( executeCommand, x=>true )
        {

        }

        public DelegateCommand(ExecuteCommandDelegate<T> executeCommand, CanExecuteCommandDelegate<T> canExecuteCommand)
        {
            Check.NotNull(executeCommand, "executeCommand");
            Check.NotNull(canExecuteCommand, "canExecuteCommand");
            
            _executeCommand = executeCommand;
            _canExecuteCommand = canExecuteCommand;
        }

        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter)
        {
            return _canExecuteCommand((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            _executeCommand((T)parameter);
        }

        #endregion
    }
}
