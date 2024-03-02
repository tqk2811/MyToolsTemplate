using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class OpenFileDialogCommand<TObject> : OpenFolderDialogCommand<TObject> where TObject : class
    {
        readonly string _filter;
        public OpenFileDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback, string filter)
            : base(@object, expression, saveCallback)
        {
            if (string.IsNullOrWhiteSpace(filter)) throw new ArgumentNullException(nameof(filter));
            this._filter = filter;
        }

        public override void Execute(object? parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                RestoreDirectory = true,
                Filter = _filter,
                CheckFileExists = true,
            };
            openFileDialog.CustomPlaces = new List<FileDialogCustomPlace>()
            {
                new FileDialogCustomPlace(Singleton.ExeDir)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Path = openFileDialog.FileName;
            }
        }
    }
    static class OpenFileDialogCommandExtension
    {
        public static OpenFileDialogCommand<T> OpenFileDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback, string filter)
            where T : class
        {
            return new OpenFileDialogCommand<T>(obj, expression, saveCallback, filter);
        }
    }
}
