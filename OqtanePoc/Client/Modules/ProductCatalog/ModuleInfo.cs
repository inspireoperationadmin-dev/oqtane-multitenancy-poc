using Oqtane.Models;
using Oqtane.Modules;

namespace OqtanePoc.Modules.ProductCatalog
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Product Catalog",
            Description = "Tenant 1 Product Catalog Module",
            Version = "1.0.0",
            ServerManagerType = "OqtanePoc.Modules.ProductCatalog.Manager.ProductManager, OqtanePoc.Server.Oqtane",
            ReleaseVersions = "1.0.0"
        };
    }
}
