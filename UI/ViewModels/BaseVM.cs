using $safeprojectname$.DataClass;
using $safeprojectname$.UI.Commands;
using TqkLibrary.WpfUi;

namespace $safeprojectname$.UI.ViewModels
{
    internal class BaseVM : BaseViewModel
    {
        protected readonly ILogger _logger;
        public BaseVM()
        {
            _logger = Singleton.ILoggerFactory.CreateLogger(this.GetType());
            this.CopyTextCommand = new BaseCommand<string>(_CopyTextCommand);
            this.LinkNavigateCommand = new BaseCommand<string>(_LinkNavigateCommand);
        }
        
        protected SettingData Setting { get { return Singleton.Setting.Data; } }
        protected void SaveSetting() => Singleton.Setting.TriggerSave();

        
        public BaseCommand<string> CopyTextCommand { get; }
        void _CopyTextCommand(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    Clipboard.SetText(text);
                }
                catch (Exception ex)
                {
                    _logger.LogErrorFunction(ex);
                }
            }
        }

        public BaseCommand<string> LinkNavigateCommand { get; }
        void _LinkNavigateCommand(string link)
        {
            if (Uri.TryCreate(link, UriKind.Absolute, out Uri? uri))
            {
                try
                {
                    using Process? process = Process.Start(new ProcessStartInfo()
                    {
                        FileName = uri.ToString(),
                        UseShellExecute = true,
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogErrorFunction(ex);
                }
            }
        }
    }
}
