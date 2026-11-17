using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;
using OqtanePoc.Models;

namespace OqtanePoc.Modules.HelpDesk.Repository
{
    public class HelpDeskContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<HelpDeskTicket> Tickets { get; set; }

        public HelpDeskContext(IDBContextDependencies dbContextDependencies) : base(dbContextDependencies)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<HelpDeskTicket>().ToTable(ActiveDatabase.RewriteName("HelpDesk_Ticket"));
        }
    }
}
