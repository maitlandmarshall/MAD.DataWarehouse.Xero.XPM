using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MAD.DataWarehouse.Xero.XPM.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiData_ApiData_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ApiData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiData_Endpoint",
                table: "ApiData",
                column: "Endpoint");

            migrationBuilder.CreateIndex(
                name: "IX_ApiData_ParentId",
                table: "ApiData",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiData_Uri",
                table: "ApiData",
                column: "Uri");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiData");
        }
    }
}
