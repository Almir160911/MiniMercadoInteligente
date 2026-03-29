using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniMercadoInteligente.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminPanelAndAmazonGoUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EffectiveFrom",
                table: "product_prices",
                newName: "EffectiveAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EffectiveAtUtc",
                table: "product_prices",
                newName: "EffectiveFrom");
        }
    }
}
