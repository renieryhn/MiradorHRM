using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanillaPM.Migrations
{
    /// <inheritdoc />
    public partial class AdministracionRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(Select Id from AspNetRoles where Id = 'a254c135-4c61-4d22-a66a-179e8aa33b13')
                BEGIN
	                INSERT AspNetRoles (Id, [Name], [NormalizedName])
	                VALUES('a254c135-4c61-4d22-a66a-179e8aa33b13','administracion','ADMINISTRACION')
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE AspNetRoles Where Id = 'a254c135-4c61-4d22-a66a-179e8aa33b13'");
        }
    }
}
