using $safeprojectname$.DataClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WpfUi;

namespace $safeprojectname$
{
    static class Singleton
    {
        static Singleton()
        {
            Directory.CreateDirectory(LogDir);
            Setting = new SaveSettingData<SettingData>(SettingJson);
        }
        internal static string ExeDir { get; } = Directory.GetCurrentDirectory();
        internal static string LogDir { get; } = Path.Combine(ExeDir, "Logs");
        internal static string AppDataDir { get; } = Path.Combine(ExeDir, "AppData");
        internal static string SettingJson { get; } = Path.Combine(ExeDir, "Setting.json");


        internal static SaveSettingData<SettingData> Setting { get; }
    }
}
