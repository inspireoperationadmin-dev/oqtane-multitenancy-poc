using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;

namespace OqtanePoc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TenantAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedTenantNames;

        public TenantAuthorizeAttribute(params string[] allowedTenantNames)
        {
            _allowedTenantNames = allowedTenantNames ?? Array.Empty<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tenantManager = context.HttpContext.RequestServices.GetService<ITenantManager>();
            Tenant currentTenant = tenantManager?.GetTenant();

            bool isAllowed = false;
            if (currentTenant != null)
            {
                isAllowed = _allowedTenantNames.Any(name => string.Equals(currentTenant.Name, name, StringComparison.OrdinalIgnoreCase));
            }

            if (!isAllowed)
            {
                string target = string.Join(" or ", _allowedTenantNames);
                context.Result = new ObjectResult(new
                {
                    error = "Forbidden",
                    message = $"This endpoint is exclusively restricted to tenant '{target}' and cannot be accessed via tenant '{currentTenant?.Name ?? "Unknown"}'."
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
