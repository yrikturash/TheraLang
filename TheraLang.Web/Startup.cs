using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TheraLang.BLL.Infrastructure;
using TheraLang.BLL.Interfaces;
using TheraLang.BLL.LiqPay;
using TheraLang.BLL.Services;
using TheraLang.BLL.Services.FileServices;
using TheraLang.Web.ActionFilters;
using TheraLang.Web.Extensions;

namespace TheraLang.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddMvc(options =>
                {
                    options.Filters.Add(new ModelValidationFilter());
                    options.Filters.Add(typeof(ExceptionFilter));
                })
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblyContaining<Startup>();
                    options.ImplicitlyValidateChildProperties = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddExceptionHandler();

            services.AddScoped<IAuthenticateService, AuthenticationService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IUserService, UserService>();

            services.AddMainContext(Configuration.GetConnectionString("DefaultConnection"));
            services.AddUnitOfWork();
            services.AddFileStorage(Configuration.GetConnectionString("AzureConnection"));
            
            services.AddAuthentication(Configuration);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<IProjectTypeService, ProjectTypeService>();
            services.AddCors(options =>
                options.AddPolicy("development mode",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader()
                        .AllowAnyMethod())); //TODO: remove after app integrated
            services.AddTransient<IResourceService, ResourceService>();
            services.AddTransient<IResourceCategoryService, ResourceCategoryService>();
            services.AddTransient<IProjectParticipationService, ProjectParticipationService>();
            services.AddTransient<ILiqPayService, LiqPayService>();
            services.AddTransient<ILiqPayInfo, LiqPayInfo>();
            services.AddTransient<IDonationService, DonationService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IHtmlContentService, HtmlContentService>();
            services.AddTransient<ISiteMapService, SiteMapService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<IMemberFeeService, MemberFeeService>();
            services.AddTransient<IPaymentHistoryService, PaymentHistoryService>();
            services.AddTransient<PaymentService>();
            services.AddTransient<INewsCommentService, NewsCommentService>();

            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("development mode");
                app.UseOpenApi();
                app.UseSwaggerUi3();
                //DbInitializer.Seed(app);
            }

            // Register middleware
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=home}/{action=index}/{id?}");

                routes.MapRoute(
                    "angular",
                    "{*template}",
                    new { controller = "Home", action = "Index" });
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";
                spa.Options.StartupTimeout = new TimeSpan(0, 1, 30);

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer("start");
                }
            });
        }
    }
}