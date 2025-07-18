using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
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
            this.ClearPathCommand = canExecute is not null ? new BaseCommand(_ClearPath, canExecute) : new BaseCommand(_ClearPath);
        }

        public virtual event Action<string?>? OnPathSelected;
        string? _Path = null;
        public virtual string? Path
        {
            get { return _Path; }
            set { _Path = value; NotifyChange(); OnPathSelected?.Invoke(value); }
        }

        public virtual string? Name
        {
            get
            {
                if (File.Exists(Path))
                {
                    return new FileInfo(Path).Name;
                }
                if (Directory.Exists(Path))
                {
                    return new DirectoryInfo(Path).Name;
                }
                return null;
            }
        }
        public virtual bool IsExist
        {
            get { return File.Exists(Path) || Directory.Exists(Path); }
        }
        public void NotifyChange()
        {
            NotifyPropertyChange(nameof(Path));
            NotifyPropertyChange(nameof(Name));
            NotifyPropertyChange(nameof(IsExist));
        }


        public override bool CanExecute(object? parameter)
        {
            if (IsForceLock) return false;
            return _canExecute?.Invoke() ?? true;
        }
        public override abstract void Execute(object? parameter);
        public override void FireCanExecuteChanged(object? sender, EventArgs e)
        {
            base.FireCanExecuteChanged(sender, e);
            NotifyPropertyChange(nameof(Path));
        }

        public static implicit operator string?(BaseDialogCommand baseDialogCommand) => baseDialogCommand?.Path;

        
        public virtual BaseCommand? ClearPathCommand { get; }
        protected virtual void _ClearPath()
        {
            Path = null;
        }
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
                var prop = (PropertyInfo)((MemberExpression)_expression.Body).Member;
                prop.SetValue(_object, value);
                // SetProperty(value);
                base.Path = value;
                NotifyChange();
                _saveCallback.Invoke();
                OnPathSelected?.Invoke(value);
            }
        }
        
        void SetProperty(string? value)
        {
            MemberExpression? memberExpression = _expression.Body as MemberExpression;
            if (memberExpression is null) throw new InvalidOperationException();

            
            List<MemberExpression> listMemberExpression = new([memberExpression]);//child to parent
            while (true)
            {
                if (memberExpression?.Expression is null)
                {
                    break;
                }
#if DEBUG
                Type type = memberExpression.Expression.GetType();
#endif
                if (memberExpression.Expression is MemberExpression child_memberExpression)
                {
                    memberExpression = child_memberExpression;
                    listMemberExpression.Add(child_memberExpression);
                }
                else if (memberExpression.Expression is ParameterExpression typedParameterExpression)
                {
                    break;
                }
                else throw new InvalidOperationException();
            }
            listMemberExpression.Reverse();


            object? _target = _object;
            foreach (MemberExpression member in listMemberExpression)
            {
                PropertyInfo? propertyInfo = member?.Member as PropertyInfo;
                if (propertyInfo is not null)
                {
                    if (listMemberExpression.Last().Equals(member))
                    {
                        if (propertyInfo.CanWrite)
                        {
                            propertyInfo.SetValue(_target, value);
                        }
                        break;
                    }
                    else if (propertyInfo.CanRead)
                    {
                        _target = propertyInfo.GetValue(_target);
                    }
                    else throw new InvalidOperationException();
                }
                else throw new InvalidOperationException();
            }
        }
    }
}
