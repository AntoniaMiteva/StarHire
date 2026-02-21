using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarHire.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillNormalizedName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Skills",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [Skills] SET [NormalizedName] = UPPER(TRIM([Name]))");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_NormalizedName",
                table: "Skills",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skills_NormalizedName",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Skills");
        }
    }
}
