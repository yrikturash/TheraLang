﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TheraLang.DAL.Migrations
{
    public partial class AddMediaVersions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Piranha_Media",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Piranha_Media",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Piranha_MediaVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(),
                    Height = table.Column<int>(nullable: true),
                    MediaId = table.Column<Guid>(),
                    Size = table.Column<long>(),
                    Width = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_MediaVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_MediaVersions_Piranha_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Piranha_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_MediaVersions_MediaId_Width_Height",
                table: "Piranha_MediaVersions",
                columns: new[] { "MediaId", "Width", "Height" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_MediaVersions");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Piranha_Media");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Piranha_Media");
        }
    }
}
