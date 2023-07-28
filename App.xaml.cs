﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace $safeprojectname$
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (System.IO.Directory.GetCurrentDirectory().StartsWith(System.IO.Path.GetTempPath()))
            {
                MessageBox.Show("Hãy giải nén ra để chạy", "Thông báo");
                Environment.Exit(-1);
            }
        }
    }
}
