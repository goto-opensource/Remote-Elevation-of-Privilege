using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rEoP.Server.Hubs;
using StackExchange.Redis;

namespace rEoP.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddSingleton<ISessionStore, SessionStore>();
            services.AddSignalR().AddHubOptions<ControlChannel>(o => o.ClientTimeoutInterval = TimeSpan.FromSeconds(10));/*.AddStackExchangeRedis(o =>
            {
                o.ConnectionFactory = async writer =>
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false
                    };
                    config.EndPoints.Add("redis-image", 6379);
                    config.SetDefaultPorts();
                    config.ConnectRetry = 10;
                    var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                    connection.ConnectionFailed += (_, e) =>
                    {
                        Console.WriteLine($"Connection to Redis failed. {e.Exception.Message} -- {e.Exception.StackTrace}");
                    };
                    if (!connection.IsConnected)
                    {
                        Console.WriteLine("Did not connect to Redis.");
                    }
                    return connection;
                };
            });*/
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            /*services.AddLettuceEncrypt(options =>
            {
                options.DomainNames = new string[] { Environment.GetEnvironmentVariable("LETSENCRYPT_DOMAINNAME") };
                options.EmailAddress = Environment.GetEnvironmentVariable("LETSENCRYPT_EMAILADDRESS");
            });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<ControlChannel>("/control");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
