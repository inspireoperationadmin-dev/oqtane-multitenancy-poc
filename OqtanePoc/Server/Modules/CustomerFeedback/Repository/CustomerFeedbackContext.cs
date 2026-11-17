using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;
using OqtanePoc.Models;

namespace OqtanePoc.Modules.CustomerFeedback.Repository
{
    public class CustomerFeedbackContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<CustomerFeedbackEntry> Feedbacks { get; set; }

        public CustomerFeedbackContext(IDBContextDependencies dbContextDependencies) : base(dbContextDependencies)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CustomerFeedbackEntry>().ToTable(ActiveDatabase.RewriteName("CustomerFeedback_Entry"));
        }
    }
}
