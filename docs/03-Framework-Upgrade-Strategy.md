# Document 3: Oqtane Framework Safe Upgrade Strategy

## Overview
This document details the recommended strategy for upgrading the Oqtane Framework (.NET 10 / ASP.NET Core) safely in a multi-tenant environment without breaking or overwriting custom modules, custom themes, or tenant database schemas.

---

## 1. Why Oqtane Framework Upgrades are Safe
Oqtane was architected from inception to prevent framework upgrades from clobbering customizations:

| Architecture Layer | Core Oqtane Framework | Custom Modules & Themes | Isolation Guarantee |
| :--- | :--- | :--- | :--- |
| **Assemblies (DLLs)** | `Oqtane.Server.dll`, `Oqtane.Client.dll`, `Oqtane.Shared.dll` | `OqtanePoc.Server.Oqtane.dll`, `OqtanePoc.Client.Oqtane.dll`, etc. | Physical separation of binaries; framework upgrades only overwrite Oqtane DLLs. |
| **Database Migrations** | Internal Oqtane Migrations (`Oqtane.01.00.00.00` to `10.x.x`) | Custom Module Migrations (`OqtanePoc.ProductCatalog.01.00.00.00`) | Oqtane tracks EF Core migrations via migration IDs. Upgrading Oqtane only touches core tables (`Site`, `Page`, `Module`, `Tenant`). |
| **Theme & Static Assets** | `/wwwroot/Themes/Oqtane.Theme.Standard/` | `/wwwroot/Themes/EnterpriseModernTheme/` | Custom themes live in their own isolated folders; framework updates never overwrite custom theme assets. |

---

## 2. Safe Upgrade Playbook (Step-by-Step)

### Phase 1: Pre-Upgrade Verification & Backup
Before performing any upgrade in staging or production:

1. **Backup All Databases**:
   Every database in the multi-tenant cluster must be backed up:
   ```sql
   BACKUP DATABASE [Oqtane_Master] TO DISK = 'C:\Backups\Oqtane_Master_PreUpgrade.bak' WITH FORMAT;
   BACKUP DATABASE [Oqtane_Tenant1] TO DISK = 'C:\Backups\Oqtane_Tenant1_PreUpgrade.bak' WITH FORMAT;
   BACKUP DATABASE [Oqtane_Tenant2] TO DISK = 'C:\Backups\Oqtane_Tenant2_PreUpgrade.bak' WITH FORMAT;
   BACKUP DATABASE [Oqtane_Tenant3] TO DISK = 'C:\Backups\Oqtane_Tenant3_PreUpgrade.bak' WITH FORMAT;
   ```

2. **Backup Configuration & Assets**:
   - Backup `Server/appsettings.json` (contains database connection strings and secret keys).
   - Backup `Server/wwwroot/Content/` (tenant media uploads).
   - Backup custom theme templates in `Server/wwwroot/Themes/`.

---

### Phase 2: Upgrading the Codebase

#### Approach A: Upgrading via NuGet (Developer / CI/CD Workflow)
If maintaining Oqtane as a solution with NuGet packages:
1. Open the project file `Server/OqtanePoc.Server.csproj` (or solution in Visual Studio).
2. Update the Oqtane package references to the target version (e.g., `10.3.1` or higher):
   ```xml
   <ItemGroup>
     <PackageReference Include="Oqtane.Server" Version="10.3.1" />
   </ItemGroup>
   ```
3. Run `dotnet restore` and `dotnet build` to verify API compatibility.
4. Deploy the updated output to the staging/production server.

#### Approach B: Upgrading via Host Admin UI (System Update)
1. Log in to the Primary Tenant (`localhost:48807`) as `host`.
2. Navigate to **Control Panel > Admin > System Update**.
3. Upload the official Oqtane upgrade package (`Oqtane.Upgrade.X.X.X.nupkg`).
4. Click **Install**. Oqtane will automatically place new core binaries into the `/bin` directory and trigger a restart.

---

### Phase 3: Post-Upgrade Verification & Multi-Tenant Migration
When the upgraded application starts:
1. `DatabaseManager.Install()` runs automatically in `Program.cs`.
2. Oqtane queries `Oqtane_Master` and all tenant databases, applying any new core framework migrations.
3. Custom module tables (`ProductCatalog_Item`, `HelpDesk_Ticket`, `CustomerFeedback_Entry`) remain completely intact.
4. **Smoke Test Every Tenant**:
   - Verify Tenant 1: `http://localhost:48807/`
   - Verify Tenant 2: `http://localhost:48807/tenant2`
   - Verify Tenant 3: `http://localhost:48807/tenant3`
   - Verify that custom theme styling and custom module functionality persist as expected.

---

## 3. Rollback Strategy
If an unforeseen breaking change occurs in an external dependency:
1. Stop the application server (IIS App Pool or Kestrel process).
2. Restore the previous DLLs from the pre-upgrade deployment folder.
3. Restore the pre-upgrade database backups (`.bak` files).
4. Restart the application server. Downtime is minimized to under 5 minutes.
