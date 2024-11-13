using System.Threading.Tasks;
using System.Threading;
using System;
using TqkLibrary.Proxy.Authentications;
using System.Linq;
using TqkLibrary.Proxy.Handlers;
using TqkLibrary.Proxy.Interfaces;
using TqkLibrary.Proxy.ProxySources;
using System.Net;
using TqkLibrary.Proxy.ProxyServers;
using System.Net.Sockets;

namespace $safeprojectname$.SeleniumProfiles
{
    internal abstract partial class MyBaseChromeProfile<TProfileData>
    {
        ProxyServer? _proxyServer = null;
        MyBaseProxyServerHandler? _proxyServerHandler = null;
        protected string WrapperProxy(string? proxy = null)
        {
            _proxyServerHandler = new MyBaseProxyServerHandler();
            _proxyServer = new ProxyServer(IPEndPoint.Parse("127.0.0.1:0"), _proxyServerHandler);
            SetProxy(proxy);
            _proxyServer.StartListen();
            return $"127.0.0.1:{_proxyServer.IPEndPoint!.Port}";
        }
        public void ShutdownCurrentConnection()
        {
            _proxyServer?.ShutdownCurrentConnection();
        }
        public void SetProxy(string? proxy = null)
        {
            _proxyServerHandler?.SetProxy(proxy);
            if (Setting.ProxySettingData.IsShutdownCurrentConnection)
                _proxyServer?.ShutdownCurrentConnection();
        }
        void ClearWrapperProxy()
        {
            _proxyServerHandler = null;
            _proxyServer?.StopListen();
            _proxyServer?.Dispose();
            _proxyServer = null;
        }
        class MyBaseProxyServerHandler : BaseProxyServerHandler
        {
            IProxySource defaultProxySource = new MyLocalProxySource();
            IProxySource? _currentProxySource = null;
            public void SetProxy(string? proxy)
            {
                if (string.IsNullOrWhiteSpace(proxy))
                {
                    _currentProxySource = null;
                }
                else
                {
                    string[] split = proxy.Split(':', '|').Select(x => x.Trim()).ToArray();
                    switch (split.Length)
                    {
                        case 2:
                            _currentProxySource = new HttpProxySource(new Uri($"http://{split[0]}:{split[1]}"));
                            break;

                        case 4:
                            _currentProxySource = new HttpProxySource(new Uri($"http://{split[0]}:{split[1]}"), new HttpProxyAuthentication(split[2], split[3]));
                            break;
                    }
                }
            }
            public override Task<IProxySource> GetProxySourceAsync(Uri? uri, IUserInfo userInfo, CancellationToken cancellationToken = default)
            {
                if (_currentProxySource is not null) return Task.FromResult(_currentProxySource);
                return Task.FromResult(defaultProxySource);
            }
        }

        class MyLocalProxySource : LocalProxySource
        {
            class MyConnectTunnel : LocalProxySource.ConnectTunnel
            {
                public MyConnectTunnel(LocalProxySource localProxySource, Guid tunnelId) : base(localProxySource, tunnelId)
                {
                }

                public override async Task ConnectAsync(Uri address, CancellationToken cancellationToken = default)
                {
                    if (address is null)
                        throw new ArgumentNullException(nameof(address));
                    CheckIsDisposed();

                    switch (address.HostNameType)
                    {
                        case UriHostNameType.Dns://http://host/abc/def
                        case UriHostNameType.IPv4:
                        case UriHostNameType.IPv6:
                            {
                                if (!_proxySource.IsSupportIpv6 && address.HostNameType == UriHostNameType.IPv6)
                                    throw new NotSupportedException($"IpV6 are not support");

                                if (_SupportUriSchemes.Any(x => x.Equals(address.Scheme, StringComparison.InvariantCulture)))
                                {
                                    if (address.HostNameType == UriHostNameType.Dns)
                                    {
                                        IPHostEntry hostInfo = Dns.GetHostEntry(address.Host);
                                        var addressList = hostInfo.AddressList//ưu tiên ip v4
                                                .Where(x => x.AddressFamily == AddressFamily.InterNetwork || x.AddressFamily == AddressFamily.InterNetworkV6)
                                                .OrderBy(x => x.AddressFamily).ToArray();

                                        await _tcpClient.ConnectAsync(
                                            addressList,
                                            address.Port
#if NET5_0_OR_GREATER
                                            , cancellationToken
#endif
                                        );
                                        _stream = _tcpClient.GetStream();
                                    }
                                    else
                                    {
                                        await _tcpClient.ConnectAsync(
                                            address.Host,
                                            address.Port
#if NET5_0_OR_GREATER
                                            , cancellationToken
#endif
                                        );
                                        _stream = _tcpClient.GetStream();
                                    }
                                }
                                else
                                {
                                    throw new NotSupportedException(address.Scheme);
                                }
                            }
                            break;

                        default:
                            throw new NotSupportedException(address.HostNameType.ToString());
                    }
                }
            }

            public override IConnectSource GetConnectSource(Guid tunnelId)
            {
                return new MyConnectTunnel(this, tunnelId);
            }
        }
    }
}
