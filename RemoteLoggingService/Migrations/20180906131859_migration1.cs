using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemoteLoggingService.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_ClientGuid",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ClientGuid",
                table: "Logs",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_ClientGuid",
                table: "Logs",
                newName: "IX_Logs_ClientId");

            migrationBuilder.AddColumn<Guid>(
                name: "DeveloperId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Users",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeveloperId",
                table: "Users",
                column: "DeveloperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_ClientId",
                table: "Logs",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DeveloperId",
                table: "Users",
                column: "DeveloperId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_ClientId",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DeveloperId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeveloperId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeveloperId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Logs",
                newName: "ClientGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_ClientId",
                table: "Logs",
                newName: "IX_Logs_ClientGuid");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(nullable: false),
                    DeveloperUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clients_Users_DeveloperUserId",
                        column: x => x.DeveloperUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_DeveloperUserId",
                table: "Clients",
                column: "DeveloperUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_ClientGuid",
                table: "Logs",
                column: "ClientGuid",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
