using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOnboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBiometricEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnableBiometric",
                table: "Customer",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnableBiometric",
                table: "Customer");
        }
    }
}
