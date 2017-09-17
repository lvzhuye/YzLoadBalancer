using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YzLoadBalancer.Code.Options
{
    public class VHostOptions
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Schme { get; set; }

        public List<string> Filters { get; set; }
    }
}
