using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Otaku_Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ItemPath = table.Column<string>(type: "TEXT", nullable: false),
                    ImgBytes = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Title);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
