﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MvcWeb.Db;
using MvcWeb.Helpers;
using MvcWeb.Models;
using MvcWeb.Services;
using MvcWeb.TheraLang.UnitOfWork;
using MvcWeb.Validators;
using Piranha;
using Piranha.AspNetCore.Identity.SQLServer;

namespace MvcWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The current configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options =>
                options.ResourcesPath = "Resources"
            );
            services.AddMvc()
                .AddPiranhaManagerOptions()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddFluentValidation(fv => { fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false; });

            #region Piranha setup

            services.AddPiranha();
            services.AddPiranhaApplication();
            services.AddPiranhaFileStorage();
            services.AddPiranhaImageSharp();
            services.AddPiranhaManager();
            services.AddPiranhaSummernote();
            //services.AddPiranhaTinyMCE();
            services.AddPiranhaApi();

            services.AddPiranhaEF(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddPiranhaIdentityWithSeed<IdentitySQLServerDb>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMemoryCache();
            services.AddPiranhaMemoryCache();

            #endregion

            #region register services via IServiceCollection

            services.AddDbContext<IttmmDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IUnitOfWork, UnitOfWork.UnitOfWork>(provider =>
                new UnitOfWork.UnitOfWork(provider.GetRequiredService<IttmmDbContext>()));

            services.AddTransient<IValidator<ProjectViewModel>, ProjectViewModelValidator>();
            services.AddTransient<IProjectService, ProjectService>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApi api, ILoggerFactory loggerFactory)
        {
            app.ConfigureExceptionHandler(loggerFactory, env.IsDevelopment());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            App.Init(api);

            // Configure cache level
            App.CacheLevel = Piranha.Cache.CacheLevel.Full;

            // Build content types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.BlogArchive))
                .AddType(typeof(Models.StandardPage))
                .AddType(typeof(Models.TeaserPage))
                .Build()
                .DeleteOrphans();
            var postTypeBuilder = new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(Models.BlogPost))
                .Build()
                .DeleteOrphans();
            var siteTypeBuilder = new Piranha.AttributeBuilder.SiteTypeBuilder(api)
                .AddType(typeof(Models.StandardSite))
                .Build()
                .DeleteOrphans();

            /**
             *
             * Test another culture in the UI
             *
            var cultureInfo = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
             */

            // Register middleware
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UsePiranha();
            app.UsePiranhaManager();
            app.UsePiranhaSummernote();
            //app.UsePiranhaTinyMCE();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });

            Seed.RunAsync(api).GetAwaiter().GetResult();
        }
    }
}
