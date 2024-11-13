using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TqkLibrary.SeleniumSupport;
using System.Drawing;
using System.Threading;
using System;
using TqkLibrary.SeleniumSupport.Helper.WaitHeplers;
using Nito.AsyncEx;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using System.Net;
using TqkLibrary.Proxy.ProxyServers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Markup;
using TqkLibrary.WinApi.FindWindowHelper;
using TqkLibrary.WinApi.WmiHelpers;
using $safeprojectname$.DataClass;
using TqkLibrary.WinApi.Helpers;
using $safeprojectname$.UI.ViewModels;

namespace $safeprojectname$.SeleniumProfiles
{
    internal abstract partial class MyBaseChromeProfile<TProfileData> : BaseChromeProfile
    {
        static SettingData Setting { get { return Singleton.Setting.Data; } }
        static protected readonly IReadOnlyList<string> _chromeArguments = new List<string>()
        {
            "--allow-insecure-localhost",
            "--allow-running-insecure-content",

            "--no-first-run",
            "--no-service-autorun",

            "--lang=en-US",

            //"--disable-gpu",
            //"--disable-cpu",
            //"--disable-webgl",
            //"--disable-canvas",
            //"--disable-audio-api",
            "--disable-translate",
            "--disable-client-rects",
            "--disable-web-security",
            //"--disable-notifications",//không login đc google
            "--disable-popup-blocking",
            "--disable-plugins-discovery",
            "--disable-save-password-bubble",
            "--disable-gpu-shader-disk-cache",
            "--disable-blink-features=AutomationControlled",
            "--disable-features=PrivacySandboxSettings4",

            "--enable-automation",
            "--enable-main-frame-before-activation",

            "--display-capture-permissions-policy-allowed",

            "--block-new-web-contents",

            "--ignore-certificate-errors",

            "--use-fake-device-for-media-stream",
            "--use-fake-ui-for-media-stream",
        };

        protected static readonly AsyncLock _asyncLock = new AsyncLock();
        
        internal TProfileData ProfileData { get; }
        public ILogger Logger { get; }
        public string UserDataDir { get; }
        public string Name { get; }

        internal MyBaseChromeProfile(TProfileData profileData)
        {
            this.ProfileData = profileData ?? throw new ArgumentNullException(nameof(profileData));
            string profileName = profileData.ToString();
            Logger = Singleton.ILoggerFactory.CreateLogger(profileName);
            Name = profileName;
            UserDataDir = Path.Combine(Singleton.UserDataDirs, profileName);
        }


        #region utilities
        public Task ClearCookieAsync()
        {
            ChromeDriver!.Manage().Cookies.DeleteAllCookies();
            return Task.CompletedTask;
        }
        public Task<string?> GetUserAgentAsync()
        {
            return Task.FromResult(ChromeDriver?.ExecuteScript("return navigator.userAgent;") as string);
        }
        public async Task<IEnumerable<OpenQA.Selenium.Cookie>> GetCookiesAsync(Uri uri)
        {
            Uri current = new Uri(ChromeDriver!.Url);
            if (!uri.Host.Equals(current.Host))
            {
                ChromeDriver!.Navigate().GoToUrl(uri);
                var waiter = _WaitHelper(CancellationToken.None);
                await waiter.WaitUntilElements("body").WithThrow().StartAsync();
            }
            return ChromeDriver!.Manage().Cookies.AllCookies;
        }
        #endregion


        #region Move Window

        static int _index = 0;
        public virtual async Task MoveWindowAsync()
        {
            using var l = await _asyncLock.LockAsync();
            (Size size, Point point) = MonitorHelper.GetLocationApp(_index++, Setting.ChromeRows, Setting.ChromeCols, Setting.ChromeScaleFactor);
            ChromeDriver!.Manage().Window.Position = point;
            ChromeDriver!.Manage().Window.Size = size;
        }
        #endregion


        #region WaitHelper
        public override WaitHelper WaitHelper(CancellationToken cancellationToken = default)
        {
            return base.WaitHelper(cancellationToken);
        }
        protected WaitHelper _WaitHelper(CancellationToken cancellationToken = default)
        {
            WaitHelper waitHelper = base.WaitHelper(cancellationToken);
            waitHelper.OnLogReceived += WaitHelper_OnLogReceived;
            waitHelper.DefaultTimeout = Setting.WaitElementTimeout;
            return waitHelper;
        }
        private void WaitHelper_OnLogReceived(string obj)
        {
            Logger.LogInformation(obj);
        }
        #endregion


        #region Setup after open
        public async Task SetUpAsync(CancellationToken cancellationToken = default)
        {
            InitUndectedChromeDriver();
            DisablePopup();
            await MoveWindowAsync();
        }
        public void DisablePopup()
        {
            if (ChromeDriver == null)
                throw new InvalidOperationException("ChromeDriver is null, need start chrome first");

            Dictionary<string, object> commandParameters = new Dictionary<string, object>
            {
                ["source"] = @"
window.alert = function(msg) { window.history.replaceState({page: 0}, msg, '?alert=' + encodeURIComponent(msg)); }; 
window.confirm = function(msg) { window.history.replaceState({page: 0}, msg, '?confirm=' + encodeURIComponent(msg)); return true; }; 
window.prompt = function(msg, defaultValue) { return window.prompt_Anwser ?? null; };
window.onbeforeunload = function(){}; 
window.setInterval(function(){window.onbeforeunload = function(){};},1);

let func_addEventListener = window.addEventListener; 
window.addEventListener = function(type,listener,options){ if(type != 'beforeunload') func_addEventListener(type,listener,options);}

function Base64UriToBlob(dataURI) {
    const splitDataURI = dataURI.split(',')
    const byteString = splitDataURI[0].indexOf('base64') >= 0 ? atob(splitDataURI[1]) : decodeURI(splitDataURI[1])
    const mimeString = splitDataURI[0].split(':')[1].split(';')[0]

    const ia = new Uint8Array(byteString.length)
    for (let i = 0; i < byteString.length; i++)
        ia[i] = byteString.charCodeAt(i)

    return new Blob([ia], { type: mimeString })
}
"
            };
            ChromeDriver.ExecuteCdpCommand("Page.addScriptToEvaluateOnNewDocument", commandParameters);
        }

        #endregion

        public async Task<string> DownloadChromeDriverAsync(string chromePath, CancellationToken cancellationToken = default)
        {
            using (var l = await _asyncLock.LockAsync())
            {
                return await ChromeDriverUpdater.DownloadAsync(Singleton.ChromeDriversDir, chromePath, cancellationToken);
            }
        }


        public abstract Task OpenChromeAsync(string? proxy = null, CancellationToken cancellationToken = default);
        public override Task CloseChromeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return base.CloseChromeAsync(cancellationToken);
            }
            finally
            {
                ClearWrapperProxy();
                KillCurrentChromeStuck();
            }

        }



        #region Cleanup
        public virtual Task<bool> DeleteUserDirAsync()
        {
            if (IsOpenChrome)
                throw new InvalidOperationException($"chrome was not close");
            try
            {
                if (Directory.Exists(UserDataDir))
                {
                    Directory.Delete(UserDataDir, true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorFunction(ex);
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        public virtual Task<bool> UpdateUserDirAsync()
        {
            return Task.FromResult(true);
        }
        public virtual async Task<bool> CleanChromeDataAsync()
        {
            if (IsOpenChrome)
                throw new InvalidOperationException($"chrome was not close");

            try
            {
                string[] paths = new string[]
                {
                    Path.Combine(UserDataDir,"Default","Cache"),
                    Path.Combine(UserDataDir,"Default","Service Worker\\CacheStorage"),
                    Path.Combine(UserDataDir,"Default","File System"),
                    Path.Combine(UserDataDir,"Default","Code Cache"),
                    Path.Combine(UserDataDir,"Default","Extensions"),
                };
                foreach (string path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            Directory.Delete(path, true);
                        }, TaskCreationOptions.LongRunning);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErrorFunction(ex);
                return false;
            }
            return true;
        }
        public static void KillChromeStuck()
        {
            Process[] processs = Process.GetProcessesByName("chrome");
            //"C:\Program Files\Google\Chrome\Application\chrome.exe" --type=renderer --user-data-dir="C:\Users\tqk2811\AppData\Local\Temp\scoped_dir4476_1275372114" --no-sandbox --disable-notifications --remote-debugging-port=0 --test-type=webdriver --allow-pre-commit-input --disable-blink-features=AutomationControlled --lang=en-US --device-scale-factor=1.75 --num-raster-threads=4 --enable-main-frame-before-activation --renderer-client-id=7 --time-ticks-at-unix-epoch=-1716779898365602 --launch-time-ticks=25471040040 --field-trial-handle=3800,i,14957578492053539325,14988567954202486643,262144 --variations-seed-version --enable-logging=handle --log-file=4080 --log-level=0 --mojo-platform-channel-handle=4076 /prefetch:1
            Regex regex = new Regex("--user-data-dir=.*?\\\\Local\\\\Temp\\\\scoped_dir");
            foreach (Process process in processs)
            {
                ProcessHelper processHelper = new ProcessHelper((uint)process.Id);
                Win32_Process? win32_Process = processHelper.Query_Win32_Process();
                if (!string.IsNullOrWhiteSpace(win32_Process?.CommandLine))
                {
                    Match match = regex.Match(win32_Process.CommandLine);
                    if (match.Success)
                    {
                        using Process? process_kill = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "taskkill",
                            Arguments = $"/f /pid {process.Id}",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                    }
                }
            }
        }
        public void KillCurrentChromeStuck()
        {
            Process[] processs = Process.GetProcessesByName("chrome");
            foreach (Process process in processs)
            {
                ProcessHelper processHelper = new ProcessHelper((uint)process.Id);
                Win32_Process? win32_Process = processHelper.Query_Win32_Process();
                if (!string.IsNullOrWhiteSpace(win32_Process?.CommandLine))
                {
                    if (win32_Process?.CommandLine?.Contains(UserDataDir, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        using Process? process_kill = Process.Start(new ProcessStartInfo()
                        {
                            FileName = "taskkill",
                            Arguments = $"/f /pid {process.Id}",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        });
                    }
                }
            }
        }
        #endregion
    }
}
