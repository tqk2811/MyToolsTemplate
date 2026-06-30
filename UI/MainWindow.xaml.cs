using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Logging;
using TqkLibrary.WinApi.FindWindowHelper;
using TqkLibrary.WinApi.WmiHelpers;
using System.Diagnostics;
using $safeprojectname$.SeleniumProfiles;
using $safeprojectname$.UI.ViewModels;
using $safeprojectname$.UI.ViewModels.WindowViewModels;

namespace $safeprojectname$.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MainWVM _mainWVM;
        readonly ILogger<MainWindow> _logger = Singleton.ILoggerFactory.CreateLogger<MainWindow>();
        public MainWindow()
        {
            InitializeComponent();
            this._mainWVM = this.DataContext as MainWVM ?? throw new InvalidOperationException();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                Win32_Process? win32_Process = processHelper.Query_Win32_Process();
                if (win32_Process?.CommandLine?.Contains(Singleton.UserDataDirs, StringComparison.OrdinalIgnoreCase) == true)
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

            MyBaseChromeProfile.KillChromeStuck();
#endif
        }
    }
}
