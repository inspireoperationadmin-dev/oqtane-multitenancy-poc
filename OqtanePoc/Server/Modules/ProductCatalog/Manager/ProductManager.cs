using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using OqtanePoc.Modules.ProductCatalog.Repository;

namespace OqtanePoc.Modules.ProductCatalog.Manager
{
    public class ProductManager : MigratableModuleBase, IInstallable
    {
        private readonly IDBContextDependencies _dbContextDependencies;

        public ProductManager(IDBContextDependencies dbContextDependencies)
        {
            _dbContextDependencies = dbContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new ProductContext(_dbContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new ProductContext(_dbContextDependencies), tenant, MigrationType.Down);
        }
    }
}
