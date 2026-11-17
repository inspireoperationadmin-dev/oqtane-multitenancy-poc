using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using OqtanePoc.Modules.CustomerFeedback.Migrations.EntityBuilders;
using OqtanePoc.Modules.CustomerFeedback.Repository;

namespace OqtanePoc.Modules.CustomerFeedback.Migrations
{
    [DbContext(typeof(CustomerFeedbackContext))]
    [Migration("OqtanePoc.CustomerFeedback.01.00.00.00")]
    public class CustomerFeedbackInitialize : MultiDatabaseMigration
    {
        public CustomerFeedbackInitialize(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new CustomerFeedbackEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new CustomerFeedbackEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
