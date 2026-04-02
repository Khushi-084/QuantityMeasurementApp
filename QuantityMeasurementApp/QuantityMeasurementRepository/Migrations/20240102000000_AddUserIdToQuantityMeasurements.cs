using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementRepository.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToQuantityMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add nullable UserId column — existing rows get NULL (anonymous)
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "QuantityMeasurements",
                type: "int",
                nullable: true,
                defaultValue: null);

            // Index for fast per-user queries
            migrationBuilder.CreateIndex(
                name: "IX_QM_UserId",
                table: "QuantityMeasurements",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QM_UserId",
                table: "QuantityMeasurements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuantityMeasurements");
        }
    }
}
