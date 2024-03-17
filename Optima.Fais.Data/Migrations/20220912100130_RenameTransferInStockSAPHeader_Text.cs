using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class RenameTransferInStockSAPHeader_Text : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Header_Text",
                table: "TransferInStockSAP",
                newName: "Header_Txt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Header_Txt",
                table: "TransferInStockSAP",
                newName: "Header_Text");
        }
    }
}
