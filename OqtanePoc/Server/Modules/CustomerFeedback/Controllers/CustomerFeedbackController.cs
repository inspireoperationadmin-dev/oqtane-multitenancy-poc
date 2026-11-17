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

namespace OqtanePoc.Modules.CustomerFeedback.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class CustomerFeedbackController : ModuleControllerBase
    {
        private readonly ICustomerFeedbackService _customerFeedbackService;

        public CustomerFeedbackController(ICustomerFeedbackService customerFeedbackService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _customerFeedbackService = customerFeedbackService;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<CustomerFeedbackEntry>> Get(string moduleid)
        {
            if (int.TryParse(moduleid, out int moduleId) && IsAuthorizedEntityId(EntityNames.Module, moduleId))
            {
                return await _customerFeedbackService.GetFeedbacksAsync(moduleId);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<CustomerFeedbackEntry> Get(int id, int moduleid)
        {
            if (IsAuthorizedEntityId(EntityNames.Module, moduleid))
            {
                return await _customerFeedbackService.GetFeedbackAsync(id, moduleid);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<CustomerFeedbackEntry> Post([FromBody] CustomerFeedbackEntry feedback)
        {
            if (ModelState.IsValid && IsAuthorizedEntityId(EntityNames.Module, feedback.ModuleId))
            {
                return await _customerFeedbackService.AddFeedbackAsync(feedback);
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return null;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<CustomerFeedbackEntry> Put(int id, [FromBody] CustomerFeedbackEntry feedback)
        {
            if (ModelState.IsValid && feedback.FeedbackId == id && IsAuthorizedEntityId(EntityNames.Module, feedback.ModuleId))
            {
                return await _customerFeedbackService.UpdateFeedbackAsync(feedback);
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
                await _customerFeedbackService.DeleteFeedbackAsync(id, moduleid);
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
