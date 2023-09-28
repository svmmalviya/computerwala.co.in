using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBService.Migrations
{
    /// <inheritdoc />
    public partial class CWCalenders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Visiters",
                table: "Visiters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.RenameTable(
                name: "Visiters",
                newName: "CWVisiters");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "CWSubscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "CWVisiters",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "CWVisiters",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CWVisiters",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "CWSubscriptions",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "CWSubscriptions",
                type: "TIMESTAMP",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "CWSubscriptions",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier(36)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CWVisiters",
                table: "CWVisiters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CWSubscriptions",
                table: "CWSubscriptions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CWCalenders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    Active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AttachmentId = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CWCalenders", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CWCalenders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CWVisiters",
                table: "CWVisiters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CWSubscriptions",
                table: "CWSubscriptions");

            migrationBuilder.RenameTable(
                name: "CWVisiters",
                newName: "Visiters");

            migrationBuilder.RenameTable(
                name: "CWSubscriptions",
                newName: "Subscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "Visiters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Visiters",
                type: "datetime2(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Visiters",
                type: "uniqueidentifier(36)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Subscriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "Subscriptions",
                type: "datetime2(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Subscriptions",
                type: "uniqueidentifier(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visiters",
                table: "Visiters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");
        }
    }
}
