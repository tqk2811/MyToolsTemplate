using $safeprojectname$.Attributes;
using System;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumVM<T> where T : Enum
    {
        public EnumVM(T t)
        {
            this.Value = t;
            this.Name = t.GetAttribute<NameAttribute>().Name;
        }

        public string Name { get; }

        public T Value { get; }
    }
}
