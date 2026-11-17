using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using OqtanePoc.Modules.HelpDesk.Migrations.EntityBuilders;
using OqtanePoc.Modules.HelpDesk.Repository;

namespace OqtanePoc.Modules.HelpDesk.Migrations
{
    [DbContext(typeof(HelpDeskContext))]
    [Migration("OqtanePoc.HelpDesk.01.00.00.00")]
    public class HelpDeskInitialize : MultiDatabaseMigration
    {
        public HelpDeskInitialize(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new HelpDeskEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new HelpDeskEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
