using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using System;
using System.Reflection;
using ToDo.Persistence.Modules;
using ToDo.Persistence.Profiles;

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
                            .AddControllersAsServices();
                        services.AddCors();
                        services.AddMvc();
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
                            endpoints.MapControllers();
                        });
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
