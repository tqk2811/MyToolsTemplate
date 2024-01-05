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


        public LimitObservableCollection<string> Logs { get{ return _Logs; } } 

		private static readonly LimitObservableCollection<string> _Logs 
			= new LimitObservableCollection<string>(() => Singleton.LogDir + $"\\{DateTime.Now:yyyy-MM-dd}.log");
		public static void WriteLog(string log, [CallerMemberName] string callFunction = null)
        {
            _Logs.Add($"{DateTime.Now:HH:mm:ss} [{callFunction}] {log}");
        }
        public static void WriteExceptionLog(string message, Exception ex, [CallerMemberName] string callFunction = null)
        {
            if (ex is AggregateException ae) ex = ae.InnerException;
            _Logs.Add($"{DateTime.Now:HH:mm:ss} [{callFunction}] [{message}] {ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
        }
        public static void WriteExceptionLog(Exception ex, [CallerMemberName] string callFunction = null)
        {
            if (ex is AggregateException ae) ex = ae.InnerException;
            _Logs.Add($"{DateTime.Now:HH:mm:ss} [{callFunction}] {ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
        }
    }
}
