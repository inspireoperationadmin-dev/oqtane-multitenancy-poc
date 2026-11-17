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
using OqtanePoc.Modules.CustomerFeedback.Repository;
using OqtanePoc.Services;

namespace OqtanePoc.Modules.CustomerFeedback.Services
{
    public class ServerCustomerFeedbackService : ICustomerFeedbackService, ITransientService
    {
        private readonly ICustomerFeedbackRepository _customerFeedbackRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        public ServerCustomerFeedbackService(ICustomerFeedbackRepository customerFeedbackRepository, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor)
        {
            _customerFeedbackRepository = customerFeedbackRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
        }

        public Task<List<CustomerFeedbackEntry>> GetFeedbacksAsync(int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_customerFeedbackRepository.GetFeedbacks(moduleId).ToList());
            }
            _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Customer Feedback Get Attempt {ModuleId}", moduleId);
            return Task.FromResult(new List<CustomerFeedbackEntry>());
        }

        public Task<CustomerFeedbackEntry> GetFeedbackAsync(int feedbackId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.View))
            {
                return Task.FromResult(_customerFeedbackRepository.GetFeedback(feedbackId));
            }
            return Task.FromResult<CustomerFeedbackEntry>(null);
        }

        public Task<CustomerFeedbackEntry> AddFeedbackAsync(CustomerFeedbackEntry feedback)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, feedback.ModuleId, PermissionNames.Edit))
            {
                return Task.FromResult(_customerFeedbackRepository.AddFeedback(feedback));
            }
            return Task.FromResult<CustomerFeedbackEntry>(null);
        }

        public Task<CustomerFeedbackEntry> UpdateFeedbackAsync(CustomerFeedbackEntry feedback)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, feedback.ModuleId, PermissionNames.Edit))
            {
                return Task.FromResult(_customerFeedbackRepository.UpdateFeedback(feedback));
            }
            return Task.FromResult<CustomerFeedbackEntry>(null);
        }

        public Task DeleteFeedbackAsync(int feedbackId, int moduleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, moduleId, PermissionNames.Edit))
            {
                _customerFeedbackRepository.DeleteFeedback(feedbackId);
            }
            return Task.CompletedTask;
        }
    }
}
