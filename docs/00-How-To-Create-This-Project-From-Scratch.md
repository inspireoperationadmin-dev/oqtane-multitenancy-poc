# How to Create This Project from Scratch: Developer & Setup Guide

## Overview
This document provides a complete, step-by-step developer guide for replicating this **Oqtane Multi-Tenant Architecture POC** from absolute scratch on any development machine. 

It explains why we intentionally avoided cloning the Oqtane source repository (`git clone`) and instead used the official **Oqtane .NET Application Template**, decoupling custom enterprise logic from core framework binaries.

---

## 1. Prerequisites

Before starting, ensure the development environment has:
- **.NET 10.0 SDK** (verify via `dotnet --version` showing `10.0.x`)
- **SQL Server LocalDB** (included with Visual Studio) or SQL Server Express / Developer edition
- **Visual Studio 2022 / 2026 / VS Code** with C# Dev Kit
- Terminal (PowerShell or Bash)

---

## 2. Why Template Scaffolding over Git Clone?

| Factor | Cloning Source Repo (`git clone`) | Official NuGet Template (`dotnet new oqtane`) |
| :--- | :--- | :--- |
| **Project Weight** | Clones all framework tests, docs, and build tools (>150MB). | Scaffolds only clean application entry points (<15MB). |
| **Upgradability** | Upgrades require complex Git merges and resolving conflicts. | Upgrades require changing a single package version in `.csproj`. |
| **Separation of Concerns** | Custom code easily mixes into framework folders. | Strict separation: Framework is consumed via NuGet; business code remains decoupled. |
| **Production Standard** | Not recommended for enterprise deployments. | **Official enterprise pattern** recommended by the Oqtane Core Team. |

---

## 3. Step 1: Install the Oqtane Application Template

Open PowerShell and install the official Oqtane 10 Application Template from NuGet:

```powershell
dotnet new install Oqtane.Application.Template::10.3.1
```

To verify the installation:
```powershell
dotnet new list | Select-String "oqtane"
```
You should see `Oqtane Application` listed as an available template with the short name `oqtane`.

---

## 4. Step 2: Scaffold the Solution

Navigate to your workspace directory and execute:

```powershell
mkdir OqtanePoc
cd OqtanePoc
dotnet new oqtane -n OqtanePoc
```

### Generated Project Structure:
```
OqtanePoc/
├── OqtanePoc.slnx              # Modern .NET solution file
├── Client/                      # Blazor WebAssembly / UI Layer
│   ├── OqtanePoc.Client.csproj
│   ├── Program.cs
│   └── Themes/                  # Custom theme location
├── Server/                      # ASP.NET Core Host & API Layer
│   ├── OqtanePoc.Server.csproj
│   ├── Program.cs
│   ├── appsettings.json         # Database connection & installation settings
│   ├── Controllers/             # REST API Controllers
│   ├── Modules/                 # Business logic, repositories, DB contexts
│   └── BackgroundWorkers/       # Hosted background jobs
└── Shared/                      # Shared Models Layer
    ├── OqtanePoc.Shared.csproj
    └── Models/                  # Shared entities, DTOs, interfaces
```

All three projects automatically reference official Oqtane packages in their `.csproj` files:
```xml
<PackageReference Include="Oqtane.Server" Version="10.3.1" />
<PackageReference Include="Oqtane.Client" Version="10.3.1" />
<PackageReference Include="Oqtane.Shared" Version="10.3.1" />
```

---

## 5. Step 3: Configure Master Database Connection

Open `Server/appsettings.json` and configure the initial Master database connection:

```json
{
  "Database": {
    "DefaultDBType": "Oqtane.Database.SqlServer.SqlServerDatabase, Oqtane.Server"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Oqtane_Master;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Installation": {
    "DefaultAlias": "localhost:48807",
    "HostUsername": "host",
    "HostPassword": "HostPassword123!",
    "HostEmail": "host@oqtane.local",
    "SiteName": "Default Site (Tenant 1)"
  }
}
```

---

## 6. Step 4: Developing the 3 Isolated Modules

In `Server/Modules/`, create three independent folders:
1. `ProductCatalog/`
2. `HelpDesk/`
3. `CustomerFeedback/`

### Anatomy of each Module:
Each module contains:
- **Entity Model**: Inherits `IAuditable` (e.g. `ProductItem.cs` with `ProductId`, `ModuleId`, `Title`, `Price`).
- **EntityBuilder**: Inherits `AuditableBaseEntityBuilder<T>` to generate database columns.
- **EF Core DbContext**: Inherits `DBContextBase, ITransientService, IMultiDatabase`.
- **Migration**: Inherits `MultiDatabaseMigration` with `[DbContext]` and `[Migration]` attributes.
- **Repository**: Injects `IDbContextFactory<TContext>` to guarantee tenant-scoped database connections.
- **Module Manager**: Inherits `MigratableModuleBase, IInstallable` to trigger migrations per tenant.

Register the module services in `Server/Startup/PocServerStartup.cs`:
```csharp
public class PocServerStartup : IServerStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextFactory<ProductContext>(opt => { }, ServiceLifetime.Transient);
        services.AddDbContextFactory<HelpDeskContext>(opt => { }, ServiceLifetime.Transient);
        services.AddDbContextFactory<CustomerFeedbackContext>(opt => { }, ServiceLifetime.Transient);

        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IHelpDeskRepository, HelpDeskRepository>();
        services.AddTransient<ICustomerFeedbackRepository, CustomerFeedbackRepository>();
    }
}
```

---

## 7. Step 5: Developing the Tenant-Scoped Theme

Create `Client/Themes/EnterpriseModernTheme/`:
1. **`ThemeInfo.cs`**: Implements `ITheme`.
   > [!IMPORTANT]
   > Must register the Bootstrap 5 bundle script in `ThemeInfo.cs` so that Oqtane's offcanvas ControlPanel drawer functions properly:
   > ```csharp
   > new Script(Constants.BootstrapScriptUrl, Constants.BootstrapScriptIntegrity, "anonymous")
   > ```
2. **`Default.razor`**: Layout markup inheriting `ThemeBase`.
3. **`Container.razor`**: Card container layout inheriting `ContainerBase`.
4. **`theme.css`**: Styling rules.

---

## 8. Step 6: Developing the Background Worker

Create `Server/BackgroundWorkers/TenantSpecificWorker.cs`:
- Inherits `HostedServiceBase`.
- Implements the **Tenant Guard Pattern**:
  ```csharp
  Tenant currentTenant = tenantManager.GetTenant();
  if (!string.Equals(currentTenant.Name, "Master", StringComparison.OrdinalIgnoreCase))
  {
      return $"[Skipped] Tenant '{currentTenant.Name}' is not targeted by this worker.\n";
  }
  ```
- Register in `PocServerStartup.cs`:
  ```csharp
  services.AddHostedService<TenantSpecificWorker>();
  ```

---

## 9. Step 7: Developing the Tenant-Restricted REST API

Create:
1. **Filter**: `Server/Filters/TenantAuthorizeAttribute.cs` (implements `IAuthorizationFilter`).
   Checks `ITenantManager.GetTenant()`. Returns `403 Forbidden` if the caller's tenant does not match the whitelist.
2. **Controller**: `Server/Controllers/TenantRestrictedApiController.cs`:
   Decorated with `[TenantAuthorize("Master", "Tenant1")]`.

---

## 10. Step 8: Building and Running the Solution

1. Build the solution:
   ```powershell
   dotnet build
   ```
2. Start the application:
   ```powershell
   cd Server
   dotnet run --urls "http://localhost:48807"
   ```
3. Open `http://localhost:48807` in your browser.
4. Log in as `host` with password `HostPassword123!`.

---

## 11. Step 9: Provisioning Tenants via Oqtane 10 UI

1. Open **Control Panel** (gear icon) > **Admin Dashboard** > **Site Management**.
2. Click **Add Site**:
   - **Site Name**: `HelpDesk Portal`
   - **Database**: Select `<Add Database>` from the dropdown.
   - **Tenant Name**: `Tenant2`
   - **Database Type**: `SQL Server`
   - **Database Name**: `Oqtane_Tenant2`
   - **Host Username / Password**: `host` / `HostPassword123!`
   - **Alias**: `localhost:48807/tenant2`
3. Click **Save**. Oqtane creates `Oqtane_Tenant2` database and applies migrations automatically.
4. Repeat for Tenant 3:
   - **Site Name**: `Survey Portal`
   - **Tenant Name**: `Tenant3`
   - **Database Name**: `Oqtane_Tenant3`
   - **Alias**: `localhost:48807/tenant3`

---

## 12. Verification Matrix

| Target Component | URL | Verification Step | Expected Result |
| :--- | :--- | :--- | :--- |
| **Primary Tenant** | `http://localhost:48807` | Log in, check ProductCatalog module, Enterprise theme. | Theme renders dark header; module reads `Oqtane_Master`. |
| **Tenant 2** | `http://localhost:48807/tenant2` | Check HelpDesk module. | Default theme renders; module reads `Oqtane_Tenant2`. |
| **Tenant 3** | `http://localhost:48807/tenant3` | Check CustomerFeedback module. | Default theme renders; module reads `Oqtane_Tenant3`. |
| **API (Authorized)** | `http://localhost:48807/api/TenantRestrictedApi/status` | Send GET request. | **`200 OK - Access Granted`** |
| **API (Forbidden)** | `http://localhost:48807/tenant2/api/TenantRestrictedApi/status` | Send GET request. | **`403 Forbidden`** |
