using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Common.Utilities;
using ShyrochenkoPatterns.DAL;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.DAL.Repository;
using ShyrochenkoPatterns.DAL.UnitOfWork;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Helpers.SwaggerFilters;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Abstraction;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using ShyrochenkoPatterns.Services.Interfaces.Exporting;
using ShyrochenkoPatterns.Services.Interfaces.External;
using ShyrochenkoPatterns.Services.Interfaces.Facade;
using ShyrochenkoPatterns.Services.Interfaces.Prototype;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;
using ShyrochenkoPatterns.Services.Services;
using ShyrochenkoPatterns.Services.Services.Abstraction.Bridge;
using ShyrochenkoPatterns.Services.Services.Bridge.Implementation;
using ShyrochenkoPatterns.Services.Services.Exporting;
using ShyrochenkoPatterns.Services.Services.External;
using ShyrochenkoPatterns.Services.Services.Facade;
using ShyrochenkoPatterns.Services.Services.Prototype;
using ShyrochenkoPatterns.Services.Services.Proxy;
using ShyrochenkoPatterns.Services.StartApp;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ShyrochenkoPatterns
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
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Connection"));
                //options.UseLazyLoadingProxies(); Lazy loading
                options.EnableSensitiveDataLogging(false);
            });

            services.AddCors();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#=";
            }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.Name = "Default";
                o.TokenLifespan = TimeSpan.FromHours(12);
            });

            #region Register services

            #region Basis services

            services.AddScoped<IDataContext>(provider => provider.GetService<DataContext>());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<HashUtility>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJWTService, JWTService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IS3Service, S3Service>();
            services.AddTransient<ICallService, CallService>();
            
            services.AddTransient<IPostPrototype, PostPrototype>();

            //services.AddScoped<IBridgeImplementation, BridgeAdminImplementation>();
            //services.AddScoped<IBridgeImplementation, BridgeUserEmailImplementation>();
            //services.AddScoped<IBridgeImplementation, BridgeUserPhoneImplementation>();
            //services.AddScoped<IBridgeImplementation, BridgeUserFacebookPhoneImplementation>();
            //services.AddScoped<IBridgeImplementation, BridgeUserFacebookEmailImplementation>();

            services.AddSingleton<BridgeAdminImplementation>();
            services.AddSingleton<BridgeUserEmailImplementation>();
            services.AddSingleton<BridgeUserPhoneImplementation>();
            services.AddSingleton<BridgeUserFacebookPhoneImplementation>();
            services.AddSingleton<BridgeUserFacebookEmailImplementation>();

            services.AddTransient<Func<string, IBridgeImplementation>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Admin":
                        return serviceProvider.GetService<BridgeAdminImplementation>();
                    case "UserEmail":
                        return serviceProvider.GetService<BridgeUserEmailImplementation>();
                    case "UserPhone":
                        return serviceProvider.GetService<BridgeUserPhoneImplementation>();
                    case "FacebookPhone":
                        return serviceProvider.GetService<BridgeUserFacebookPhoneImplementation>();
                    case "FacebookEmail":
                        return serviceProvider.GetService<BridgeUserFacebookEmailImplementation>();
                    default:
                        return serviceProvider.GetService<BridgeUserEmailImplementation>(); // вот тут может быть опасно
                }
            });

            services.AddSingleton<BridgeAdmin>();
            services.AddSingleton<BridgeUserEmail>();
            services.AddSingleton<BridgeUserPhone>();
            services.AddSingleton<BridgeUserFacebookPhone>();
            services.AddSingleton<BridgeUserFacebookEmail>();

            services.AddTransient<Func<string, IBridgeAbstraction>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "Admin":
                        return serviceProvider.GetService<BridgeAdmin>();
                    case "UserEmail":
                        return serviceProvider.GetService<BridgeUserEmail>();
                    case "UserPhone":
                        return serviceProvider.GetService<BridgeUserPhone>();
                    case "FacebookPhone":
                        return serviceProvider.GetService<BridgeUserFacebookPhone>();
                    case "FacebookEmail":
                        return serviceProvider.GetService<BridgeUserFacebookEmail>();
                    default:
                        return serviceProvider.GetService<BridgeUserEmail>(); // вот тут может быть опасно
                }
            });

            //services.AddScoped<IBridgeAbstraction, BridgeAdmin>();
            //services.AddScoped<IBridgeAbstraction, BridgeUserEmail>();
            //services.AddScoped<IBridgeAbstraction, BridgeUserPhone>();
            //services.AddScoped<IBridgeAbstraction, BridgeUserFacebookPhone>();
            //services.AddScoped<IBridgeAbstraction, BridgeUserFacebookEmail>();

            services.AddTransient<IScaner, Scaner>();
            services.AddTransient<IPrinter, Printer>();
            services.AddTransient<ICopier, Copier>();
            services.AddTransient<IMFU, MFU>();

            services.AddTransient<IReadStoryService, ReadStoryService>();
            services.AddTransient<IProxy, Proxy>();

            #endregion

            #region External Authentication services

            services.AddScoped<IFacebookService, FacebookService>();
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddScoped<ILinkedInService, LinkedInService>();
            services.AddHttpClient<IFacebookService, FacebookService>();
            services.AddHttpClient<IGoogleService, GoogleService>();
            services.AddHttpClient<ILinkedInService, LinkedInService>();

            #endregion

            #region Exporting services

            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IHtmlTableConverter, HtmlTableConverter>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IXlsService, XlsService>();

            #endregion

            #region Notification services

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISMSService, SMSService>();

            services.AddScoped<ITwillioService, TwillioService>();
            services.AddScoped<IFCMService, FCMService>();
            services.AddScoped<ISESService, SESService>();

            #endregion

            #region Payment services

            services.AddScoped<IBraintreeService, BraintreeService>();
            services.AddScoped<IStripeService, StripeService>();

            #endregion

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfileConfiguration());
            });

            services.AddSingleton(config.CreateMapper());

            #endregion

            services
                .AddDetectionCore()
                .AddDevice();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddVersionedApiExplorer(
                 options =>
                 {
                     options.GroupNameFormat = "'v'VVV";

                     // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                     // can also be used to control the format of the API version in route templates
                     options.SubstituteApiVersionInUrl = true;
                 });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddMvc(options =>
            {
                // Allow use optional parameters in actions
                options.AllowEmptyInputInBodyModelBinding = true;
                options.EnableEndpointRouting = false;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true)
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Access token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                // add a custom operation filter which sets default values

                // integrate xml comments
                options.IncludeXmlComments(XmlCommentsFilePath);
                options.IgnoreObsoleteActions();

                options.OperationFilter<DefaultValues>();
                options.OperationFilter<SecurityRequirementsOperationFilter>("Bearer");
            });

            // instead of options.DescribeAllEnumsAsStrings()
            services.AddSwaggerGenNewtonsoftSupport();

            var sp = services.BuildServiceProvider();
            var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                  {
                      options.RequireHttpsMetadata = false;
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidIssuer = AuthOptions.ISSUER,
                          ValidateAudience = true,
                          ValidateActor = false,
                          ValidAudience = AuthOptions.AUDIENCE,
                          ValidateLifetime = true,
                          //SignatureValidator = (string token, TokenValidationParameters validationParameters) => {

                          //    var jwt = new JwtSecurityToken(token);

                          //    var signKey = AuthOptions.GetSigningCredentials().Key as SymmetricSecurityKey;

                          //    var encodedData = jwt.EncodedHeader + "." + jwt.EncodedPayload;
                          //    var compiledSignature = Encode(encodedData, signKey.Key);

                          //    //Validate the incoming jwt signature against the header and payload of the token
                          //    if (compiledSignature != jwt.RawSignature)
                          //    {
                          //        throw new Exception("Token signature validation failed.");
                          //    }

                          //    /// TO DO: initialize user claims

                          //    return jwt;
                          //},
                          LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                          {
                              var jwt = securityToken as JwtSecurityToken;

                              if (!notBefore.HasValue || !expires.HasValue || DateTime.Compare(expires.Value, DateTime.UtcNow) <= 0)
                              {
                                  return false;
                              }

                              if (jwt == null)
                                  return false;

                              var isRefresStr = jwt.Claims.FirstOrDefault(t => t.Type == "isRefresh")?.Value;

                              if (isRefresStr == null)
                                  return false;

                              var isRefresh = Convert.ToBoolean(isRefresStr);

                              if (!isRefresh)
                              {
                                  try
                                  {
                                      using (var scope = serviceScopeFactory.CreateScope())
                                      {
                                          var hash = scope.ServiceProvider.GetService<HashUtility>().GetHash(jwt.RawData);
                                          return scope.ServiceProvider.GetService<IRepository<UserToken>>().Find(t => t.AccessTokenHash == hash && t.IsActive) != null;
                                      }
                                  }
                                  catch (Exception ex)
                                  {
                                      var logger = sp.GetService<ILogger<Startup>>();
                                      logger.LogError(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + ": Exception occured in token validator. Exception message: " + ex.Message + ". InnerException: " + ex.InnerException?.Message);
                                      return false;
                                  }
                              }

                              return false;
                          },
                          IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                          ValidateIssuerSigningKey = true
                      };
                  });

            services.AddRouting();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            app.UseDefaultFiles();

            var cultures = Configuration.GetSection("SupportedCultures").Get<string[]>();

            var supportedCultures = new List<CultureInfo>();

            foreach (var culture in cultures)
            {
                supportedCultures.Add(new CultureInfo(culture));
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            env.EnvironmentName = Environments.Development;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor,

                    // IIS is also tagging a X-Forwarded-For header on, so we need to increase this limit, 
                    // otherwise the X-Forwarded-For we are passing along from the browser will be ignored
                    ForwardLimit = 2
                });
            }

            if (!Directory.Exists("Logs"))
            {
                Directory.CreateDirectory("Logs");
            }

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(5),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    //swagger.Host = httpReq.Host.Value;

                    var ampersand = "&amp;";

                    foreach (var path in swagger.Paths)
                    {
                        if (path.Value.Operations.Any(x => x.Key == OperationType.Get && x.Value.Deprecated))
                            path.Value.Operations.First(x => x.Key == OperationType.Get).Value.Description = path.Value.Operations.First(x => x.Key == OperationType.Get).Value.Description.Replace(ampersand, "&");

                        if (path.Value.Operations.Any(x => x.Key == OperationType.Delete && x.Value?.Description != null))
                            path.Value.Operations.First(x => x.Key == OperationType.Delete).Value.Description = path.Value.Operations.First(x => x.Key == OperationType.Delete).Value.Description.Replace(ampersand, "&");
                    }

                    var paths = swagger.Paths.ToDictionary(p => p.Key, p => p.Value);
                    foreach (KeyValuePair<string, OpenApiPathItem> path in paths)
                    {
                        swagger.Paths.Remove(path.Key);
                        swagger.Paths.Add(path.Key.ToLowerInvariant(), path.Value);
                    }
                });
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                options.IndexStream = () => File.OpenRead("Views/Swagger/swagger-ui.html");
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                options.EnableFilter();
            });

            app.UseCors(builder => builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
            app.UseStaticFiles();
            app.UseRouting();

            #region Error handler

            // Different middleware for api and ui requests  
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            {
                var localizer = serviceProvider.GetService<IStringLocalizer<ErrorsResource>>();
                var logger = loggerFactory.CreateLogger("GlobalErrorHandling");

                // Exception handler - show exception data in api response
                appBuilder.UseExceptionHandler(new ExceptionHandlerOptions
                {
                    ExceptionHandler = async context =>
                    {
                        var errorModel = new ErrorResponseModel(localizer);
                        var result = new ContentResult();

                        var exception = context.Features.Get<IExceptionHandlerPathFeature>();

                        if (exception.Error is CustomException)
                        {
                            var ex = (CustomException)exception.Error;

                            result = errorModel.Error(ex);
                        }
                        else
                        {
                            var message = exception.Error.InnerException?.Message ?? exception.Error.Message;
                            logger.LogError($"{exception.Path} - {message}");

                            errorModel.AddError("general", message);
                            result = errorModel.InternalServerError(env.IsDevelopment() ? exception.Error.StackTrace : null);
                        }

                        context.Response.StatusCode = result.StatusCode.Value;
                        context.Response.ContentType = result.ContentType;

                        await context.Response.WriteAsync(result.Content);
                    }
                });

                // Handles responses with status codes (correctly executed requests, without any exceptions)
                appBuilder.UseStatusCodePages(async context =>
                    {
                        string message = "";

                        List<ErrorKeyValue> errors = new List<ErrorKeyValue>();

                        switch (context.HttpContext.Response.StatusCode)
                        {
                            case 400:
                                message = "Bad Request";
                                break;
                            case 401:
                                message = "Unauthorized";
                                errors.Add(new ErrorKeyValue("token", "Token invalid"));
                                break;
                            case 403:
                                message = "Forbidden";
                                break;
                            case 404:
                                message = "Not found";
                                break;
                            case 500:
                                message = "Internal Server Error";
                                break;
                        }

                        context.HttpContext.Response.ContentType = "application/json";
                        await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponseModel(localizer)
                        {
                            Code = message,
                            StackTrace = "",
                            Errors = errors
                        }, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                    });
            });

            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
            {
                appBuilder.UseExceptionHandler("/Error");
                appBuilder.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
            });

            #endregion

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id}");
            });
        }

        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"ShyrochenkoPatterns API {description.ApiVersion}",
                Version = description.ApiVersion.ToString(),
                Description = "The ShyrochenkoPatterns application with Swagger and API versioning."
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }

        private string Encode(string input, byte[] key)
        {
            HMACSHA256 myhmacsha = new HMACSHA256(key);
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            byte[] hashValue = myhmacsha.ComputeHash(stream);
            return Base64UrlEncoder.Encode(hashValue);
        }
    }
}
