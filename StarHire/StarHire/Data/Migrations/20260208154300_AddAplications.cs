using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarHire.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Applications",
                newName: "AlienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlienId",
                table: "Applications",
                newName: "UserId");
        }
    }
}
