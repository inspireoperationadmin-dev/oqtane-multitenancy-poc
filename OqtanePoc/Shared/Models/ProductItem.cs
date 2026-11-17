using System;
using Oqtane.Models;

namespace OqtanePoc.Models
{
    public class ProductItem : IAuditable
    {
        public int ProductId { get; set; }
        public int ModuleId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime ModifiedOn { get; set; }
    }
}
