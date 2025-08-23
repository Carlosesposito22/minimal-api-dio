using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minimal_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ADMINS",
                columns: new[] { "Id", "Email", "Role", "Senha" },
                values: new object[] { 1, "administrador@teste.com", "Admin", "123456" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ADMINS",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
