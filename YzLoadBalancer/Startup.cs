using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using static YzLoadBalancer.Code.Options.System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Options;

namespace YzLoadBalancer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //干嘛的???
            services.AddOptions();
            services.Configure<BalancerSettings>(Configuration.GetSection("Balancersettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            //reloadOnChange???
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("conf/appsettings.json", optional: true, reloadOnChange: true);

            string[] files = Directory.GetFiles(Path.Combine(env.ContentRootPath, "conf", "vhosts"));

            foreach (var s in files)
            {
                builder = builder.AddJsonFile(s);
            }

            builder = builder.AddEnvironmentVariables();

            Configuration = builder.Build();

            //loggerFactory.AddConsole();

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }


        public static IConfiguration Configuration { get; private set; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<BalancerSettings> init)
        {
            BalancerSettings.Init(init);

            foreach (var item in BalancerSettings.Current.MiddleWares)
            {
                item.Value.Register(app, Configuration, env, loggerFactory);
            }
        }
    }
}
