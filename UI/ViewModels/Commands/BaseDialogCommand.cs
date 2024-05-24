using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal abstract class BaseDialogCommand : BaseCommand, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChange([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        readonly Func<bool>? _canExecute;

        public BaseDialogCommand()
        {

        }
        public BaseDialogCommand(Func<bool>? canExecute) : this(null, canExecute)
        {
            
        }
        public BaseDialogCommand(Action<string?>? pathCallback): this(pathCallback, null)
        {
            
        }
        public BaseDialogCommand(Action<string?>? pathCallback, Func<bool>? canExecute)
        {
            if (pathCallback is not null)
                OnPathSelected += pathCallback;
            this._canExecute = canExecute;
        }

        public virtual event Action<string?>? OnPathSelected;
        string? _Path = null;
        public virtual string? Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChange(); OnPathSelected?.Invoke(value); }
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }
        public override abstract void Execute(object? parameter);
        public override void FireCanExecuteChanged(object? sender, EventArgs e)
        {
            base.FireCanExecuteChanged(sender, e);
            NotifyPropertyChange(nameof(Path));
        }

        public static implicit operator string?(BaseDialogCommand baseDialogCommand) => baseDialogCommand?.Path;
    }
    internal abstract class BaseDialogCommand<TObject> : BaseDialogCommand
    {
        protected readonly TObject _object;
        protected readonly Expression<Func<TObject, string?>> _expression;
        protected readonly Action _saveCallback;
        public BaseDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback)
            : this(@object, expression, saveCallback, null)
        {

        }
        public BaseDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback, Func<bool>? canExecute)
            : base(canExecute)
        {
            this._object = @object ?? throw new ArgumentNullException(nameof(@object));
            this._expression = expression ?? throw new ArgumentNullException(nameof(expression));
            this._saveCallback = saveCallback ?? throw new ArgumentNullException(nameof(saveCallback));

            MemberExpression? memberExpression = expression.Body as MemberExpression;
            PropertyInfo? propertyInfo = memberExpression?.Member as PropertyInfo;
            if (propertyInfo is null) throw new InvalidOperationException($"{nameof(expression)} invalid selector");
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite) throw new InvalidOperationException($"{nameof(expression)} invalid selector, must be get and set");
        }

        public override event Action<string?>? OnPathSelected;
        public override string? Path
        {
            get { return _expression.Compile().Invoke(_object); }
            set
            {
                base.Path = value;
                var prop = (PropertyInfo)((MemberExpression)_expression.Body).Member;
                prop.SetValue(_object, value);
                NotifyPropertyChange();
                _saveCallback.Invoke();
                OnPathSelected?.Invoke(value);
            }
        }
    }
}
