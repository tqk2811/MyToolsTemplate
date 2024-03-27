using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class OpenFolderDialogCommand : BaseDialogCommand
    {
        public OpenFolderDialogCommand()
        {

        }
        public OpenFolderDialogCommand(Action<string?>? pathCallback) : base(pathCallback)
        {
            if (pathCallback is not null)
                OnPathSelected += pathCallback;
        }

        public override void Execute(object? parameter)
        {
            this.OpenFolderDialogHelper();
        }
    }
    internal class OpenFolderDialogCommand<TObject> : BaseDialogCommand<TObject>
    {
        public OpenFolderDialogCommand(TObject @object, Expression<Func<TObject, string?>> expression, Action saveCallback)
            : base(@object, expression, saveCallback)
        {
        }

        public override void Execute(object? parameter)
        {
            this.OpenFolderDialogHelper();
        }
    }

    internal static class OpenFolderDialogCommandExtension
    {
        internal static void OpenFolderDialogHelper(this BaseDialogCommand baseDialogCommand)
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
                baseDialogCommand.Path = commonOpenFileDialog.FileName;
            }
        }


        public static OpenFolderDialogCommand<T> OpenFolderDialogCommand<T>(this T obj, Expression<Func<T, string?>> expression, Action saveCallback)
            where T : class
        {
            return new OpenFolderDialogCommand<T>(obj, expression, saveCallback);
        }
    }
}
