using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RHF.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemovedColumnDonationsHeaderTB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "DonationsHeader");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "DonationsHeader",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
