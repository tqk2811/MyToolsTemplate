using $safeprojectname$.Enums;
using System.Collections.Generic;

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
        public ProxyListData ProxyListData { get; set; } = new();
        public Dictionary<ProxyServiceType, List<string>> ProxyApiKeys { get; set; } = new Dictionary<ProxyServiceType, List<string>>();
        public int MaxUseCountPerApi { get; set; } = 1;
    }
}
