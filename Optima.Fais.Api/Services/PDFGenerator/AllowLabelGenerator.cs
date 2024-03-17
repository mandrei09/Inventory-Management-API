using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using Optima.Faia.Api.Services;
using Optima.Fais.Model.Reporting;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Optima.Fais.Repository.Inventory;
using Org.BouncyCastle.Asn1.Pkcs;
using PdfSharp.Drawing;
using PdfSharp.Drawing.BarCodes;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class AllowLabelGenerator : IAllowLabelGenerator
	{
        private IInventoryRepository _inventoryRepository = null;
		private IAdministrationsRepository _administrationsRepository = null;
		private IDepartmentsRepository _departmentsRepository = null;
		private IDivisionsRepository _divisionsRepository = null;
		private ICostCentersRepository _costCentersRepository = null;
		private IAssetsRepository _assetRepository = null;
		private IEmployeeCostCentersRepository _employeeCostCentersRepository = null;
        private IEmployeesRepository _employeesRepository = null;
        private IDocumentHelperService _documentHelperService = null;
        private IInvCommitteeMembersRepository _invCommitteeMembersRepository = null;
        private IInventoryPlansRepository _inventoryPlansRepository = null;

		private readonly string _basePath;
        private readonly string _resourcesFolder;
        private readonly string _resourcesPath;
        private readonly string _valueFormat = "{0:#,##0.##}";
        private readonly string _cultureInfo = "ro-RO";

        public AllowLabelGenerator(IConfiguration configuration, IInventoryRepository inventoryRepository,
            ICostCentersRepository costCentersRepository, IDocumentHelperService documentHelperService,
            IAssetsRepository assetRepository, IEmployeeCostCentersRepository employeeCostCentersRepository,
			IAdministrationsRepository administrationsRepository, IDepartmentsRepository departmentsRepository,
			IDivisionsRepository divisionsRepository, IEmployeesRepository employeesRepository,
            IInvCommitteeMembersRepository invCommitteeMembersRepository, IInventoryPlansRepository inventoryPlansRepository
			)
        {
            this._inventoryRepository = inventoryRepository;
			this._costCentersRepository = costCentersRepository;
			this._administrationsRepository = administrationsRepository;
			this._departmentsRepository = departmentsRepository;
			this._divisionsRepository = divisionsRepository;
			this._assetRepository = assetRepository;
            this._employeesRepository = employeesRepository;
            this._invCommitteeMembersRepository = invCommitteeMembersRepository;
            this._inventoryPlansRepository = inventoryPlansRepository;
			_documentHelperService = documentHelperService;

            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
            this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
            this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
			this._employeeCostCentersRepository = employeeCostCentersRepository;
		}

        public async Task<PdfDocumentResult> GenerateDocumentAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd)
        {
			Model.Administration administration = null;
			Model.Department department = null;
			Model.Division division = null;
			Model.CostCenter costCenter = null;
			Model.Inventory inventory = null;

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

            inventoryDateEnd = inventoryPlan.DateFinished;

            inventory = await this._inventoryRepository.GetByIdAsync(inventoryId);
                var items = await this._assetRepository.GetInventoryListAllowLabelAsync(inventoryId, reportFilter);

            //employeeCostCenters = await _employeeCostCentersRepository.GetAllByCostCenter(reportFilter);
			//employeeCostCenters = RemoveDuplicatesSet(employeeCostCenters);

			if (items.Count == 0) return null;

            decimal totalValue = 0;

            totalValue = items.Sum(a => a.CurrBkValue);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Document document = new Document();

            document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1);
            document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);

            Section section = document.AddSection();
            section.PageSetup.TopMargin = Unit.FromMillimeter(80);
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.Orientation = Orientation.Landscape;
            section.PageSetup.HeaderDistance = Unit.FromMillimeter(10);
            section.PageSetup.BottomMargin = Unit.FromMillimeter(20);

            AddHeaderLogo(section, this._resourcesPath);
            AddHeader(section, administration, department, division, costCenter, reportFilter, inventoryDateEnd, totalValue);

            AddDocumentDetail(section, items, reportFilter);

            int committeeIndex = 1;
            var table = AddEPMainCommitteeHeader(section, committeeIndex);

            AddEPFooter(section, document, false);

            Section signatureSection = document.AddSection();
            signatureSection.PageSetup.TopMargin = Unit.FromMillimeter(60);
            signatureSection.PageSetup.LeftMargin = Unit.FromMillimeter(20);
            signatureSection.PageSetup.PageFormat = PageFormat.A4;
            signatureSection.PageSetup.Orientation = Orientation.Landscape;
            signatureSection.PageSetup.HeaderDistance = Unit.FromMillimeter(10);
            signatureSection.PageSetup.BottomMargin = Unit.FromMillimeter(20);

            AddHeaderLogo(signatureSection, this._resourcesPath);
            AddHeader(signatureSection, administration,department, division, costCenter, reportFilter, inventoryDateEnd, totalValue, false);

            var participantAreaList = this._documentHelperService.AddSignatureAreaCommitteeMember(signatureSection, "SIGNATURE_AREA", signatureEmployees, true);

            return new PdfDocumentResult
            {
                Document = document,
                DocumentNumber = 0,
                Participants = participantAreaList,
            };
        }

		private void AddHeaderLogo(Section section, string resourcesPath)
        {
            HeaderFooter header = section.Headers.Primary;

            string logoPath = resourcesPath + "logo.png";

            var tableLogo = header.AddTable();
            tableLogo.AddColumn("7cm");
            tableLogo.AddColumn("20cm");

            tableLogo.Borders.Visible = false;
            var rowLogo = tableLogo.AddRow();


            rowLogo.Borders.Visible = false;
            rowLogo.HeightRule = RowHeightRule.Exactly;
            rowLogo.Height = "1cm";
            var paragraph = rowLogo.Cells[0];
            paragraph.AddParagraph("");
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.Format.Font.Size = 8;
            paragraph.Format.Font.Bold = true;

            var paragraphRight = rowLogo.Cells[1];
            paragraphRight.Format.Alignment = ParagraphAlignment.Right;
            var image = rowLogo.Cells[1].AddParagraph().AddImage(logoPath);
            image.Height = "10mm";
        }

        private void AddHeader(Section section, Model.Administration administration, Model.Department department, 
            Model.Division division, Model.CostCenter costCenter, ReportFilter reportFilter, DateTime? inventoryDateEnd, decimal totalValue, bool complete = true)
        {
            HeaderFooter header = section.Headers.Primary;

            var tableLogo = header.AddTable();
            tableLogo.AddColumn("1cm"); // Nr. Crt
            tableLogo.AddColumn("4cm"); // Denumire
            tableLogo.AddColumn("2.0cm"); // Unitate Economica
            tableLogo.AddColumn("2.0cm"); // Nr. SAP
            tableLogo.AddColumn("1cm"); // SubNo
            tableLogo.AddColumn("1cm"); // UM
            tableLogo.AddColumn("1cm"); // Faptic
            tableLogo.AddColumn("1.1cm"); // Scriptic
            tableLogo.AddColumn("1cm"); // Plus
            tableLogo.AddColumn("1cm"); // Minus
            tableLogo.AddColumn("2cm"); // Pret Unitar
            tableLogo.AddColumn("2cm"); // Valoarea
            tableLogo.AddColumn("1.5cm"); // Plus
            tableLogo.AddColumn("1.5cm"); // Minus
            tableLogo.AddColumn("2.0cm"); // Valoare inventar
            tableLogo.AddColumn("1.1cm"); // Valoare
            tableLogo.AddColumn("1.9cm"); // Motiv

            tableLogo.Borders.Visible = true;
            // tableLogo.Borders.Width = 1;
            // tableLogo.Format.LeftIndent = Unit.FromCentimeter(1);
            var row1 = tableLogo.AddRow();
            row1.Borders.Visible = true;
            row1.HeightRule = RowHeightRule.Exactly;
            row1.Height = Unit.FromMillimeter(20);
            row1.Cells[0].MergeRight = 1;

            var paragraph1 = row1.Cells[0];
            paragraph1.AddParagraph("SC Dante International SA\r\nJ40/372002\r\nCUI: RO 1439984\r\nSos. Virtutii nr 148, sector 6, Bucuresti");
            paragraph1.Format.Alignment = ParagraphAlignment.Left;
            paragraph1.Format.Font.Size = 8;
            paragraph1.Format.Font.Bold = true;

            row1.Cells[2].AddParagraph($"REGISTRU NEETICHETABILE\r\n\r\n\r\nData: {String.Format("{0:dd.MM.yyyy}", inventoryDateEnd)}");
            row1.Cells[2].MergeRight = 8;
            row1.Cells[2].MergeDown = 1;
            row1.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            row1.Cells[2].Format.Font.Size = 10;
            row1.Cells[2].Format.Font.Bold = true;

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
			paragraphInfo += $"\r\n Total NBV: {String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, totalValue)}";

            row1.Cells[11].AddParagraph(paragraphInfo);
            row1.Cells[11].MergeRight = 4;
            row1.Cells[11].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[11].VerticalAlignment = VerticalAlignment.Center;
            row1.Cells[11].Format.Font.Size = 8;
            row1.Cells[11].Format.Font.Bold = true;

            row1.Cells[16].AddParagraph("Pagina\r\n").AddPageField();
            row1.Cells[16].AddParagraph("din ").AddNumPagesField();
            row1.Cells[16].MergeDown = 1;
            row1.Cells[16].Format.Alignment = ParagraphAlignment.Center;
            row1.Cells[16].VerticalAlignment = VerticalAlignment.Center;
            row1.Cells[16].Format.Font.Size = 8;
            row1.Cells[16].Format.Font.Bold = true;


            int? costCenterId = null;
            int? administrationId = null;
            if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0)) costCenterId = reportFilter.CostCenterIds[0];
            costCenter = this._costCentersRepository.GetById(costCenterId.GetValueOrDefault());
            if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0)) administrationId = reportFilter.AdministrationIds[0];
            administration = this._administrationsRepository.GetById(administrationId.GetValueOrDefault());
            string costCenterName = (costCenter != null ? costCenter.Code + " | " + costCenter.Name : string.Empty);
            string administrationName = (administration != null ? administration.Code + " | " + administration.Name : string.Empty);

            var row2 = tableLogo.AddRow();
            row2.Borders.Visible = true;
            row2.HeightRule = RowHeightRule.Exactly;
            row2.Height = Unit.FromMillimeter(12);
            row2.Cells[0].MergeRight = 1;
            var paragraph2 = row2.Cells[0];
            if (!string.IsNullOrEmpty(costCenterName)) paragraph2.AddParagraph($"Centru de cost: {costCenterName}");
            else paragraph2.AddParagraph($"Locatia: {administrationName}");
            paragraph2.Format.Alignment = ParagraphAlignment.Left;
            paragraph2.Format.Font.Size = 8;
            paragraph2.Format.Font.Bold = true;

            paragraphInfo = "";
            row2.Cells[11].AddParagraph(paragraphInfo);
            row2.Cells[11].MergeRight = 4;
            row2.Cells[11].Format.Alignment = ParagraphAlignment.Center;
            row2.Cells[11].VerticalAlignment = VerticalAlignment.Center;
            row2.Cells[11].Format.Font.Size = 8;
            row2.Cells[11].Format.Font.Bold = true;

            if (!complete) return;

            var row3 = tableLogo.AddRow();
            row3.Borders.Visible = true;
            row3.HeightRule = RowHeightRule.Exactly;
            row3.Height = "0.5cm";
            var paragraph3 = row3.Cells[0];
            paragraph3.AddParagraph("Nr Crt.");
            paragraph3.MergeDown = 2;
            paragraph3.Format.Alignment = ParagraphAlignment.Center;
            paragraph3.VerticalAlignment = VerticalAlignment.Center;
            paragraph3.Format.Font.Size = 8;
            paragraph3.Format.Font.Bold = true;

            row3.Cells[1].AddParagraph("Denumirea bunurilor inventariate");
            row3.Cells[1].MergeDown = 2;
            row3.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[1].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[1].Format.Font.Size = 8;
            row3.Cells[1].Format.Font.Bold = true;

            row3.Cells[2].AddParagraph("Codul / Nr. de inventar");
            row3.Cells[2].MergeRight = 2;
            row3.Cells[2].MergeDown = 1;
            row3.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[2].Format.Font.Size = 8;
            row3.Cells[2].Format.Font.Bold = true;

            row3.Cells[5].AddParagraph("UM");
            row3.Cells[5].MergeDown = 2;
            row3.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[5].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[5].Format.Font.Size = 8;
            row3.Cells[5].Format.Font.Bold = true;

            row3.Cells[6].AddParagraph("CANTITATI");
            row3.Cells[6].MergeRight = 3;
            row3.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[6].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[6].Format.Font.Size = 8;
            row3.Cells[6].Format.Font.Bold = true;

            row3.Cells[10].AddParagraph("PRET\r\nUNIT.");
            row3.Cells[10].MergeDown = 2;
            row3.Cells[10].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[10].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[10].Format.Font.Size = 8;
            row3.Cells[10].Format.Font.Bold = true;

            row3.Cells[11].AddParagraph("VALOAREA CONTABILA");
            row3.Cells[11].MergeRight = 2;
            row3.Cells[11].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[11].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[11].Format.Font.Size = 8;
            row3.Cells[11].Format.Font.Bold = true;

            row3.Cells[14].AddParagraph("Valoare\r\ninventar");
            row3.Cells[14].MergeDown = 2;
            row3.Cells[14].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[14].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[14].Format.Font.Size = 8;
            row3.Cells[14].Format.Font.Bold = true;

            row3.Cells[15].AddParagraph("DEPRECIEREA");
            row3.Cells[15].MergeRight = 1;
            row3.Cells[15].Format.Alignment = ParagraphAlignment.Center;
            row3.Cells[15].VerticalAlignment = VerticalAlignment.Center;
            row3.Cells[15].Format.Font.Size = 8;
            row3.Cells[15].Format.Font.Bold = true;


            var row4 = tableLogo.AddRow();
            row4.Borders.Visible = true;
            row4.HeightRule = RowHeightRule.Exactly;
            row4.Height = "0.6cm";
            var paragraph4 = row4.Cells[6];
            paragraph4.AddParagraph("Stocuri");
            paragraph4.MergeRight = 1;
            paragraph4.Format.Alignment = ParagraphAlignment.Center;
            paragraph4.VerticalAlignment = VerticalAlignment.Center;
            paragraph4.Format.Font.Size = 8;
            paragraph4.Format.Font.Bold = true;

            row4.Cells[8].AddParagraph("Diferente");
            row4.Cells[8].MergeRight = 1;
            row4.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[8].VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[8].Format.Font.Size = 8;
            row4.Cells[8].Format.Font.Bold = true;

            row4.Cells[11].AddParagraph("Valoarea");
            row4.Cells[11].MergeDown = 1;
            row4.Cells[11].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[11].VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[11].Format.Font.Size = 8;
            row4.Cells[11].Format.Font.Bold = true;

            row4.Cells[12].AddParagraph("Diferente");
            row4.Cells[12].MergeRight = 1;
            row4.Cells[12].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[12].VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[12].Format.Font.Size = 8;
            row4.Cells[12].Format.Font.Bold = true;

            row4.Cells[15].AddParagraph("Valoare");
            row4.Cells[15].MergeDown = 1;
            row4.Cells[15].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[15].VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[15].Format.Font.Size = 8;
            row4.Cells[15].Format.Font.Bold = true;

            row4.Cells[16].AddParagraph("Motiv\r\n(Cod)");
            row4.Cells[16].MergeDown = 1;
            row4.Cells[16].Format.Alignment = ParagraphAlignment.Center;
            row4.Cells[16].VerticalAlignment = VerticalAlignment.Center;
            row4.Cells[16].Format.Font.Size = 8;
            row4.Cells[16].Format.Font.Bold = true;


            var row5 = tableLogo.AddRow();
            row5.Borders.Visible = true;
            row5.HeightRule = RowHeightRule.Exactly;
            row5.Height = "1cm";

            row5.Cells[2].AddParagraph("Nr. SAP");
            row5.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[2].Format.Font.Size = 8;
            row5.Cells[2].Format.Font.Bold = true;
			row5.Cells[2].MergeRight = 1;

			//row5.Cells[3].AddParagraph("Nr. SAP");
   //         row5.Cells[3].Format.Alignment = ParagraphAlignment.Center;
   //         row5.Cells[3].VerticalAlignment = VerticalAlignment.Center;
   //         row5.Cells[3].Format.Font.Size = 8;
   //         row5.Cells[3].Format.Font.Bold = true;

            row5.Cells[4].AddParagraph("SubNr.\r\nSAP");
            row5.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[4].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[4].Format.Font.Size = 8;
            row5.Cells[4].Format.Font.Bold = true;

            row5.Cells[6].AddParagraph("Faptic");
            row5.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[6].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[6].Format.Font.Size = 8;
            row5.Cells[6].Format.Font.Bold = true;

            row5.Cells[7].AddParagraph("Scriptic");
            row5.Cells[7].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[7].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[7].Format.Font.Size = 8;
            row5.Cells[7].Format.Font.Bold = true;

            row5.Cells[8].AddParagraph("Plus");
            row5.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[8].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[8].Format.Font.Size = 8;
            row5.Cells[8].Format.Font.Bold = true;

            row5.Cells[9].AddParagraph("Minus");
            row5.Cells[9].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[9].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[9].Format.Font.Size = 8;
            row5.Cells[9].Format.Font.Bold = true;

            row5.Cells[12].AddParagraph("Plus");
            row5.Cells[12].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[12].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[12].Format.Font.Size = 8;
            row5.Cells[12].Format.Font.Bold = true;

            row5.Cells[13].AddParagraph("Minus");
            row5.Cells[13].Format.Alignment = ParagraphAlignment.Center;
            row5.Cells[13].VerticalAlignment = VerticalAlignment.Center;
            row5.Cells[13].Format.Font.Size = 8;
            row5.Cells[13].Format.Font.Bold = true;

            var row6 = tableLogo.AddRow();
            row6.Borders.Visible = true;
            // row6.Borders.Width = 1;
            row6.HeightRule = RowHeightRule.Exactly;
            row6.Height = "0.6cm";
            row6.Cells[0].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[0].Format.Font.Color = Colors.White;
            row6.Cells[0].Format.Font.Bold = true;

            row6.Cells[0].AddParagraph("0");
            row6.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[0].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[0].Format.Font.Color = Colors.White;
            row6.Cells[0].Format.Font.Bold = true;
            row6.Cells[0].Format.Font.Size = 8;

            row6.Cells[1].AddParagraph("1");
            row6.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[1].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[1].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[1].Format.Font.Color = Colors.White;
            row6.Cells[1].Format.Font.Bold = true;
            row6.Cells[1].Format.Font.Size = 8;


            row6.Cells[2].AddParagraph("2");
            row6.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[2].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[2].Format.Font.Color = Colors.White;
            row6.Cells[2].Format.Font.Bold = true;
            row6.Cells[2].Format.Font.Size = 8;
			row6.Cells[2].MergeRight = 1;

			//row6.Cells[3].AddParagraph("2");
			//row6.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			//row6.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			//row6.Cells[3].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
			//row6.Cells[3].Format.Font.Color = Colors.White;
			//row6.Cells[3].Format.Font.Bold = true;
			//row6.Cells[3].Format.Font.Size = 8;

			row6.Cells[4].AddParagraph("3");
            row6.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[4].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[4].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[4].Format.Font.Color = Colors.White;
            row6.Cells[4].Format.Font.Bold = true;
            row6.Cells[4].Format.Font.Size = 8;

            row6.Cells[5].AddParagraph("4");
            row6.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[5].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[5].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[5].Format.Font.Color = Colors.White;
            row6.Cells[5].Format.Font.Bold = true;
            row6.Cells[5].Format.Font.Size = 8;

            row6.Cells[6].AddParagraph("5");
            row6.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[6].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[6].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[6].Format.Font.Color = Colors.White;
            row6.Cells[6].Format.Font.Bold = true;
            row6.Cells[6].Format.Font.Size = 8;

            row6.Cells[7].AddParagraph("6");
            row6.Cells[7].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[7].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[7].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[7].Format.Font.Color = Colors.White;
            row6.Cells[7].Format.Font.Bold = true;
            row6.Cells[7].Format.Font.Size = 8;

            row6.Cells[8].AddParagraph("7");
            row6.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[8].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[8].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[8].Format.Font.Color = Colors.White;
            row6.Cells[8].Format.Font.Bold = true;
            row6.Cells[8].Format.Font.Size = 8;

            row6.Cells[9].AddParagraph("8");
            row6.Cells[9].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[9].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[9].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[9].Format.Font.Color = Colors.White;
            row6.Cells[9].Format.Font.Bold = true;
            row6.Cells[9].Format.Font.Size = 8;

            row6.Cells[10].AddParagraph("9");
            row6.Cells[10].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[10].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[10].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[10].Format.Font.Color = Colors.White;
            row6.Cells[10].Format.Font.Bold = true;
            row6.Cells[10].Format.Font.Size = 8;

            row6.Cells[11].AddParagraph("10");
            row6.Cells[11].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[11].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[11].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[11].Format.Font.Color = Colors.White;
            row6.Cells[11].Format.Font.Bold = true;
            row6.Cells[11].Format.Font.Size = 8;

            row6.Cells[12].AddParagraph("11");
            row6.Cells[12].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[12].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[12].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[12].Format.Font.Color = Colors.White;
            row6.Cells[12].Format.Font.Bold = true;
            row6.Cells[12].Format.Font.Size = 8;

            row6.Cells[13].AddParagraph("12");
            row6.Cells[13].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[13].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[13].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[13].Format.Font.Color = Colors.White;
            row6.Cells[13].Format.Font.Bold = true;
            row6.Cells[13].Format.Font.Size = 8;

            row6.Cells[14].AddParagraph("13");
            row6.Cells[14].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[14].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[14].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[14].Format.Font.Color = Colors.White;
            row6.Cells[14].Format.Font.Bold = true;
            row6.Cells[14].Format.Font.Size = 8;

            row6.Cells[15].AddParagraph("14");
            row6.Cells[15].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[15].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[15].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[15].Format.Font.Color = Colors.White;
            row6.Cells[15].Format.Font.Bold = true;
            row6.Cells[15].Format.Font.Size = 8;

            row6.Cells[16].AddParagraph("15");
            row6.Cells[16].Format.Alignment = ParagraphAlignment.Center;
            row6.Cells[16].VerticalAlignment = VerticalAlignment.Center;
            row6.Cells[16].Shading.Color = new MigraDoc.DocumentObjectModel.Color(4, 50, 125);
            row6.Cells[16].Format.Font.Color = Colors.White;
            row6.Cells[16].Format.Font.Bold = true;
            row6.Cells[16].Format.Font.Size = 8;
        }

        private void AddDocumentDetail(Section section, List<InventoryListAppendixA> items, ReportFilter reportFilter)
        {
            Table table = new Table();
            Row row = null;
            Column column = null;
            Paragraph paragraph = null;

            //section.PageSetup.TopMargin = Unit.FromCentimeter(7.13);

            table = section.AddTable();
            table.AddColumn("1cm"); // Nr. Crt
            table.AddColumn("4cm"); // Denumire
            table.AddColumn("2.0cm"); // Unitate Economica
            table.AddColumn("2.0cm"); // Nr. SAP
            table.AddColumn("1cm"); // SubNo
            table.AddColumn("1cm"); // UM
            table.AddColumn("1cm"); // Faptic
            table.AddColumn("1.1cm"); // Scriptic
            table.AddColumn("1cm"); // Plus
            table.AddColumn("1cm"); // Minus
            table.AddColumn("2cm"); // Pret Unitar
            table.AddColumn("2cm"); // Valoarea
            table.AddColumn("1.5cm"); // Plus
            table.AddColumn("1.5cm"); // Minus
            table.AddColumn("2cm"); // Valoare inventar
            table.AddColumn("1.1cm"); // Valoare
            table.AddColumn("1.9cm"); // Motiv

            table.Borders.Visible = true;

            decimal sumQRegister = 0;
            decimal sumQActual = 0;

            decimal sumPlus = 0;
            decimal sumMinus = 0;

            decimal sumPlusValue = 0;
            decimal sumMinusValue = 0;

            int index = 0;

            foreach (var item in items)
            {
                index++;
                row = table.AddRow();
                row.HeightRule = RowHeightRule.Exactly;
                row.Height = "1.6cm";


                row.Format.Alignment = ParagraphAlignment.Center;
                row.VerticalAlignment = VerticalAlignment.Center;
                row.Format.Font.Size = 6;
                row.Format.Font.Bold = false;
                decimal plus = 0;
                decimal minus = 0;
                decimal plusCurrBkValue = 0;
                decimal minusCurrBkValue = 0;

				if (item.IsTemp == false)
                {
					if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
					{
						if ((item.CostCenterIdInitial > 0) && (!reportFilter.CostCenterIds.Contains(item.CostCenterIdInitial)))
						{
							plus = 1;
							plusCurrBkValue = item.CurrBkValue;
							item.QInitial = 0;
							minus = 0;
							minusCurrBkValue = 0;
						}
						else
						{
							if (item.CostCenterIdInitial > 0 && item.CostCenterIdFinal > 0)
							{
								if (reportFilter.CostCenterIds.Contains(item.CostCenterIdFinal))
								{
									item.QFinal = 1;
									minus = 0;
									minusCurrBkValue = 0;
								}
								else
								{
									item.QFinal = 0;
									minus = 1;
									minusCurrBkValue = item.CurrBkValue;
								}
							}
							else
							{
								item.QFinal = 0;
								minus = 1;
								minusCurrBkValue = item.CurrBkValue;
							}

						}
					}
                    else
                    {
						if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
						{
							if ((item.DivisionIdInitial > 0) && (!reportFilter.DivisionIds.Contains(item.DivisionIdInitial)))
							{
								plus = 1;
								plusCurrBkValue = item.CurrBkValue;
								item.QInitial = 0;
								minus = 0;
								minusCurrBkValue = 0;
							}
							else
							{
								if (item.DivisionIdInitial > 0 && item.DivisionIdFinal > 0)
								{
									if (reportFilter.DivisionIds.Contains(item.DivisionIdFinal))
									{
										item.QFinal = 1;
										minus = 0;
										minusCurrBkValue = 0;
									}
									else
									{
										item.QFinal = 0;
										minus = 1;
										minusCurrBkValue = item.CurrBkValue;
									}
								}
								else
								{
									item.QFinal = 0;
									minus = 1;
									minusCurrBkValue = item.CurrBkValue;
								}

							}
						}
						else
						{
							if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
							{
								if ((item.DepartmentIdInitial > 0) && (!reportFilter.DepartmentIds.Contains(item.DepartmentIdInitial)))
								{
									plus = 1;
									plusCurrBkValue = item.CurrBkValue;
									item.QInitial = 0;
									minus = 0;
									minusCurrBkValue = 0;
								}
								else
								{
									if (item.DepartmentIdInitial > 0 && item.DepartmentIdFinal > 0)
									{
										if (reportFilter.DepartmentIds.Contains(item.DepartmentIdFinal))
										{
											item.QFinal = 1;
											minus = 0;
											minusCurrBkValue = 0;
										}
										else
										{
											item.QFinal = 0;
											minus = 1;
											minusCurrBkValue = item.CurrBkValue;
										}
									}
									else
									{
										item.QFinal = 0;
										minus = 1;
										minusCurrBkValue = item.CurrBkValue;
									}

								}
							}
							else
							{
								if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
								{
									if ((item.AdministrationIdInitial > 0) && (!reportFilter.AdministrationIds.Contains(item.AdministrationIdInitial)))
									{
										plus = 1;
										plusCurrBkValue = item.CurrBkValue;
										item.QInitial = 0;
										minus = 0;
										minusCurrBkValue = 0;
									}
									else
									{
										if (item.AdministrationIdInitial > 0 && item.AdministrationIdFinal > 0)
										{
											if (reportFilter.AdministrationIds.Contains(item.AdministrationIdFinal))
											{
												item.QFinal = 1;
												minus = 0;
												minusCurrBkValue = 0;
											}
											else
											{
												item.QFinal = 0;
												minus = 1;
												minusCurrBkValue = item.CurrBkValue;
											}
										}
										else
										{
											item.QFinal = 0;
											minus = 1;
											minusCurrBkValue = item.CurrBkValue;
										}

									}
								}
							}
						}
					}
					
					
					
                }
                else
                {
					minus = 0;
					minusCurrBkValue = 0;
					plus = 1;
					plusCurrBkValue = 0;
					item.QFinal = 1;
					item.QInitial = 0;
				}

			

				QRCodeGenerator qrGenerator = new QRCodeGenerator();
				QRCodeData qrCodeData = qrGenerator.CreateQrCode(item.InvNo, QRCodeGenerator.ECCLevel.Q);
				QRCode qrCode = new QRCode(qrCodeData);
				Bitmap qrCodeImage = qrCode.GetGraphic(20);
				
				row.Cells[0].AddParagraph(index.ToString()); // Nr. Crt
                row.Cells[1].AddParagraph(item.AssetName); // Denumire

				using (Bitmap bitMap = qrCode.GetGraphic(2))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						byte[] byteImage = ms.ToArray();

                        if(index % 2 != 1)
                        {
							row.Cells[2].AddParagraph().AddImage($"base64:{Convert.ToBase64String(byteImage)}");
							row.Cells[3].AddParagraph(item.InvNo);
						}
                        else
                        {
							row.Cells[2].AddParagraph(item.InvNo);
							row.Cells[3].AddParagraph().AddImage($"base64:{Convert.ToBase64String(byteImage)}");
						}

						
						
					}
				}

				//row.Cells[2].AddParagraph((committeeType.Id == 2 || committeeType.Id == 10) ? item.InventoryNumber : item.IdleGroup); // Unitate Economica
				//row.Cells[3].AddParagraph(item.InvNo); // Nr. SAP
                row.Cells[4].AddParagraph(item.SubNo); // SubNo
                row.Cells[5].AddParagraph("BUC"); // UM
                row.Cells[6].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, item.QFinal)); // Faptic
                row.Cells[7].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, item.QInitial)); // Scriptic
                row.Cells[8].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, plus)); // Plus
                row.Cells[9].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, minus)); // Minus
				row.Cells[10].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, item.CurrentAPC));                                                                                                                                     //row.Cells[10].AddParagraph(String.Format("{0:#,##0.##}", item.ValueInv)); // Pret Unitar

				row.Cells[11].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, item.CurrBkValue)); // Valoarea

                row.Cells[12].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, plusCurrBkValue)); // Plus
                row.Cells[13].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, minusCurrBkValue)); // Minus
				row.Cells[14].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, item.CurrBkValue)); // Valoarea
																																									//row.Cells[14].AddParagraph(String.Format("{0:#,##0.##}", item.ValueDep)); // Valoare inventar
				row.Cells[15].AddParagraph(string.Empty); // Valoare
                row.Cells[16].AddParagraph(item.Info == null ? string.Empty : item.Info); // Motiv
                //cell = row.Cells[3];
                //cell.AddParagraph(ticket.due_date.ToString("MM/dd/yyyy"));
            }




			if ((reportFilter.CostCenterIds != null) && (reportFilter.CostCenterIds.Count > 0))
			{
				sumQRegister = items.Sum(a => a.IsTemp || (!reportFilter.CostCenterIds.Contains(a.CostCenterIdInitial)) ? 0 : 1);
				sumQActual = items.Sum(a => ((a.CostCenterIdInitial > 0) && (a.CostCenterIdFinal > 0) && (reportFilter.CostCenterIds.Contains(a.CostCenterIdFinal))) ? 1 : 0);

				sumPlus = items.Sum(a => a.IsTemp || (!reportFilter.CostCenterIds.Contains(a.CostCenterIdInitial)) ? 1 : 0);
				sumMinus = items.Sum(a => (a.CostCenterIdInitial > 0 && a.CostCenterIdFinal > 0 && (reportFilter.CostCenterIds.Contains(a.CostCenterIdFinal))) ? 0 : 1);

				sumPlusValue = items.Sum(a => a.IsTemp || (!reportFilter.CostCenterIds.Contains(a.CostCenterIdInitial)) ? a.CurrBkValue : 0);
				sumMinusValue = items.Sum(a => (a.CostCenterIdInitial > 0 && a.CostCenterIdFinal > 0 && (reportFilter.CostCenterIds.Contains(a.CostCenterIdFinal))) ? 0 : a.CurrBkValue);

				//sumCurrentAPC = items.Sum(a => a.CurrentAPC);
			}
            else
            {
				if ((reportFilter.DivisionIds != null) && (reportFilter.DivisionIds.Count > 0))
				{
					sumQRegister = items.Sum(a => a.IsTemp || (!reportFilter.DivisionIds.Contains(a.DivisionIdInitial)) ? 0 : 1);
					sumQActual = items.Sum(a => ((a.DivisionIdInitial > 0) && (a.DivisionIdFinal > 0) && (reportFilter.DivisionIds.Contains(a.DivisionIdFinal))) ? 1 : 0);

					sumPlus = items.Sum(a => a.IsTemp || (!reportFilter.DivisionIds.Contains(a.DivisionIdInitial)) ? 1 : 0);
					sumMinus = items.Sum(a => (a.DivisionIdInitial > 0 && a.DivisionIdFinal > 0 && (reportFilter.DivisionIds.Contains(a.DivisionIdFinal))) ? 0 : 1);

					sumPlusValue = items.Sum(a => a.IsTemp || (!reportFilter.DivisionIds.Contains(a.DivisionIdInitial)) ? a.CurrBkValue : 0);
					sumMinusValue = items.Sum(a => (a.DivisionIdInitial > 0 && a.DivisionIdFinal > 0 && (reportFilter.DivisionIds.Contains(a.DivisionIdFinal))) ? 0 : a.CurrBkValue);

					//sumCurrentAPC = items.Sum(a => a.CurrentAPC);
				}
				else
				{
					if ((reportFilter.DepartmentIds != null) && (reportFilter.DepartmentIds.Count > 0))
					{
						sumQRegister = items.Sum(a => a.IsTemp || (!reportFilter.DepartmentIds.Contains(a.DepartmentIdInitial)) ? 0 : 1);
						sumQActual = items.Sum(a => ((a.DepartmentIdInitial > 0) && (a.DepartmentIdFinal > 0) && (reportFilter.DepartmentIds.Contains(a.DepartmentIdFinal))) ? 1 : 0);

						sumPlus = items.Sum(a => a.IsTemp || (!reportFilter.DepartmentIds.Contains(a.DepartmentIdInitial)) ? 1 : 0);
						sumMinus = items.Sum(a => (a.DepartmentIdInitial > 0 && a.DepartmentIdFinal > 0 && (reportFilter.DepartmentIds.Contains(a.DepartmentIdFinal))) ? 0 : 1);

						sumPlusValue = items.Sum(a => a.IsTemp || (!reportFilter.DepartmentIds.Contains(a.DepartmentIdInitial)) ? a.CurrBkValue : 0);
						sumMinusValue = items.Sum(a => (a.DepartmentIdInitial > 0 && a.DepartmentIdFinal > 0 && (reportFilter.DepartmentIds.Contains(a.DepartmentIdFinal))) ? 0 : a.CurrBkValue);

						//sumCurrentAPC = items.Sum(a => a.CurrentAPC);
					}
					else
					{
						if ((reportFilter.AdministrationIds != null) && (reportFilter.AdministrationIds.Count > 0))
						{
							sumQRegister = items.Sum(a => a.IsTemp || (!reportFilter.AdministrationIds.Contains(a.AdministrationIdInitial)) ? 0 : 1);
							sumQActual = items.Sum(a => ((a.AdministrationIdInitial > 0) && (a.AdministrationIdFinal > 0) && (reportFilter.AdministrationIds.Contains(a.AdministrationIdFinal))) ? 1 : 0);

							sumPlus = items.Sum(a => a.IsTemp || (!reportFilter.AdministrationIds.Contains(a.AdministrationIdInitial)) ? 1 : 0);
							sumMinus = items.Sum(a => (a.AdministrationIdInitial > 0 && a.AdministrationIdFinal > 0 && (reportFilter.AdministrationIds.Contains(a.AdministrationIdFinal))) ? 0 : 1);

							sumPlusValue = items.Sum(a => a.IsTemp || (!reportFilter.AdministrationIds.Contains(a.AdministrationIdInitial)) ? a.CurrBkValue : 0);
							sumMinusValue = items.Sum(a => (a.AdministrationIdInitial > 0 && a.AdministrationIdFinal > 0 && (reportFilter.AdministrationIds.Contains(a.AdministrationIdFinal))) ? 0 : a.CurrBkValue);

							//sumCurrentAPC = items.Sum(a => a.CurrentAPC);
						}
					}
				}
			}
			
			row = table.AddRow();
            row.HeightRule = RowHeightRule.Exactly;
            row.Height = Unit.FromCentimeter(1);


            row.Format.Alignment = ParagraphAlignment.Center;
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Format.Font.Size = 6;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("TOTAL");
            row.Cells[0].MergeRight = 5;

            row.Cells[6].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, sumQActual));
            row.Cells[7].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, sumQRegister));
            row.Cells[8].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, sumPlus));
            row.Cells[9].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, sumMinus));
			row.Cells[10].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items.Sum(a => a.CurrentAPC)));
			//row.Cells[10].AddParagraph();

			row.Cells[11].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items.Sum(a => a.CurrBkValue)));



            row.Cells[12].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, sumPlusValue));
            row.Cells[13].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, sumMinusValue));
			row.Cells[14].AddParagraph(String.Format(System.Globalization.CultureInfo.GetCultureInfo(this._cultureInfo), this._valueFormat, items.Sum(a => a.CurrBkValue)));
			//row.Cells[14].AddParagraph(String.Format("{0:#,##0.##}", items.Sum(a => a.ValueDep)));
			row.Cells[15].AddParagraph();
            row.Cells[15].MergeRight = 1;
        }

        private Table AddEPMainCommitteeHeader(Section section, int index)
        {
            Table table = null;
            Row row = null;
            Column column = null;
            Paragraph paragraph = null;

            table = section.AddTable();
            table.Borders.Visible = false;
            table.TopPadding = 5;
            table.BottomPadding = 5;
            //table.KeepTogether = false;

            float columnWidth = Unit.FromCentimeter(8);

            column = table.AddColumn();
            column.Width = columnWidth;

            column = table.AddColumn();
            column.Width = columnWidth;

            return table;
        }

        private void AddEPFooter(Section section, Document document, bool complete = true)
        {

            HeaderFooter footer = section.Footers.Primary;

            var table = footer.AddTable();
            Row row = null;

            table.AddColumn("1cm"); // Nr. Crt
            table.AddColumn("4cm"); // Denumire
            table.AddColumn("2.4cm"); // Unitate Economica
            table.AddColumn("1.6cm"); // Nr. SAP
            table.AddColumn("1cm"); // SubNo
            table.AddColumn("1cm"); // UM
            table.AddColumn("1cm"); // Faptic
            table.AddColumn("1.1cm"); // Scriptic
            table.AddColumn("1cm"); // Plus
            table.AddColumn("1cm"); // Minus
            table.AddColumn("2cm"); // Pret Unitar
            table.AddColumn("2cm"); // Valoarea
            table.AddColumn("1.5cm"); // Plus
            table.AddColumn("1.5cm"); // Minus
            table.AddColumn("2cm"); // Valoare inventar
            table.AddColumn("1.1cm"); // Valoare
            table.AddColumn("1.9cm"); // Motiv

            if (complete)
            {
                // table.Borders.Width = 1;
                row = table.AddRow();
                row.HeightRule = RowHeightRule.Exactly;
                row.Height = "1cm";
                row.Borders.Visible = true;

                row.Cells[0].AddParagraph("Nume si Prenume");
                row.Cells[0].MergeRight = 1;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[0].Format.Font.Bold = true;
                row.Cells[0].Format.Font.Size = 8;

                row.Cells[2].AddParagraph("Comisia de inventariere");
                row.Cells[2].MergeRight = 1;
                row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[2].Format.Font.Bold = true;
                row.Cells[2].Format.Font.Size = 8;

                row.Cells[4].AddParagraph($"Gestionar (responsabil)");
                row.Cells[4].MergeRight = 6;
                row.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[4].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[4].Format.Font.Bold = true;
                row.Cells[4].Format.Font.Size = 8;

                row.Cells[11].AddParagraph("Contabilitate");
                row.Cells[11].MergeRight = 5;
                row.Cells[11].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[11].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[11].Format.Font.Bold = true;
                row.Cells[11].Format.Font.Size = 8;
            }

            //row = table.AddRow();
            //row.HeightRule = RowHeightRule.Exactly;
            //row.Height = "0.5cm";
            //row.Borders.Visible = false;
            //// row.Borders.Color = Colors.White;

            //row.Cells[0].AddParagraph("DECLARATIE GESTIONAR");
            //row.Cells[0].MergeRight = 16;
            //row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            //row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            //row.Cells[0].Format.Font.Bold = true;
            //row.Cells[0].Format.Font.Size = 8;


            row = table.AddRow();
            row.HeightRule = RowHeightRule.Exactly;
            row.Height = "0.5cm";
            row.Borders.Visible = false;
            // row.Borders.Color = Colors.White;

            row.Cells[0].AddParagraph("DECLARATIE GESTIONAR: TOATE CANTITATILE DIN PREZENTA LISTA AU FOST STABILITE IN PREZENTA MEA. BUNURILE RESPECTIVE SE AFLA IN PASTRAREA SI RASPUNDEREA MEA. NU AM OBIECTIUNI DE FACUT. NU AU RAMAS BUNURI NEINVENTARIATE.");
            row.Cells[0].MergeRight = 16;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Font.Size = 7;


            if (complete)
            {
                row = table.AddRow();
                row.HeightRule = RowHeightRule.Exactly;
                row.Height = "0.5cm";
                row.Borders.Visible = false;
                // row.Borders.Color = Colors.White;

                row.Cells[0].AddParagraph("Comisia de inventariere Nume - Prenume - Semnatura");
                row.Cells[0].MergeRight = 12;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[0].Format.Font.Bold = true;
                row.Cells[0].Format.Font.Size = 8;

                row.Cells[13].AddParagraph("Gestionar Nume - Prenume - Semnatura");
                row.Cells[13].MergeRight = 3;
                row.Cells[13].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[13].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[13].Format.Font.Bold = true;
                row.Cells[13].Format.Font.Size = 8;


                row = table.AddRow();
                row.HeightRule = RowHeightRule.Exactly;
                row.Height = "0.5cm";
                row.Borders.Visible = false;
                // row.Borders.Color = Colors.White;

                row.Cells[0].AddParagraph($"");
                row.Cells[0].MergeRight = 12;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[0].Format.Font.Bold = true;
                row.Cells[0].Format.Font.Size = 8;

                row.Cells[13].AddParagraph($"");
                row.Cells[13].MergeRight = 3;
                row.Cells[13].Format.Alignment = ParagraphAlignment.Center;
                row.Cells[13].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[13].Format.Font.Bold = true;
                row.Cells[13].Format.Font.Size = 8;
            }
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
