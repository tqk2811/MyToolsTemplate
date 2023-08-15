using $safeprojectname$.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using TqkLibrary.WpfUi;

namespace $safeprojectname$
{
    static partial class Singleton
    {
        static Singleton()
        {
            Directory.CreateDirectory(LogDir);
            Setting = new SaveSettingData<SettingData>(SettingJson);
            UiSetting = new SaveSettingData<UiSettingData>(UiSettingJson);
        }
        internal static string ExeDir { get; } = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;//Directory.GetCurrentDirectory();
        internal static string LogDir { get; } = Path.Combine(ExeDir, "Logs");
        internal static string AppDataDir { get; } = Path.Combine(ExeDir, "AppData");
        internal static string SettingJson { get; } = Path.Combine(ExeDir, "Setting.json");
        internal static string UiSettingJson { get; } = Path.Combine(ExeDir, "UiSetting.json");


        internal static SaveSettingData<SettingData> Setting { get; }
        internal static SaveSettingData<UiSettingData> UiSetting { get; }
    }
}
