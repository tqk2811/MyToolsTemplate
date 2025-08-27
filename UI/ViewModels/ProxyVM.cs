using $safeprojectname$.Enums;
using $safeprojectname$.UI.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Net.Proxy.Wrapper.Implements;
using TqkLibrary.Net.Proxy.Wrapper;
using $safeprojectname$.DataClass;
using System.IO;
using $safeprojectname$.Services;
using TqkLibrary.Linq;

namespace $safeprojectname$.UI.ViewModels
{
    internal class ProxyVM : BaseDataVM<ProxySettingData>
    {
        public ProxyVM(ProxySettingData data, Action saveCallback) : base(data, saveCallback)
        {
            this.LoadHttpProxyListCommand = new OpenFileDialogCommand("txt file|*.txt|All file|*.*", _LoadHttpProxyListCommand);
            this.LoadTinsoftProxyKeyCommand = new OpenFileDialogCommand("txt file|*.txt|All file|*.*", _LoadTinsoftProxyKeyCommand);
            this.LoadUseProxyOnlyForHostCommand = new OpenFileDialogCommand("txt file|*.txt|All file|*.*", _LoadUseProxyOnlyForHostCommand);
            this.LoadProxyNo1ProxyKeyCommand = new OpenFileDialogCommand("txt file|*.txt|All file|*.*", _LoadProxyNo1ProxyKeyCommand);
            foreach (var item in Enum.GetValues<ProxyServiceType>().Except(ProxyServiceType.None, ProxyServiceType.ProxyList))
            {
                if (!data.ProxyApiKeys.ContainsKey(item))
                    data.ProxyApiKeys[item] = new();
            }
        }

        public bool IsUseProxyForImap
        {
            get { return Data.IsUseProxyForImap; }
            set { Data.IsUseProxyForImap = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public bool IsUseProxyForLoginGoogle
        {
            get { return Data.IsUseProxyForLoginGoogle; }
            set { Data.IsUseProxyForLoginGoogle = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public bool IsShutdownCurrentConnection
        {
            get { return Data.IsShutdownCurrentConnection; }
            set { Data.IsShutdownCurrentConnection = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public bool IsEnableUseProxyOnlyForHost
        {
            get { return Data.IsEnableUseProxyOnlyForHost; }
            set { Data.IsEnableUseProxyOnlyForHost = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public BaseCommand LoadUseProxyOnlyForHostCommand { get; }
        async void _LoadUseProxyOnlyForHostCommand(string? filePath)
        {
            try
            {
                Data.UseProxyOnlyForHostList.Clear();
                if (File.Exists(filePath))
                {
                    IEnumerable<string> lines = await File.ReadAllLinesAsync(filePath);
                    lines = lines.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                    Data.UseProxyOnlyForHostList.AddRange(lines);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorFunction(ex);
                ex.ShowMessageBox();
            }
            finally
            {
                SaveAndRefresh();
            }
        }
        public int UseProxyOnlyForHost_Count { get { return Data.UseProxyOnlyForHostList.Count; } }

        public bool IsCheckProxyLive
        {
            get { return Data.IsCheckProxyLive; }
            set { Data.IsCheckProxyLive = value; NotifyPropertyChange(); SaveSetting(); }
        }

        public ProxyServiceType ProxyServiceType
        {
            get { return Data.ProxyServiceType; }
            set { Data.ProxyServiceType = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public int MaxUseCountPerApi
        {
            get { return Data.MaxUseCountPerApi; }
            set { Data.MaxUseCountPerApi = value; NotifyPropertyChange(); SaveSetting(); }
        }

        public void SaveAndRefresh()
        {
            SaveSetting();
            Refresh();
        }
        public void Refresh()
        {
            NotifyPropertyChange(nameof(HttpProxyList_Count));
            NotifyPropertyChange(nameof(TinsoftProxy_KeyCount));
            NotifyPropertyChange(nameof(ProxyNo1_KeyCount));
            NotifyPropertyChange(nameof(UseProxyOnlyForHost_Count));
        }


        #region Http Proxy List
        public int HttpProxyList_Count
        {
            get { return Data.ProxyListData.Proxies.Count(); }
        }
        public bool HttpProxyList_IsSelectRandom
        {
            get { return Data.ProxyListData.IsSelectRandom; }
            set { Data.ProxyListData.IsSelectRandom = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public bool HttpProxyList_IsDeleteAfterUse
        {
            get { return Data.ProxyListData.IsDeleteAfterUse; }
            set { Data.ProxyListData.IsDeleteAfterUse = value; NotifyPropertyChange(); SaveSetting(); }
        }
        public OpenFileDialogCommand LoadHttpProxyListCommand { get; }
        async void _LoadHttpProxyListCommand(string? filePath)
        {
            try
            {
                Data.ProxyListData.Proxies.Clear();
                if (File.Exists(filePath))
                {
                    IEnumerable<string> lines = await File.ReadAllLinesAsync(filePath);
                    lines = lines
                        .Where(x =>
                        {
                            var split = x.Split(':');
                            return (split.Length == 2 || split.Length == 4) && !split.Any(y => string.IsNullOrWhiteSpace(y));
                        });
                    Data.ProxyListData.Proxies.AddRange(lines.Select(x => ProxyInfo.ParseHttpProxy(x)).Where(x => x is not null)!.Distinct()!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorFunction(ex);
                ex.ShowMessageBox();
            }
            finally
            {
                SaveAndRefresh();
            }
        }

        #endregion

        #region TinsoftProxy
        public int TinsoftProxy_KeyCount
        {
            get { return Data.ProxyApiKeys[ProxyServiceType.TinsoftProxy].Count(); }
        }
        public OpenFileDialogCommand LoadTinsoftProxyKeyCommand { get; }
        async void _LoadTinsoftProxyKeyCommand(string? filePath)
        {
            try
            {
                Data.ProxyApiKeys[ProxyServiceType.TinsoftProxy].Clear();
                if (File.Exists(filePath))
                {
                    IEnumerable<string> lines = await File.ReadAllLinesAsync(filePath);
                    lines = lines
                        .Where(x => !string.IsNullOrWhiteSpace(x));

                    Data.ProxyApiKeys[ProxyServiceType.TinsoftProxy].AddRange(lines);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorFunction(ex);
                ex.ShowMessageBox();
            }
            finally
            {
                SaveAndRefresh();
            }
        }
        #endregion

        #region ProxyNo1
        public int ProxyNo1_KeyCount
        {
            get { return Data.ProxyApiKeys[ProxyServiceType.ProxyNo1].Count(); }
        }
        public OpenFileDialogCommand LoadProxyNo1ProxyKeyCommand { get; }
        async void _LoadProxyNo1ProxyKeyCommand(string? filePath)
        {
            try
            {
                Data.ProxyApiKeys[ProxyServiceType.ProxyNo1].Clear();
                if (File.Exists(filePath))
                {
                    IEnumerable<string> lines = await File.ReadAllLinesAsync(filePath);
                    lines = lines
                        .Where(x => !string.IsNullOrWhiteSpace(x));

                    Data.ProxyApiKeys[ProxyServiceType.ProxyNo1].AddRange(lines);
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorFunction(ex);
                ex.ShowMessageBox();
            }
            finally
            {
                SaveAndRefresh();
            }
        }
        #endregion


        public async IAsyncEnumerable<IProxyApiWrapper> GetProxyApiWrapperAsync()
        {
            var values = Enum.GetValues<ProxyServiceType>().Except(ProxyServiceType.None);
            await Task.CompletedTask;
            foreach (var value in values.Where(x => Data.ProxyServiceType.HasFlag(x)))
            {
                switch (value)
                {
                    case ProxyServiceType.ProxyList:
                        {
                            yield return new ProxyList_ProxyApiWrapper(Data.ProxyListData, SaveAndRefresh);
                        }
                        break;

                    case ProxyServiceType.TinsoftProxy:
                        {
                            foreach (var item in Data.ProxyApiKeys[ProxyServiceType.TinsoftProxy])
                            {
                                yield return new TinsoftProxyApiWrapper(item);
                            }
                        }
                        break;

                    case ProxyServiceType.ProxyNo1:
                        {
                            foreach (var item in Data.ProxyApiKeys[ProxyServiceType.ProxyNo1])
                            {
                                yield return new ProxyNo1ComApiWrapper(item);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
