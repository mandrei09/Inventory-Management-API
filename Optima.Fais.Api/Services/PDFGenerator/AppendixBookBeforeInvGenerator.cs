﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Optima.Fais.Data;
using Optima.Fais.EfRepository;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class AppendixBookBeforeInvGenerator : IAppendixBookBeforeInvGenerator
    {

		public IServiceProvider _services { get; }
		private readonly string _resourcesPath;
		private readonly string _basePath;
		private readonly string _resourcesFolder;
		private IAdministrationsRepository _administrationsRepository = null;
		private IDepartmentsRepository _departmentsRepository = null;
		private IDivisionsRepository _divisionsRepository = null;
		private ICostCentersRepository _costCentersRepository = null;
		private IDocumentHelperService _documentHelperService = null;
		private IEmployeeCostCentersRepository _employeeCostCentersRepository = null;
        private IEmployeesRepository _employeesRepository = null;
        private IInvCommitteeMembersRepository _invCommitteeMembersRepository = null;
        private IInventoryPlansRepository _inventoryPlansRepository = null;
        private IDocumentHelperService documentHelperService = null;
        public AppendixBookBeforeInvGenerator(IServiceProvider services, IConfiguration configuration, 
            ICostCentersRepository costCentersRepository,IAdministrationsRepository administrationsRepository, 
            IDepartmentsRepository departmentsRepository, IDivisionsRepository divisionsRepository, 
            IEmployeeCostCentersRepository employeeCostCentersRepository, IEmployeesRepository employeesRepository,
            IInvCommitteeMembersRepository invCommitteeMembersRepository, IInventoryPlansRepository inventoryPlansRepository,
            IDocumentHelperService documentHelperService)
		{
			_services = services;
			this._costCentersRepository = costCentersRepository;
			this._administrationsRepository = administrationsRepository;
			this._departmentsRepository = departmentsRepository;
			this._divisionsRepository = divisionsRepository;
			this._employeeCostCentersRepository = employeeCostCentersRepository;
            this._employeesRepository = employeesRepository;
            this._invCommitteeMembersRepository = invCommitteeMembersRepository;
            this._inventoryPlansRepository = inventoryPlansRepository;
            this._documentHelperService = documentHelperService;

            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
			this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
			this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
		}

		public async Task<PdfDocumentResult> GenerateDocumentAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart)
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
				List<Model.EmployeeCostCenter> employeeCostCenters = new List<Model.EmployeeCostCenter>();
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

                //employeeCostCenters = await _employeeCostCentersRepository.GetAllByCostCenter(reportFilter);
                //employeeCostCenters = RemoveDuplicatesSet(employeeCostCenters);

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				Document document = new Document();
				document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1);
				document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);

				Section section = document.AddSection();
				section.PageSetup.TopMargin = Unit.FromCentimeter(2.5);
				section.PageSetup.PageFormat = PageFormat.A4;
				section.PageSetup.BottomMargin = Unit.FromCentimeter(2.0);
				AddHeaderLogo(section, this._resourcesPath);

				AddEPDocumentDetail(section, administration, department, division, costCenter, employeeCostCenters, reportFilter, inventoryDateStart);
				AddEPFooter(section, document, false);

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

		private void AddEPDocumentDetail(Section section, Model.Administration administration, 
            Model.Department department, Model.Division division, Model.CostCenter costCenter, 
            List<Model.EmployeeCostCenter> employeeCostCenters, ReportFilter reportFilter, DateTime? inventoryDateStart)
        {
            Table table = null;
            Table table1 = null;
            Table table2 = null;
            Row row = null;
			Column column = null;
			Paragraph paragraph = null;
           
            table1 = section.AddTable();
            table1.Borders.Visible = true;
            table1.AddColumn("18.0cm");

            table2 = section.AddTable();
            table2.Borders.Visible = false;
            table2.AddColumn("0.5cm");
            table2.AddColumn("1.0cm");
            table2.AddColumn("4.0cm");
            table2.AddColumn("2.0cm");
            table2.AddColumn("2.0cm");
            table2.AddColumn("1.5cm");
            table2.AddColumn("2.5cm");
            table2.AddColumn("3.0cm");
            table2.AddColumn("1.5cm");


            var rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "2.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "0.8cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 11;
            var p18 = rowA.Cells[0].AddParagraph();
            p18.AddFormattedText(" \r\n DECLARAȚIA ", TextFormat.Bold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "0.8cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 11;
            var p19 = rowA.Cells[0].AddParagraph();
            p19.AddFormattedText(" \r\n responsabilului de locatie/showroom/depozit ", TextFormat.Bold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "0.8cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Center;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 11;
            var p20 = rowA.Cells[0].AddParagraph();
            p20.AddFormattedText(" \r\n inainte de INVENTARIERE ", TextFormat.Bold);

            var rowB = table1.AddRow();
            rowB.HeightRule = RowHeightRule.Exactly;
            rowB.Height = "3.0cm";
            rowB.Borders.Visible = false;
            rowB.Format.Alignment = ParagraphAlignment.Left;
            rowB.VerticalAlignment = VerticalAlignment.Center;
            rowB.Format.Font.Size = 10;
            var p1 = rowB.Cells[0].AddParagraph();
            p1.AddFormattedText(" \t\t ", TextFormat.NotBold);

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

			p1.AddFormattedText($" \n\r\tSubsemnatul (a) . . . . ................................. .. .. ............................................... ., în calitate de responsabil, răspunzător de locatia/showroom/ \r depozit {paragraphInfo}, numit prin decizia nr . . . . . . . .. ., declar pe  propria răspundere că: ", TextFormat.NotBold);


            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.2cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p2 = rowA.Cells[0].AddParagraph();
            p2.AddFormattedText("\r\n 1. Toate valorile materiale și bănești aflate în  locatia/showroom/depozit se găsesc în încăperile (locurile) . . . . .      .........  . ... . . . . . și în alte locuri de depozitare . . . . . .      ............. .....  . . . . . .", TextFormat.NotBold);


            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p3 = rowA.Cells[0].AddParagraph();
            p3.AddFormattedText(" \r\n 2. Posed (nu posed) în afara valorilor materiale ale societatii Dante International SA și alte materiale apartinanad terților, primite cu sau fără documente . . . . . . . . . . . .", TextFormat.NotBold);


            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p4 = rowA.Cells[0].AddParagraph();
            p4.AddFormattedText(" \r\n 3. Am (nu am) cunoștință de existența unor plusuri/minusuri în valoare (cantitate) de . . . . . . . . . . ", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p5 = rowA.Cells[0].AddParagraph();
            p5.AddFormattedText(" \r\n 4. Am (nu am) valori materiale nerecepționate sau care trebuie expediate (livrate), pentru care s-au întocmit documentele aferente în cantitate/valoare de . . . . . . . . . . . .", TextFormat.NotBold);


            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p6 = rowA.Cells[0].AddParagraph();
            p6.AddFormattedText(" \r\n 5. Am (nu am) eliberat valori materiale sau bănești fără documente legale . . . . . . . . . . . .", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p7 = rowA.Cells[0].AddParagraph();
            p7.AddFormattedText(" \r\n  6. Dețin (nu dețin) numerar sau alte hârtii de valoare rezultate din vânzarea bunurilor  . . . . . . .. . . .. . . . .", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p8 = rowA.Cells[0].AddParagraph();
            p8.AddFormattedText(" \r\n 7. Am (nu am) documente de primire care nu au fost operate în evidența locatiei/showroom-ului/depozit-ului sau care nu au fost predate la contabilitate . . . . . . . . . . . .", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p9 = rowA.Cells[0].AddParagraph();
            p9.AddFormattedText(" \r\n  8. Dețin (nu dețin) numerar din vânzarea mărfurilor aflate în locatia/showroom/depozit, de  . . . . . . lei.", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p10 = rowA.Cells[0].AddParagraph();
            p10.AddFormattedText(" \r\n 9. Ultimul document de intrare este: ", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p11 = rowA.Cells[0].AddParagraph();
            p11.AddFormattedText(" \r\n fel document . . . . . . nr . . . …. . . din . . . . . . ….", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p12 = rowA.Cells[0].AddParagraph();
            p12.AddFormattedText(" \r\n  10. Ultimul document de ieșire este: ", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p13 = rowA.Cells[0].AddParagraph();
            p13.AddFormattedText(" \r\n  fel document . . . . . . nr . . . …. . . din . . . . . . …. ", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p14 = rowA.Cells[0].AddParagraph();
            p14.AddFormattedText(" \r\n 11. Alte mențiuni . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "2.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p15 = rowA.Cells[0].AddParagraph();
            p15.AddFormattedText($" \r\n Data: {String.Format("{0:dd/MM/yyyy}", inventoryDateStart)}", TextFormat.NotBold);

            rowA = table1.AddRow();
            rowA.HeightRule = RowHeightRule.Exactly;
            rowA.Height = "1.0cm";
            rowA.Borders.Visible = false;
            rowA.Format.Alignment = ParagraphAlignment.Left;
            rowA.VerticalAlignment = VerticalAlignment.Center;
            rowA.Format.Font.Size = 10;
            var p16 = rowA.Cells[0].AddParagraph();
            p16.AddFormattedText(" \r\n Responsabil locatie/showroom/depozit: ", TextFormat.NotBold);

            var rowR = table1.AddRow();
            rowR.HeightRule = RowHeightRule.Exactly;
            rowR.Height = "0.6cm";
            rowR.Borders.Visible = false;
            rowR.Format.Alignment = ParagraphAlignment.Left;
            rowR.VerticalAlignment = VerticalAlignment.Center;
            rowR.Format.Font.Size = 10;
            var p17 = rowR.Cells[0].AddParagraph();
            p17.AddFormattedText(" \r\n  . . . . . . . . . . . . . . .  ", TextFormat.NotBold);

        }


        private void AddEPFooter(Section section, Document document, bool resourcesPath)
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
		
			paragraph.Format.Alignment = ParagraphAlignment.Right;
			rowLogo.Cells[2].Format.Font.Color = Colors.Black;
			rowLogo.VerticalAlignment = VerticalAlignment.Center;
			paragraph.Format.Font.Size = 7;
			var pageNumber = section.Headers.Primary.AddParagraph();
			pageNumber.AddFormattedText("", TextFormat.Bold);
			pageNumber.Format.Alignment = ParagraphAlignment.Right;
			pageNumber.Format.Font.Size = 7;
			DocumentRenderer docRenderer = new DocumentRenderer(document);
			docRenderer.PrepareDocument();
			int pageCount = docRenderer.FormattedDocument.PageCount;
			paragraph = rowLogo.Cells[2].AddParagraph();
			paragraph.AddFormattedText(" Pagina ");
			paragraph.AddPageField();
			paragraph.AddFormattedText(" din " + pageCount);
			paragraph.Format.Alignment = ParagraphAlignment.Right;
		}

		//public static List<Model.EmployeeCostCenter> RemoveDuplicatesSet(List<Model.EmployeeCostCenter> items)
		//{
		//	// Use HashSet to maintain table of duplicates encountered.
		//	var result = new List<Model.EmployeeCostCenter>();
		//	var set = new HashSet<Model.Employee>();
		//	for (int i = 0; i < items.Count; i++)
		//	{
		//		// If not duplicate, add to result.
		//		if (!set.Contains(items[i].Employee))
		//		{
		//			result.Add(items[i]);
		//			// Record as a future duplicate.
		//			set.Add(items[i].Employee);
		//		}
		//	}
		//	return result;
		//}
	}
}
