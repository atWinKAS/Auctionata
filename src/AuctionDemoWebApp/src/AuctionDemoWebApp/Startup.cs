using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuctionDemoWebApp.Models;
using AuctionDemoWebApp.Services;
using AuctionDemoWebApp.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Serialization;

namespace AuctionDemoWebApp
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IApplicationEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
//#if !DEBUG
//                config.Filters.Add(new RequireHttpsAttribute());
//#endif
            })
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new CamelCasePropertyNamesContractResolver();
                    });

            services.AddSignalR();

            services.AddIdentity<AuctionUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 6;
                config.Cookies.ApplicationCookie.LoginPath = "/auth/login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0); 
                    }
                };
            })
            .AddEntityFrameworkStores<AuctionContext>();

            services.AddLogging();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<AuctionContext>();

            services.TryAddScoped<CalculationService>();
            services.AddTransient<AuctionContextSeedData>(); // new instance each time
            services.AddScoped<IAuctionRepository, AuctionRepository>(); // in each request we will have single repository instance. 
 
//#if DEBUG
            services.AddScoped<IMailService, DebugMailService>(); // in request scope
//#else
            //// some real mailing feature here
//#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, AuctionContextSeedData seeder, ILoggerFactory loggerFactory)
        {
            //// change log level in production
            loggerFactory.AddDebug(LogLevel.Information);

            app.UseSignalR();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(options => options.EnableAll());
                app.UseRuntimeInfoPage(); // default path is /runtimeinfo
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            Mapper.Initialize(config =>
            {
                config.CreateMap<Item, ItemViewModel>().ReverseMap();
                config.CreateMap<Bid, BidViewModel>().ReverseMap();
            });

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    );
            });

            
            await seeder.EnsureSeedData();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
