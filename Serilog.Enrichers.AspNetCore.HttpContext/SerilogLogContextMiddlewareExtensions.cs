using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Serilog.Enrichers.AspNetCore.HttpContext
{
    public static class SerilogLogContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseSerilogLogContext(this IApplicationBuilder builder, Action<SerilogLogContextMiddlewareOptions> settings)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            SerilogLogContextMiddlewareOptions options = new SerilogLogContextMiddlewareOptions();
            settings(options);
            return builder.UseMiddleware<SerilogLogContextMiddleware>(Options.Create(options));
        }
    }
}