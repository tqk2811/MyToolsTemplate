using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using TqkLibrary.SeleniumSupport;

namespace $safeprojectname$.SeleniumProfiles
{
    internal class ChromeProfile<TProfileData> : MyBaseChromeProfile<TProfileData>
    {
        public ChromeProfile(TProfileData profileData) : base(profileData)
        {
        }

        public override async Task OpenChromeAsync(string? proxy = null, CancellationToken cancellationToken = default)
        {
            if (IsOpenChrome)
                return;

            ChromeOptions options = new ChromeOptions();
            options.AddArguments(_chromeArguments);
            options.AddUserDataDir(UserDataDir);
            if (Singleton.Setting.Data.ChromeScaleFactor.HasValue && Singleton.Setting.Data.ChromeScaleFactor.Value > 0)
            {
                options.AddArgument($"--force-device-scale-factor={Singleton.Setting.Data.ChromeScaleFactor.Value}");
            }
            //string wrapperProxy = WrapperProxy(proxy);
            //options.AddArgument($"--proxy-server=http://{wrapperProxy}");

            List<string> extensions = new();
            if (extensions.Any())
                options.AddExtensions(extensions);

            string chromeDriverDir = await DownloadChromeDriverAsync(ChromeDriverUpdater.GetChromePath(), cancellationToken);

            var service = ChromeDriverService.CreateDefaultService(chromeDriverDir);
            service.HideCommandPromptWindow = true;

            base.OpenChrome(options, service);
            
            await SetUpAsync(cancellationToken);
        }
    }
}
