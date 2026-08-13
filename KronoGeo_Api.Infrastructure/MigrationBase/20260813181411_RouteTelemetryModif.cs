using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KronoGeo_Api.Infrastructure.MigrationBase
{
    /// <inheritdoc />
    public partial class RouteTelemetryModif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalisationGroup_RouteTelemetry_RouteTelemetryId",
                table: "LocalisationGroup");

            migrationBuilder.DropIndex(
                name: "IX_LocalisationGroup_RouteTelemetryId",
                table: "LocalisationGroup");

            migrationBuilder.DropColumn(
                name: "RouteTelemetryId",
                table: "LocalisationGroup");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTelemetry_LocalisationGroupId",
                table: "RouteTelemetry",
                column: "LocalisationGroupId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTelemetry_LocalisationGroup_LocalisationGroupId",
                table: "RouteTelemetry",
                column: "LocalisationGroupId",
                principalTable: "LocalisationGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTelemetry_LocalisationGroup_LocalisationGroupId",
                table: "RouteTelemetry");

            migrationBuilder.DropIndex(
                name: "IX_RouteTelemetry_LocalisationGroupId",
                table: "RouteTelemetry");

            migrationBuilder.AddColumn<int>(
                name: "RouteTelemetryId",
                table: "LocalisationGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalisationGroup_RouteTelemetryId",
                table: "LocalisationGroup",
                column: "RouteTelemetryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalisationGroup_RouteTelemetry_RouteTelemetryId",
                table: "LocalisationGroup",
                column: "RouteTelemetryId",
                principalTable: "RouteTelemetry",
                principalColumn: "Id");
        }
    }
}
