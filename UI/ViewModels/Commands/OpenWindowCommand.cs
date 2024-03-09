using System;
using System.Windows;

namespace $safeprojectname$.UI.ViewModels.Commands
{
    internal class OpenWindowCommand : BaseCommand
    {
        readonly Func<Window> _func_window;
        public OpenWindowCommand(Func<Window> func_window)
        {
            this._func_window = func_window ?? throw new ArgumentNullException(nameof(func_window));
        }

        public override void Execute(object? parameter)
        {
            this._func_window.Invoke().ShowDialog();
        }
    }
}
