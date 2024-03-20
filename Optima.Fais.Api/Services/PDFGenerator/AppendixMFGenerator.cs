using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using Remotion.Linq.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class AppendixMFGenerator : IAppendixMFGenerator
	{

		public IServiceProvider _services { get; }

		public AppendixMFGenerator(IServiceProvider services)
		{
			_services = services;
		}

		public async Task<PdfDocumentResult> GenerateDocumentAsync(int assetId, string resourcesPath)
		{
			using (var scope = _services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.Asset asset = null;

				asset = await dbContext.Set<Model.Asset>()
					.Include(c => c.AssetCategory)
					.Include(c => c.AssetType)
					.Include(c => c.CostCenter).ThenInclude(r => r.Room)
					.Where(a => a.Id == assetId).SingleAsync();

				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				Document document = new Document();

				document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1);
				document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);

				Section section = document.AddSection();

				section.PageSetup.TopMargin = Unit.FromCentimeter(2.5);
				section.PageSetup.PageFormat = PageFormat.A4;

				section.PageSetup.BottomMargin = Unit.FromCentimeter(2.0);

				//AddEPHeader(section, resourcesPath);
				AddEPDocumentDetail(section, asset);

				AddEPFooter(section, document, resourcesPath);

				//AddEPFooter2(section, document);

				return new PdfDocumentResult
				{
					Document = document,
					DocumentNumber = assetId,
				};
			}
			

		
		}

		private void AddEPHeader(Section section, string resourcesPath)
		{
			HeaderFooter header = section.Headers.Primary;

            string logoPath = resourcesPath + @"logo.png";

			var tableLogo = header.AddTable();
			tableLogo.AddColumn("4.5cm");
			tableLogo.AddColumn("4.5cm");
			tableLogo.AddColumn("3.8cm");
			tableLogo.AddColumn("3.2cm");
			tableLogo.AddColumn("1.0cm");
			tableLogo.AddColumn("1.0cm");
			tableLogo.Borders.Visible = false;
			var rowLogo = tableLogo.AddRow();
			rowLogo.HeightRule = RowHeightRule.Exactly;
			rowLogo.Height = "1.0cm";
			var paragraph = rowLogo.Cells[0].AddParagraph("");
			paragraph.Format.Alignment = ParagraphAlignment.Left;
			
			rowLogo.Cells[0].Format.Font.Color = Colors.Gray;
			var image = rowLogo.Cells[0].AddParagraph().AddImage(logoPath);
			rowLogo.Cells[0].MergeRight = 3;
			rowLogo.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			image.Height = "6mm";

        }

		private void AddEPDocumentDetail(Section section, Model.Asset asset)
		{
			//Table table = null;
            Table table1 = null;
			Row row = null;
			Column column = null;
			Paragraph paragraph = null;


            table1 = section.AddTable();
            table1.Borders.Visible = true;
            table1.AddColumn("1.0cm");
            table1.AddColumn("1.0cm");
            table1.AddColumn("3.5cm");
            table1.AddColumn("3.0cm");
            table1.AddColumn("1.0cm");
            table1.AddColumn("2.0cm");
            table1.AddColumn("1.5cm");
            table1.AddColumn("1.0cm");
            table1.AddColumn("4.0cm");

			var rowH = table1.AddRow();
			rowH.Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Format.Font.Color = Color.FromRgb(100, 145, 217);
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.8cm";
			rowH.Borders.Visible = true;
			rowH.Cells[0].AddParagraph("Fisa mijlocului fix");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 10;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].Format.Font.Color = Color.FromRgb(255, 255, 255);
			rowH.Cells[0].MergeRight = 8;



			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph("Numar inventar:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(asset.InvNo);
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("Data punerii in functiune:");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph(asset.PurchaseDate != null ? $"{String.Format("{0:dd/MM/yyyy}", asset.PurchaseDate)}" : "");


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph("Denumire:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(asset.Name);
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("Data amortizarii complete:");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph(asset.PurchaseDate != null ? $"{String.Format("{0:dd/MM/yyyy }", asset.PurchaseDate)}" : "");


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph("Locatia:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(asset.CostCenter != null && asset.CostCenter.Room != null ? asset.CostCenter.Room.Name : "");
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("CC:");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph(asset.CostCenter != null ? asset.CostCenter.Code : "");


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph("Grupa principala:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(asset.AssetType != null ? asset.AssetType.Name : "");
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("Amortizare lunara:");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph(Convert.ToString(asset.TaxAmount) != null ? Convert.ToString(asset.TaxAmount) : "");



			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph();
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("Cota de amortizare (%):");
			rowH.Cells[6].MergeRight = 1;
			var valRem = Convert.ToString(asset.TaxAmount);
			var valRemCota = Convert.ToDouble(valRem) / 100;
			var valRemCotaAmortizare = Convert.ToString(valRemCota);

			rowH.Cells[8].AddParagraph(Convert.ToString(valRem) != null ? valRemCotaAmortizare : "");

			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph("Grupa:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(asset.AssetCategory != null ? asset.AssetCategory.Code + " " + asset.AssetCategory.Name : "");
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("Clasificare:");
			rowH.Cells[6].MergeRight = 1;
			//rowH.Cells[8].AddParagraph(Convert.ToString(asset.SubCategory != null ? asset.SubCategory.Name : ""));


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph();
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph();
			rowH.Cells[4].MergeRight = 1;
			rowH.Cells[6].AddParagraph("Durata Functionare:");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph();

			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.05cm";
			rowH.Borders.Visible = true;
			rowH.Cells[0].AddParagraph("");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Left;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 8;
			rowH.Shading.Color = Color.FromRgb(0, 0, 0);


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph();
			rowH.Cells[2].MergeRight = 1;
			rowH.Cells[4].AddParagraph("Opratie");
			rowH.Cells[5].AddParagraph();
			rowH.Cells[6].AddParagraph("Nr./data doc:");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph();



			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph("Valoare");
			rowH.Cells[3].AddParagraph("Amortizare");
			rowH.Cells[4].AddParagraph("Detalii\r\nOperatie");
			rowH.Cells[5].AddParagraph();
			rowH.Cells[6].AddParagraph("Observatii: ");
			rowH.Cells[6].MergeRight = 1;
			rowH.Cells[8].AddParagraph();



			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.05cm";
			rowH.Borders.Visible = true;
			rowH.Cells[0].AddParagraph("");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Left;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 8;
			rowH.Shading.Color = Color.FromRgb(0, 0, 0);


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph("August");
			rowH.Cells[0].MergeRight = 8;

			rowH.Cells[8].AddParagraph();


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(Convert.ToString(asset.TotalAmountRon) != null ? Convert.ToString(asset.TotalAmountRon) : "");
			rowH.Cells[3].AddParagraph();
			rowH.Cells[4].AddParagraph("Preluare valoare inventar luna anterioara");
			rowH.Cells[4].MergeRight = 4;

			rowH.Cells[8].AddParagraph();




			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph();
			rowH.Cells[3].AddParagraph(Convert.ToString(asset.TaxAmount) != null ? Convert.ToString(asset.TaxAmount) : "");
			rowH.Cells[4].AddParagraph("Preluare amortizare cmulata luna anterioara");
			rowH.Cells[4].MergeRight = 4;

			rowH.Cells[8].AddParagraph();

			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;

			rowH.Cells[0].AddParagraph();
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph();
			rowH.Cells[3].AddParagraph(Convert.ToString(asset.TaxAmount) != null ? Convert.ToString(asset.TaxAmount) : "");
			rowH.Cells[4].AddParagraph("Amortizare");
			rowH.Cells[4].MergeRight = 4;

			rowH.Cells[8].AddParagraph();


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.05cm";
			rowH.Borders.Visible = true;
			rowH.Cells[0].AddParagraph("");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Left;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 8;
			rowH.Shading.Color = Color.FromRgb(0, 0, 0);




			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;
			rowH.Shading.Color = Color.FromRgb(109, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("Total:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph(Convert.ToString(asset.TotalAmountRon) != null ? Convert.ToString(asset.TotalAmountRon) : "");
			rowH.Cells[3].AddParagraph(Convert.ToString(asset.TotalAmountRon) != null ? Convert.ToString(asset.TotalAmountRon) : "");
			rowH.Cells[4].AddParagraph();

			rowH.Cells[4].MergeRight = 4;

			rowH.Cells[8].AddParagraph();


			rowH = table1.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Format.Alignment = ParagraphAlignment.Left;
			rowH.VerticalAlignment = VerticalAlignment.Center;
			rowH.Format.Font.Size = 6;
			rowH.Format.Font.Bold = true;
			rowH.Shading.Color = Color.FromRgb(109, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("Valoare Ramasa:");
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[2].AddParagraph("0");
			rowH.Cells[3].AddParagraph("0");
			rowH.Cells[4].AddParagraph();

			rowH.Cells[4].MergeRight = 4;

			rowH.Cells[8].AddParagraph();




			//rowH.Cells[4].MergeRight = 1;
			//rowH.Cells[6].AddParagraph(item.Asset.InvNo + "/" + item.Asset.SubNo);
			//rowH.Cells[7].AddParagraph(String.Format("{0:#,##0.##}", item.Asset.Quantity));
			//rowH.Cells[8].AddParagraph(String.Format("{0:#,##0.##}", item.Asset.ValueInv));


		}

		private void AddEPFooter(Section section, Document document, string resourcesPath)
		{
			string logoPath = resourcesPath + @"logo.png";

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
		
			var image = rowLogo.Cells[0].AddParagraph().AddImage(logoPath);
			rowLogo.Cells[0].MergeRight = 1;
			rowLogo.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			image.Height = "6mm";


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
