using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Data.Migrations
{
    public partial class ChangePartnerLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocation_Address_AddressDetailId",
                table: "PartnerLocation");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerLocation_Company_CompanyId",
                table: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocation_AddressDetailId",
                table: "PartnerLocation");

            migrationBuilder.DropIndex(
                name: "IX_PartnerLocation_CompanyId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "AddressDetailId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "ERPId",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "ErpCode",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "FiscalCode",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "PayingAccount",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "RegistryNumber",
                table: "PartnerLocation");

            migrationBuilder.AddColumn<string>(
                name: "Act",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adresa",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodPostal",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cui",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataActualizareTvaInc",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataAnulareSplitTVA",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataInactivare",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataInceputSplitTVA",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataInceputTvaInc",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataPublicare",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataPublicareTvaInc",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataRadiere",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataReactivare",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataSfarsitTvaInc",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Data_anul_imp_ScpTVA",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Data_inceput_ScpTVA",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Data_sfarsit_ScpTVA",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Denumire",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mesaj_ScpTVA",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NrRegCom",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ScpTVA",
                table: "PartnerLocation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Stare_inregistrare",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StatusInactivi",
                table: "PartnerLocation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StatusRO_e_Factura",
                table: "PartnerLocation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StatusSplitTVA",
                table: "PartnerLocation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StatusTvaIncasare",
                table: "PartnerLocation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipActTvaInc",
                table: "PartnerLocation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Act",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Adresa",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "CodPostal",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Cui",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataActualizareTvaInc",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataAnulareSplitTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataInactivare",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataInceputSplitTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataInceputTvaInc",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataPublicare",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataPublicareTvaInc",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataRadiere",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataReactivare",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "DataSfarsitTvaInc",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Data_anul_imp_ScpTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Data_inceput_ScpTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Data_sfarsit_ScpTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Denumire",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Mesaj_ScpTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "NrRegCom",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "ScpTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Stare_inregistrare",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "StatusInactivi",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "StatusRO_e_Factura",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "StatusSplitTVA",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "StatusTvaIncasare",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "PartnerLocation");

            migrationBuilder.DropColumn(
                name: "TipActTvaInc",
                table: "PartnerLocation");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PartnerLocation",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressDetailId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "PartnerLocation",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "PartnerLocation",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "PartnerLocation",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ERPId",
                table: "PartnerLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErpCode",
                table: "PartnerLocation",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalCode",
                table: "PartnerLocation",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PartnerLocation",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayingAccount",
                table: "PartnerLocation",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistryNumber",
                table: "PartnerLocation",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocation_AddressDetailId",
                table: "PartnerLocation",
                column: "AddressDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerLocation_CompanyId",
                table: "PartnerLocation",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocation_Address_AddressDetailId",
                table: "PartnerLocation",
                column: "AddressDetailId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerLocation_Company_CompanyId",
                table: "PartnerLocation",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
