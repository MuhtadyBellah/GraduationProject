using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DTime",
                table: "Deliveries",
                newName: "DeliveryTime");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Deliveries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "DeliveryTime",
                table: "Deliveries",
                newName: "DTime");
        }
    }
}
