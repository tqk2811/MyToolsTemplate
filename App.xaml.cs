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
            ParseResult parseResult = rootCommand.Parse(e.Args);
            if (parseResult.Errors.Any())
            {
                foreach (var item in parseResult.Errors)
                {
                    Console.WriteLine($"{item.Message}");
                }
                Shutdown(1);
            }
            else
            {
                Work();
            }
        }

        private void Work()
        {
            Window window = new MainWindow();
            window.Closed += Window_Closed;
            window.Show();
        }
        private void Window_Closed(object? sender, EventArgs e)
        {
            this.Shutdown(0);
        }

        private Task WorkAsync()
        {
            Window window = new MainWindow();
            window.ShowDialog();
            return Task.CompletedTask;
        }
    }
}
