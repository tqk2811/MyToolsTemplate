using $safeprojectname$.DataClass;
using $safeprojectname$.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Linq;
using TqkLibrary.Net.Proxy.Wrapper;

namespace $safeprojectname$.Services
{
    class HttpProxyList_ProxyApiWrapper : IProxyApiWrapper
    {
        static readonly Random _random = new Random(DateTime.Now.GetHashCode());
        readonly DictConfigureData _configureData;
        readonly List<string> _proxies;
        readonly Action _saveData;
        int _index = 0;
        public HttpProxyList_ProxyApiWrapper(DictConfigureData configureData, Action saveData)
        {
            _configureData = configureData ?? throw new ArgumentNullException(nameof(configureData));
            _proxies = configureData.Proxies ?? throw new InvalidOperationException($"HttpProxyList rỗng");
            _saveData = saveData ?? throw new ArgumentNullException(nameof(saveData));
        }
        void SaveProxies()
        {
            _configureData.Proxies = _proxies;
            _saveData.Invoke();
        }

        public bool IsAllowGetNewOnUsing => true;

        public Task<IProxyApiResponseWrapper> GetNewProxyAsync(CancellationToken cancellationToken)
        {
            if (_proxies.Count == 0)
            {
                throw new ProxyExhaustedException();
            }
            string? proxy = null;
            try
            {
                if (_configureData.IsSelectRandom)
                {
                    proxy = _proxies.At(_random.Next(_proxies.Count));
                }
                else
                {
                    proxy = _proxies.At(_index++ % _proxies.Count);
                }

                return Task.FromResult<IProxyApiResponseWrapper>(new ProxyApiResponseWrapper()
                {
                    Proxy = proxy,
                    ExpiredTime = DateTime.Now.AddDays(1),
                    IsSuccess = true,
                    NextTime = DateTime.Now,
                });
            }
            finally
            {
                if (_configureData.IsDeleteAfterUse && !string.IsNullOrWhiteSpace(proxy))
                {
                    _proxies.Remove(proxy);
                    SaveProxies();
                }
            }
        }

        public override string ToString()
        {
            return "HttpProxyList";
        }
    }

}
