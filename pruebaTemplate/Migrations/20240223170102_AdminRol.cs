using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanillaPM.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS(Select Id from AspNetRoles where Id = '2c59b302-0de3-4bcc-a7c6-91c5d6e15e94')
                BEGIN
	                INSERT AspNetRoles (Id, [Name], [NormalizedName])
	                VALUES('2c59b302-0de3-4bcc-a7c6-91c5d6e15e94','admin','ADMIN')
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE AspNetRoles Where Id = '2c59b302-0de3-4bcc-a7c6-91c5d6e15e94'");
        }
    }
}
