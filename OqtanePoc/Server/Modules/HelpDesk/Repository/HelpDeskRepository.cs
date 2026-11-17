using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using OqtanePoc.Models;

namespace OqtanePoc.Modules.HelpDesk.Repository
{
    public interface IHelpDeskRepository
    {
        IEnumerable<HelpDeskTicket> GetTickets(int moduleId);
        HelpDeskTicket GetTicket(int ticketId);
        HelpDeskTicket AddTicket(HelpDeskTicket ticket);
        HelpDeskTicket UpdateTicket(HelpDeskTicket ticket);
        void DeleteTicket(int ticketId);
    }

    public class HelpDeskRepository : IHelpDeskRepository, ITransientService
    {
        private readonly IDbContextFactory<HelpDeskContext> _factory;

        public HelpDeskRepository(IDbContextFactory<HelpDeskContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<HelpDeskTicket> GetTickets(int moduleId)
        {
            using var db = _factory.CreateDbContext();
            return db.Tickets.Where(item => item.ModuleId == moduleId).AsNoTracking().ToList();
        }

        public HelpDeskTicket GetTicket(int ticketId)
        {
            using var db = _factory.CreateDbContext();
            return db.Tickets.Find(ticketId);
        }

        public HelpDeskTicket AddTicket(HelpDeskTicket ticket)
        {
            using var db = _factory.CreateDbContext();
            db.Tickets.Add(ticket);
            db.SaveChanges();
            return ticket;
        }

        public HelpDeskTicket UpdateTicket(HelpDeskTicket ticket)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return ticket;
        }

        public void DeleteTicket(int ticketId)
        {
            using var db = _factory.CreateDbContext();
            var ticket = db.Tickets.Find(ticketId);
            if (ticket != null)
            {
                db.Tickets.Remove(ticket);
                db.SaveChanges();
            }
        }
    }
}
