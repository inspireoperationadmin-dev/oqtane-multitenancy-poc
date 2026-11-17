using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using OqtanePoc.Models;
using OqtanePoc.Services;

namespace OqtanePoc.Modules.HelpDesk.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class HelpDeskController : ModuleControllerBase
    {
        private readonly IHelpDeskService _helpDeskService;

        public HelpDeskController(IHelpDeskService helpDeskService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _helpDeskService = helpDeskService;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<HelpDeskTicket>> Get(string moduleid)
        {
            if (int.TryParse(moduleid, out int moduleId) && IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                return await _helpDeskService.GetTicketsAsync(moduleId);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<HelpDeskTicket> Get(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _helpDeskService.GetTicketAsync(id, moduleid);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<HelpDeskTicket> Post([FromBody] HelpDeskTicket ticket)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, ticket.ModuleId))
            {
                return await _helpDeskService.AddTicketAsync(ticket);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<HelpDeskTicket> Put(int id, [FromBody] HelpDeskTicket ticket)
        {
            if (ModelState.IsValid && ticket.TicketId == id && IsAuthorizedEntityId(EntityNames.Module, ticket.ModuleId))
            {
                return await _helpDeskService.UpdateTicketAsync(ticket);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                await _helpDeskService.DeleteTicketAsync(id, moduleid);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
