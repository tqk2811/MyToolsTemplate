using System;
using TqkLibrary.WpfUi.ObservableCollections;

namespace $safeprojectname$.UI.ViewModels
{
    internal class BaseDataVM<TData> : BaseVM, IViewModel<TData> where TData : class
    {
        readonly Action? _saveCallback;
        public event ChangeCallBack<TData>? Change;
        public TData Data { get; }

        protected BaseDataVM(TData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
        protected BaseDataVM(TData data, Action saveCallback) : this(data)
        {
            this._saveCallback = saveCallback ?? throw new ArgumentNullException(nameof(saveCallback));
        }

        protected void Save()
        {
            _saveCallback?.Invoke();
            Change?.Invoke(this, Data);
        }
    }
}