using System;
using Oqtane.Models;

namespace OqtanePoc.Models
{
    public class CustomerFeedbackEntry : IAuditable
    {
        public int FeedbackId { get; set; }
        public int ModuleId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public int Rating { get; set; } = 5;
        public string Comments { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime ModifiedOn { get; set; }
    }
}
