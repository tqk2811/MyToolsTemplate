﻿using $safeprojectname$.DataClass;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.UI.ViewModels
{
    internal class BaseVM : BaseViewModel
    {
        protected SettingData Setting { get { return Singleton.Setting.Data; } }
        protected void SaveSetting() => Singleton.Setting.TriggerSave();
    }
}
