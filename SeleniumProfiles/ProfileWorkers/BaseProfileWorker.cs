using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using TqkLibrary.SeleniumSupport.Helper.WaitHeplers;

namespace $safeprojectname$.SeleniumProfiles.ProfileWorkers
{
    internal class BaseProfileWorker
    {
        readonly ILogger _logger;
        protected readonly MyBaseChromeProfile _profile;
        public BaseProfileWorker(MyBaseChromeProfile profile)
        {
            this._profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _logger = Singleton.ILoggerFactory.CreateLogger($"{this.GetType().Name}({profile.ProfileName})");
        }

        protected WaitHelper WaitHelper(CancellationToken cancellationToken = default)
        {
            WaitHelper waitHelper = _profile.WaitHelper(cancellationToken);
            waitHelper.DefaultTimeout = 30;
            waitHelper.OnLogReceived += WaitHelper_OnLogReceived;
            return waitHelper;
        }

        private void WaitHelper_OnLogReceived(string obj)
        {
            _logger.LogInformation(obj);
        }
    }
}
