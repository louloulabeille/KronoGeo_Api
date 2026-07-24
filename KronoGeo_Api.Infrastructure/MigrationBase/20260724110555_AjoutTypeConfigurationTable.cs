using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KronoGeo_Api.Infrastructure.MigrationBase
{
    /// <inheritdoc />
    public partial class AjoutTypeConfigurationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalisationGroups_AspNetUsers_ApplicationUserId",
                table: "LocalisationGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Localisations_LocalisationGroups_LocalisationGroupId",
                table: "Localisations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Localisations",
                table: "Localisations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalisationGroups",
                table: "LocalisationGroups");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Localisations");

            migrationBuilder.RenameTable(
                name: "Localisations",
                newName: "Localisation");

            migrationBuilder.RenameTable(
                name: "LocalisationGroups",
                newName: "LocalisationGroup");

            migrationBuilder.RenameIndex(
                name: "IX_Localisations_LocalisationGroupId",
                table: "Localisation",
                newName: "IX_Localisation_LocalisationGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_LocalisationGroups_ApplicationUserId",
                table: "LocalisationGroup",
                newName: "IX_LocalisationGroup_ApplicationUserId");

            migrationBuilder.AddColumn<string>(
                name: "TypeLocalisation",
                table: "Localisation",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Localisation",
                table: "Localisation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalisationGroup",
                table: "LocalisationGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Localisation_LocalisationGroup_LocalisationGroupId",
                table: "Localisation",
                column: "LocalisationGroupId",
                principalTable: "LocalisationGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalisationGroup_AspNetUsers_ApplicationUserId",
                table: "LocalisationGroup",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Localisation_LocalisationGroup_LocalisationGroupId",
                table: "Localisation");

            migrationBuilder.DropForeignKey(
                name: "FK_LocalisationGroup_AspNetUsers_ApplicationUserId",
                table: "LocalisationGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocalisationGroup",
                table: "LocalisationGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Localisation",
                table: "Localisation");

            migrationBuilder.DropColumn(
                name: "TypeLocalisation",
                table: "Localisation");

            migrationBuilder.RenameTable(
                name: "LocalisationGroup",
                newName: "LocalisationGroups");

            migrationBuilder.RenameTable(
                name: "Localisation",
                newName: "Localisations");

            migrationBuilder.RenameIndex(
                name: "IX_LocalisationGroup_ApplicationUserId",
                table: "LocalisationGroups",
                newName: "IX_LocalisationGroups_ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Localisation_LocalisationGroupId",
                table: "Localisations",
                newName: "IX_Localisations_LocalisationGroupId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Localisations",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocalisationGroups",
                table: "LocalisationGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Localisations",
                table: "Localisations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalisationGroups_AspNetUsers_ApplicationUserId",
                table: "LocalisationGroups",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Localisations_LocalisationGroups_LocalisationGroupId",
                table: "Localisations",
                column: "LocalisationGroupId",
                principalTable: "LocalisationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
