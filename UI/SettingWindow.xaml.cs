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

namespace $safeprojectname$.UI
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
		readonly SettingWVM settingWVM;
        public SettingWindow()
        {
            InitializeComponent();
			this.settingWVM = this.DataContext as SettingWVM;
        }
    }
}
