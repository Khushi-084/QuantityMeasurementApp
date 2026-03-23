using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuantityMeasurementRepository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Users table ────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id           = table.Column<int>(type: "int", nullable: false)
                                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username     = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email        = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Role         = table.Column<string>(type: "nvarchar(20)",  maxLength: 20,  nullable: false, defaultValue: "User"),
                    CreatedAt    = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginAt  = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive     = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            // ── QuantityMeasurements table ─────────────────────────────────
            migrationBuilder.CreateTable(
                name: "QuantityMeasurements",
                columns: table => new
                {
                    Id                  = table.Column<int>(type: "int", nullable: false)
                                               .Annotation("SqlServer:Identity", "1, 1"),
                    OperationType       = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: false),
                    MeasurementCategory = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    Operand1Value       = table.Column<double>(type: "float", nullable: true),
                    Operand1Unit        = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    Operand2Value       = table.Column<double>(type: "float", nullable: true),
                    Operand2Unit        = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    ResultValue         = table.Column<double>(type: "float", nullable: true),
                    ResultUnit          = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    ResultCategory      = table.Column<string>(type: "nvarchar(50)",  maxLength: 50,  nullable: true),
                    HasError            = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ErrorMessage        = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt           = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuantityMeasurements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QM_OperationType",
                table: "QuantityMeasurements",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_QM_Category",
                table: "QuantityMeasurements",
                column: "MeasurementCategory");

            migrationBuilder.CreateIndex(
                name: "IX_QM_CreatedAt",
                table: "QuantityMeasurements",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "QuantityMeasurements");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}