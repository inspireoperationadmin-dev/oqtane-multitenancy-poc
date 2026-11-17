using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using OqtanePoc.Modules.ProductCatalog.Migrations.EntityBuilders;
using OqtanePoc.Modules.ProductCatalog.Repository;

namespace OqtanePoc.Modules.ProductCatalog.Migrations
{
    [DbContext(typeof(ProductContext))]
    [Migration("OqtanePoc.ProductCatalog.01.00.00.00")]
    public class ProductInitialize : MultiDatabaseMigration
    {
        public ProductInitialize(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new ProductEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new ProductEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
