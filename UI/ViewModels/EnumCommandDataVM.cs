using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Input;
using $safeprojectname$.UI.ViewModels.Commands;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumCommandDataVM<TEnum, TData> : EnumCommandVM<TEnum> where TEnum : Enum
    {
        public EnumCommandDataVM(TEnum t, ICommand command) : base(t, command)
        {
        }
        public EnumCommandDataVM(TEnum t, string name, ICommand command) : base(t, name, command)
        {
        }


        public EnumCommandDataVM(TEnum t, TData data, Action<TData> execute)
            : this(t, new BaseCommandData<TData>(data, execute))
        {
        }
        public EnumCommandDataVM(TEnum t, TData data, Action<TData> execute, Func<TData, bool> canExecute)
            : this(t, new BaseCommandData<TData>(data, execute, canExecute))
        {

        }


        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData> execute)
            : this(t, name, new BaseCommandData<TData>(data, execute))
        {
        }
        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData> execute, Func<TData, bool> canExecute)
            : this(t, name, new BaseCommandData<TData>(data, execute, canExecute))
        {
        }


        public EnumCommandDataVM(TEnum t, IEnumerable<EnumVM<TEnum>> childs) : base(t, childs)
        {
        }
        public EnumCommandDataVM(TEnum t, string name, IEnumerable<EnumVM<TEnum>> childs) : base(t, name, childs)
        {
        }
    }
    internal class EnumCommandDataVM<TEnum, TData, TParam> : EnumCommandDataVM<TEnum, TData> where TEnum : Enum
    {
        public EnumCommandDataVM(TEnum t, ICommand command) : base(t, command)
        {
        }
        public EnumCommandDataVM(TEnum t, string name, ICommand command) : base(t, name, command)
        {
        }


        public EnumCommandDataVM(TEnum t, TData data, Action<TData, TParam> execute)
            : base(t, new BaseCommandData<TData, TParam>(data, execute))
        {
        }
        public EnumCommandDataVM(TEnum t, TData data, Action<TData, TParam> execute, Func<TData, TParam, bool> canExecute)
            : base(t, new BaseCommandData<TData, TParam>(data, execute, canExecute))
        {
        }


        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData, TParam> execute)
            : base(t, name, new BaseCommandData<TData, TParam>(data, execute))
        {
        }
        public EnumCommandDataVM(TEnum t, string name, TData data, Action<TData, TParam> execute, Func<TData, TParam, bool> canExecute)
            : base(t, name, new BaseCommandData<TData, TParam>(data, execute, canExecute))
        {
        }


        public EnumCommandDataVM(TEnum t, IEnumerable<EnumVM<TEnum>> childs) : base(t, childs)
        {
        }
        public EnumCommandDataVM(TEnum t, string name, IEnumerable<EnumVM<TEnum>> childs) : base(t, name, childs)
        {
        }
}
