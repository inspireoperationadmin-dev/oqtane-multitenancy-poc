# Document 5: Tenant-Specific Background Worker Architecture & Guide

## Overview
This document explains how to develop, deploy, configure, and maintain background workers in Oqtane Framework (.NET 10) that execute tasks **only for a designated tenant** and access **strictly that tenant's isolated database**.

---

## 1. Architectural Challenge: Hosted Services in Multi-Tenancy
In ASP.NET Core and Oqtane:
- Standard background services (`IHostedService` / `HostedServiceBase`) execute outside of any incoming HTTP request.
- Therefore, there is **no ambient HTTP Tenant Context** (`TenantMiddleware` is not involved).
- By default, Oqtane's `HostedServiceBase` loops through every tenant in `Oqtane_Master.dbo.Tenant`.
- If a background job performs tasks specific to one business unit (e.g. nightly inventory audit for Tenant 1 only), running it unconditionally on all tenants would cause errors or data corruption.

---

## 2. Implementation: The Tenant Guard Pattern

To restrict execution, the worker implements the **Tenant Guard Pattern**:
1. It retrieves the current tenant instance from `ITenantManager`.
2. It verifies whether `currentTenant.Name` matches the allowed target tenant.
3. If not, it skips execution immediately with a clear log entry.
4. If it matches, it resolves tenant-scoped repositories to perform isolated database operations.

### Source Code: `TenantSpecificWorker.cs`
Located in `Server/BackgroundWorkers/TenantSpecificWorker.cs`:

```csharp
using System;
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
        // Allowed target tenant name
        private const string TargetTenantName = "Master"; // Or "Tenant1"

        public TenantSpecificWorker(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            Name = "Tenant-Specific Inventory Audit Worker";
            Frequency = "d"; // Runs daily ('m'=minutes, 'H'=hours, 'd'=days, 'w'=weeks, 'M'=months)
            Interval = 1;
            IsEnabled = true;
            RetentionHistory = 30; // Retain 30 execution logs
        }

        public override string ExecuteJob(IServiceProvider provider)
        {
            var tenantManager = provider.GetRequiredService<ITenantManager>();
            var logger = provider.GetRequiredService<ILogger<TenantSpecificWorker>>();
            Tenant currentTenant = tenantManager.GetTenant();

            if (currentTenant == null)
            {
                return "No active tenant context found.";
            }

            // CRITICAL TENANT GUARD:
            // Skip execution if current iteration belongs to another tenant
            if (!string.Equals(currentTenant.Name, TargetTenantName, StringComparison.OrdinalIgnoreCase))
            {
                return $"[Skipped] Tenant '{currentTenant.Name}' is not targeted by this worker.\n";
            }

            try
            {
                logger.LogInformation("Executing background audit for Tenant '{TenantName}' (ID: {TenantId})", 
                    currentTenant.Name, currentTenant.TenantId);

                // This repository connects ONLY to the targeted tenant's database
                var productRepository = provider.GetRequiredService<IProductRepository>();
                var products = productRepository.GetProducts(-1);

                int productCount = 0;
                foreach (var product in products)
                {
                    productCount++;
                }

                string summary = $"[SUCCESS] Tenant '{currentTenant.Name}': Audited {productCount} products in isolated DB at {DateTime.UtcNow:u}.\n";
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
```

---

## 3. Dependency Injection Registration

In Oqtane, background workers are registered in an implementation of `IServerStartup` (`Server/Startup/PocServerStartup.cs`):

```csharp
public class PocServerStartup : IServerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register repository and module services
        services.AddTransient<IProductRepository, ProductRepository>();

        // Register background worker hosted service
        services.AddHostedService<TenantSpecificWorker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) { }
    public void ConfigureMvc(IMvcBuilder mvcBuilder) { }
}
```

---

## 4. Deployment and Operational Maintenance

### Viewing and Managing Jobs in Oqtane UI
1. Log into Oqtane as `host`.
2. Navigate to **Control Panel > Admin > Job Management**.
3. Locate **Tenant-Specific Inventory Audit Worker** in the job list.
4. Administrators can:
   - **Edit Schedule**: Alter execution interval (e.g. from every 1 day to every 15 minutes).
   - **Run Now**: Manually trigger an immediate execution.
   - **View History**: Inspect status, duration, and output messages for each execution.
   - **Enable / Disable**: Pause background job without stopping the web server.

### Job Logs in the Master Database
All job execution logs are tracked in `Oqtane_Master.dbo.JobLog`:
```sql
SELECT TOP 10 
    JobName, 
    StartDate, 
    FinishDate, 
    Succeeded, 
    Status 
FROM [Oqtane_Master].[dbo].[JobLog]
ORDER BY StartDate DESC;
```
Logs will clearly indicate `[SUCCESS]` for the designated tenant and `[Skipped]` for all other tenants.
