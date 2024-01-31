using System;
using System.Windows.Input;
using System.Collections.Generic;
using $safeprojectname$.UI.ViewModels.Commands;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumCommandVM<TEnum> : EnumVM<TEnum> where TEnum : Enum
    {
        public EnumCommandVM(TEnum t, ICommand command)
            : base(t)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
        }


        public EnumCommandVM(TEnum t, Action execute)
            : base(t)
        {
            this.Command = new BaseCommand(execute);
        }
        public EnumCommandVM(TEnum t, Action execute, Func<bool> canExecute)
            : base(t)
        {
            this.Command = new BaseCommand(execute, canExecute);
        }


        public EnumCommandVM(TEnum t, IEnumerable<EnumVM<TEnum>?> childs)
            : base(t, childs)
        {
        }


        public ICommand? Command { get; }
    }

    internal class EnumCommandVM<TEnum, TParam> : EnumCommandVM<TEnum> where TEnum : Enum
    {
        public EnumCommandVM(TEnum t, ICommand command)
            : base(t, command)
        {
        }


        public EnumCommandVM(TEnum t, Action<TParam> execute)
            : base(t, new BaseCommand<TParam>(execute))
        {
        }
        public EnumCommandVM(TEnum t, Action<TParam> execute, Func<TParam, bool> canExecute)
            : base(t, new BaseCommand<TParam>(execute, canExecute))
        {
        }


        public EnumCommandVM(TEnum t, IEnumerable<EnumVM<TEnum>?> childs)
            : base(t, childs)
        {
        }
    }
}
