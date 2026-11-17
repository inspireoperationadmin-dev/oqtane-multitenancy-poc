using Microsoft.EntityFrameworkCore;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;
using OqtanePoc.Models;

namespace OqtanePoc.Modules.ProductCatalog.Repository
{
    public class ProductContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<ProductItem> Products { get; set; }

        public ProductContext(IDBContextDependencies dbContextDependencies) : base(dbContextDependencies)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ProductItem>().ToTable(ActiveDatabase.RewriteName("ProductCatalog_Item"));
        }
    }
}
