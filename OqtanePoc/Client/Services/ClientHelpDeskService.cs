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
    public class ClientHelpDeskService : ServiceBase, IHelpDeskService, ITransientService
    {
        public ClientHelpDeskService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("HelpDesk");

        public async Task<List<HelpDeskTicket>> GetTicketsAsync(int moduleId)
        {
            List<HelpDeskTicket> tickets = await GetJsonAsync<List<HelpDeskTicket>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={moduleId}", EntityNames.Module, moduleId), Enumerable.Empty<HelpDeskTicket>().ToList());
            return tickets;
        }

        public async Task<HelpDeskTicket> GetTicketAsync(int ticketId, int moduleId)
        {
            return await GetJsonAsync<HelpDeskTicket>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ticketId}/{moduleId}", EntityNames.Module, moduleId));
        }

        public async Task<HelpDeskTicket> AddTicketAsync(HelpDeskTicket ticket)
        {
            return await PostJsonAsync<HelpDeskTicket>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, ticket.ModuleId), ticket);
        }

        public async Task<HelpDeskTicket> UpdateTicketAsync(HelpDeskTicket ticket)
        {
            return await PutJsonAsync<HelpDeskTicket>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ticket.TicketId}", EntityNames.Module, ticket.ModuleId), ticket);
        }

        public async Task DeleteTicketAsync(int ticketId, int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{ticketId}/{moduleId}", EntityNames.Module, moduleId));
        }
    }
}
