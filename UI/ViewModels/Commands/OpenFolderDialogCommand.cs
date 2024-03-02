using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class OpenFolderDialogCommand<TObject> : BaseCommand, INotifyPropertyChanged where TObject : class
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChange([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected readonly TObject _object;
        protected readonly Expression<Func<TObject, string?>> _expression;
        protected readonly Action _saveCallback;
        public OpenFolderDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback)
        {
            this._object = @object ?? throw new ArgumentNullException(nameof(@object));
            this._expression = expression ?? throw new ArgumentNullException(nameof(expression));
            this._saveCallback = saveCallback ?? throw new ArgumentNullException(nameof(saveCallback));

            MemberExpression? memberExpression = expression.Body as MemberExpression;
            PropertyInfo? propertyInfo = memberExpression?.Member as PropertyInfo;
            if (propertyInfo is null) throw new InvalidOperationException($"{nameof(expression)} invalid selector");
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite) throw new InvalidOperationException($"{nameof(expression)} invalid selector, must be get and set");
        }

        public string? Path
        {
            get { return _expression.Compile().Invoke(_object); }
            set
            {
                var prop = (PropertyInfo)((MemberExpression)_expression.Body).Member;
                prop.SetValue(_object, value);
                NotifyPropertyChange();
                _saveCallback.Invoke();
            }
        }
        public override void Execute(object? parameter)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog()
            {
                Multiselect = false,
                RestoreDirectory = true,
                IsFolderPicker = true,
                EnsurePathExists = true,
            };
            commonOpenFileDialog.AddPlace(Singleton.ExeDir, Microsoft.WindowsAPICodePack.Shell.FileDialogAddPlaceLocation.Top);
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Path = commonOpenFileDialog.FileName;
            }
        }

        public static implicit operator string?(OpenFolderDialogCommand<TObject> openFolderDialogCommand) => openFolderDialogCommand?.Path;
    }
    static class OpenFolderDialogCommandExtension
    {
        public static OpenFolderDialogCommand<T> OpenFolderDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback)
            where T : class
        {
            return new OpenFolderDialogCommand<T>(obj, expression, saveCallback);
        }
    }
}
