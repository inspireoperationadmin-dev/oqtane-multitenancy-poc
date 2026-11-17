using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using OqtanePoc.Models;

namespace OqtanePoc.Modules.CustomerFeedback.Repository
{
    public interface ICustomerFeedbackRepository
    {
        IEnumerable<CustomerFeedbackEntry> GetFeedbacks(int moduleId);
        CustomerFeedbackEntry GetFeedback(int feedbackId);
        CustomerFeedbackEntry AddFeedback(CustomerFeedbackEntry feedback);
        CustomerFeedbackEntry UpdateFeedback(CustomerFeedbackEntry feedback);
        void DeleteFeedback(int feedbackId);
    }

    public class CustomerFeedbackRepository : ICustomerFeedbackRepository, ITransientService
    {
        private readonly IDbContextFactory<CustomerFeedbackContext> _factory;

        public CustomerFeedbackRepository(IDbContextFactory<CustomerFeedbackContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<CustomerFeedbackEntry> GetFeedbacks(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.Feedbacks.Where(item => item.ModuleId == moduleId).AsNoTracking().ToList();
        }

        public CustomerFeedbackEntry GetFeedback(int feedbackId)
        {
            using var db = _factory.CreateDbContext();
            return db.Feedbacks.Find(feedbackId);
        }

        public CustomerFeedbackEntry AddFeedback(CustomerFeedbackEntry feedback)
        {
            using var db = _factory.CreateDbContext();
            db.Feedbacks.Add(feedback);
            db.SaveChanges();
            return feedback;
        }

        public CustomerFeedbackEntry UpdateFeedback(CustomerFeedbackEntry feedback)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(feedback).State = EntityState.Modified;
            db.SaveChanges();
            return feedback;
        }

        public void DeleteFeedback(int feedbackId)
        {
            using var db = _factory.CreateDbContext();
            var feedback = db.Feedbacks.Find(feedbackId);
            if (feedback != null)
            {
                db.Feedbacks.Remove(feedback);
                db.SaveChanges();
            }
        }
    }
}
