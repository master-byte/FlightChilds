using System.Linq;

using CorrelationId;

using Kidstarter.Api.Auth;
using Kidstarter.Api.Configuration;
using Kidstarter.Api.Extensions;
using Kidstarter.Api.Mappings;
using Kidstarter.Api.Middlewares;
using Kidstarter.Api.Tools;
using Kidstarter.BusinessLogic;
using Kidstarter.CloudStorage;
using Kidstarter.Core;
using Kidstarter.Core.Models.EF;
using Kidstarter.Core.Workflow;
using Kidstarter.Infrastructure;
using Kidstarter.Infrastructure.EF;
using Kidstarter.Media;

using Lamar;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using Prometheus;

using Serilog;

namespace Kidstarter.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ServiceRegistry services)
        {
            TinyMapperConfig.Register();
            services.AddLogging();
            services.AddAutoMapper(typeof(CommonMappings));
            services.AddDistributedMemoryCache();
            services.AddPolly(this.Configuration.GetValue<int>("RetriesCount"));
            services.AddHealthChecks(this.Configuration);

            services
                .AddControllers(
                    options =>
                        {
                            options.UseGlobalRoutePrefix("api/v{version:apiVersion}");
                            options.Filters.Add(new HttpResponseExceptionFilter());
                        })
                .ConfigureApiBehaviorOptions(
                    options =>
                        {
                            options.InvalidModelStateResponseFactory = context
                                => new ObjectResult(
                                       new ApiResponse<string>(
                                           context.ModelState.Values
                                               .SelectMany(x => x.Errors)
                                               .Select(x => x.ErrorMessage)
                                               .ToList()))
                                { StatusCode = StatusCodes.Status400BadRequest };
                        })
                .AddControllersAsServices();

            services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });

            services.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = false;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });

            services.AddCors(
                options =>
                    {
                        options.AddPolicy(
                            "AllowAll",
                            builder =>
                                {
                                    builder.AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .SetIsOriginAllowed(_ => true)
                                        .AllowCredentials();
                                });
                    });

            services
                .Configure<ApplicationSettings>(this.Configuration.GetSection(nameof(ApplicationSettings)))
                .Configure<AuthSettings>(this.Configuration.GetSection(nameof(AuthSettings)));

            services.AddSingleton(_ => new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
            services
                .AddScoped<SplitUserMapperService>()
                .AddScoped<IUserProvider, HttpContextUserProvider>();

            services
                .RegisterCoreModuleDependencies()
                .RegisterCloudStorageModuleDependencies(this.Configuration)
                .RegisterEventsModuleDependencies(this.Configuration)
                .RegisterInfrastructuresModuleDependencies(this.Configuration)
                .RegisterMediaModuleDependencies();

            services.AddEntityFramework(this.Configuration.GetConnectionString("KidstarterDb"));

            services.AddIdentity<User, Role>()
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddHttpClient(this.Configuration);
            services.AddJwt(this.Configuration.GetSection("AuthSettings").Get<AuthSettings>());

            services.AddAuthorization(
                config =>
                    {
                        config.AddPolicy(Policies.Admin, Policies.AdminPolicy());
                        config.AddPolicy(Policies.Parent, Policies.ParentPolicy());
                        config.AddPolicy(Policies.Producer, Policies.ProducerPolicy());
                        config.AddPolicy(Policies.Organizer, Policies.OrganizerPolicy());
                        config.AddPolicy(Policies.Partner, Policies.PartnerPolicy());
                    });

            services.AddMvc(o => { o.Filters.Add(typeof(ApiValidateModelFilter)); })
                .AddNewtonsoftJson(
                    o =>
                        {
                            o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            o.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                            o.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
                        })
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddSwagger();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelationId();
            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);
            //// app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseForwardedHeaders();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.Map("/metrics", metricsApp =>
                {
                    metricsApp.UseMiddleware<BasicAuthMiddleware>();
                    metricsApp.UseMetricServer(string.Empty);
                });

            app.UseHsts();

            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    c =>
                        {
                            c.SwaggerEndpoint("/swagger/api/swagger.json", "Kidstarter API");
                        });
            }

            app.UseHttpMetrics();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllers();
                        endpoints.MapMetrics();
                    });
        }
    }
}
