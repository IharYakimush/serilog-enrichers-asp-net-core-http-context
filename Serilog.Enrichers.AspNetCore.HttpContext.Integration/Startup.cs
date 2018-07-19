using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core.Enrichers;
using Serilog.Filters;

namespace Serilog.Enrichers.AspNetCore.HttpContext.Integration
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.Filter.ByIncludingOnly(Matching.FromSource("Demo"));
            loggerConfiguration.Enrich.FromLogContext(); // mandatory
            loggerConfiguration.WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {EventId} {Message:lj} {Properties}{NewLine}{Exception}{NewLine}");

            Log.Logger = loggerConfiguration.CreateLogger();

            services.AddLogging(builder => builder.ClearProviders().AddSerilog(dispose: true));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSerilogLogContext(options =>
            {
                options.EnrichersForContextFactory = context => new[]
                {
                    new PropertyEnricher("TraceIdentifier", context.TraceIdentifier)
                };
            });

            app.Run(async (context) =>
            {
                context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Demo").LogInformation("Hellow World Log. TraceId: {TraceIdentifier}");
                await Task.Delay(10000);
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
