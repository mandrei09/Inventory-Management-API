using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddOrderMatrixValidators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeL1Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL2Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL3Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeL4Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeS1Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeS2Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeS3Id",
                table: "Order",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeL1Id",
                table: "Order",
                column: "EmployeeL1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeL2Id",
                table: "Order",
                column: "EmployeeL2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeL3Id",
                table: "Order",
                column: "EmployeeL3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeL4Id",
                table: "Order",
                column: "EmployeeL4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeS1Id",
                table: "Order",
                column: "EmployeeS1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeS2Id",
                table: "Order",
                column: "EmployeeS2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeS3Id",
                table: "Order",
                column: "EmployeeS3Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeL1Id",
                table: "Order",
                column: "EmployeeL1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeL2Id",
                table: "Order",
                column: "EmployeeL2Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeL3Id",
                table: "Order",
                column: "EmployeeL3Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeL4Id",
                table: "Order",
                column: "EmployeeL4Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeS1Id",
                table: "Order",
                column: "EmployeeS1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeS2Id",
                table: "Order",
                column: "EmployeeS2Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeS3Id",
                table: "Order",
                column: "EmployeeS3Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeL1Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeL2Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeL3Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeL4Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeS1Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeS2Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeS3Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeL1Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeL2Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeL3Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeL4Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeS1Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeS2Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeS3Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeL1Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeL2Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeL3Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeL4Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeS1Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeS2Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeS3Id",
                table: "Order");
        }
    }
}
