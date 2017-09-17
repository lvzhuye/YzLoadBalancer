using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YzLoadBalancer.Code.Classes;
using YzLoadBalancer.Code.Options;
using NLog.Extensions.Logging;
using NLog.Web;

namespace YzLoadBalancer.Code.Middleware
{
    public class LogMiddleware:FilterMiddleWare
    {
        public override string Name => "Log";

        public override bool IsActive(HttpContext context)
        {
            return true;
        }
        public override Task InvokeImpl(HttpContext context, string host, VHostOptions vhost, IConfigurationSection settings)
        {
            return null;
        }

        public override IApplicationBuilder Register(IApplicationBuilder app, IConfiguration con, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog(".\\conf\\nlog.config");
            return app;
        }
    }
}
