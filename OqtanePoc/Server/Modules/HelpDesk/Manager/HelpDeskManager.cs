using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using OqtanePoc.Modules.HelpDesk.Repository;

namespace OqtanePoc.Modules.HelpDesk.Manager
{
    public class HelpDeskManager : MigratableModuleBase, IInstallable
    {
        private readonly IDBContextDependencies _dbContextDependencies;

        public HelpDeskManager(IDBContextDependencies dbContextDependencies)
        {
            _dbContextDependencies = dbContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new HelpDeskContext(_dbContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new HelpDeskContext(_dbContextDependencies), tenant, MigrationType.Down);
        }
    }
}
