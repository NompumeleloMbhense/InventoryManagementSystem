using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerApp.Migrations
{
    /// <inheritdoc />
    public partial class FixStockTransactionSpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerfomedBy",
                table: "StockTransactions",
                newName: "PerformedBy");

            migrationBuilder.RenameColumn(
                name: "DateOccured",
                table: "StockTransactions",
                newName: "DateOccurred");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerformedBy",
                table: "StockTransactions",
                newName: "PerfomedBy");

            migrationBuilder.RenameColumn(
                name: "DateOccurred",
                table: "StockTransactions",
                newName: "DateOccured");
        }
    }
}
