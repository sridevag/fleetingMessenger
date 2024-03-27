using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunkyChatt.Migrations
{
    /// <inheritdoc />
    public partial class changed_hash_type2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "keyTimeStamp",
                table: "hashDict",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "publicPrivatePairs",
                table: "hashDict",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "keyTimeStamp",
                table: "hashDict");

            migrationBuilder.DropColumn(
                name: "publicPrivatePairs",
                table: "hashDict");
        }
    }
}
