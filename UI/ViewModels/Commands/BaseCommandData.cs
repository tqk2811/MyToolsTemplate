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
        public BaseCommandData(TData data, Action<TData> execute) : this(data, execute, (d) => true)
        {
        }
        public BaseCommandData(TData data, Action<TData> execute, Func<TData, bool> canExecute) : this(data, execute, canExecute, App.Current.Dispatcher)
        {
        }
        public BaseCommandData(TData data, Action<TData> execute, Func<TData, bool> canExecute, Dispatcher dispatcher) : base(dispatcher)
        {
            this._data = data ?? throw new ArgumentNullException(nameof(data));
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        protected readonly TData _data;
        readonly Func<TData, bool>? _canExecute;
        readonly Action<TData>? _execute;

        public override bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(_data) ?? true;
        }

        public override void Execute(object? parameter)
        {
            _execute?.Invoke(_data);
        }
    }

    internal class BaseCommandData<TData, TParam> : BaseCommandData<TData>
    {
        public BaseCommandData(TData data, Action<TData, TParam> execute) : this(data, execute, (d, p) => true)
        {
        }
        public BaseCommandData(TData data, Action<TData, TParam> execute, Func<TData, TParam, bool> canExecute) : this(data, execute, canExecute, App.Current.Dispatcher)
        {

        }
        public BaseCommandData(TData data, Action<TData, TParam> execute, Func<TData, TParam, bool> canExecute, Dispatcher dispatcher)
            : base(data, dispatcher)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        readonly Func<TData, TParam, bool> _canExecute;
        readonly Action<TData, TParam> _execute;


        public override bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke(_data, (TParam)parameter!);
        }

        public override void Execute(object? parameter)
        {
            _execute?.Invoke(_data, (TParam)parameter!);
        }
    }
}
