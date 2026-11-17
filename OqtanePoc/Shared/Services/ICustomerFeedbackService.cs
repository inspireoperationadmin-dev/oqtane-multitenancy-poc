using System.Collections.Generic;
using System.Threading.Tasks;
using OqtanePoc.Models;

namespace OqtanePoc.Services
{
    public interface ICustomerFeedbackService
    {
        Task<List<CustomerFeedbackEntry>> GetFeedbacksAsync(int moduleId);
        Task<CustomerFeedbackEntry> GetFeedbackAsync(int feedbackId, int moduleId);
        Task<CustomerFeedbackEntry> AddFeedbackAsync(CustomerFeedbackEntry feedback);
        Task<CustomerFeedbackEntry> UpdateFeedbackAsync(CustomerFeedbackEntry feedback);
        Task DeleteFeedbackAsync(int feedbackId, int moduleId);
    }
}
