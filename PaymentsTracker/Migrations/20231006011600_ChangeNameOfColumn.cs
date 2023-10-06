using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentsTracker.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameOfColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descrption",
                table: "Payments",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Payments",
                newName: "Descrption");
        }
    }
}
