using System;
using Oqtane.Models;

namespace OqtanePoc.Models
{
    public class HelpDeskTicket : IAuditable
    {
        public int TicketId { get; set; }
        public int ModuleId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal";
        public string Status { get; set; } = "Open";
        public string Description { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime ModifiedOn { get; set; }
    }
}
