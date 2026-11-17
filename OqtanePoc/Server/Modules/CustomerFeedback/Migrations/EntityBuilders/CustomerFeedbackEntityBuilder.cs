using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace OqtanePoc.Modules.CustomerFeedback.Migrations.EntityBuilders
{
    public class CustomerFeedbackEntityBuilder : AuditableBaseEntityBuilder<CustomerFeedbackEntityBuilder>
    {
        private const string _entityTableName = "CustomerFeedback_Entry";
        private readonly PrimaryKey<CustomerFeedbackEntityBuilder> _primaryKey = new("PK_CustomerFeedback_Entry", x => x.FeedbackId);
        private readonly ForeignKey<CustomerFeedbackEntityBuilder> _moduleForeignKey = new("FK_CustomerFeedback_Entry_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public CustomerFeedbackEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override CustomerFeedbackEntityBuilder BuildTable(ColumnsBuilder table)
        {
            FeedbackId = AddAutoIncrementColumn(table, "FeedbackId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            CustomerEmail = AddStringColumn(table, "CustomerEmail", 150);
            Rating = AddIntegerColumn(table, "Rating");
            Comments = AddMaxStringColumn(table, "Comments");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> FeedbackId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> CustomerEmail { get; set; }
        public OperationBuilder<AddColumnOperation> Rating { get; set; }
        public OperationBuilder<AddColumnOperation> Comments { get; set; }
    }
}
