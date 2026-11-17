using Oqtane.Models;
using Oqtane.Modules;

namespace OqtanePoc.Modules.HelpDesk
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Help Desk Tickets",
            Description = "Tenant 2 Help Desk Ticket Management Module",
            Version = "1.0.0",
            ServerManagerType = "OqtanePoc.Modules.HelpDesk.Manager.HelpDeskManager, OqtanePoc.Server.Oqtane",
            ReleaseVersions = "1.0.0"
        };
    }
}
