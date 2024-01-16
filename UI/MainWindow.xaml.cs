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
    }
}
