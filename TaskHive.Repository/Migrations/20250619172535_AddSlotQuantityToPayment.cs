using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskHive.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotQuantityToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SlotPurchases_Users_UserId",
                table: "SlotPurchases");

            migrationBuilder.DropIndex(
                name: "IX_SlotPurchases_UserId",
                table: "SlotPurchases");

            migrationBuilder.DropColumn(
                name: "RemainingSlots",
                table: "UserMemberships");

            migrationBuilder.DropColumn(
                name: "PurchasedSlots",
                table: "SlotPurchases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SlotPurchases");

            migrationBuilder.RenameColumn(
                name: "PurchasedAt",
                table: "SlotPurchases",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<decimal>(
                name: "SlotQuantity",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlotQuantity",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "SlotPurchases",
                newName: "PurchasedAt");

            migrationBuilder.AddColumn<int>(
                name: "RemainingSlots",
                table: "UserMemberships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedSlots",
                table: "SlotPurchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SlotPurchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SlotPurchases_UserId",
                table: "SlotPurchases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SlotPurchases_Users_UserId",
                table: "SlotPurchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
