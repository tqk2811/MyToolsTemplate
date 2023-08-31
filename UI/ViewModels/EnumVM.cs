using $safeprojectname$.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollection;

namespace $safeprojectname$.UI.ViewModels
{
    internal class EnumVM<TEnum> where TEnum : Enum
    {
        public EnumVM(TEnum t)
        {
            this.Value = t;
            this.Name = t.GetAttribute<NameAttribute>()?.Name ?? t.ToString();
            this.Image = t.GetAttribute<ImageAttribute>()?.GetImage();
            Childs.CollectionChanged += Childs_CollectionChanged;
        }
        public EnumVM(TEnum t, IEnumerable<EnumVM<TEnum>> childs) : this(t)
        {
            foreach (var item in childs) this.Childs.Add(item);
        }

        private void Childs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null)
                    {
                        foreach (var item in e.NewItems.Cast<EnumVM<TEnum>>().Where(x => x is not null))
                        {
                            if (item.Parent is not null)
                                throw new InvalidOperationException();

                            item.Parent = this;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null)
                    {
                        foreach (var item in e.OldItems.Cast<EnumVM<TEnum>>().Where(x => x is not null))
                        {
                            if (item.Parent is null)
                                throw new InvalidOperationException();

                            item.Parent = null;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems is not null)
                    {
                        foreach (var item in e.NewItems.Cast<EnumVM<TEnum>>().Where(x => x is not null))
                        {
                            if (item.Parent is not null)
                                throw new InvalidOperationException();

                            item.Parent = this;
                        }
                    }
                    if (e.OldItems is not null)
                    {
                        foreach (var item in e.OldItems.Cast<EnumVM<TEnum>>().Where(x => x is not null))
                        {
                            if (item.Parent is null)
                                throw new InvalidOperationException();

                            item.Parent = null;
                        }
                    }
                    break;
            }
        }


        public string Name { get; }

        public TEnum Value { get; }

        public ImageSource Image { get; }

        public EnumVM<TEnum> Parent { get; private set; }

        public DispatcherObservableCollection<EnumVM<TEnum>> Childs { get; } = new DispatcherObservableCollection<EnumVM<TEnum>>();
    }
}
