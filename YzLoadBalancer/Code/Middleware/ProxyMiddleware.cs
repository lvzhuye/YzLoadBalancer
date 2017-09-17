using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YzLoadBalancer.Code.Classes;

namespace YzLoadBalancer.Code.Middleware
{
    public class ProxyMiddleware:FilterMiddleWare
    {

        private readonly HttpClient _httpClient;
        private readonly InternalProxyOptions _defaultOptions;

        public override string Name => "Proxy";

        public ProxyMiddleware()
        {

        }
    }
}
