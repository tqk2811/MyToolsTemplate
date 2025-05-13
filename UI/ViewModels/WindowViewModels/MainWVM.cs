using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.WpfUi;
using TqkLibrary.WpfUi.ObservableCollections;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$.UI.ViewModels.WindowViewModels
{
    class MainWVM : BaseVM
    {
        public string WindowTitle => $"$safeprojectname$ - build {Singleton.BuildDate:HH:mm:ss dd/MM/yyyy}";






        public LimitObservableCollection<string> Logs { get { return Singleton.Logs; } } 
    }
}
