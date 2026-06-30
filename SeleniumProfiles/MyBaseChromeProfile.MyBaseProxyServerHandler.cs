using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Proxy;
using TqkLibrary.Proxy.Authentications;
using TqkLibrary.Proxy.Handlers;
using TqkLibrary.Proxy.Interfaces;
using TqkLibrary.Proxy.ProxyServers;
using TqkLibrary.Proxy.ProxySources;
using TqkLibrary.Http.Api.Proxy.Wrapper.Enums;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;

namespace $safeprojectname$.SeleniumProfiles
{
    internal abstract partial class MyBaseChromeProfile
    {
        ProxyServer? _proxyServer = null;
        MyBaseProxyServerHandler? _proxyServerHandler = null;
        protected string WrapperProxy(IProxyInfo? proxyInfo = null)
        {
            _proxyServerHandler = new MyBaseProxyServerHandler();
            _proxyServer = new ProxyServer(IPEndPoint.Parse("127.0.0.1:0"), _proxyServerHandler);
            SetProxy(proxyInfo);
            _proxyServer.StartListen();
            return $"127.0.0.1:{_proxyServer.IPEndPoint!.Port}";
        }
        public void ShutdownCurrentConnection()
        {
            _proxyServer?.ShutdownCurrentConnection();
        }
        public void SetProxy(IProxyInfo? proxyInfo = null)
        {
            _proxyServerHandler?.SetProxy(proxyInfo);
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
            public void SetProxy(IProxyInfo? proxyInfo = null)
            {
                if (proxyInfo is null)
                {
                    _currentProxySource = null;
                }
                else
                {
                    switch (proxyInfo.ProxyType)
                    {
                        case ProxyType.Http:
                            Uri uri;
                            if (string.IsNullOrWhiteSpace(proxyInfo.UserName) || string.IsNullOrWhiteSpace(proxyInfo.Password))
                            {
                                uri = new Uri($"http://{proxyInfo.Address}:{proxyInfo.Port}");
                            }
                            else
                            {
                                uri = new Uri($"http://{proxyInfo.UserName}:{proxyInfo.Password}@{proxyInfo.Address}:{proxyInfo.Port}");
                            }
                            _currentProxySource = new HttpProxySource(uri);
                            break;

                        case ProxyType.Socks4:
                            _currentProxySource = new Socks4ProxySource(new IPEndPoint(IPAddress.Parse(proxyInfo.Address), proxyInfo.Port));
                            break;

                        case ProxyType.Socks5:
                            if (string.IsNullOrWhiteSpace(proxyInfo.UserName) || string.IsNullOrWhiteSpace(proxyInfo.Password))
                            {
                                _currentProxySource = new Socks5ProxySource(new IPEndPoint(IPAddress.Parse(proxyInfo.Address), proxyInfo.Port));
                            }
                            else
                            {
                                _currentProxySource = new Socks5ProxySource(
                                    new IPEndPoint(IPAddress.Parse(proxyInfo.Address), proxyInfo.Port),
                                    new ProxyCredential(proxyInfo.UserName, proxyInfo.Password)
                                    );
                            }
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

            public override Task<IConnectSource> GetConnectSourceAsync(Guid tunnelId, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IConnectSource>(new MyConnectTunnel(this, tunnelId));
            }
        }
    }
}
