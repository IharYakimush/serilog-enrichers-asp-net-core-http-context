

namespace Serilog.Enrichers.AspNetCore.HttpContext
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Http;
    using Serilog.Core;

    public class SerilogLogContextMiddlewareOptions : IOptions<SerilogLogContextMiddlewareOptions>
    {
        public SerilogLogContextMiddlewareOptions Value => this;

        public Func<HttpContext,IEnumerable<ILogEventEnricher>> EnrichersForContextFactory { get; set; }

        public bool ReThrowEnricherFactoryExceptions { get; set; } = true;
    }
}