using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using PdfSharp.Pdf.Filters;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Document = MigraDoc.DocumentObjectModel.Document;

namespace Optima.Fais.Api.Services
{
	public class AppendixPVGenerator : IAppendixPVGenerator
	{

		public IServiceProvider _services { get; }
		private readonly string _resourcesPath;
		private readonly string _basePath;
		private readonly string _resourcesFolder;
		private IAssetsRepository _assetRepository = null;
		private IInventoryRepository _inventoryRepository = null;
		private IAdministrationsRepository _administrationsRepository = null;
		private IDepartmentsRepository _departmentsRepository = null;
		private IDivisionsRepository _divisionsRepository = null;
		private ICostCentersRepository _costCentersRepository = null;
		private IEmployeeCostCentersRepository _employeeCostCentersRepository = null;
		private IEmployeesRepository _employeesRepository = null;
        private IDocumentHelperService _documentHelperService = null;
        private IInvCommitteeMembersRepository _invCommitteeMembersRepository = null;
        private IInventoryPlansRepository _inventoryPlansRepository = null;
        private readonly string _valueFormat = "{0:#,##0.##}";
		private readonly string _cultureInfo = "ro-RO";

		public AppendixPVGenerator(
            IServiceProvider services, 
            IConfiguration configuration, 
            IAssetsRepository assetRepository, 
            IInventoryRepository inventoryRepository, 
            ICostCentersRepository costCentersRepository,
			IAdministrationsRepository administrationsRepository, IDepartmentsRepository departmentsRepository,
			IDivisionsRepository divisionsRepository, IEmployeeCostCentersRepository employeeCostCentersRepository,
            IEmployeesRepository employeesRepository, IDocumentHelperService documentHelperService,
            IInvCommitteeMembersRepository invCommitteeMembersRepository, IInventoryPlansRepository inventoryPlansRepository)
		{
			_services = services;
            _assetRepository = assetRepository;
			this._inventoryRepository = inventoryRepository;
			this._costCentersRepository = costCentersRepository;
			this._administrationsRepository = administrationsRepository;
			this._departmentsRepository = departmentsRepository;
			this._divisionsRepository = divisionsRepository;
			this._employeeCostCentersRepository = employeeCostCentersRepository;
            this._employeesRepository = employeesRepository;
            this._documentHelperService = documentHelperService;
            this._invCommitteeMembersRepository = invCommitteeMembersRepository;
            this._inventoryPlansRepository = inventoryPlansRepository;
            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
			this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
			this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
		}

		public async Task<PdfDocumentResult> GenerateDocumentAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd)
		{
			using (var scope = _services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.Administration administration = null;
				Model.Department department = null;
				Model.Division division = null;
				Model.CostCenter costCenter = null;
				Model.Inventory inventory = null;

                List<SigningEmployeeDetail> signatureEmployees = new List<SigningEmployeeDetail>();

                int? administrationId = null;
                int? costCenterId = null;
                if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0)) administrationId = reportFilter.AdministrationIds[0];
                if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0)) costCenterId = reportFilter.CostCenterIds[0];

                var inventoryPlan = await this._inventoryPlansRepository.GetByAdministrationAndCostCenterAsync(administrationId, costCenterId);
                var members = await this._invCommitteeMembersRepository.GetInInvCommitteeAsync(inventoryPlan.InvCommitteeId);
                signatureEmployees = members.Select(m => new SigningEmployeeDetail()
                {
                    Employee = m.Employee,
                    Info = "Membru comisie",
                    InvCommitteePosition = m.InvCommitteePosition
                }).ToList();

                inventoryDateStart = inventoryPlan.DateStarted;
                inventoryDateEnd = inventoryPlan.DateFinished;

                inventory = await this._inventoryRepository.GetByIdAsync(inventoryId);
				var items = await this._assetRepository.GetCostCenterAuditInventoryAsync(inventoryId, reportFilter);

				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                MigraDoc.DocumentObjectModel.Document document = new MigraDoc.DocumentObjectModel.Document();

				document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1);
				document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);

				Section section = document.AddSection();
				section.PageSetup.TopMargin = Unit.FromCentimeter(2.5);
				section.PageSetup.PageFormat = PageFormat.A4;
				section.PageSetup.BottomMargin = Unit.FromCentimeter(2.0);

                if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0)) costCenterId = reportFilter.CostCenterIds[0];
                costCenter = this._costCentersRepository.GetById(costCenterId.GetValueOrDefault());
                if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0)) administrationId = reportFilter.AdministrationIds[0];
                administration = this._administrationsRepository.GetById(administrationId.GetValueOrDefault());

                AddHeaderLogo(section, this._resourcesPath);
				AddEPDocumentDetail(section, inventory, items, administration, department, 
                    division, costCenter, inventoryDateStart, inventoryDateEnd);
                AddEPFooter(section, document, true);

                Section signatureSection = document.AddSection();
                signatureSection.PageSetup.TopMargin = Unit.FromMillimeter(25);
                signatureSection.PageSetup.LeftMargin = Unit.FromMillimeter(20);
                signatureSection.PageSetup.PageFormat = PageFormat.A4;
                signatureSection.PageSetup.Orientation = Orientation.Portrait;
                signatureSection.PageSetup.HeaderDistance = Unit.FromMillimeter(10);
                signatureSection.PageSetup.BottomMargin = Unit.FromMillimeter(20);

                AddHeaderLogo(signatureSection, this._resourcesPath);

                var participantAreaList = this._documentHelperService.AddSignatureAreaCommitteeMember(signatureSection, "SIGNATURE_AREA", signatureEmployees, false);

				return new PdfDocumentResult
				{
					Document = document,
					DocumentNumber = 0,
				};
			}
		}

        private void AddHeaderLogo(Section section, string resourcesPath)
		{
			HeaderFooter header = section.Headers.Primary;

			string logoPath = resourcesPath + "logo.png";

			var tableLogo = header.AddTable();
			tableLogo.AddColumn("7cm");
			tableLogo.AddColumn("11cm");

			tableLogo.Borders.Visible = false;
			var rowLogo = tableLogo.AddRow();


			rowLogo.Borders.Visible = false;
			rowLogo.HeightRule = RowHeightRule.Exactly;
			rowLogo.Height = "1cm";
			var paragraph = rowLogo.Cells[0];
			paragraph.AddParagraph("SC Dante International SA\r\nJ40/372002\r\nCUI: RO 1439984\r\nSos. Virtutii nr 148, sector 6, Bucuresti");
			paragraph.Format.Alignment = ParagraphAlignment.Left;
			paragraph.Format.Font.Size = 8;
			paragraph.Format.Font.Bold = true;

			var paragraphRight = rowLogo.Cells[1];
			paragraphRight.Format.Alignment = ParagraphAlignment.Right;
			var image = rowLogo.Cells[1].AddParagraph().AddImage(logoPath);
			image.Height = "10mm";
		}

		private void AddEPDocumentDetail(Section section, Model.Inventory inventory, 
            List<Model.AuditInventoryResult> items, Model.Administration administration, 
            Model.Department department, Model.Division division, Model.CostCenter costCenter, 
            DateTime? inventoryDateStart, DateTime? inventoryDateEnd)
		{
            Table table = null;
            Table table1 = null;
            Table table2 = null;
            Row row = null;
			Column column = null;
			Paragraph paragraph = null;

            table1 = section.AddTable();
            table1.Borders.Visible = false;
            table1.AddColumn("18.00cm");

            table2 = section.AddTable();
            table2.Borders.Visible = true;
            table2.AddColumn("2.5cm");
            table2.AddColumn("1.0cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("2.0cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("2.0cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("1.5cm");

            table = section.AddTable();
            table.Borders.Visible = false;
            table.AddColumn("18.00cm");

            var rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "0.8cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 12;
            var p18 = rowA.Cells[0].AddParagraph();
            p18.AddFormattedText(" \r\n Proces verbal privind rezultate inventarierii patrimoniului ", TextFormat.Bold);
            p18.Format.Alignment = ParagraphAlignment.Center;

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 11;
            var p19 = rowA.Cells[0].AddParagraph();
            p19.Format.Font.Color = Colors.Green;
            p19.AddFormattedText($" \r\n {inventory.Description}", TextFormat.Bold);

            var rowC = table1.AddRow();
            rowC.HeightRule = RowHeightRule.Exactly;
            rowC.Height = "2.0cm";
            rowC.Borders.Visible = false;
            rowC.Format.Alignment = ParagraphAlignment.Left;
            rowC.VerticalAlignment = VerticalAlignment.Center;
            rowC.Format.Font.Size = 10;
            var p2 = rowC.Cells[0].AddParagraph();

			string administrationDetails = (administration != null ? administration.Name : string.Empty);
			string departmentDetails = (department != null ? department.Name : string.Empty);
			string divisionDetails = (division != null ? division.Name : string.Empty);
			string costCenterDetails = (costCenter != null ? costCenter.Code + " | " + costCenter.Name : string.Empty);

			administrationDetails = $"Locatia: {administrationDetails}";
			departmentDetails = $"B.U.: {departmentDetails}";
			divisionDetails = $"Departament: {divisionDetails}";
			costCenterDetails = $"Centru de cost: {costCenterDetails}";

            string paragraphInfo = string.Empty;
			if (administration != null) paragraphInfo = administrationDetails;
			if (department != null) paragraphInfo = departmentDetails;
			if (division != null) paragraphInfo = divisionDetails;
			if (costCenter != null) paragraphInfo = costCenterDetails;

			p2.AddFormattedText(" \t\t ", TextFormat.NotBold);
            p2.AddFormattedText($" Subsemnatul (a) . . . . ................................. .. .. ............................................... ., în calitate de responsabil, in gestiunea ", TextFormat.NotBold);
            var TextColorp2 =   p2.AddFormattedText("", TextFormat.NotBold);
            p2.AddFormattedText($"{paragraphInfo}, adresa . . . . . . . .. ...numit prin decizia nr . . . . . . . .. ..   am procedat la inventarierea fizica a mijloacelor fixe.", TextFormat.NotBold);
            TextColorp2.Font.Color = Colors.Green;

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p3 = rowA.Cells[0].AddParagraph();
            p3.AddFormattedText(" \r\n Inventarierea s-a realizat in perioada: ", TextFormat.NotBold);
            var TextColorp3 = p3.AddFormattedText($" {String.Format("{0:dd/MM/yyyy}", inventoryDateStart)} -  {String.Format("{0:dd/MM/yyyy}", inventoryDateEnd)}, ", TextFormat.Bold);
            TextColorp3.Font.Color = Colors.Green;
            p3.AddFormattedText(" rezultatele acesteia sunt:", TextFormat.NotBold);

            //----------table

            var rowE = table2.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "0.9cm"; ;
            rowE.Borders.Visible = true;
            rowE.Format.Alignment = ParagraphAlignment.Center;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 8;
            rowE.Format.Font.Bold = true;
            rowE.Shading.Color = Color.FromRgb(129, 195, 228);
            rowE.Cells[0].AddParagraph("Repere");
            rowE.Cells[0].MergeDown = 2;
            rowE.Cells[1].AddParagraph("Scriptic");
            rowE.Cells[1].MergeRight = 1;
            rowE.Cells[3].AddParagraph("Inventar");
            rowE.Cells[3].MergeRight = 1;
            rowE.Cells[5].AddParagraph("Intrari/Iesiri");
            rowE.Cells[5].MergeRight = 3;
            rowE.Cells[9].AddParagraph("Status");
            rowE.Cells[9].MergeRight = 1;

            rowE = table2.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = true;
            rowE.Format.Alignment = ParagraphAlignment.Center;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 8;
            rowE.Format.Font.Bold = true;
            rowE.Shading.Color = Color.FromRgb(129, 195, 228);
            rowE.Cells[0].AddParagraph("");
            rowE.Cells[1].AddParagraph("Numar");
            rowE.Cells[2].AddParagraph("Valoare");
            rowE.Cells[3].AddParagraph("Numar");
            rowE.Cells[4].AddParagraph("Valoare");
            rowE.Cells[5].AddParagraph("Iesiri");
            rowE.Cells[6].AddParagraph("Valoare iesiri");
            rowE.Cells[7].AddParagraph("Intrari");
            rowE.Cells[8].AddParagraph("Valoare intrari");
            rowE.Cells[9].AddParagraph("Numar\r\n(3 + 7)");
            rowE.Cells[10].AddParagraph("Valoare\r\n(4 + 8)");

            rowE = table2.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "0.5cm";
            rowE.Borders.Visible = true;
            rowE.Format.Alignment = ParagraphAlignment.Center;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 6;
            rowE.Format.Font.Bold = true;
            rowE.Shading.Color = Color.FromRgb(129, 195, 228);
            rowE.Cells[0].AddParagraph("");
            rowE.Cells[1].AddParagraph("1");
            rowE.Cells[2].AddParagraph("2");
            rowE.Cells[3].AddParagraph("3");
            rowE.Cells[4].AddParagraph("4");
            rowE.Cells[5].AddParagraph("5");
            rowE.Cells[6].AddParagraph("6");
            rowE.Cells[7].AddParagraph("7");
            rowE.Cells[8].AddParagraph("8");
            rowE.Cells[9].AddParagraph("9");
            rowE.Cells[10].AddParagraph("10");

            rowE = table2.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "0.7cm";
            rowE.Borders.Visible = true;
            rowE.Format.Alignment = ParagraphAlignment.Center;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 6;
            rowE.Format.Font.Bold = true;
            rowE.Format.Font.Color = Colors.Green;
            rowE.Cells[0].AddParagraph("Mijloace fixe");
            rowE.Cells[1].AddParagraph($"{items[0].Items}");
            rowE.Cells[2].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueItems));
            rowE.Cells[3].AddParagraph($"{items[0].Scanned}");
            rowE.Cells[4].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueScanned));
            rowE.Cells[5].AddParagraph($"{items[0].TransOut}");
            rowE.Cells[6].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueTransOut));
            rowE.Cells[7].AddParagraph($"{items[0].TransIn}");
            rowE.Cells[8].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueTransIn));
            rowE.Cells[9].AddParagraph($"{items[0].Scanned + items[0].TransIn}");
            rowE.Cells[10].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, (items[0].ValueScanned + items[0].ValueTransIn)));

            //-----------------

            rowE = table.AddRow();
          
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.9cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p4 = rowE.Cells[0].AddParagraph();

            p4.AddFormattedText(" Reperele neidentificate", TextFormat.Bold);
            p4.AddFormattedText(" in numar de", TextFormat.NotBold);
            var textColorp4 = p4.AddFormattedText($" {items[0].Minus}", TextFormat.Bold);
            textColorp4.Font.Color = Colors.Green;
            p4.AddFormattedText($" si cu o valoare contabila de ", TextFormat.NotBold);
            textColorp4 = p4.AddFormattedText(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueMinus), TextFormat.Bold);
            textColorp4.Font.Color = Colors.Green;
            p4.AddFormattedText(" RON", TextFormat.Bold);
            p4.AddFormattedText(" sunt prezentate in", TextFormat.NotBold);
            p4.AddFormattedText(" Anexa 1", TextFormat.Bold);
            p4.AddFormattedText(" prezentului proces-verbal. Cauzele lipsurilor constatate sunt:", TextFormat.NotBold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p5 = rowE.Cells[0].AddParagraph();
            p5.AddFormattedText(" 1 ", TextFormat.Bold);
            p5.AddFormattedText(" - Retrase dupa data de inventar: ", TextFormat.NotBold);
            var textColorp5 = p5.AddFormattedText(" 0 ", TextFormat.Bold);
            textColorp5.Font.Color = Colors.Green;
            p5.AddFormattedText(" repere in valoare de ", TextFormat.NotBold);
            textColorp5 = p5.AddFormattedText("0.00 ", TextFormat.Bold);
            textColorp5.Font.Color = Colors.Green;
            p5.AddFormattedText(" RON.", TextFormat.Bold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p6 = rowE.Cells[0].AddParagraph();
            p6.AddFormattedText(" 2 ", TextFormat.Bold);
            p6.AddFormattedText(" - Negasite - minus de inventar: ", TextFormat.NotBold);
            var textColorp6 = p6.AddFormattedText($" {items[0].Minus} ", TextFormat.Bold);
            textColorp6.Font.Color = Colors.Green;
            p6.AddFormattedText(" repere in valoare de ", TextFormat.NotBold);
            textColorp6 = p6.AddFormattedText(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueMinus), TextFormat.Bold);
            textColorp6.Font.Color = Colors.Green;
            p6.AddFormattedText(" RON.", TextFormat.Bold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p7 = rowE.Cells[0].AddParagraph();
            p7.AddFormattedText(" 3 ", TextFormat.Bold);
            p7.AddFormattedText(" - Pierdute: ", TextFormat.NotBold);
            var textColorp7 = p7.AddFormattedText(" 0 ", TextFormat.Bold);
            textColorp7.Font.Color = Colors.Green;
            p7.AddFormattedText(" repere in valoare de ", TextFormat.NotBold);
            textColorp7 = p7.AddFormattedText("0.00 ", TextFormat.Bold);
            textColorp7.Font.Color = Colors.Green;
            p7  .AddFormattedText(" RON.", TextFormat.Bold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p8 = rowE.Cells[0].AddParagraph();
            p8.AddFormattedText(" 4 ", TextFormat.Bold);
            p8.AddFormattedText(" - Furate: ", TextFormat.NotBold);
            var textColorp8 = p8.AddFormattedText(" 0 ", TextFormat.Bold);
            textColorp8.Font.Color = Colors.Green;
            p8.AddFormattedText(" repere in valoare de ", TextFormat.NotBold);
            textColorp8 = p8.AddFormattedText("0.00 ", TextFormat.Bold);
            textColorp8.Font.Color = Colors.Green;
            p8.AddFormattedText(" RON.", TextFormat.Bold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p11 = rowE.Cells[0].AddParagraph();
            p11.AddFormattedText("  Pentru reperele minus de inventar, pierdute sau furate, anexam ", TextFormat.NotBold);
            p11.AddFormattedText(" declaratiile utilizatorilor ", TextFormat.Bold);
            p11.AddFormattedText(" acestora, iar pentru cele fara utilizator anexam ", TextFormat.NotBold);
            p11.AddFormattedText(" detalii explicative ", TextFormat.Bold);
            p11.AddFormattedText(" ale comisiei de inventariere. ", TextFormat.NotBold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "1.0cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p12 = rowE.Cells[0].AddParagraph();
            p12.AddFormattedText("  Diferentele in plus identificate", TextFormat.Bold);
            p12.AddFormattedText(" in urma inventarierii sunt prezentate in", TextFormat.NotBold);
            p12.AddFormattedText(" Anexa 2 ", TextFormat.Bold);
            p12.AddFormattedText(" a prezentului proces-verbal. ", TextFormat.NotBold);

            rowE = table.AddRow();
            rowE.HeightRule = RowHeightRule.Exactly;
            rowE.Height = "0.5cm";
            rowE.Borders.Visible = false;
            rowE.Format.Alignment = ParagraphAlignment.Left;
            rowE.VerticalAlignment = VerticalAlignment.Center;
            rowE.Format.Font.Size = 10;
            var p13 = rowE.Cells[0].AddParagraph();
            p13.AddFormattedText("  Procesul verbal a fost intocmit in doua exemplare, din care un exemplar s-a trimis catre Departamentul Contabilitate.", TextFormat.NotBold);


            //rowE = table.AddRow();
            //rowE.HeightRule = RowHeightRule.Exactly;
            //rowE.Height = "2.0cm";
            //rowE.Borders.Visible = false;
            //rowE.Format.Alignment = ParagraphAlignment.Center;
            //rowE.VerticalAlignment = VerticalAlignment.Center;
            //rowE.Format.Font.Size = 12;
            //var p14 = rowE.Cells[0].AddParagraph();
            //p14.AddFormattedText("  Comisie Inventariere", TextFormat.Bold);

        }

        private void AddEPFooter(Section section, MigraDoc.DocumentObjectModel.Document document, bool resourcesPath)
        {
            string logoPath = resourcesPath + @"emag.png";

            HeaderFooter header = section.Headers.Primary;

            var tableLogo = header.AddTable();
            tableLogo.AddColumn("8cm");
            tableLogo.AddColumn("4cm");
            tableLogo.AddColumn("6cm");
            tableLogo.Borders.Visible = false;

            var rowLogo = tableLogo.AddRow();
            rowLogo.HeightRule = RowHeightRule.Exactly;
            rowLogo.Height = "0.8cm";
            var paragraph = rowLogo.Cells[0].AddParagraph("");

            //var image = rowLogo.Cells[0].AddParagraph().AddImage(logoPath);
            //rowLogo.Cells[0].MergeRight = 1;
            //rowLogo.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            //image.Height = "6mm";


            paragraph.Format.Alignment = ParagraphAlignment.Right;
            rowLogo.Cells[2].Format.Font.Color = Colors.Black;
            rowLogo.VerticalAlignment = VerticalAlignment.Center;
            paragraph.Format.Font.Size = 7;
            var pageNumber = section.Headers.Primary.AddParagraph();
            pageNumber.AddFormattedText("", TextFormat.Bold);
            pageNumber.Format.Alignment = ParagraphAlignment.Right;
            pageNumber.Format.Font.Size = 7;
            //pageNumber.AddPageField();
            //pageNumber.AddNumPagesField();
            DocumentRenderer docRenderer = new DocumentRenderer(document);
            docRenderer.PrepareDocument();
            int pageCount = docRenderer.FormattedDocument.PageCount;
            paragraph = rowLogo.Cells[2].AddParagraph();
            paragraph.AddFormattedText(" Pagina ");
            paragraph.AddPageField();
            paragraph.AddFormattedText(" din " + pageCount);
            paragraph.Format.Alignment = ParagraphAlignment.Right;
        }

	}
   
}
