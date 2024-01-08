using System;
using System.Windows.Input;
using System.Collections.Generic;
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
        public EnumCommandVM(TEnum t, IEnumerable<EnumCommandVM<TEnum>> childs) : base(t, childs)
        {

        }

        public ICommand? Command { get; }
    }
    internal class EnumCommandVM<TEnum, TParam> : EnumCommandVM<TEnum> where TEnum : Enum
    {
        public EnumCommandVM(TEnum t, Action<TParam> commandExecute) : base(t, new BaseCommand<TParam>(commandExecute))
        {

        }
        public EnumCommandVM(TEnum t, Action<TParam> commandExecute, Func<TParam, bool> canExecute) : base(t, new BaseCommand<TParam>(commandExecute, canExecute))
        {

        }
        public EnumCommandVM(TEnum t, IEnumerable<EnumCommandVM<TEnum>> childs) : base(t, childs)
        {

        }
        public EnumCommandVM(TEnum t, IEnumerable<EnumCommandVM<TEnum, TParam>> childs) : base(t, childs)
        {

        }
    }
}
