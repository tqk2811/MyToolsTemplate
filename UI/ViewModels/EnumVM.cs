using $safeprojectname$.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollections;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumVM<TEnum> where TEnum : Enum
    {
        public EnumVM(TEnum t)
        {
            this.Value = t;
            this.Name = t.GetAttribute<NameAttribute>()?.Name ?? t.ToString();
            this.Image = t.GetAttribute<ImageResourceAttribute>()?.GetImage();
            Childs.OnItemAdded += Childs_OnItemAdded;
            Childs.OnItemRemoved += Childs_OnItemRemoved;
        }

        public EnumVM(TEnum t, IEnumerable<EnumVM<TEnum>> childs) : this(t)
        {
            foreach (var item in childs) this.Childs.Add(item);
        }

        private void Childs_OnItemAdded(EnumVM<TEnum> item)
        {
            if (item is not null)
            {
                if (item.Parent is not null)
                    throw new InvalidOperationException($"Menu '{item.Name}' are under '{item.Parent.Name}'");

                item.Parent = this;
            }
        }
        private void Childs_OnItemRemoved(EnumVM<TEnum> item)
        {
            if (item is not null)
            {
                if (item.Parent is null)
                    throw new InvalidOperationException($"Menu '{item.Name}' are not under parent");

                item.Parent = null;
            }
        }

        public string Name { get; set; }

        public TEnum Value { get; }

        public ImageSource? Image { get; set; }

        public EnumVM<TEnum>? Parent { get; private set; }

        public DispatcherObservableCollection<EnumVM<TEnum>> Childs { get; } = new DispatcherObservableCollection<EnumVM<TEnum>>();
    }
}
