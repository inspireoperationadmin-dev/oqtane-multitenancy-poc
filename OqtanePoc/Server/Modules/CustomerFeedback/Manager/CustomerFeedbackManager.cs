using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Repository;
using OqtanePoc.Modules.CustomerFeedback.Repository;

namespace OqtanePoc.Modules.CustomerFeedback.Manager
{
    public class CustomerFeedbackManager : MigratableModuleBase, IInstallable
    {
        private readonly IDBContextDependencies _dbContextDependencies;

        public CustomerFeedbackManager(IDBContextDependencies dbContextDependencies)
        {
            _dbContextDependencies = dbContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new CustomerFeedbackContext(_dbContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new CustomerFeedbackContext(_dbContextDependencies), tenant, MigrationType.Down);
        }
    }
}
