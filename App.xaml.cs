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
            
            RootCommand rootCommand = new RootCommand("ViewYoutube.Manager.App")
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
            Window window = new MainWindow();
            window.ShowDialog();
            return Task.CompletedTask;
        }
    }
}
