using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddReqBMaterialParentAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasParentAsset",
                table: "RequestBudgetForecastMaterial",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ParentAsset",
                table: "RequestBudgetForecastMaterial",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasParentAsset",
                table: "RequestBudgetForecastMaterial");

            migrationBuilder.DropColumn(
                name: "ParentAsset",
                table: "RequestBudgetForecastMaterial");
        }
    }
}
