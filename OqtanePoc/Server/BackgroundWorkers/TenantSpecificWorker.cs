using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using OqtanePoc.Modules.ProductCatalog.Repository;

namespace OqtanePoc.BackgroundWorkers
{
    public class TenantSpecificWorker : HostedServiceBase
    {
        // Target Primary Tenant Names
        private static readonly string[] TargetTenantNames = new[] { "Master", "Tenant1" };

        public TenantSpecificWorker(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            Name = "Tenant-Specific Inventory Audit Worker";
            Frequency = "d"; // Runs daily
            Interval = 1;
            IsEnabled = true;
            RetentionHistory = 30;
        }

        public override string ExecuteJob(IServiceProvider provider)
        {
            var tenantManager = provider.GetRequiredService<ITenantManager>();
            var logger = provider.GetRequiredService<ILogger<TenantSpecificWorker>>();
            Tenant currentTenant = tenantManager.GetTenant();

            if (currentTenant == null)
            {
                return "No active tenant found.";
            }

            // Verify if the current tenant is the intended tenant
            if (!TargetTenantNames.Contains(currentTenant.Name, StringComparer.OrdinalIgnoreCase))
            {
                // Silently skip processing for non-target tenants (e.g. Tenant2, Tenant3)
                return $"[Skipped] Tenant '{currentTenant.Name}' is not targeted by this job.\n";
            }

            try
            {
                logger.LogInformation("Starting Tenant-Specific Job execution for Tenant: {TenantName} (ID: {TenantId})", currentTenant.Name, currentTenant.TenantId);

                // Access the tenant-specific product repository (connected to this tenant's isolated database)
                var productRepository = provider.GetRequiredService<IProductRepository>();
                var products = productRepository.GetProducts(-1);

                int processedCount = products?.Count() ?? 0;

                string summary = $"[SUCCESS] Tenant '{currentTenant.Name}': Audited products in isolated DB at {DateTime.UtcNow:u}. Processed: {processedCount}.\n";
                logger.LogInformation("{Summary}", summary);
                return summary;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing background worker for Tenant '{TenantName}'", currentTenant.Name);
                return $"[ERROR] Tenant '{currentTenant.Name}': {ex.Message}\n";
            }
        }
    }
}
