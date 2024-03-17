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
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Document = MigraDoc.DocumentObjectModel.Document;

namespace Optima.Fais.Api.Services
{
	public class AppendixPVFinalGenerator : IAppendixPVFinalGenerator
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

		public AppendixPVFinalGenerator(
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

                var inventoryPlan = await this._inventoryPlansRepository.GetByFinalReportAsync();

                var members = await this._invCommitteeMembersRepository.GetPVGInvCommitteeMemberAsync();
                signatureEmployees = members.Select(m => new SigningEmployeeDetail()
                {
                    Employee = m.Employee,
                    Info = "Membru comisie",
                    InvCommitteePosition = m.InvCommitteePosition
                }).ToList();

                inventoryDateStart = inventoryPlan.DateStarted;
                inventoryDateEnd = inventoryPlan.DateFinished;

                inventory = await this._inventoryRepository.GetByIdAsync(inventoryId);
				var items = await this._assetRepository.GetLocationAuditInventoryAsync(inventoryId, reportFilter);

				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                Document document = new Document();

                document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1);
				document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);

				Section section = document.AddSection();
				section.PageSetup.TopMargin = Unit.FromCentimeter(2.5);
				section.PageSetup.PageFormat = PageFormat.A4;
				section.PageSetup.BottomMargin = Unit.FromCentimeter(2.0);

				AddHeaderLogo(section, this._resourcesPath);
				AddEPDocumentDetail(section, inventory, items, administration, department, 
                    division, costCenter, inventoryDateStart, inventoryDateEnd);

                /*
                Section signatureSection = document.AddSection();
                signatureSection.PageSetup.TopMargin = Unit.FromMillimeter(25);
                signatureSection.PageSetup.LeftMargin = Unit.FromMillimeter(20);
                signatureSection.PageSetup.PageFormat = PageFormat.A4;
                signatureSection.PageSetup.Orientation = Orientation.Portrait;
                signatureSection.PageSetup.HeaderDistance = Unit.FromMillimeter(10);
                signatureSection.PageSetup.BottomMargin = Unit.FromMillimeter(20);

                AddHeaderLogo(signatureSection, this._resourcesPath);
                */

                var participantAreaList = this._documentHelperService.AddSignatureAreaCommitteeMember(section, "SIGNATURE_AREA", signatureEmployees, false);

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
            rowA.Format.Font.Size = 14;
            rowA.Format.Font.Bold = true;
            var p18 = rowA.Cells[0].AddParagraph();
            p18.AddFormattedText(" \r\n PROCES VERBAL DE INVENTARIERE", TextFormat.Underline);
            p18.Format.Alignment = ParagraphAlignment.Center;

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 11;
            var p19 = rowA.Cells[0].AddParagraph();
            p19.AddFormattedText($" \r\n a mijloacelor fixe si a obiectelor de inventar conform Decizie anuala FY24/12 Iunie 2023 \r\n " +
                $"Intocmit azi {String.Format("{0:dd/MM/yyyy}", inventoryDateEnd)}", TextFormat.NotBold);

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
		

			string paragraphInfo = string.Empty;
			if (administration != null) paragraphInfo = administrationDetails;
			if (department != null) paragraphInfo = departmentDetails;
			if (division != null) paragraphInfo = divisionDetails;
			if (costCenter != null) paragraphInfo = costCenterDetails;

			p2.AddFormattedText(" \t\t ", TextFormat.NotBold);
            p2.AddFormattedText($"\tComisia de inventariere a procedat la inventarierea Mijloacelor Fixe si a Obiectelor de Inventar apartinand societatii " +
                $"{costCenterDetails} SC Dante International SA J40/372002 CUI: RO 1439984 Sos. Virtutii nr 148, sector 6, Bucuresti " +
                $"in conformitate cu Decizia de inventariere anuala din data de 12/06/2023", TextFormat.NotBold);
        
            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p3 = rowA.Cells[0].AddParagraph();
            p3.AddFormattedText($"\tInventarierea a inceput la data de {String.Format("{0:dd/MM/yyyy}", inventoryDateStart)}" +
                $" si s-a terminat la data de {String.Format("{0:dd/MM/yyyy}", inventoryDateEnd)}", TextFormat.Bold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p4 = rowA.Cells[0].AddParagraph();
            p4.AddFormattedText($"\tIn urma inventarierii, notam urmatoarele observatii:", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p5 = rowA.Cells[0].AddParagraph();
            p5.AddFormattedText($"1) Inventarierea s-a realizat cu aplicatia Optima:", TextFormat.NotBold);
            p5.AddFormattedText($"\r\n - prin scanare efectiva de responsabili/asset controller in locatii/showroom/depozite", TextFormat.NotBold);
            p5.AddFormattedText($"\r\n - prin inventarierea echipamentelor personale de lucru de natura IT (WFH) (laptop, monitor, telefon, tableta)", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p6 = rowA.Cells[0].AddParagraph();
            p6.AddFormattedText($"2) Aplicatia Optima a facilitat accesul si vizualizarea listelor de inventar, " +
                $"a declaratiilor si a formularelor specifice de inventar pentru responsabili.", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p7 = rowA.Cells[0].AddParagraph();
            p7.AddFormattedText($"3) Reconcilierea intre minusuri si plusuri s-a efectuat prin aplicatia Optima si prin " +
                $"verificari ale responsabililor intre elementele existente si cele scriptice la 30 Septembrie 2023. " +
                $"Diferentele aparute au fost verificate de catre responsabilul/asset controller din locatie/showroom/depozit , " +
                $"presedinte si un membru al  comisiei", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p8 = rowA.Cells[0].AddParagraph();
            p8.AddFormattedText($"4) Toate valorile materiale aflate in locatia/shoroom/depozit-ul inventariat au fost " +
                $"trecute in listele de inventar anexate, in mod corect fara omisiuni.", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p9 = rowA.Cells[0].AddParagraph();
            p9.AddFormattedText($"5) S-au propus liste de casari cu actiune si efect pana la finalul FY 24.", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p10 = rowA.Cells[0].AddParagraph();
            p10.AddFormattedText($"6) Conform FAR - Fixed Asset Register (En)/ Registrul de Mijloace Fixe (Ro), " +
                $"la 30 Septembrie 2023, am avut {items[0].Items} itemuri cu valoare neta contabila de " +
                String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueItems) +
                $" Ron si valoare de intrare de " + String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueIn) +
                $" Ron, supuse inventarierii.", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p11 = rowA.Cells[0].AddParagraph();
            p11.AddFormattedText($"7) In urma inventarierii s-a constatat un minus de inventar de " +
                $" itemuri, " + String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].Minus) +
                $" cu valoare neta contabila de " + String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueMinus) + 
                $" Ron si valoare de intrare de " + String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueInMinus) +
                $" Ron, reprezentand " + String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items[0].ValueMinus) +
                $" bunuri neidentificate fizic in locatii.", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p12 = rowA.Cells[0].AddParagraph();
            p12.AddFormattedText($"\tPrezentul Proces Verbal s-a intocmit astazi, {String.Format("{0:dd/MM/yyyy}", inventoryDateEnd)} si s-a semnat de catre " +
                $"toti membrii comisiei de inventariere.", TextFormat.NotBold);


            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "5cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 14;
            var p13 = rowA.Cells[0].AddParagraph();
            p13.AddFormattedText("COMISIA DE INVENTARIERE", TextFormat.Bold);
        }

        public static List<Model.EmployeeCostCenter> RemoveDuplicatesSet(List<Model.EmployeeCostCenter> items)
		{
			// Use HashSet to maintain table of duplicates encountered.
			var result = new List<Model.EmployeeCostCenter>();
			var set = new HashSet<Model.Employee>();
			for (int i = 0; i < items.Count; i++)
			{
				// If not duplicate, add to result.
				if (!set.Contains(items[i].Employee))
				{
					result.Add(items[i]);
					// Record as a future duplicate.
					set.Add(items[i].Employee);
				}
			}
			return result;
		}
	}
}
