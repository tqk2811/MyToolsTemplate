using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Linq;
using TqkLibrary.Proxy.Authentications;
using TqkLibrary.Proxy.Interfaces;
using TqkLibrary.Proxy.ProxyServers;
using TqkLibrary.Proxy.ProxySources;
using TqkLibrary.SeleniumSupport;
using TqkLibrary.SeleniumSupport.Interfaces;

namespace $safeprojectname$.SeleniumProfiles
{
    internal class ChromeProcessProfile<TProfileData> : MyBaseChromeProfile<TProfileData>
    {
        public ChromeProcessProfile(TProfileData profileData) : base(profileData)
        {
        }


        public override async Task OpenChromeAsync(string? proxy = null, CancellationToken cancellationToken = default)
        {
            if (IsOpenChrome)
                return;

            List<string> arguments =
            [
                $"--user-data-dir={UserDataDir}",
                .. _chromeArguments,
            ];
            if (Singleton.Setting.Data.ChromeScaleFactor.HasValue && Singleton.Setting.Data.ChromeScaleFactor.Value > 0)
            {
                arguments.Add($"--force-device-scale-factor={Singleton.Setting.Data.ChromeScaleFactor.Value}");
            }


            string wrapperProxy = WrapperProxy(proxy);
            arguments.Add($"--proxy-server=http://{wrapperProxy}");


            List<string> extensions = new();
            
            if (extensions.Any())
                arguments.Add($"--load-extension={string.Join(",", extensions)}");


            string chromeDriverDir = await DownloadChromeDriverAsync(ChromeDriverUpdater.GetChromePath(), cancellationToken);
            var service = ChromeDriverService.CreateDefaultService(chromeDriverDir);
            service.HideCommandPromptWindow = true;


            await base.OpenChromeConnectExistedDebugAsync(
                new ControlChromeProcess(arguments),
                service
                );


            await SetUpAsync(cancellationToken);
        }

        class ControlChromeProcess : IControlChromeProcess
        {
            readonly string[] _arguments;
            Process? _process = null;
            int _port = 0;
            public int? ProcessId => _process?.Id;

            public ControlChromeProcess(params string[] arguments) : this(arguments.AsEnumerable())
            {
            }
            public ControlChromeProcess(IEnumerable<string> arguments)
            {
                _arguments = arguments?.Where(x => !string.IsNullOrWhiteSpace(x))?.ToArray() ?? new string[] { };
            }

            public async Task CloseChromeAsync(CancellationToken cancellationToken = default)
            {
                if (_process is not null)
                {
                    using CancellationTokenSource cancel = new CancellationTokenSource(1000);
                    using var register = cancel.Token.Register(() => { try { _process?.Kill(); } catch { } });
                    await _process.WaitForExitAsync();
                    _process.Dispose();
                    _process = null;
                }
            }

            public Task<ChromeOptions> GetChromeOptionsAsync(CancellationToken cancellationToken = default)
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.DebuggerAddress = $"127.0.0.1:{_port}";
                return Task.FromResult(chromeOptions);
            }

            public Task<bool> GetIsOpenChromeAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_process is not null);
            }

            public Task OpenChromeAsync(string? advArgs = null, CancellationToken cancellationToken = default)
            {
                List<string> arguments = new List<string>(_arguments);
                if (!string.IsNullOrWhiteSpace(advArgs)) arguments.Add(advArgs);

                _port = PortUtilities.FindFreePort();

                arguments.Add($"--remote-debugging-port={_port}");

                string chromePath = ChromeDriverUpdater.GetChromePath();

                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = chromePath,
                    WorkingDirectory = new FileInfo(chromePath).Directory!.FullName,
                    UseShellExecute = false,
                };
                arguments.Where(x => !string.IsNullOrWhiteSpace(x)).ForEach(x => processStartInfo.ArgumentList.Add(x));
                _process = Process.Start(processStartInfo);

                return Task.CompletedTask;
            }
        }
    }
}
