using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql.Logging;
using StackExchange.Redis;
using Stephs_Shop.Filters;
using Stephs_Shop.Models.Entities;
using Stephs_Shop.Models.Options;
using Stephs_Shop.Repositories;
using Stephs_Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Stephs_Shop
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
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("CommerceDb"));
            });

            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<InfoBipOptions>(Configuration.GetSection("InfoBip"));
            services.Configure<ConnectionStringOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<MicroServiceOption>(Configuration.GetSection("MicroServiceOption"));
            services.Configure<FileOption>(Configuration.GetSection("FileOption"));

            services.AddScoped<IPgRepository, PgRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IFileService, FileService>();


            var connection = ConnectionMultiplexer.Connect("localhost");
            services.AddSingleton<IConnectionMultiplexer>(connection);
            services.AddSingleton<MyNgpsqlLoggingProvider>();
            services.AddSingleton<CustomNpgsqlLogger>();
            services.AddScoped<CustomerDataInjectior>();

           // services.AddHttpClient<HttpClient>();
			

            services.ConfigureApplicationCookie(option =>
            {
                option.Cookie.Name = "StephsShopCookie";
                option.LoginPath = "/Home/Login";
                option.ExpireTimeSpan = TimeSpan.FromHours(1);

            });
            

			//  services.AddSession();


			services.AddControllersWithViews();
            /**
            services.AddMvc(opt =>
            {
                opt.EnableEndpointRouting = false;
            });
            */
            //services.AddMediatR();
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
            // NpgsqlLogManager.Provider = new ConsoleLoggingProvider(printLevel: true, printConnectorId : true);
            NpgsqlLogManager.Provider = app.ApplicationServices.GetRequiredService<MyNgpsqlLoggingProvider>();
			NpgsqlLogManager.IsParameterLoggingEnabled = true;


			app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseSession();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

			app.UseEndpoints(routes =>
			{
				routes.MapControllerRoute(
				   name: "areas",
				   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				 );


				routes.MapControllerRoute(
					name: "default",
					pattern : "{controller=Home}/{action=Index}/{id?}");

			});

			/**
            app.UseMvc(routes =>
            {
				routes.MapRoute(
		           name: "areas",
		           template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
		         );


				routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
               
            });
            */
		}
    }
}
