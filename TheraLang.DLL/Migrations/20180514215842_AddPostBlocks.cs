﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TheraLang.DLL.Migrations
{
    public partial class AddPostBlocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_PostBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(),
                    BlockId = table.Column<Guid>(),
                    PostId = table.Column<Guid>(),
                    SortOrder = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_PostBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_PostBlocks_Piranha_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Piranha_Blocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Piranha_PostBlocks_Piranha_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Piranha_Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostBlocks_BlockId",
                table: "Piranha_PostBlocks",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PostBlocks_PostId_SortOrder",
                table: "Piranha_PostBlocks",
                columns: new[] { "PostId", "SortOrder" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_PostBlocks");
        }
    }
}
