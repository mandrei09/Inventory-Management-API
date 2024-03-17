using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class AddEmailStatusReminder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DstEmployeeReminder1At",
                table: "EmailStatus",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DstEmployeeReminder1EmailSend",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DstEmployeeReminder2At",
                table: "EmailStatus",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DstEmployeeReminder2EmailSend",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DstEmployeeReminder3At",
                table: "EmailStatus",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DstEmployeeReminder3EmailSend",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DstEmployeeReminder4At",
                table: "EmailStatus",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DstEmployeeReminder4EmailSend",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotDstEmployeeReminder1Sync",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotDstEmployeeReminder2Sync",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotDstEmployeeReminder3Sync",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotDstEmployeeReminder4Sync",
                table: "EmailStatus",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReminderDays",
                table: "EmailStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncDstEmployeeReminder1ErrorCount",
                table: "EmailStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncDstEmployeeReminder2ErrorCount",
                table: "EmailStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncDstEmployeeReminder3ErrorCount",
                table: "EmailStatus",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncDstEmployeeReminder4ErrorCount",
                table: "EmailStatus",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder1At",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder1EmailSend",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder2At",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder2EmailSend",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder3At",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder3EmailSend",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder4At",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "DstEmployeeReminder4EmailSend",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "NotDstEmployeeReminder1Sync",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "NotDstEmployeeReminder2Sync",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "NotDstEmployeeReminder3Sync",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "NotDstEmployeeReminder4Sync",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "ReminderDays",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "SyncDstEmployeeReminder1ErrorCount",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "SyncDstEmployeeReminder2ErrorCount",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "SyncDstEmployeeReminder3ErrorCount",
                table: "EmailStatus");

            migrationBuilder.DropColumn(
                name: "SyncDstEmployeeReminder4ErrorCount",
                table: "EmailStatus");
        }
    }
}
