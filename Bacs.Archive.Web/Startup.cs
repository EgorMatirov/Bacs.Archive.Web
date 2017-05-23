using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bacs.Archive.Web.Data;
using Bacs.Archive.Web.Models;
using Bacs.Archive.Web.Services.ArchiveClient;
using Bacs.Archive.Web.Services.TestsFetcher;
using Sakura.AspNetCore.Mvc;

namespace Bacs.Archive.Web
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            Console.WriteLine(Configuration.GetConnectionString("DefaultConnection"));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SecurityStampValidationInterval = TimeSpan.Zero)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IArchiveClientService, ArchiveClientService>(provider => new ArchiveClientService(
                Configuration["Archive.Host"],
                int.Parse(Configuration["Archive.Port"]),
                Configuration["Archive.ClientCertificate"],
                Configuration["Archive.ClientKey"],
                Configuration["Archive.CACertificate"]));
            services.AddTransient<ITestsFetcher, TestsFetcher>();
            services.AddBootstrapPagerGenerator(options =>
            {
                // Use default pager options.
                options.ConfigureDefault();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                // TODO: Add page for exceptions
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = Configuration["Google.ClientId"],
                ClientSecret = Configuration["Google.ClientSecret"]
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Problem}/{action=Index}/{id?}");
            });
        }
    }
}
