using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KronoGeo_Api.Infrastructure.MigrationBase
{
    /// <inheritdoc />
    public partial class UpdateRouteTelemetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AverageHeartRate",
                table: "RouteTelemetry",
                newName: "TotalTime");

            migrationBuilder.AlterColumn<double>(
                name: "TotalTimePaused",
                table: "RouteTelemetry",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalTime",
                table: "RouteTelemetry",
                newName: "AverageHeartRate");

            migrationBuilder.AlterColumn<int>(
                name: "TotalTimePaused",
                table: "RouteTelemetry",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
