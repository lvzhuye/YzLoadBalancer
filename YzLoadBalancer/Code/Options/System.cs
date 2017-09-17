using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YzLoadBalancer.Code.Classes;

namespace YzLoadBalancer.Code.Options
{
    public class System
    {
        public class Mapping {
            public string Host { get; set; }
            public string SettingsName { get; set; }
        }
        public class PluginInfo
        {
            public string Name { get; set; }
            public string Impl { get; set; }
        }

        public class BalancerSettings
        {
            private static BalancerSettings _current;
            public static BalancerSettings Current
            {
                //线程安全吗？
                get { return _current ?? new BalancerSettings(); }
            }

            public List<Mapping> Mappings { get; set; }

            public PluginInfo[] Plugins { get; set; }

            bool optimized = false;

            public void Optimize()
            {
                foreach(var item in Mappings)
                {
                    hostToSettingsMap[item.Host] = item.SettingsName;
                    hostToSettingsMap[item.SettingsName] = item.Host;
                }
                optimized = true;
            }


            Dictionary<string, string> hostToSettingsMap = new Dictionary<string, string>();
            Dictionary<string, string> settingsToHostMap = new Dictionary<string, string>();

            public Dictionary<string,FilterMiddleWare> MiddleWares { get; set; }

            public BalancerSettings()
            {
                Plugins = new PluginInfo[] { };
                MiddleWares = new Dictionary<string, FilterMiddleWare>();
            }

            public void Init()
            {
                //空的？？
                Optimize();
                foreach(var item in Plugins)
                {
                    var impl = (FilterMiddleWare)Activator.CreateInstance(Type.GetType(item.Impl));
                    MiddleWares.Add(item.Name, impl);
                }
            }

            //自动映射？？？
            internal static void Init(IOptions<BalancerSettings> init)
            {
                _current = init.Value;
                _current.Init();
            }

            public IConfigurationSection GetSettingsSection(string host)
            {
                string settingsName = GetSettingsName(host);
                return Startup.Configuration.GetSection(settingsName);
            }



            public string GetSettingsName(string host)
            {
                if (!optimized) Optimize();
                return hostToSettingsMap[host];
            }
        }
    }
}
