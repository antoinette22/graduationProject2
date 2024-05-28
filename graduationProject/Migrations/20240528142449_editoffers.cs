using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace graduationProject.Migrations
{
    /// <inheritdoc />
    public partial class editoffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rrice",
                table: "Offers",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "NationalId",
                table: "Offers",
                newName: "SignatureUser");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Offers",
                newName: "NationalcardUser");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdInvestor",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdUser",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalcardInvestor",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalIdInvestor",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "NationalIdUser",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "NationalcardInvestor",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "SignatureUser",
                table: "Offers",
                newName: "NationalId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Offers",
                newName: "Rrice");

            migrationBuilder.RenameColumn(
                name: "NationalcardUser",
                table: "Offers",
                newName: "Image");
        }
    }
}
