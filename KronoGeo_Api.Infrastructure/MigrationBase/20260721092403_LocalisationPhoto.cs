using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KronoGeo_Api.Infrastructure.MigrationBase
{
    /// <inheritdoc />
    public partial class LocalisationPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Localisations",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Localisations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PathPhoto",
                table: "Localisations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Localisations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Localisations");

            migrationBuilder.DropColumn(
                name: "PathPhoto",
                table: "Localisations");
        }
    }
}
