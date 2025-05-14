using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class adddeliveryPostion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "lateLatiude",
                table: "Deliveries",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "longLatiude",
                table: "Deliveries",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lateLatiude",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "longLatiude",
                table: "Deliveries");
        }
    }
}
