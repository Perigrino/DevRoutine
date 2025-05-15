using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevRoutine.Api.Migrations.Application;

/// <inheritdoc />
public partial class RoutineTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "routine_tags",
            schema: "dev_routine",
            columns: table => new
            {
                routine_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                tag_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_routine_tags", x => new { x.routine_id, x.tag_id });
                table.ForeignKey(
                    name: "fk_routine_tags_routines_routine_id",
                    column: x => x.routine_id,
                    principalSchema: "dev_routine",
                    principalTable: "routines",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_routine_tags_tags_tag_id",
                    column: x => x.tag_id,
                    principalSchema: "dev_routine",
                    principalTable: "tags",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_routine_tags_tag_id",
            schema: "dev_routine",
            table: "routine_tags",
            column: "tag_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "routine_tags",
            schema: "dev_routine");
    }
}
