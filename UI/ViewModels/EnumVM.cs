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
            this.Name = t.GetAttribute<NameAttribute>()?.Name ?? t.ToString();
            Childs.CollectionChanged += Childs_CollectionChanged;
        }
        public EnumVM(T t, IEnumerable<EnumVM<T>> childs) : this(t)
        {
            foreach(var item in childs) this.Childs.Add(item);
        }

        private void Childs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                foreach (var item in e.NewItems.Cast<EnumVM<T>>())
                {
                    if (item.Parent is not null)
                        throw new InvalidOperationException();

                    item.Parent = this;
                }
            }
            if (e.OldItems is not null)
            {
                foreach (var item in e.OldItems.Cast<EnumVM<T>>())
                {
                    if (item.Parent is null)
                        throw new InvalidOperationException();

                    item.Parent = null;
                }
            }
        }


        public string Name { get; }

        public T Value { get; }

        public ImageSource Image { get; }

        public EnumVM<T> Parent { get; private set; }

        public ObservableCollection<EnumVM<T>> Childs { get; } = new ObservableCollection<EnumVM<T>>();
    }
}
