using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using OpenQA.Selenium.Chrome;
using TqkLibrary.SeleniumSupport.Interfaces;
using OpenQA.Selenium.Internal;
using System.Diagnostics;
using TqkLibrary.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace $safeprojectname$.SeleniumProfiles
{
    internal class OrbitalProfile<TProfileData> : MyBaseChromeProfile<TProfileData>
    {
        public string ChromeDir { get; }
        public string ChromePath { get; }
        internal OrbitalProfile(TProfileData profileData) : base(profileData)
        {
            string gologinPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".gologin");
            ChromeDir = Directory.GetDirectories(Path.Combine(gologinPath, "browser"), "orbita-browser*").Last();
            ChromePath = Path.Combine(ChromeDir, "chrome.exe");
            if (!File.Exists(ChromePath))
                throw new InvalidOperationException($"Hãy cài gologin để sử dụng Orbital Browser");
        }

        public override async Task OpenChromeAsync(string? proxy = null, CancellationToken cancellationToken = default)
        {
            if (IsOpenChrome)
                return;

            await GenerateOrbitalProfileAsync();


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

            string chromeDriverDir = await DownloadChromeDriverAsync(ChromePath, cancellationToken);
            var service = ChromeDriverService.CreateDefaultService(chromeDriverDir);
            service.HideCommandPromptWindow = true;

            await base.OpenChromeConnectExistedDebugAsync(
                new ControlChromeProcess(this, arguments),
                service
                );

            await SetUpAsync(cancellationToken);
        }


        async Task GenerateOrbitalProfileAsync(CancellationToken cancellationToken = default)
        {
            string filePath = Path.Combine(UserDataDir, "Default\\Preferences");
            if (File.Exists(filePath))
                return;

            OrbitaConfigure configure = new OrbitaConfigure();

            Random random = new Random(DateTime.Now.GetHashCode());

            dynamic json;
            if (File.Exists(filePath))
            {
                json = JsonConvert.DeserializeObject(await File.ReadAllTextAsync(filePath)) ?? new JObject();
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(UserDataDir, "Default"));
                json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Resource.Preferences)) ?? new JObject();
            }

            if (json.gologin is null)
                json.gologin = new JObject();

            if (json.gologin.audioContext is null)
                json.gologin.audioContext = new JObject();
            if (configure.IsAudioNoise)
            {
                json.gologin.audioContext.enable = true;
                json.gologin.audioContext.noiseValue = (float)configure.AudioNoiseValue;
            }

            if (configure.IsCanvasNoise)
            {
                json.gologin.canvasMode = "noise";
            }
            else
            {
                json.gologin.canvasMode = "off";
            }
            json.gologin.canvasNoise = (float)configure.CanvasNoiseValue;

            json.gologin.dns = string.Empty;
            json.gologin.doNotTrack = true;

            json.gologin.deviceMemory = configure.DeviceMemory;
            json.gologin.hardwareConcurrency = configure.CpuCount;
            json.gologin.is_m1 = false;


            if (json.gologin.geoLocation is null)
                json.gologin.geoLocation = new JObject();

            json.gologin.geoLocation.accuracy = configure.GeoAccuracy;
            json.gologin.geoLocation.latitude = (float)configure.GeoLat;
            json.gologin.geoLocation.longitude = (float)configure.GeoLon;
            json.gologin.geoLocation.mode = configure.GeoMode;

            json.gologin.client_rects_noise_enable = configure.IsClientRectsNoise;
            if (configure.IsClientRectsNoise)
            {
                json.gologin.getClientRectsNoice = (float)configure.ClientRectsNoiseValue;
                json.gologin.get_client_rects_noise = json.gologin.getClientRectsNoice;
            }

            json.gologin.name = Name;

            json.gologin.screenHeight = configure.Height;
            json.gologin.screenWidth = configure.Width;

            if (!string.IsNullOrWhiteSpace(configure.UserAgent))
                json.gologin.userAgent = configure.UserAgent;

            json.gologin.webglNoiceEnable = configure.IsWebGlNoise;
            json.gologin.webgl_noice_enable = configure.IsWebGlNoise;
            if (configure.IsWebGlNoise)
            {
                json.gologin.webglNoiseValue = (float)configure.WebGlNoiseValue;
                json.gologin.webgl_noise_value = json.gologin.webglNoiseValue;
            }

            if (json.gologin.webrtc is null)
                json.gologin.webrtc = new JObject();

            json.gologin.webrtc.enable = configure.IsEnableWebrtc;
            json.gologin.webrtc.mode = "alerted";
            json.gologin.webrtc.should_fill_empty_ice_list = true;

            string[] proxy_split = (configure.Proxy ?? string.Empty).Split(':');

            if (json.gologin.proxy is null)
                json.gologin.proxy = new JObject();
            json.gologin.proxy.username = proxy_split.Length == 4 ? proxy_split[2] : "";
            json.gologin.proxy.password = proxy_split.Length == 4 ? proxy_split[3] : "";


            if (json.gologin.mobile is null)
                json.gologin.mobile = new JObject();
            json.gologin.mobile.device_scale_factor = "";
            json.gologin.mobile.enable = "";
            json.gologin.mobile.height = "";
            json.gologin.mobile.width = "";


            string jsonText = JsonConvert.SerializeObject(json);
            await File.WriteAllTextAsync(filePath, jsonText);
        }

        class ControlChromeProcess : IControlChromeProcess
        {
            readonly string[] _arguments;
            readonly OrbitalProfile<TProfileData> _orbitalProfile;
            Process? _process = null;
            int _port = 0;
            public int? ProcessId => _process?.Id;

            public ControlChromeProcess(OrbitalProfile<TProfileData> orbitalProfile, params string[] arguments) : this(orbitalProfile, arguments.AsEnumerable())
            {
            }
            public ControlChromeProcess(OrbitalProfile<TProfileData> orbitalProfile, IEnumerable<string> arguments)
            {
                this._orbitalProfile = orbitalProfile ?? throw new ArgumentNullException(nameof(orbitalProfile));
                _arguments = arguments?.Where(x => !string.IsNullOrWhiteSpace(x))?.ToArray() ?? new string[] { };
            }

            public async Task CloseChromeAsync(CancellationToken cancellationToken = default)
            {
                if (_process is not null)
                {
                    _process.Kill();
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

                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = _orbitalProfile.ChromePath,
                    WorkingDirectory = _orbitalProfile.ChromeDir,
                    UseShellExecute = false,
                };
                arguments.Where(x => !string.IsNullOrWhiteSpace(x)).ForEach(x => processStartInfo.ArgumentList.Add(x));
                _process = Process.Start(processStartInfo);

                return Task.CompletedTask;
            }
        }

        class OrbitaConfigure
        {
            readonly Random _random = new Random(DateTime.Now.GetHashCode());
            public OrbitaConfigure()
            {
                DeviceMemory = 1024 * _random.Next(1, 97);
                ClientRectsNoiseValue = _random.NextDouble() * 10;
                Height = 16 * _random.Next(48, 256 + 1);//768-4096
                Width = 16 * _random.Next(64, 150 + 1);//1024-2400
                CpuCount = 2 * _random.Next(1, 97);
                WebGlNoiseValue = _random.NextDouble() * 100;
                CanvasNoiseValue = _random.NextDouble();
                AudioNoiseValue = _random.NextDouble() * 0.00000001;

                GeoAccuracy = _random.Next(50, 200);
                GeoLat = _random.NextDouble() * 80 + 22;
                GeoLon = _random.NextDouble() * 7 + 102;
            }
            void NotifyPropertyChange() { }



            string? _ProfileName = $"Profile mới của {Environment.MachineName}";
            public string? ProfileName
            {
                get { return _ProfileName; }
                set { _ProfileName = value; NotifyPropertyChange(); }
            }

            string? _UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
            public string? UserAgent
            {
                get { return _UserAgent; }
                set { _UserAgent = value; NotifyPropertyChange(); }
            }

            string? _Proxy;
            public string? Proxy
            {
                get { return _Proxy; }
                set { _Proxy = value; NotifyPropertyChange(); }
            }

            bool _IsGenGeoBaseOnProxy = false;
            public bool IsGenGeoBaseOnProxy
            {
                get { return _IsGenGeoBaseOnProxy; }
                set { _IsGenGeoBaseOnProxy = value; NotifyPropertyChange(); }
            }






            int _DeviceMemory = 0;
            public int DeviceMemory
            {
                get { return _DeviceMemory; }
                set { _DeviceMemory = value; NotifyPropertyChange(); }
            }
            int _CpuCount = 0;
            public int CpuCount
            {
                get { return _CpuCount; }
                set { _CpuCount = value; NotifyPropertyChange(); }
            }





            int _Width = 0;
            public int Width
            {
                get { return _Width; }
                set { _Width = value; NotifyPropertyChange(); }
            }
            int _Height = 0;
            public int Height
            {
                get { return _Height; }
                set { _Height = value; NotifyPropertyChange(); }
            }







            int _GeoAccuracy = 100;
            public int GeoAccuracy
            {
                get { return _GeoAccuracy; }
                set { _GeoAccuracy = value; NotifyPropertyChange(); }
            }
            double _GeoLat = 10.822;
            public double GeoLat
            {
                get { return _GeoLat; }
                set { _GeoLat = value; NotifyPropertyChange(); }
            }
            double _GeoLon = 106.6257;
            public double GeoLon
            {
                get { return _GeoLon; }
                set { _GeoLon = value; NotifyPropertyChange(); }
            }
            string _GeoMode = "prompt";
            public string GeoMode
            {
                get { return _GeoMode; }
                set { _GeoMode = value; NotifyPropertyChange(); }
            }




            bool _IsClientRectsNoise = true;
            public bool IsClientRectsNoise
            {
                get { return _IsClientRectsNoise; }
                set { _IsClientRectsNoise = value; NotifyPropertyChange(); }
            }
            double _ClientRectsNoiseValue;
            public double ClientRectsNoiseValue
            {
                get { return _ClientRectsNoiseValue; }
                set { _ClientRectsNoiseValue = value; NotifyPropertyChange(); }
            }



            bool _IsAudioNoise = true;
            public bool IsAudioNoise
            {
                get { return _IsAudioNoise; }
                set { _IsAudioNoise = value; NotifyPropertyChange(); }
            }
            double _AudioNoiseValue;
            public double AudioNoiseValue
            {
                get { return _AudioNoiseValue; }
                set { _AudioNoiseValue = value; NotifyPropertyChange(); }
            }





            bool _IsWebGlNoise = true;
            public bool IsWebGlNoise
            {
                get { return _IsWebGlNoise; }
                set { _IsWebGlNoise = value; NotifyPropertyChange(); }
            }
            double _WebGlNoiseValue;
            public double WebGlNoiseValue
            {
                get { return _WebGlNoiseValue; }
                set { _WebGlNoiseValue = value; NotifyPropertyChange(); }
            }



            bool _IsCanvasNoise = true;
            public bool IsCanvasNoise
            {
                get { return _IsCanvasNoise; }
                set { _IsCanvasNoise = value; NotifyPropertyChange(); }
            }
            double _CanvasNoiseValue;
            public double CanvasNoiseValue
            {
                get { return _CanvasNoiseValue; }
                set { _CanvasNoiseValue = value; NotifyPropertyChange(); }
            }






            bool _IsEnableWebrtc = false;
            public bool IsEnableWebrtc
            {
                get { return _IsEnableWebrtc; }
                set { _IsEnableWebrtc = value; NotifyPropertyChange(); }
            }


        }
    }
}
