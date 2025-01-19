using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueFrames.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDeletedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Customer");
        }
    }
}
