using Newtonsoft.Json;
using System.Collections.Generic;

namespace $safeprojectname$.DataClass
{
    internal class DictConfigureData : Dictionary<string, string>
    {
        #region Proxy List
        public List<string>? Proxies
        {
            get
            {
                if (TryGetValue(nameof(Proxies), out string? val))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<List<string>>(val);
                    }
                    catch { }
                }
                return null;
            }
            set
            {
                this[nameof(Proxies)] = JsonConvert.SerializeObject(value);
            }
        }
        public bool IsDeleteAfterUse
        {
            get
            {
                if (TryGetValue(nameof(IsDeleteAfterUse), out string? val) && bool.TryParse(val, out bool r))
                {
                    return r;
                }
                return false;
            }
            set
            {
                this[nameof(IsDeleteAfterUse)] = value.ToString();
            }
        }
        public bool IsSelectRandom
        {
            get
            {
                if (TryGetValue(nameof(IsSelectRandom), out string? val) && bool.TryParse(val, out bool r))
                {
                    return r;
                }
                return false;
            }
            set
            {
                this[nameof(IsSelectRandom)] = value.ToString();
            }
        }
        #endregion



        public List<string>? ApiKeys
        {
            get
            {
                if (TryGetValue(nameof(ApiKeys), out string? val))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<List<string>>(val);
                    }
                    catch { }
                }
                return null;
            }
            set
            {
                this[nameof(ApiKeys)] = JsonConvert.SerializeObject(value);
            }
        }
    }
}
