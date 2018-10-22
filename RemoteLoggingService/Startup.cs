using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RemoteLoggingService.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using RemoteLoggingService.BL.Interfaces;
using RemoteLoggingService.Services;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using React.AspNet;
using Microsoft.AspNetCore.Http;
using System;

namespace RemoteLoggingService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            
            // Injection of Notification sender
            services.AddTransient<INotificationSender, EmailNotificationSender>();            
            services.AddSingleton<IRepository, MainAppRepo>();
            
            // Set data context           
            //var connectionString = Configuration.GetConnectionString("DefaultConnection"); 
            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

            // Authorization settings
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)                
               .AddCookie(options => 
               {
                   options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
               });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();

            services.AddMvc();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();               
            }

            app.UseReact(config => { });
            app.UseDefaultFiles();
            app.UseStaticFiles();            
            app.UseAuthentication();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Monitoring}/{action=Index}/{id?}");
            });           
        }   
    }
}
