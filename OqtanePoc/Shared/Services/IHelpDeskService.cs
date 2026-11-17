using System.Collections.Generic;
using System.Threading.Tasks;
using OqtanePoc.Models;

namespace OqtanePoc.Services
{
    public interface IHelpDeskService
    {
        Task<List<HelpDeskTicket>> GetTicketsAsync(int moduleId);
        Task<HelpDeskTicket> GetTicketAsync(int ticketId, int moduleId);
        Task<HelpDeskTicket> AddTicketAsync(HelpDeskTicket ticket);
        Task<HelpDeskTicket> UpdateTicketAsync(HelpDeskTicket ticket);
        Task DeleteTicketAsync(int ticketId, int moduleId);
    }
}
