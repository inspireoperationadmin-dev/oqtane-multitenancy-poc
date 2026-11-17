using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Services;
using OqtanePoc.Services;

namespace OqtanePoc.Client.Startup
{
    public class PocClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IProductService)))
            {
                services.AddScoped<IProductService, ClientProductService>();
            }
            if (!services.Any(s => s.ServiceType == typeof(IHelpDeskService)))
            {
                services.AddScoped<IHelpDeskService, ClientHelpDeskService>();
            }
            if (!services.Any(s => s.ServiceType == typeof(ICustomerFeedbackService)))
            {
                services.AddScoped<ICustomerFeedbackService, ClientCustomerFeedbackService>();
            }
        }
    }
}
