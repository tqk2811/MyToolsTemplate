using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.DataClass
{
    class SettingData
    {
        public float? ChromeScaleFactor{ get; set; } = null;
        public int ChromeRows { get; set; } = 2;
        public int ChromeCols { get; set; } = 2;
        public int WaitElementTimeout{ get; set; } = 30000;
        public ProxySettingData ProxySettingData { get; set; } = new();
    }
}
