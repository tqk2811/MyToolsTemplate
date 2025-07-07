using System;
using System.Windows.Threading;
using System.Windows.Input;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class BaseCommandData<TData> : BaseCommand
    {
        protected BaseCommandData(TData data) : this(data, App.Current.Dispatcher)
        {

        }
        protected BaseCommandData(TData data, Dispatcher dispatcher) : base(dispatcher)
        {
            this._data = data ?? throw new ArgumentNullException(nameof(data));
        }
        public BaseCommandData(TData data, Action<TData>? execute = null) : this(data, execute, null)
        {
        }
        public BaseCommandData(TData data, Action<TData>? execute, Func<TData, bool>? canExecute) : this(data, execute, canExecute, App.Current.Dispatcher)
        {
        }
        public BaseCommandData(TData data, Action<TData>? execute, Func<TData, bool>? canExecute, Dispatcher dispatcher) : base(dispatcher)
        {
            this._data = data ?? throw new ArgumentNullException(nameof(data));
            this._canExecute = canExecute;
            if (execute is not null)
            {
                OnExecute += execute;
            }
        }

        protected readonly TData _data;
        readonly Func<TData, bool>? _canExecute;
        public new event Action<TData>? OnExecute;

        public override bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(_data) ?? true;
        }

        public override void Execute(object? parameter)
        {
            OnExecute?.Invoke(_data);
        }
    }
    internal class BaseCommandData<TData, TParam> : BaseCommandData<TData>
    {
        public BaseCommandData(TData data, Action<TData, TParam>? execute = null) : this(data, execute, null)
        {
        }
        public BaseCommandData(TData data, Action<TData, TParam>? execute, Func<TData, TParam, bool>? canExecute) : this(data, execute, canExecute, App.Current.Dispatcher)
        {

        }
        public BaseCommandData(TData data, Action<TData, TParam>? execute, Func<TData, TParam, bool>? canExecute, Dispatcher dispatcher)
            : base(data, dispatcher)
        {
            this._canExecute = canExecute;
            if (execute is not null)
            {
                OnExecute += execute;
            }
        }

        readonly Func<TData, TParam, bool>? _canExecute;
        public new event Action<TData, TParam>? OnExecute;

        public override bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(_data, (TParam)parameter!) ?? true;
        }

        public override void Execute(object? parameter)
        {
            OnExecute?.Invoke(_data, (TParam)parameter!);
        }
    }
}
