using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KronoGeo_Api.Infrastructure.MigrationBase
{
    /// <inheritdoc />
    public partial class AjoutDescriptionPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Localisation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Localisation");
        }
    }
}
