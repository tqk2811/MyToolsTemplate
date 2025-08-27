using System;

namespace $safeprojectname$.Enums
{
    [Flags]
    internal enum ProxyServiceType
    {
        None = 0,
        ProxyList = 1 << 0,
        TinsoftProxy = 1 << 1,
        ProxyNo1 = 1 << 2,

    }
}
