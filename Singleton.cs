using $safeprojectname$.DataClass;
using $safeprojectname$.UI.ViewModels;
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
using Newtonsoft.Json;
using TqkLibrary.Data.Json;
using Newtonsoft.Json.Converters;

namespace $safeprojectname$
{
    static partial class Singleton
    {
        static Singleton()
        {
            Directory.CreateDirectory(LogDir);
            Logs = new MyLoggerProviderVM(() => LogDir + $"\\{DateTime.Now:yyyy-MM-dd HH}.log");
            ILoggerFactory = LoggerFactory.Create(c => c.AddProvider(Logs));

            JsonSerializerSettings = new JsonSerializerSettings();
            JsonSerializerSettings.Converters.Add(new StringEnumConverter());
            Setting = new SaveJsonData<SettingData>(SettingJson, JsonSerializerSettings);
            UiSetting = new SaveJsonData<UiSettingData>(UiSettingJson, JsonSerializerSettings);
        }
        internal static string ExeDir { get; } = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;//Directory.GetCurrentDirectory();
        internal static string LogDir { get; } = Path.Combine(ExeDir, "Logs");
        internal static string AppDataDir { get; } = Path.Combine(ExeDir, "AppData");
        internal static string SettingJson { get; } = Path.Combine(ExeDir, "Setting.json");
        internal static string UiSettingJson { get; } = Path.Combine(ExeDir, "UiSetting.json");
 
		internal static MyLoggerProviderVM Logs { get; }
        internal static JsonSerializerSettings JsonSerializerSettings { get; }
        internal static ILoggerFactory ILoggerFactory { get; set; }
        internal static SaveJsonData<SettingData> Setting { get; }
        internal static SaveJsonData<UiSettingData> UiSetting { get; }
    }
}
