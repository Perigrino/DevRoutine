using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevRoutine.Api.Migrations.Application;

/// <inheritdoc />
public partial class UpdatedRoutines : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            schema: "dev_routine",
            table: "routines",
            newName: "updated_at");

        migrationBuilder.RenameColumn(
            name: "last_completed_at_utc",
            schema: "dev_routine",
            table: "routines",
            newName: "last_completed_at");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            schema: "dev_routine",
            table: "routines",
            newName: "created_at");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "updated_at",
            schema: "dev_routine",
            table: "routines",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "last_completed_at",
            schema: "dev_routine",
            table: "routines",
            newName: "last_completed_at_utc");

        migrationBuilder.RenameColumn(
            name: "created_at",
            schema: "dev_routine",
            table: "routines",
            newName: "created_at_utc");
    }
}
