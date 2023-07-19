using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZooAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnimalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FoodNeeded",
                table: "Animals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodNeeded",
                table: "Animals");
        }
    }
}
