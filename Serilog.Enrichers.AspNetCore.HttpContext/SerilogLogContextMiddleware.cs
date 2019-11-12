using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Serilog.Core;

namespace Serilog.Enrichers.AspNetCore.HttpContext
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class SerilogLogContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SerilogLogContextMiddlewareOptions _options;

        public SerilogLogContextMiddleware(RequestDelegate next, IOptions<SerilogLogContextMiddlewareOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ILogEventEnricher[] enrichers = null;
            if (this._options.EnrichersForContextFactory != null)
            {
                try
                {
                    enrichers = this._options.EnrichersForContextFactory(context)?.ToArray();
                }
                catch
                {
                    if (this._options.ReThrowEnricherFactoryExceptions)
                    {
                        throw;
                    }
                }
            }

            bool nextExecuted = false;
            if (enrichers != null)
            {
                if (enrichers.Any())
                {
                    using (LogContext.Push(enrichers))
                    {
                        await _next(context);
                        nextExecuted = true;
                    }
                }                
            }

            if (!nextExecuted)
            {
                await _next(context);
            }
        }
    }
}
