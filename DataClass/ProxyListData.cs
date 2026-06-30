using System.Collections.Generic;
using TqkLibrary.Http.Api.Proxy.Wrapper;
namespace $safeprojectname$.DataClass
{
    public class ProxyListData
    {
        public List<ProxyInfo> Proxies { get; set; } = new();
        public bool IsDeleteAfterUse { get; set; } = false;
        public bool IsSelectRandom { get; set; } = false;
    }
}
