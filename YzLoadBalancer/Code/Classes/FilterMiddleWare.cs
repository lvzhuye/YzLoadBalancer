using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YzLoadBalancer.Code.Interfaces;
using YzLoadBalancer.Code.Options;
using static YzLoadBalancer.Code.Options.System;

namespace YzLoadBalancer.Code.Classes
{
    public abstract class FilterMiddleWare:IFilter
    {
        private ILogger<FilterMiddleWare> logger;

        public abstract string Name { get; }

        public FilterMiddleWare()
        {

        }

        public virtual bool IsActive(HttpContext context)
        {
            string host = context.Items["bal-host"] as string;
            if (string.IsNullOrEmpty(host)) throw new Exception("HOST is empty.Please check configuration.");
            VHostOptions VHost = context.Items["bal-vhost"] as VHostOptions;
            if (VHost == null) throw new Exception("VHOST is missing for " + host + ". Please check configuration.");
            //StringComparer.InvariantCultureIgnoreCase ???
            return VHost.Filters != null && VHost.Filters.Contains(this.Name, StringComparer.CurrentCulture);
        }

        public override async Task Invoke(HttpContext context)
        {
            var endRequest = false;
            if (this.IsActive(context))
            {
                //object urlToProxy = null;
                string host = context.Items["bal-host"] as string;
                VHostOptions vHost = context.Items["bal-host"] as VHostOptions;
                IConfigurationSection settings = BalancerSettings.Current.GetSettingsSection(host);
                await InvokeImpl(context, host, vHost, settings);
                endRequest = this.Terminate(context);
            }
            if (!endRequest && NextStep != null)
            {
                await NextStep(context);
            }

        }

        public virtual IApplicationBuilder Register(IApplicationBuilder app,IConfiguration con,IHostingEnvironment env,ILoggerFactory loggerFactory)
        {
            return app.Use(next =>
            {
                var instance = (IFilter)Activator.CreateInstance(this.GetType());
                return instance.Init(next).Invoke;
            });
        }

        public virtual bool Terminate(HttpContext httpContext)
        {
            return false;
        }


        public abstract Task InvokeImpl(HttpContext context, string host, VHostOptions vhost, IConfigurationSection settings);

    }
}
