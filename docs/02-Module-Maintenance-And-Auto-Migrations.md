# Document 2: Module Maintenance & Automatic Multi-Tenant Database Migrations

> **Client Requirement**: 
> *"Document how to maintain these modules and themes separately. For instance if we create a new tenant, and it needs to use an existing module, how are the tables created? How to create them automatically. Further, if there are any changes to the module, how do we push all changes to each tenant automatically?"*

---

## 1. How Modules & Themes are Maintained Separately

In Oqtane, modules and themes are built as **decoupled projects** outside the core framework:
- **Shared Project**: Holds entity models (`IAuditable`) and service interfaces.
- **Server Project**: Contains `DBContextBase`, `MultiDatabaseMigration`, repository, controllers, and `IInstallable` managers.
- **Client Project**: Contains Blazor Razor components (`Index.razor`) and `IModule` metadata.

Because each module defines its own `DBContextBase` and `EntityBuilder`, module tables have unique names and schemas (e.g. `ProductCatalog_Item`, `HelpDesk_Ticket`, `CustomerFeedback_Entry`). They do not collide with framework tables or other modules.

---

## 2. Automatic Table Creation for New Tenants

### The Problem:
When Tenant 4 is created in the future, how does it get the database tables for an existing module (like `ProductCatalog_Item`) automatically?

### The Automated Solution:
Oqtane provides a built-in lifecycle via `IInstallable` and `DatabaseManager`:

```
New Tenant Provisioned
         │
         ▼
DatabaseManager.Install() scans registered assemblies
         │
         ▼
Finds classes implementing IInstallable (e.g., ProductManager)
         │
         ▼
Invokes ProductManager.Install(newTenant, "1.0.0")
         │
         ▼
MigratableModuleBase executes EF Core MultiDatabaseMigration against newTenant DB
         │
         ▼
Table 'ProductCatalog_Item' created automatically in Tenant 4 DB!
```

### Key Implementation Code (`ProductManager.cs`):
```csharp
public class ProductManager : MigratableModuleBase, IInstallable
{
    private readonly IDBContextDependencies _dbContextDependencies;

    public ProductManager(IDBContextDependencies dbContextDependencies)
    {
        _dbContextDependencies = dbContextDependencies;
    }

    // Called automatically by Oqtane for each tenant
    public bool Install(Tenant tenant, string version)
    {
        return Migrate(new ProductContext(_dbContextDependencies), tenant, MigrationType.Up);
    }

    public bool Uninstall(Tenant tenant)
    {
        return Migrate(new ProductContext(_dbContextDependencies), tenant, MigrationType.Down);
    }
}
```
**Result**: No manual SQL scripts are required when spinning up a new tenant. The tables are generated immediately upon tenant registration or when the module is added to a page.

---

## 3. Pushing Schema Changes to All Tenants Automatically

### The Problem:
If you modify a module (e.g., adding a new `Discount` column to `ProductCatalog_Item`), how do you push this change to all tenant databases automatically without manual SQL execution?

### The Automated Solution:

#### Step 1: Create an Incremental Migration Class
Add a new migration class inside the module's `Migrations` folder with the updated version attribute:

```csharp
[DbContext(typeof(ProductContext))]
[Migration("OqtanePoc.ProductCatalog.01.00.01.00")] // Version 1.0.1
public class AddDiscountColumn : MultiDatabaseMigration
{
    public AddDiscountColumn(IDatabase database) : base(database) { }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var entityBuilder = new AddColumnEntityBuilder(migrationBuilder, ActiveDatabase);
        entityBuilder.AddDecimalColumn("ProductCatalog_Item", "Discount", 18, 2, defaultValue: 0.0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var entityBuilder = new AddColumnEntityBuilder(migrationBuilder, ActiveDatabase);
        entityBuilder.DropColumn("ProductCatalog_Item", "Discount");
    }
}
```

#### Step 2: Update Module Version
In `Client/Modules/ProductCatalog/ModuleInfo.cs`:
```csharp
public ModuleDefinition ModuleDefinition => new ModuleDefinition
{
    Name = "Product Catalog",
    Version = "1.0.1",
    ReleaseVersions = "1.0.0,1.0.1",
    ServerManagerType = "OqtanePoc.Modules.ProductCatalog.Manager.ProductManager, OqtanePoc.Server.Oqtane"
};
```

#### Step 3: Deploy & Restart
1. Compile and deploy the updated module assembly.
2. When Oqtane restarts, `DatabaseManager.Install()` in `Program.cs` executes.
3. `DatabaseManager` discovers that version `1.0.1` is newer than the version recorded in `ModuleDefinition`.
4. It automatically loops through **every registered tenant** in `Oqtane_Master.dbo.Tenant`:
   ```csharp
   foreach (Tenant tenant in tenantRepository.GetTenants())
   {
       moduleManager.Install(tenant, "1.0.1"); // Applies pending migrations on each tenant DB!
   }
   ```
5. Every tenant database receives the new column update automatically.

---

## 4. Programmatic CI/CD Migration Runner (Optional Automation)
If your deployment pipeline prefers running migrations without waiting for a web request, you can trigger migrations via a CLI or deployment task:

```csharp
public class TenantMigrationRunner
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IEnumerable<IInstallable> _moduleManagers;

    public TenantMigrationRunner(ITenantRepository tenantRepository, IEnumerable<IInstallable> moduleManagers)
    {
        _tenantRepository = tenantRepository;
        _moduleManagers = moduleManagers;
    }

    public void MigrateAllTenants()
    {
        foreach (var tenant in _tenantRepository.GetTenants())
        {
            foreach (var manager in _moduleManagers)
            {
                manager.Install(tenant, Constants.Version);
            }
        }
    }
}
```
