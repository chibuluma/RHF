using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RHF.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedProjectIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "DonationsHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "DonationsHeader");
        }
    }
}
