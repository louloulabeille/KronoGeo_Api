using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KronoGeo_Api.Infrastructure.MigrationBase
{
    /// <inheritdoc />
    public partial class AddModelRouteTelemetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteTelemetryId",
                table: "LocalisationGroup",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RouteTelemetry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    DistanceUnit = table.Column<int>(type: "integer", nullable: false),
                    AverageSpeed = table.Column<double>(type: "double precision", nullable: false),
                    PositiveElevationGain = table.Column<double>(type: "double precision", nullable: false),
                    NegativeElevationGain = table.Column<double>(type: "double precision", nullable: false),
                    AverageHeartRate = table.Column<double>(type: "double precision", nullable: false),
                    DateTimeBegin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateTimeEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalTimePaused = table.Column<int>(type: "integer", nullable: false),
                    TotalLocalisations = table.Column<int>(type: "integer", nullable: false),
                    LocalisationGroupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTelemetry", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalisationGroup_RouteTelemetry_RouteTelemetryId",
                table: "LocalisationGroup");

            migrationBuilder.DropTable(
                name: "RouteTelemetry");

            migrationBuilder.DropIndex(
                name: "IX_LocalisationGroup_RouteTelemetryId",
                table: "LocalisationGroup");

            migrationBuilder.DropColumn(
                name: "RouteTelemetryId",
                table: "LocalisationGroup");
        }
    }
}
