using MBM.Common.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace MBM.Common.Middleware
{
    public class DomainRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedDomains;

        public DomainRestrictionMiddleware(RequestDelegate next, DomainRestrictionOptions options)
        {
            _next = next;
            _allowedDomains = options.AllowedDomains;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var host = context.Request.Host.Host;

            // Extract domain name from the host
            var domain = host.Split('.').LastOrDefault();

            // Check if the domain is in the allowed list
            if (!_allowedDomains.Contains(domain))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Domain not authorized.");
                return;
            }

            await _next(context);
        }
    }
}
