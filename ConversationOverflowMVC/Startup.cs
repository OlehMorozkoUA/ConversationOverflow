using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConversationOverflowMVC.Helper;
using Microsoft.AspNetCore.Http;
using ConnectToDB;
using Microsoft.EntityFrameworkCore;
using ConversationOverflowMVC.Hubs;

namespace ConversationOverflowMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("ConversationOverflowConnection");

            services.AddDbContext<ConversationOverflowDbContext>(options => options.UseSqlServer(connectionString));

            services.AddControllersWithViews();
            //services.AddSingleton<IConversationOverflowAPI, ConversationOverflowAPI>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddHttpClient<IConversationOverflowAPI, ConversationOverflowAPI>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => //CookieAuthenticationOptions
                    {
                        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/User/LogIn");
                    });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=LogIn}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
