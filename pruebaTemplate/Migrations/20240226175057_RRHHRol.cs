using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanillaPM.Migrations
{
    /// <inheritdoc />
    public partial class RRHHRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(Select Id from AspNetRoles where Id = '85676837-4b58-49b4-ac2b-58c3146a655b')
                BEGIN
	                INSERT AspNetRoles (Id, [Name], [NormalizedName])
	                VALUES('85676837-4b58-49b4-ac2b-58c3146a655b','rrhh','RRHH')
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE AspNetRoles Where Id = '85676837-4b58-49b4-ac2b-58c3146a655b'");
        }
    }
}
