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
using System.Windows.Shapes;
using $safeprojectname$.UI.ViewModels;
using $safeprojectname$.UI.ViewModels.WindowViewModels;

namespace $safeprojectname$.UI
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
		readonly SettingWVM _settingWVM;
        readonly ILogger<MainWindow> _logger = Singleton.ILoggerFactory.CreateLogger<MainWindow>();
        public SettingWindow()
        {
            InitializeComponent();            
            MainWindow? mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow is not null) this.Owner = mainWindow;
			this._settingWVM = this.DataContext as SettingWVM ?? throw new InvalidOperationException();
        }
    }
}
