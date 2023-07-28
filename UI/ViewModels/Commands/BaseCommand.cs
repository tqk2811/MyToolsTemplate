using System;
using System.Windows.Input;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class BaseCommand : ICommand
    {
        protected BaseCommand()
        {

        }

        public BaseCommand(Action execute) : this(execute, () => true)
        {
        }
        public BaseCommand(Action execute, Func<bool> canExecute)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        readonly Func<bool> _canExecute;
        readonly Action _execute;

        public virtual bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public virtual void Execute(object parameter)
        {
            _execute?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
        protected void FireCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }
    }
    internal class BaseCommand<TParam> : BaseCommand
    {
        public BaseCommand(Action<TParam> execute) : this(execute, (p) => true)
        {
        }
        public BaseCommand(Action<TParam> execute, Func<TParam, bool> canExecute)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        readonly Func<TParam, bool> _canExecute;
        readonly Action<TParam> _execute;

        public override bool CanExecute(object parameter)
        {
            return _canExecute.Invoke((TParam)parameter);
        }

        public override void Execute(object parameter)
        {
            _execute?.Invoke((TParam)parameter);
        }
    }
}
