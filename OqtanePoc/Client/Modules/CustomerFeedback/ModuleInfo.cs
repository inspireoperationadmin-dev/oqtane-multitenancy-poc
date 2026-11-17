using Oqtane.Models;
using Oqtane.Modules;

namespace OqtanePoc.Modules.CustomerFeedback
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Customer Feedback",
            Description = "Tenant 3 Customer Feedback Module",
            Version = "1.0.0",
            ServerManagerType = "OqtanePoc.Modules.CustomerFeedback.Manager.CustomerFeedbackManager, OqtanePoc.Server.Oqtane",
            ReleaseVersions = "1.0.0"
        };
    }
}
