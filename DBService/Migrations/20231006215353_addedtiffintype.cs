using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBService.Migrations
{
    /// <inheritdoc />
    public partial class addedtiffintype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CWAttendance",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "CWAttendance");
        }
    }
}
