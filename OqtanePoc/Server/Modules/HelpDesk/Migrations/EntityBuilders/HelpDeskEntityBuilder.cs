using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;

namespace OqtanePoc.Modules.HelpDesk.Migrations.EntityBuilders
{
    public class HelpDeskEntityBuilder : AuditableBaseEntityBuilder<HelpDeskEntityBuilder>
    {
        private const string _entityTableName = "HelpDesk_Ticket";
        private readonly PrimaryKey<HelpDeskEntityBuilder> _primaryKey = new("PK_HelpDesk_Ticket", x => x.TicketId);
        private readonly ForeignKey<HelpDeskEntityBuilder> _moduleForeignKey = new("FK_HelpDesk_Ticket_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public HelpDeskEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override HelpDeskEntityBuilder BuildTable(ColumnsBuilder table)
        {
            TicketId = AddAutoIncrementColumn(table, "TicketId");
            ModuleId = AddIntegerColumn(table, "ModuleId");
            TicketNumber = AddStringColumn(table, "TicketNumber", 50);
            Subject = AddStringColumn(table, "Subject", 255);
            Priority = AddStringColumn(table, "Priority", 20);
            Status = AddStringColumn(table, "Status", 20);
            Description = AddMaxStringColumn(table, "Description");
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> TicketId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> TicketNumber { get; set; }
        public OperationBuilder<AddColumnOperation> Subject { get; set; }
        public OperationBuilder<AddColumnOperation> Priority { get; set; }
        public OperationBuilder<AddColumnOperation> Status { get; set; }
        public OperationBuilder<AddColumnOperation> Description { get; set; }
    }
}
