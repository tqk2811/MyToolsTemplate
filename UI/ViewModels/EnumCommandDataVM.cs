using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Input;
using $safeprojectname$.UI.ViewModels.Commands;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumCommandDataVM<TEnum, TData> : EnumVM<TEnum> where TEnum : Enum
    {
        public EnumCommandDataVM(TEnum t, BaseCommandData<TData> baseCommandData) : base(t)
        {
            this.Command = baseCommandData ?? throw new ArgumentNullException(nameof(baseCommandData));
        }
        public EnumCommandDataVM(TEnum t, TData data, Action<TData> execute) : base(t)
        {
            this.Command = new BaseCommandData<TData>(data, execute);
        }
        public EnumCommandDataVM(TEnum t, TData data, Action<TData> execute, Func<TData, bool> canExecute) : base(t)
        {
            this.Command = new BaseCommandData<TData>(data, execute, canExecute);
        }


        public EnumCommandDataVM(TEnum t, string name, BaseCommandData<TData> baseCommandData) : base(t, name)
        {
            this.Command = baseCommandData ?? throw new ArgumentNullException(nameof(baseCommandData));
        }
        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData> execute) : base(t, name)
        {
            this.Command = new BaseCommandData<TData>(data, execute);
        }
        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData> execute, Func<TData, bool> canExecute) : base(t, name)
        {
            this.Command = new BaseCommandData<TData>(data, execute, canExecute);
        }


        public EnumCommandDataVM(TEnum t, IEnumerable<EnumVM<TEnum>> childs) : base(t, childs)
        {
        }
        public EnumCommandDataVM(TEnum t, string name, IEnumerable<EnumVM<TEnum>> childs) : base(t, name, childs)
        {
        }

        public ICommand? Command { get; }
    }

    internal class EnumCommandDataVM<TEnum, TData, TParam> : EnumVM<TEnum> where TEnum : Enum
    {
        public EnumCommandDataVM(TEnum t, BaseCommandData<TData, TParam> baseCommandData) : base(t)
        {
            this.Command = baseCommandData ?? throw new ArgumentNullException(nameof(baseCommandData));
        }
        public EnumCommandDataVM(TEnum t, TData data, Action<TData, TParam> execute) : base(t)
        {
            this.Command = new BaseCommandData<TData, TParam>(data, execute);
        }
        public EnumCommandDataVM(TEnum t, TData data, Action<TData, TParam> execute, Func<TData, TParam, bool> canExecute) : base(t)
        {
            this.Command = new BaseCommandData<TData, TParam>(data, execute, canExecute);
        }


        public EnumCommandDataVM(TEnum t, string name, BaseCommandData<TData, TParam> baseCommandData) : base(t, name)
        {
            this.Command = baseCommandData ?? throw new ArgumentNullException(nameof(baseCommandData));
        }
        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData, TParam> execute) : base(t, name)
        {
            this.Command = new BaseCommandData<TData, TParam>(data, execute);
        }
        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData, TParam> execute, Func<TData, TParam, bool> canExecute) : base(t, name)
        {
            this.Command = new BaseCommandData<TData, TParam>(data, execute, canExecute);
        }


        public EnumCommandDataVM(TEnum t, IEnumerable<EnumVM<TEnum>> childs) : base(t, childs)
        {
        }
        public EnumCommandDataVM(TEnum t, string name, IEnumerable<EnumVM<TEnum>> childs) : base(t, name, childs)
        {
        }

        public ICommand? Command { get; }
    }
}
