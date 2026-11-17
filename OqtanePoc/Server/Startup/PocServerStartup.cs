using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using OqtanePoc.BackgroundWorkers;
using OqtanePoc.Modules.CustomerFeedback.Repository;
using OqtanePoc.Modules.CustomerFeedback.Services;
using OqtanePoc.Modules.HelpDesk.Repository;
using OqtanePoc.Modules.HelpDesk.Services;
using OqtanePoc.Modules.ProductCatalog.Repository;
using OqtanePoc.Modules.ProductCatalog.Services;
using OqtanePoc.Services;

namespace OqtanePoc.Startup
{
    public class PocServerStartup : IServerStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext Factories for all 3 isolated modules
            services.AddDbContextFactory<ProductContext>(opt => { }, ServiceLifetime.Transient);
            services.AddDbContextFactory<HelpDeskContext>(opt => { }, ServiceLifetime.Transient);
            services.AddDbContextFactory<CustomerFeedbackContext>(opt => { }, ServiceLifetime.Transient);

            // Register Module Server Services
            services.AddTransient<IProductService, ServerProductService>();
            services.AddTransient<IHelpDeskService, ServerHelpDeskService>();
            services.AddTransient<ICustomerFeedbackService, ServerCustomerFeedbackService>();

            // Register tenant-specific hosted service worker
            services.AddHostedService<TenantSpecificWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
        }
    }
}
