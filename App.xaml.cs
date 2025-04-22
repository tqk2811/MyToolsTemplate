using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using TqkLibrary.WpfUi;
using System.CommandLine;
using TqkLibrary.WinApi.FindWindowHelper;
using System.Diagnostics;
using $safeprojectname$.UI;

namespace $safeprojectname$
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;
            if (System.IO.Directory.GetCurrentDirectory().StartsWith(System.IO.Path.GetTempPath()))
            {
                MessageBox.Show("Hãy giải nén ra để chạy", "Thông báo");
                Environment.Exit(-1);
            }
            
            RootCommand rootCommand = new RootCommand("$safeprojectname$")
            {

            };
            rootCommand.SetHandler(WorkAsync);
            rootCommand.InvokeAsync(e.Args).ContinueWith(Shutdown);
        }

        private async void Shutdown(Task<int> task)
        {
            this.Shutdown(await task);
        }

        private Task WorkAsync()
        {
#if DEBUG
            {
                using Process? process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "taskkill",
                    Arguments = "/f /im chromedriver.exe",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                process?.WaitForExit();
            }

            foreach (ProcessHelper processHelper in Process.GetProcessesByName("chrome").Select(x => new ProcessHelper((uint)x.Id)))
            {
                var Win32_Process = processHelper.Query_Win32_Process();
                string? arguments = Win32_Process?.CommandLine;
                if (arguments?.Contains(Singleton.UserDataDirs, StringComparison.OrdinalIgnoreCase) == true)
                {
                    using Process? process = Process.Start(new ProcessStartInfo()
                    {
                        FileName = "taskkill",
                        Arguments = $"/f /pid {processHelper.ProcessId}",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                }
            }
#endif
            Window window = new MainWindow();
            window.ShowDialog();
            return Task.CompletedTask;
        }
    }
}
