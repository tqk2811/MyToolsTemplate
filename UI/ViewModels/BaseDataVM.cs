using System;
using TqkLibrary.WpfUi.Interfaces;

namespace $safeprojectname$.UI.ViewModels
{
    internal class BaseDataVM<TData> : BaseVM, IViewModel<TData> where TData : class
    {
        readonly Action? _saveCallback;
        TData _data;
        public event ChangeCallBack<TData>? Change;
        public virtual TData Data { get { return _data; } }

        public BaseDataVM(TData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }
        public BaseDataVM(TData data, Action saveCallback) : this(data)
        {
            this._saveCallback = saveCallback ?? throw new ArgumentNullException(nameof(saveCallback));
        }

        public virtual void Save()
        {
            _saveCallback?.Invoke();
            Change?.Invoke(this, Data);
        }
        
        public virtual void Update(TData data)
        {
            this._data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}