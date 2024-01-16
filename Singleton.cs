using $safeprojectname$.DataClass;
using $safeprojectname$.UI.ViewModels.WindowViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using TqkLibrary.WpfUi;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$
{
    static partial class Singleton
    {
        static Singleton()
        {
            Directory.CreateDirectory(LogDir);
            Setting = new SaveJsonData<SettingData>(SettingJson);
            UiSetting = new SaveJsonData<UiSettingData>(UiSettingJson);            
            ILoggerFactory = LoggerFactory.Create(c => c.AddProvider(MainWVM.LoggerProvider));
        }
        internal static string ExeDir { get; } = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;//Directory.GetCurrentDirectory();
        internal static string LogDir { get; } = Path.Combine(ExeDir, "Logs");
        internal static string AppDataDir { get; } = Path.Combine(ExeDir, "AppData");
        internal static string SettingJson { get; } = Path.Combine(ExeDir, "Setting.json");
        internal static string UiSettingJson { get; } = Path.Combine(ExeDir, "UiSetting.json");

        public static ILoggerFactory ILoggerFactory { get; set; }
        internal static SaveJsonData<SettingData> Setting { get; }
        internal static SaveJsonData<UiSettingData> UiSetting { get; }
    }
}
