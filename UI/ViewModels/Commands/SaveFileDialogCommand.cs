using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class SaveFileDialogCommand : BaseDialogCommand
    {
        readonly string _filter;
        public SaveFileDialogCommand(string filter) : this(filter, null, null)
        {
        }
        public SaveFileDialogCommand(string filter, Action<string?>? pathCallback) : this(filter, pathCallback, null)
        {
        }
        public SaveFileDialogCommand(string filter, Func<bool>? canExecute) : this(filter, null, canExecute)
        {
        }
        public SaveFileDialogCommand(string filter, Action<string?>? pathCallback, Func<bool>? canExecute) : base(pathCallback, canExecute)
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            _filter = filter;
        }

        public override void Execute(object? parameter)
        {
            this.SaveFileDialogHelper(_filter);
        }
    }
    internal class SaveFileDialogCommand<TObject> : BaseDialogCommand<TObject>
    {
        readonly string _filter;
        public SaveFileDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback, string filter)
            : this(@object, expression, saveCallback, filter, null)
        {
        }
        public SaveFileDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback, string filter, Func<bool>? canExecute)
            : base(@object, expression, saveCallback, canExecute)
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            this._filter = filter;
        }

        public override void Execute(object? parameter)
        {
            this.SaveFileDialogHelper(_filter);
        }
    }

    internal static class SaveFileDialogCommandExtension
    {
        internal static void SaveFileDialogHelper(this BaseDialogCommand baseDialogCommand, string filter)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog()
            {
                RestoreDirectory = true,
                Filter = filter,
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

        public static SaveFileDialogCommand<T> SaveFileDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback, string filter)
            where T : class
        {
            return new SaveFileDialogCommand<T>(obj, expression, saveCallback, filter);
        }
        public static SaveFileDialogCommand<T> SaveFileDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback, string filter, Func<bool>? canExecute)
            where T : class
        {
            return new SaveFileDialogCommand<T>(obj, expression, saveCallback, filter, canExecute);
        }
    }
}
