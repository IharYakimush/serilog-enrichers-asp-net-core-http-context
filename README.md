
# Serilog Enrichers AspNetCore HttpContext
AspNetCore middleware to add to Serilog LogContext properties from HttpContext

# Usage
Register this middleware as early as possible to use HttpContext properties in chained middlewares.

## Code Sample
```
        public void ConfigureServices(IServiceCollection services)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.Enrich.FromLogContext(); // mandatory
            loggerConfiguration.WriteTo.Console(
                outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {EventId} {Message:lj} {Properties}{NewLine}{Exception}{NewLine}");

            Log.Logger = loggerConfiguration.CreateLogger();

            services.AddLogging(builder => builder.ClearProviders().AddSerilog(dispose: true));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSerilogLogContext(options =>
            {
                options.EnrichersForContextFactory = context => new[]
                {
                    // TraceIdentifier property will be available in all chained middlewares. And yes - it is HttpContext specific
                    new PropertyEnricher("TraceIdentifier", context.TraceIdentifier) 
                };
            });

            app.Run(async (context) =>
            {
                // No need to add TraceIdentifier as a parameter to "LogInformation" method
                context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Demo").LogInformation("Hellow World Log. TraceId: {TraceIdentifier}");
                await context.Response.WriteAsync("Hello World!");
            });
        }
```
## Console Optput
`[17:26:46 INF]  Hellow World Log. TraceId: 0HLFDGEJTQ5RQ:00000001 {SourceContext="Demo", RequestId="0HLFDGEJTQ5RQ:00000001", RequestPath="/", CorrelationId=null, ConnectionId="0HLFDGEJTQ5RQ"}`

# Nuget
https://www.nuget.org/packages/Serilog.Enrichers.AspNetCore.HttpContext