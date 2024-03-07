using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Kidstarter.Api.Configuration;
using Kidstarter.Infrastructure.EF;

using Lamar;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Polly;
using Polly.Registry;

using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using Prometheus;

namespace Kidstarter.Api.Extensions
{
    public static class ServiceRegistryExtensions
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(x => { x.AddConsole(); });

        public static void AddPolly(this ServiceRegistry serviceCollection, int retriesCount)
        {
            var registry = new PolicyRegistry();

            serviceCollection.AddSingleton<IReadOnlyPolicyRegistry<string>>(
                x =>
                    {
                        var logger = x.GetRequiredService<ILogger<Startup>>();

                        var retryPolicy = Policy.Handle<Exception>()
                            .WaitAndRetryAsync(
                                retriesCount,
                                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                (exception, _, attempt) =>
                                    {
                                        logger.LogError(
                                            "Операция завершилась неудачно. Попытка {attempt}. exception={exception}",
                                            attempt,
                                            exception);
                                    });

                        registry.Add("defaultRetryPolicy", retryPolicy);

                        return registry;
                    });
        }

        public static void AddSwagger(this ServiceRegistry services)
        {
            services.AddSwaggerGen(
                c =>
                {
                    c.CustomSchemaIds(x => x.FullName);
                    c.SwaggerDoc("api", new OpenApiInfo { Title = "Kidstarter API", Version = "v1" });
                    c.AddSecurityDefinition(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Description = "Please insert JWT with Bearer into field",
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey
                        });

                    c.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                            {
                                    {
                                        new OpenApiSecurityScheme
                                            {
                                                Description =
                                                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                                                Name = "Authorization",
                                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                                                Scheme = "oauth2",
                                                In = ParameterLocation.Header,
                                            },
                                        new List<string>()
                                    }
                            });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
        }

        public static void AddHttpClient(this ServiceRegistry services, IConfiguration configuration)
        {
            services.AddHttpClient("default")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseDefaultCredentials = true });

            services.AddHttpClient(
                "Payments",
                client =>
                    {
                        client.BaseAddress = new Uri(configuration["PaymentSettings:Server"]);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");
                    });
        }

        public static void AddEntityFramework(this ServiceRegistry services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(
                        connectionString,
                        ServerVersion.FromString("8.0.17-mysql"),
                        x =>
                            {
                                x.UseNetTopologySuite();
                                x.CharSetBehavior(CharSetBehavior.NeverAppend);
                                x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                            })
                    .UseLoggerFactory(MyLoggerFactory));
        }

        public static void AddJwt(this ServiceRegistry services, AuthSettings settings)
        {
            services.AddAuthentication(
                    options =>
                        {
                            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                .AddJwtBearer(
                    options =>
                        {
                            options.RequireHttpsMetadata = false;
                            options.SaveToken = true;
                            options.TokenValidationParameters =
                                new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = settings.Issuer,
                                    ValidAudience = settings.Audience,
                                    IssuerSigningKey = settings.SecurityKey,
                                    ClockSkew = TimeSpan.Zero
                                };

                            options.Events = new JwtBearerEvents
                            {
                                OnAuthenticationFailed = context =>
                                    {
                                        if (context.Exception is SecurityTokenExpiredException)
                                        {
                                            context.Response.Headers.Add("Token-Expired", "true");
                                        }

                                        return Task.CompletedTask;
                                    }
                            };
                        });
        }

        public static void AddHealthChecks(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddMySql(configuration.GetConnectionString("KidstarterDb"), "KidstarterDb")
                .ForwardToPrometheus();
        }
    }
}