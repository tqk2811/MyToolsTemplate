using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollection;
namespace $safeprojectname$.UI.ViewModels.WindowViewModels
{
    class MainWVM : BaseVM
    {
        public string WindowTitle => $"$safeprojectname$ - build {Singleton.BuildDate:HH:mm:ss dd/MM/yyyy}";


        public LimitObservableCollection<string> Logs { get{ return _Logs; } } 

		private static readonly LimitObservableCollection<string> _Logs 
			= new LimitObservableCollection<string>(() => Singleton.LogDir + $"\\{DateTime.Now:yyyy-MM-dd}.log");
		public static void WriteLog(string log)
		{
			_Logs.Add($"{DateTime.Now:HH:mm:ss} {log}");
		}
    }
}
