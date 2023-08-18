using System;
using System.Windows.Input;
using $safeprojectname$.UI.ViewModels.Commands;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumCommandVM<TEnum> : EnumVM<TEnum> where TEnum : Enum
    {
        public EnumCommandVM(TEnum t, ICommand command) : base(t)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
        }
        public EnumCommandVM(TEnum t, Action action) : base(t)
        {
            this.Command = new BaseCommand(action ?? throw new ArgumentNullException(nameof(action)));
        }

        public ICommand Command { get; }
    }
    internal class EnumCommandVM<TEnum, TParam> : EnumCommandVM<TEnum> where TEnum : Enum
    {
        public EnumCommandVM(TEnum t, Action<TParam> commandExecute) : base(t, new BaseCommand<TParam>(commandExecute))
        {

        }
        public EnumCommandVM(TEnum t, Action<TParam> commandExecute, Func<TParam, bool> canExecute) : base(t, new BaseCommand<TParam>(commandExecute, canExecute))
        {

        }
    }
}
