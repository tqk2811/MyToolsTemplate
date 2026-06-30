using System;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Linq;
using TqkLibrary.Http.Api.Proxy.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;
using $safeprojectname$.DataClass;
using $safeprojectname$.Exceptions;

namespace $safeprojectname$.Services
{
    internal class ProxyList_ProxyApiWrapper : IProxyApiWrapper
    {
        static readonly Random _random = new Random(DateTime.Now.GetHashCode());
        readonly ProxyListData _proxyListData;
        readonly Action _saveData;
        int _index = 0;
        public ProxyList_ProxyApiWrapper(ProxyListData proxyListData, Action saveData)
        {
            _proxyListData = proxyListData ?? throw new ArgumentNullException(nameof(proxyListData));
            _saveData = saveData ?? throw new ArgumentNullException(nameof(saveData));
        }
        void SaveProxies()
        {
            _saveData.Invoke();
        }

        public bool IsAllowGetNewOnUsing => true;

        public Task<IProxyApiResponseWrapper?> GetNewProxyAsync(CancellationToken cancellationToken)
        {
            if (_proxyListData.Proxies.Count == 0)
            {
                throw new ProxyExhaustedException();
            }
            ProxyInfo? proxy = null;
            try
            {
                if (_proxyListData.IsSelectRandom)
                {
                    proxy = _proxyListData.Proxies.At(_random.Next(_proxyListData.Proxies.Count));
                }
                else
                {
                    proxy = _proxyListData.Proxies.At(_index++ % _proxyListData.Proxies.Count);
                }
                ProxyApiResponseWrapper? proxyApiResponseWrapper = null;
                if (proxy is not null)
                {
                    proxyApiResponseWrapper = new ProxyApiResponseWrapper()
                    {
                        Proxy = proxy,
                        ExpiredTime = DateTime.Now.AddDays(1),
                        IsSuccess = true,
                        NextTime = DateTime.Now,
                    };
                }
                return Task.FromResult<IProxyApiResponseWrapper?>(proxyApiResponseWrapper);
            }
            finally
            {
                if (_proxyListData.IsDeleteAfterUse && proxy is not null)
                {
                    _proxyListData.Proxies.Remove(proxy);
                    SaveProxies();
                }
            }
        }

        public override string ToString()
        {
            return "ProxyList";
        }
    }

}
