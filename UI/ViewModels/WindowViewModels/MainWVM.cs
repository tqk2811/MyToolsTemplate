using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollections;
using System.Runtime.CompilerServices;

namespace $safeprojectname$.UI.ViewModels.WindowViewModels
{
    class MainWVM : BaseVM
    {
        public string WindowTitle => $"$safeprojectname$ - build {Singleton.BuildDate:HH:mm:ss dd/MM/yyyy}";






        public LimitObservableCollection<string> Logs { get { return _Logs; } } 
        public static ILoggerProvider LoggerProvider { get { return _Logs; } }        
		private static readonly MyLoggerProvider _Logs = new MyLoggerProvider();
    }
}
