using System;
using System.Windows.Threading;
using System.Windows.Input;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class BaseCommand : ICommand
    {
        protected readonly Dispatcher _dispatcher;
        protected BaseCommand() : this(App.Current.Dispatcher)
        {

        }
        protected BaseCommand(Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public BaseCommand(Action? execute = null) : this(execute, () => true)
        {
        }
        public BaseCommand(Func<bool> canExecute) : this(() => { }, canExecute)
        {
        }
        public BaseCommand(Action? execute, Func<bool> canExecute) : this(execute, canExecute, App.Current.Dispatcher)
        {
        }
        public BaseCommand(Action? execute, Func<bool> canExecute, Dispatcher dispatcher)
        {
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            this._dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            if (execute is not null)
            {
                this.OnExecute += execute;
            }
        }

        readonly Func<bool>? _canExecute;
        public event Action? OnExecute;
        public event EventHandler? CanExecuteChanged;
        
        public virtual bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public virtual void Execute(object? parameter)
        {
            OnExecute?.Invoke();
        }

        public virtual void FireCanExecuteChanged(object? sender, EventArgs e)
        {
            _dispatcher.TrueThreadInvokeAsync(() => CanExecuteChanged?.Invoke(sender, e));
        }
        public virtual void FireCanExecuteChanged() => FireCanExecuteChanged(null, EventArgs.Empty);
    }
    internal class BaseCommand<TParam> : BaseCommand
    {
        public BaseCommand(Action<TParam>? execute = null) : this(execute, (p) => true)
        {
        }
        public BaseCommand(Action<TParam>? execute, Func<TParam, bool> canExecute)
        {
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            if (execute is not null)
            {
                OnExecute += execute;
            }
        }

        readonly Func<TParam, bool> _canExecute;
        public new event Action<TParam>? OnExecute;
        

        public override bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke((TParam)parameter!);
        }

        public override void Execute(object? parameter)
        {
            OnExecute?.Invoke((TParam)parameter!);
        }
    }
}
