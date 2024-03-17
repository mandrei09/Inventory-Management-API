using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddMatrixPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeB1Id",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeB1Id",
                table: "Matrix",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL1",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL2",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL3",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL4",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS1",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS2",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS3",
                table: "Matrix",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeB1EmailSend",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeB1EmailSkip",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeB1Id",
                table: "EmailOrderStatus",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmployeeB1ValidateAt",
                table: "EmailOrderStatus",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeB1ValidateBy",
                table: "EmailOrderStatus",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotEmployeeB1Sync",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL1",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL2",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL3",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityL4",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS1",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS2",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriorityS3",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncEmployeeB1ErrorCount",
                table: "EmailOrderStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Order_EmployeeB1Id",
                table: "Order",
                column: "EmployeeB1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matrix_EmployeeB1Id",
                table: "Matrix",
                column: "EmployeeB1Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOrderStatus_EmployeeB1Id",
                table: "EmailOrderStatus",
                column: "EmployeeB1Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmailOrderStatus_EmployeeB1ValidateBy",
                table: "EmailOrderStatus",
                column: "EmployeeB1ValidateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOrderStatus_Employee_EmployeeB1Id",
                table: "EmailOrderStatus",
                column: "EmployeeB1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmailOrderStatus_AspNetUsers_EmployeeB1ValidateBy",
                table: "EmailOrderStatus",
                column: "EmployeeB1ValidateBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrix_Employee_EmployeeB1Id",
                table: "Matrix",
                column: "EmployeeB1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Employee_EmployeeB1Id",
                table: "Order",
                column: "EmployeeB1Id",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailOrderStatus_Employee_EmployeeB1Id",
                table: "EmailOrderStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailOrderStatus_AspNetUsers_EmployeeB1ValidateBy",
                table: "EmailOrderStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrix_Employee_EmployeeB1Id",
                table: "Matrix");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Employee_EmployeeB1Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EmployeeB1Id",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Matrix_EmployeeB1Id",
                table: "Matrix");

            migrationBuilder.DropIndex(
                name: "IX_EmailOrderStatus_EmployeeB1Id",
                table: "EmailOrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_EmailOrderStatus_EmployeeB1ValidateBy",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeB1Id",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "EmployeeB1Id",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL1",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL2",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL3",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityL4",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityS1",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityS2",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "PriorityS3",
                table: "Matrix");

            migrationBuilder.DropColumn(
                name: "EmployeeB1EmailSend",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeB1EmailSkip",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeB1Id",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeB1ValidateAt",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "EmployeeB1ValidateBy",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "NotEmployeeB1Sync",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL1",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL2",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL3",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityL4",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityS1",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityS2",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "PriorityS3",
                table: "EmailOrderStatus");

            migrationBuilder.DropColumn(
                name: "SyncEmployeeB1ErrorCount",
                table: "EmailOrderStatus");
        }
    }
}
