using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace OqtanePoc.Modules.ProductCatalog.Migrations.EntityBuilders
{
    public class ProductEntityBuilder : AuditableBaseEntityBuilder<ProductEntityBuilder>
    {
        private const string _entityTableName = "ProductCatalog_Item";
        private readonly PrimaryKey<ProductEntityBuilder> _primaryKey = new("PK_ProductCatalog_Item", x => x.ProductId);
        private readonly ForeignKey<ProductEntityBuilder> _moduleForeignKey = new("FK_ProductCatalog_Item_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public ProductEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override ProductEntityBuilder BuildTable(ColumnsBuilder table)
        {
            ProductId = AddAutoIncrementColumn(table, "ProductId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            Sku = AddStringColumn(table, "Sku", 50);
            Title = AddStringColumn(table, "Title", 255);
            Price = AddDecimalColumn(table, "Price", 18, 2);
            StockQuantity = AddIntegerColumn(table, "StockQuantity");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> ProductId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Sku { get; set; }
        public OperationBuilder<AddColumnOperation> Title { get; set; }
        public OperationBuilder<AddColumnOperation> Price { get; set; }
        public OperationBuilder<AddColumnOperation> StockQuantity { get; set; }
    }
}
