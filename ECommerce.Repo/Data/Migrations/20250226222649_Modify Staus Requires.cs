using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyStausRequires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Deliveries",
                type: "text",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Deliveries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "Pending");
        }
    }
}
