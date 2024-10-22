using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SanThuongMaiG15.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace SanThuongMaiG15
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
            
            var stringConnectdb = Configuration.GetConnectionString("dbEce");
            services.AddDbContext<EcC2CContext>(options => options.UseSqlServer(stringConnectdb));
            //services.AddMvc();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(p =>
                        {
                            p.LoginPath = "/dang-nhap.html";
                            p.LogoutPath = "/dang-xuat.html";
                            p.AccessDeniedPath = "/";
                            p.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Hết hạn sau 30 phút
                            p.SlidingExpiration = true;
                        });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("3"));
                options.AddPolicy("SellerPolicy", policy => policy.RequireRole("2"));
            });
            services.AddDistributedMemoryCache();

            services.AddSession();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

            services.AddMemoryCache();

            services.AddResponseCompression();

            services.AddResponseCaching();

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
                                        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc();
            services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
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
            app.UseSession(); 
            app.UseAuthentication(); 
            app.UseAuthorization();
       
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
