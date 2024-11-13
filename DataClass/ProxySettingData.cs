using $safeprojectname$.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$.DataClass
{
    internal class ProxySettingData
    {
        public bool IsUseProxyForImap { get; set; } = true;
        public bool IsUseProxyForLoginGoogle { get; set; } = true;
        public bool IsShutdownCurrentConnection { get; set; } = true;
        public bool IsEnableUseProxyOnlyForHost { get; set; } = false;
        public List<string> UseProxyOnlyForHostList { get; set; } = new();
        public bool IsCheckProxyLive { get; set; } = true;
        public ProxyServiceType ProxyServiceType { get; set; }
        public Dictionary<ProxyServiceType, DictConfigureData> ProxyServiceConfigure { get; set; } = new Dictionary<ProxyServiceType, DictConfigureData>();
        public int MaxUseCountPerApi { get; set; } = 1;
    }
}
