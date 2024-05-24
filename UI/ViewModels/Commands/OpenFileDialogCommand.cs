using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class OpenFileDialogCommand : BaseDialogCommand
    {
        readonly string _filter;
        public OpenFileDialogCommand(string filter) : this(filter, null, null)
        {
        }
        public OpenFileDialogCommand(string filter, Action<string?>? pathCallback) : this(filter, pathCallback, null)
        {
        }
        public OpenFileDialogCommand(string filter, Func<bool>? canExecute) : this(filter, null, canExecute)
        {
        }
        public OpenFileDialogCommand(string filter, Action<string?>? pathCallback, Func<bool>? canExecute) : base(pathCallback, canExecute)
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            _filter = filter;
        }

        public override void Execute(object? parameter)
        {
            this.OpenFileDialogHelper(_filter);
        }
    }
    internal class OpenFileDialogCommand<TObject> : BaseDialogCommand<TObject>
    {
        readonly string _filter;
        public OpenFileDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback, string filter)
            : this(@object, expression, saveCallback, filter, null)
        {
        }
        public OpenFileDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback, string filter, Func<bool>? canExecute)
            : base(@object, expression, saveCallback, canExecute)
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            this._filter = filter;
        }

        public override void Execute(object? parameter)
        {
            this.OpenFileDialogHelper(_filter);
        }
    }

    internal static class OpenFileDialogCommandExtension
    {
        internal static void OpenFileDialogHelper(this BaseDialogCommand baseDialogCommand, string filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                RestoreDirectory = true,
                Filter = filter,
                CheckFileExists = true,
            };
            openFileDialog.CustomPlaces = new List<FileDialogCustomPlace>()
            {
                new FileDialogCustomPlace(Singleton.ExeDir)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                baseDialogCommand.Path = openFileDialog.FileName;
            }
        }

        public static OpenFileDialogCommand<T> OpenFileDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback, string filter)
            where T : class
        {
            return new OpenFileDialogCommand<T>(obj, expression, saveCallback, filter);
        }
        public static OpenFileDialogCommand<T> OpenFileDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback, string filter, Func<bool>? canExecute)
            where T : class
        {
            return new OpenFileDialogCommand<T>(obj, expression, saveCallback, filter, canExecute);
        }
    }
}
