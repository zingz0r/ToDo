using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using System;
using System.Reflection;
using ToDo.Application.Models;
using ToDo.Persistence.Modules;
using ToDo.Persistence.Profiles;
using ToDo.WebApi.Filters;
using ToDo.WebApi.Hub;

namespace ToDo.WebApi
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            LogContext.PushProperty("SourceContext", "Main");

            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(context => new AutofacServiceProviderFactory() as IServiceProviderFactory<ContainerBuilder>)
                    .ConfigureServices((context, services) =>
                    {
                        services.AddControllers().AddApplicationPart(Assembly.GetEntryAssembly())
                            .AddControllersAsServices()
                            .AddNewtonsoftJson();

                        services.AddCors();

                        services.AddSignalR(options => { options.EnableDetailedErrors = true; });

                        services.AddMvc(setup => { setup.Filters.Add(new ValidationFilter()); })
                            .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<ToDoModel>())
                            .AddNewtonsoftJson(settings =>
                                {
                                    settings.SerializerSettings.Converters.Add(
                                        new Newtonsoft.Json.Converters.StringEnumConverter());
                                });

                        services.AddSwaggerDocument(settings => { settings.Title = "ToDo Application"; });
                    })
                    .ConfigureContainer<ContainerBuilder>((context, builder) =>
                    {
                        builder.RegisterModule<PostgresModule>();

                        builder.Register(mContext => new MapperConfiguration(config =>
                        {
                            config.AddProfile<DatabaseProfile>();
                        }).CreateMapper()).As<IMapper>().SingleInstance();
                    })
                    .ConfigureWebHostDefaults(builder => builder.Configure((context, app) =>
                    {
                        if (context.HostingEnvironment.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        app.UseCors(cBuilder => cBuilder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<ToDoHub>("/todo");
                            endpoints.MapControllers();
                        });

                        app.UseOpenApi();
                        app.UseSwaggerUi3();
                    }))
                    .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration))
                    .Build();

                Log.Information("Running application");

                host.Run();
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application failed");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
