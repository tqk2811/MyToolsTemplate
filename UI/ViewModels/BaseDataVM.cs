using System;

namespace $safeprojectname$.UI.ViewModels
{
    internal class BaseDataVM<TData> : BaseVM
    {
        protected readonly TData _data;
        readonly Action _saveCallback;
        protected void Save() => _saveCallback.Invoke();
        protected BaseDataVM(TData data, Action saveCallback)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            this._saveCallback = saveCallback ?? throw new ArgumentNullException(nameof(saveCallback));
        }
    }
}
