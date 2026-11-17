using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Modules;
using Oqtane.Security;
using Oqtane.Shared;
using OqtanePoc.Models;
using OqtanePoc.Modules.HelpDesk.Repository;
using OqtanePoc.Services;

namespace OqtanePoc.Modules.HelpDesk.Services
{
    public class ServerHelpDeskService : IHelpDeskService, ITransientService
    {
        private readonly IHelpDeskRepository _helpDeskRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerHelpDeskService(IHelpDeskRepository helpDeskRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _helpDeskRepository = helpDeskRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<HelpDeskTicket>> GetTicketsAsync(int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_helpDeskRepository.GetTickets(moduleId).ToList());
            }
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized HelpDesk Ticket Get Attempt {ModuleId}", moduleId);
            return Task.FromResult(new List<HelpDeskTicket>());
        }

        public Task<HelpDeskTicket> GetTicketAsync(int ticketId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_helpDeskRepository.GetTicket(ticketId));
            }
            return Task.FromResult<HelpDeskTicket>(null);
        }

        public Task<HelpDeskTicket> AddTicketAsync(HelpDeskTicket ticket)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ticket.ModuleId, PermissionNames.Edit))
            {
                return Task.FromResult(_helpDeskRepository.AddTicket(ticket));
            }
            return Task.FromResult<HelpDeskTicket>(null);
        }

        public Task<HelpDeskTicket> UpdateTicketAsync(HelpDeskTicket ticket)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ticket.ModuleId, PermissionNames.Edit))
            {
                return Task.FromResult(_helpDeskRepository.UpdateTicket(ticket));
            }
            return Task.FromResult<HelpDeskTicket>(null);
        }

        public Task DeleteTicketAsync(int ticketId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.Edit))
            {
                _helpDeskRepository.DeleteTicket(ticketId);
            }
            return Task.CompletedTask;
        }
    }
}
