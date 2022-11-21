using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM
{
    #region No parameter DelegateCommand
    public sealed class RelayCommand : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;
        [DebuggerHidden()]

        public RelayCommand(Action execute) : this(execute, () => true)
        {
        }
        [DebuggerHidden()]

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute()
        {
            return _canExecute();
        }
        [DebuggerHidden()]
        public void Execute()
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #region ICommand
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }
        void ICommand.Execute(object parameter)
        {
            Execute();
        }
        #endregion
    }
    #endregion

    #region Parameter DelegateCommand
    public sealed class RelayCommand<T> : ICommand
    {
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        private static readonly bool IS_VALUE_TYPE;

        static RelayCommand()
        {
            IS_VALUE_TYPE = typeof(T).IsValueType;
        }

        [DebuggerHidden()]
        public RelayCommand(Action<T> execute) : this(execute, o => true)
        {
        }
        [DebuggerHidden()]
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(T parameter)
        {
            return _canExecute(parameter);
        }
        [DebuggerHidden()]
        public void Execute(T parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #region ICommand
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(Cast(parameter));
        }

        void ICommand.Execute(object parameter)
        {
            Execute(Cast(parameter));
        }
        #endregion

        /// <summary>
        /// convert parameter value
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private T Cast(object parameter)
        {
            if (parameter == null && IS_VALUE_TYPE)
            {
                return default(T);
            }
            return (T)parameter;
        }
    }

    #endregion
}
