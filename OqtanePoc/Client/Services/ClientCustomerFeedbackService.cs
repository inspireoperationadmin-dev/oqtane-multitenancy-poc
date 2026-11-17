using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using OqtanePoc.Models;

namespace OqtanePoc.Services
{
    public class ClientCustomerFeedbackService : ServiceBase, ICustomerFeedbackService, ITransientService
    {
        public ClientCustomerFeedbackService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("CustomerFeedback");

        public async Task<List<CustomerFeedbackEntry>> GetFeedbacksAsync(int moduleId)
        {
            List<CustomerFeedbackEntry> feedbacks = await GetJsonAsync<List<CustomerFeedbackEntry>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={moduleId}", EntityNames.Module, moduleId), Enumerable.Empty<CustomerFeedbackEntry>().ToList());
            return feedbacks;
        }

        public async Task<CustomerFeedbackEntry> GetFeedbackAsync(int feedbackId, int moduleId)
        {
            return await GetJsonAsync<CustomerFeedbackEntry>(CreateAuthorizationPolicyUrl($"{Apiurl}/{feedbackId}/{moduleId}", EntityNames.Module, moduleId));
        }

        public async Task<CustomerFeedbackEntry> AddFeedbackAsync(CustomerFeedbackEntry feedback)
        {
            return await PostJsonAsync<CustomerFeedbackEntry>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, feedback.ModuleId), feedback);
        }

        public async Task<CustomerFeedbackEntry> UpdateFeedbackAsync(CustomerFeedbackEntry feedback)
        {
            return await PutJsonAsync<CustomerFeedbackEntry>(CreateAuthorizationPolicyUrl($"{Apiurl}/{feedback.FeedbackId}", EntityNames.Module, feedback.ModuleId), feedback);
        }

        public async Task DeleteFeedbackAsync(int feedbackId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{feedbackId}/{moduleId}", EntityNames.Module, moduleId));
        }
    }
}
