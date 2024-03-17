using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class Appendix1Generator : IAppendix1Generator
	{

		public IServiceProvider _services { get; }

		public Appendix1Generator(IServiceProvider services)
		{
			_services = services;
		}

		public async Task<PdfDocumentResult> GenerateDocumentAsync(int ducumentNumber, string resourcesPath)
		{
			using (var scope = _services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				List<Model.EmailStatus> emailManagers = null;

				Model.Division srcDivision = null;
				Model.CostCenter srcCostCenter = null;
				Model.Department srcDepartment = null;

				Model.Division dstDivision = null;
				Model.CostCenter dstCostCenter = null;
				Model.Department dstDepartment = null;

				Model.Employee srcEmployee = null;
				Model.Employee dstEmployee = null;

				Model.ApplicationUser srcEmployeeValidateUser = null;
				//Model.ApplicationUser srcEmployeeManagerValidateUser = null;
				Model.ApplicationUser dstEmployeeValidateUser = null;
				//Model.ApplicationUser dstEmployeeManagerValidateUser = null;

				emailManagers = await dbContext.Set<Model.EmailStatus>()
					.Include(a => a.Asset)
					.Include(a => a.CostCenterInitial).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
					.Include(a => a.CostCenterFinal).ThenInclude(d => d.Division).ThenInclude(d => d.Department)
					.Include(a => a.SrcEmployeeValidateUser)
					//.Include(a => a.SrcManagerValidateUser)
					.Include(a => a.DstEmployeeValidateUser)
					//.Include(a => a.DstManagerValidateUser)
					.Where(a => a.DocumentNumber == ducumentNumber && a.IsDeleted == false).ToListAsync();

				if (emailManagers.Count > 0)
				{
					if (emailManagers[0].CostCenterIdInitial != null)
					{
						srcCostCenter = await dbContext.Set<Model.CostCenter>().Where(a => a.Id == emailManagers[0].CostCenterIdInitial).SingleOrDefaultAsync();

						if (srcCostCenter != null && srcCostCenter.DivisionId != null)
						{
							srcDivision = await dbContext.Set<Model.Division>().Where(a => a.Id == srcCostCenter.DivisionId).SingleOrDefaultAsync();

							if (srcDivision != null && srcDivision.DepartmentId != null)
							{
								srcDepartment = await dbContext.Set<Model.Department>().Where(a => a.Id == srcDivision.DepartmentId).SingleOrDefaultAsync();
							}
							else
							{
								srcDepartment = new Model.Department() { Code = "", Name = "" };
							}
						}
						else
						{
							srcDivision = new Model.Division() { Code = "", Name = "" };
							srcDepartment = new Model.Department() { Code = "", Name = "" };
						}

					}
					else
					{
						srcCostCenter = new Model.CostCenter() { Code = "", Name = "" };
						srcDivision = new Model.Division() { Code = "", Name = "" };
						srcDepartment = new Model.Department() { Code = "", Name = "" };
					}


					if (emailManagers[0].EmployeeIdInitial != null)
					{
						srcEmployee = await dbContext.Set<Model.Employee>().Include(c => c.CostCenter).ThenInclude(d => d.Division).ThenInclude(d => d.Department).Where(a => a.Id == emailManagers[0].EmployeeIdInitial).SingleOrDefaultAsync();
					}
					else
					{
						srcEmployee = new Model.Employee() { 
							FirstName = "", 
							LastName = "", 
							InternalCode = "", 
							CostCenter = new Model.CostCenter { Code = "", Name= "" },
							Division = new Model.Division { Code = "", Name = "" },
							Department = new Model.Department { Code = "", Name = "" }
						};
					}

					if (emailManagers[0].SrcEmployeeValidateUser != null)
					{
						srcEmployeeValidateUser = await dbContext.Set<Model.ApplicationUser>().Where(a => a.Id == emailManagers[0].SrcEmployeeValidateBy).SingleOrDefaultAsync();

						if (emailManagers[0].SkipSrcEmployee)
						{
							srcEmployeeValidateUser = new Model.ApplicationUser() { UserName = "" };
						}
					}
					else
					{
						srcEmployeeValidateUser = new Model.ApplicationUser() { UserName = "" };
					}



					if (emailManagers[0].CostCenterIdFinal != null)
					{
						dstCostCenter = await dbContext.Set<Model.CostCenter>().Where(a => a.Id == emailManagers[0].CostCenterIdFinal).SingleOrDefaultAsync();

						if (dstCostCenter != null && dstCostCenter.DivisionId != null)
						{
							dstDivision = await dbContext.Set<Model.Division>().Where(a => a.Id == dstCostCenter.DivisionId).SingleOrDefaultAsync();

							if (dstDivision != null && dstDivision.DepartmentId != null)
							{
								dstDepartment = await dbContext.Set<Model.Department>().Where(a => a.Id == dstDivision.DepartmentId.Value).SingleOrDefaultAsync();
							}
							else
							{
								dstDepartment = new Model.Department() { Code = "", Name = "" };
							}
						}
						else
						{
							dstDivision = new Model.Division() { Code = "", Name = "" };
							dstDepartment = new Model.Department() { Code = "", Name = "" };
						}

					}
					else
					{
						dstCostCenter = new Model.CostCenter() { Code = "", Name = "" };
						dstDivision = new Model.Division() { Code = "", Name = "" };
						dstDepartment = new Model.Department() { Code = "", Name = "" };
					}


					if (emailManagers[0].EmployeeIdFinal != null)
					{
						dstEmployee = await dbContext.Set<Model.Employee>().Include(c => c.CostCenter).ThenInclude(d => d.Division).ThenInclude(d => d.Department).Where(a => a.Id == emailManagers[0].EmployeeIdFinal).SingleOrDefaultAsync();
					}
					else
					{
						dstEmployee = new Model.Employee() { 
						FirstName = "", 
						LastName = "", 
						InternalCode = "",
						CostCenter = new Model.CostCenter { Code = "", Name = "" },
						Division = new Model.Division { Code = "", Name = "" },
						Department = new Model.Department { Code = "", Name = "" }
						};
					}

					if (emailManagers[0].DstEmployeeValidateUser != null)
					{
						dstEmployeeValidateUser = await dbContext.Set<Model.ApplicationUser>().Where(a => a.Id == emailManagers[0].DstEmployeeValidateBy).SingleOrDefaultAsync();

						if (emailManagers[0].SkipDstEmployee)
						{
							dstEmployeeValidateUser = new Model.ApplicationUser() { UserName = "" };
						}
					}
					else
					{
						dstEmployeeValidateUser = new Model.ApplicationUser() { UserName = "" };
					}

				}

				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				Document document = new Document();

				document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(1);
				document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);

				Section section = document.AddSection();

				section.PageSetup.TopMargin = Unit.FromCentimeter(2.5);
				section.PageSetup.PageFormat = PageFormat.A4;

				section.PageSetup.BottomMargin = Unit.FromCentimeter(2.0);

				//AddEPHeader(section, resourcesPath);
				AddEPDocumentDetail(section,
					srcDepartment, srcCostCenter, srcDivision, srcEmployee,
					dstDepartment, dstCostCenter, dstDivision, dstEmployee,
					srcEmployeeValidateUser, dstEmployeeValidateUser, //dstEmployeeManagerValidateUser,
					emailManagers[0].SrcEmployeeValidateAt.Value, emailManagers[0].DstEmployeeValidateAt.Value, //emailManagers[0].DstManagerValidateAt.Value,
					emailManagers[0].DocumentNumber, emailManagers[0].CreatedAt, emailManagers);

				AddEPFooter(section, document, resourcesPath);

				//AddEPFooter2(section, document);

				return new PdfDocumentResult
				{
					Document = document,
					DocumentNumber = emailManagers[0].DocumentNumber,
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


			//rowLogo.Cells[1].Format.Font.Color = Colors.Gray;
			//paragraph.Format.Font.Size = 9;
			//paragraph = rowLogo.Cells[2].AddParagraph("Clasa documentului");
			//paragraph.Format.Alignment = ParagraphAlignment.Left;
			//rowLogo.Cells[2].Format.Font.Color = Colors.Gray;
			//paragraph.Format.Font.Size = 9;

			//rowLogo.Cells[3].MergeDown = 1;
			//rowLogo.Cells[2].Borders.Right.Visible = true;
			//rowLogo.Cells[3].Format.Alignment = ParagraphAlignment.Right;
			////rowLogo.Cells[2].AddParagraph().AddFormattedText(InvDecisionConfig.epHeaderRight, TextFormat.Bold);
			//rowLogo.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			//rowLogo.Cells[3].Borders.Left.Visible = true;
			//header.AddParagraph().AddFormattedText(InvDecisionConfig.EPHeaderLeft(invDecision.Company.Code, invDecision.AdmAsset.Name), TextFormat.Bold);

			//rowLogo = tableLogo.AddRow();
			//rowLogo.HeightRule = RowHeightRule.Exactly;
			//rowLogo.Height = "1.1cm";
			//var paragraphSecondRow = rowLogo.Cells[0].AddParagraph("Entitate");
			//paragraphSecondRow.Format.Alignment = ParagraphAlignment.Left;
			//paragraphSecondRow.Format.Font.Size = 11;
			//rowLogo.Cells[1].AddParagraph("Standard");
			//paragraphSecondRow.Format.Alignment = ParagraphAlignment.Left;
			//paragraphSecondRow.Format.Font.Size = 11;
			//rowLogo.Cells[2].AddParagraph("Anexă");
			//paragraphSecondRow.Format.Alignment = ParagraphAlignment.Left;
			//paragraphSecondRow.Format.Font.Size = 11;


			//rowLogo = tableLogo.AddRow();
			//rowLogo.HeightRule = RowHeightRule.Exactly;
			//rowLogo.Height = ".4cm";
			//rowLogo.Cells[0].MergeRight = 3;
			//var paragraphText = rowLogo.Cells[0].AddParagraph("Titlul anexei");
			//paragraphText.Format.Alignment = ParagraphAlignment.Left;
			//rowLogo.Cells[0].Format.Font.Color = Colors.Gray;
			//paragraphText.Format.Font.Size = 7;

			//rowLogo = tableLogo.AddRow();
			//rowLogo.HeightRule = RowHeightRule.Exactly;
			//rowLogo.Height = "1cm";
			//rowLogo.Cells[0].MergeRight = 3;
			//var paragraphText2 = rowLogo.Cells[0].AddParagraph("Anexa 8 Bon de mișcare a mijloacelor fixe");
			//paragraphText2.Format.Alignment = ParagraphAlignment.Center;
			//rowLogo.Cells[0].Format.Font.Color = Colors.Black;
			//paragraphText2.Format.Font.Size = 16;

		}

		private void AddEPDocumentDetail(Section section,
			Model.Department srcDepartment, Model.CostCenter srcCostCenter, Model.Division srcDivision, Model.Employee srcEmployee,
			Model.Department dstDepartment, Model.CostCenter dstCostCenter, Model.Division dstDivision, Model.Employee dstEmployee,
			Model.ApplicationUser srcEmployeeValidateUser, Model.ApplicationUser dstEmployeeValidateUser, //Model.ApplicationUser dstEmployeeManagerValidateUser,
			DateTime srcEmployeeValidateAt, DateTime dstEmployeeValidateAt, //DateTime dstEmployeeManagerValidateAt,
			int documentNumber, DateTime documentDate, List<Model.EmailStatus> emailManagers)
		{
			Table table = null;
			Row row = null;
			Column column = null;
			Paragraph paragraph = null;

			table = section.AddTable();
			table.Borders.Visible = true;
			table.AddColumn("1.5cm");
			table.AddColumn("1.5cm");
			table.AddColumn("2.5cm");
			table.AddColumn("3.0cm");
			table.AddColumn("2.0cm");
			table.AddColumn("1.5cm");
			table.AddColumn("2.5cm");
			table.AddColumn("1.0cm");
			table.AddColumn("2.5cm");
			//table.TopPadding = 5;
			//table.BottomPadding = 5;
			//table.KeepTogether = false

			var empIni = srcEmployee.InternalCode == "_NSP" ? "STOCK IT" :
										 srcEmployee.InternalCode == "VIRTUAL" ? "STOCK IT" :
										 srcEmployee.IsDeleted == true ? "PLECAT DIN COMPANIE" :
										 srcEmployeeValidateUser.UserName;

			var empFin = dstEmployee.InternalCode == "_NSP" ? "STOCK IT" :
						 dstEmployee.InternalCode == "VIRTUAL" ? "STOCK IT" :
						 dstEmployee.IsDeleted == true ? "PLECAT DIN COMPANIE" :
						 dstEmployeeValidateUser.UserName;

			var rowH = table.AddRow();
			rowH.Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Format.Font.Color = Color.FromRgb(100, 145, 217);
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.8cm";
			rowH.Borders.Visible = true;
			

			rowH.Cells[0].AddParagraph("BON DE MISCARE A MIJLOACELOR FIXE");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 10;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].Format.Font.Color = Color.FromRgb(255, 255, 255);
			rowH.Cells[0].MergeRight = 8;


			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Cells[0].AddParagraph("Numar document");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 1;
			rowH.Cells[0].MergeDown = 1;

			rowH.Cells[2].AddParagraph("Data intocmire");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 7;
			rowH.Cells[2].Format.Font.Bold = true;
			rowH.Cells[2].MergeRight = 2;

			rowH.Cells[5].AddParagraph("Predator");
			rowH.Cells[5].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[5].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[5].Format.Font.Size = 7;
			rowH.Cells[5].Format.Font.Bold = true;
			rowH.Cells[5].MergeRight = 1;

			rowH.Cells[7].AddParagraph("Primitor");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Cells[2].AddParagraph("Ziua");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 7;
			rowH.Cells[2].Format.Font.Bold = true;
			rowH.Cells[2].Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Cells[2].Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[3].AddParagraph("Luna");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 7;
			rowH.Cells[3].Format.Font.Bold = true;
			rowH.Cells[3].Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Cells[3].Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[4].AddParagraph("Anul");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Cells[4].Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[5].AddParagraph($"{srcEmployee.Email}");
			rowH.Cells[5].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[5].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[5].Format.Font.Size = 7;
			rowH.Cells[5].Format.Font.Bold = true;
			rowH.Cells[5].MergeRight = 1;
			rowH.Cells[5].MergeDown = 1;

			rowH.Cells[7].AddParagraph($"{dstEmployee.Email}");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;
			rowH.Cells[7].MergeDown = 1;

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Cells[0].AddParagraph($"{documentNumber}");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 1;

			rowH.Cells[2].AddParagraph($"{String.Format("{0:dd}", documentDate)}");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 7;
			rowH.Cells[2].Format.Font.Bold = true;

			rowH.Cells[3].AddParagraph($"{String.Format("{0:MM}", documentDate)}");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 7;
			rowH.Cells[3].Format.Font.Bold = true;

			rowH.Cells[4].AddParagraph($"{String.Format("{0:yyyy}", documentDate)}");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;

			rowH = table.AddRow();
			rowH.Cells[0].Format.Font.Size = 8;
			rowH.Cells[0].MergeRight = 8;
			rowH.Height = "0.5cm";

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.8cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("Nr. Crt.");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;


			rowH.Cells[1].AddParagraph("Denumirea mijlocului fix si caracteristici tehnice");
			rowH.Cells[1].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[1].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[1].Format.Font.Size = 7;
			rowH.Cells[1].Format.Font.Bold = true;
			rowH.Cells[1].MergeRight = 2;

			rowH.Cells[4].AddParagraph("Serie (*)");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph("Numarul de inventar");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 7;
			rowH.Cells[6].Format.Font.Bold = true;

			rowH.Cells[7].AddParagraph("Buc.");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;

			rowH.Cells[8].AddParagraph("Valoarea de inventar");
			rowH.Cells[8].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[8].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[8].Format.Font.Size = 7;
			rowH.Cells[8].Format.Font.Bold = true;

			int crtIndex = 0;

			emailManagers = emailManagers.OrderBy(a => a.Asset.Name).ToList();

			foreach (var item in emailManagers)
			{
				crtIndex++;
				rowH = table.AddRow();
				rowH.HeightRule = RowHeightRule.Exactly;
				rowH.Height = "0.6cm";
				rowH.Borders.Visible = true;
				rowH.Format.Alignment = ParagraphAlignment.Center;
				rowH.VerticalAlignment = VerticalAlignment.Center;
				rowH.Format.Font.Size = 6;
				rowH.Format.Font.Bold = true;

				rowH.Cells[0].AddParagraph(crtIndex.ToString());
				rowH.Cells[1].AddParagraph(item.Asset.Name);
				rowH.Cells[1].MergeRight = 2;
				rowH.Cells[4].AddParagraph(item.Asset.SerialNumber);
				rowH.Cells[4].MergeRight = 1;
				rowH.Cells[6].AddParagraph(item.Asset.InvNo + "/" + item.Asset.SubNo);
				rowH.Cells[7].AddParagraph(String.Format("{0:#,##0.##}", item.Asset.Quantity));
				rowH.Cells[8].AddParagraph(String.Format("{0:#,##0.##}", item.Asset.ValueInv));
			}

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(100, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("TOTAL");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Left;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 6;

			rowH.Cells[7].AddParagraph(String.Format("{0:#,##0.##}", emailManagers.Sum(a => a.Asset.Quantity)));
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[8].AddParagraph(String.Format("{0:#,##0.##}", emailManagers.Sum(a => a.Asset.ValueInv)));
			rowH.Cells[8].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[8].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[8].Format.Font.Size = 7;
			rowH.Cells[8].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH = table.AddRow();
			rowH.Cells[0].Format.Font.Size = 8;
			rowH.Cells[0].MergeRight = 8;
			rowH.Height = "0.5cm";


			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("DETALII PREDATOR");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 8;

			//rowH.Cells[4].AddParagraph("");
			//rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[4].Format.Font.Size = 7;
			//rowH.Cells[4].Format.Font.Bold = true;

			//rowH.Cells[5].AddParagraph("");
			//rowH.Cells[5].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[5].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[5].Format.Font.Size = 7;
			//rowH.Cells[5].Format.Font.Bold = true;
			//rowH.Cells[5].MergeRight = 1;

			//rowH.Cells[7].AddParagraph("");
			//rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[7].Format.Font.Size = 7;
			//rowH.Cells[7].Format.Font.Bold = true;
			//rowH.Cells[7].MergeRight = 1;



			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(100, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("Marca");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[1].AddParagraph("Nume");
			rowH.Cells[1].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[1].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[1].Format.Font.Size = 7;
			rowH.Cells[1].Format.Font.Bold = true;

			rowH.Cells[2].AddParagraph("Prenume");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 7;
			rowH.Cells[2].Format.Font.Bold = true;

			rowH.Cells[3].AddParagraph("Centru de cost");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 7;
			rowH.Cells[3].Format.Font.Bold = true;
			//rowH.Cells[4].MergeRight = 2;

			rowH.Cells[4].AddParagraph("Departament");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph("B.U.");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 7;
			rowH.Cells[6].Format.Font.Bold = true;
			//rowH.Cells[4].MergeRight = 2;

			rowH.Cells[7].AddParagraph("Aprobat");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;

			//FormattedText ftextSrcEmployeeManager = new FormattedText();
			//ftextSrcEmployeeManager.AddText($"\r\n");
			//ftextSrcEmployeeManager.Color = Colors.Blue;

			//rowH.Cells[7].AddParagraph().Add(ftextSrcEmployeeManager);
			//rowH.Cells[7].AddParagraph().AddFormattedText("");
			//rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[7].Format.Font.Size = 6;
			//rowH.Cells[7].Format.Font.Bold = true;
			//rowH.Cells[7].MergeRight = 1;
			//rowH.Cells[7].MergeDown = 1;
			//rowH.Cells[7].Shading.Color = Color.FromRgb(255, 255, 255);


			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.8cm";
			rowH.Borders.Visible = true;

			rowH.Cells[0].AddParagraph($"{srcEmployee.InternalCode}");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 6;
			rowH.Cells[0].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[1].AddParagraph($"{srcEmployee.FirstName}");
			rowH.Cells[1].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[1].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[1].Format.Font.Size = 6;
			rowH.Cells[1].Format.Font.Bold = true;

			rowH.Cells[2].AddParagraph($"{srcEmployee.LastName}");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 6;
			rowH.Cells[2].Format.Font.Bold = true;

			rowH.Cells[3].AddParagraph($"{(srcEmployee.CostCenter != null ? srcEmployee.CostCenter.Code : "-")}");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 6;
			rowH.Cells[3].Format.Font.Bold = true;
			//rowH.Cells[3].MergeRight = 2;

			rowH.Cells[4].AddParagraph($"{(srcEmployee.CostCenter != null && srcEmployee.CostCenter.Division != null ? srcEmployee.CostCenter.Division.Name : "-")}");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 6;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph($"{(srcEmployee.CostCenter != null && srcEmployee.CostCenter.Division != null && srcEmployee.CostCenter.Division.Department != null ? srcEmployee.CostCenter.Division.Department.Name : "-")}");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 6;
			rowH.Cells[6].Format.Font.Bold = true;
			//rowH.Cells[6].MergeRight = 2;

			FormattedText ftextSrcEmployee = new FormattedText();
			ftextSrcEmployee.AddText($"{empIni}\r\n");
			ftextSrcEmployee.Color = Colors.Blue;
			rowH.Cells[7].AddParagraph().Add(ftextSrcEmployee);
			rowH.Cells[7].AddParagraph().AddFormattedText($"{String.Format("{0:dd/MM/yyyy HH:mm:ss}", srcEmployeeValidateAt)}");

			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 6;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;
			rowH.Cells[7].Shading.Color = Color.FromRgb(255, 255, 255);



			// primitor //

			rowH = table.AddRow();
			rowH.Cells[0].Format.Font.Size = 8;
			rowH.Cells[0].MergeRight = 8;
			rowH.Height = "0.5cm";

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("DETALII PRIMITOR");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 8;

			//rowH.Cells[4].AddParagraph($"{dstDepartment.Name}");
			//rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[4].Format.Font.Size = 7;
			//rowH.Cells[4].Format.Font.Bold = true;

			//rowH.Cells[5].AddParagraph($"{dstDivision.Name}");
			//rowH.Cells[5].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[5].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[5].Format.Font.Size = 7;
			//rowH.Cells[5].Format.Font.Bold = true;
			//rowH.Cells[5].MergeRight = 1;

			//rowH.Cells[7].AddParagraph("APROBAT");
			//rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[7].Format.Font.Size = 7;
			//rowH.Cells[7].Format.Font.Bold = true;
			//rowH.Cells[7].MergeRight = 1;



			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(100, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("Marca");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[1].AddParagraph("Nume");
			rowH.Cells[1].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[1].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[1].Format.Font.Size = 7;
			rowH.Cells[1].Format.Font.Bold = true;

			rowH.Cells[2].AddParagraph("Prenume");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 7;
			rowH.Cells[2].Format.Font.Bold = true;

			rowH.Cells[3].AddParagraph("Centru de cost");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 7;
			rowH.Cells[3].Format.Font.Bold = true;
			//rowH.Cells[4].MergeRight = 2;

			rowH.Cells[4].AddParagraph("Departament");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph("B.U.");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 7;
			rowH.Cells[6].Format.Font.Bold = true;
			//rowH.Cells[4].MergeRight = 2;

			rowH.Cells[7].AddParagraph("Aprobat");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;

			//FormattedText ftextDstEmployeeManager = new FormattedText();
			//ftextDstEmployeeManager.AddText($"{dstEmployeeManagerValidateUser.UserName}\r\n");
			//ftextDstEmployeeManager.Color = Colors.Blue;
			//rowH.Cells[7].AddParagraph().Add(ftextDstEmployeeManager);
			//rowH.Cells[7].AddParagraph().AddFormattedText($"{String.Format("{0:dd/MM/yyyy HH:mm:ss}", dstEmployeeManagerValidateAt)}");

			//rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			//rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			//rowH.Cells[7].Format.Font.Size = 6;
			//rowH.Cells[7].Format.Font.Bold = true;
			//rowH.Cells[7].MergeRight = 1;
			//rowH.Cells[7].MergeDown = 1;
			//rowH.Cells[7].Shading.Color = Color.FromRgb(255, 255, 255);


			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.8cm";
			rowH.Borders.Visible = true;

			rowH.Cells[0].AddParagraph($"{dstEmployee.InternalCode}");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 6;
			rowH.Cells[0].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[1].AddParagraph($"{dstEmployee.FirstName}");
			rowH.Cells[1].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[1].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[1].Format.Font.Size = 6;
			rowH.Cells[1].Format.Font.Bold = true;

			rowH.Cells[2].AddParagraph($"{dstEmployee.LastName}");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 6;
			rowH.Cells[2].Format.Font.Bold = true;

			rowH.Cells[3].AddParagraph($"{(dstEmployee.CostCenter != null ? dstEmployee.CostCenter.Code : "-")}");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 6;
			rowH.Cells[3].Format.Font.Bold = true;
			//rowH.Cells[3].MergeRight = 2;

			rowH.Cells[4].AddParagraph($"{(dstEmployee.CostCenter != null && dstEmployee.CostCenter.Division != null ? dstEmployee.CostCenter.Division.Name : "-")}");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 6;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph($"{(dstEmployee.CostCenter != null && dstEmployee.CostCenter.Division != null && dstEmployee.CostCenter.Division.Department != null ? dstEmployee.CostCenter.Division.Department.Name : "-")}");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 6;
			rowH.Cells[6].Format.Font.Bold = true;
			//rowH.Cells[6].MergeRight = 2;

			FormattedText ftextDstEmployee = new FormattedText();
			ftextDstEmployee.AddText($"{empFin}\r\n");
			ftextDstEmployee.Color = Colors.Blue;
			rowH.Cells[7].AddParagraph().Add(ftextDstEmployee);
			rowH.Cells[7].AddParagraph().AddFormattedText($"{String.Format("{0:dd/MM/yyyy HH:mm:ss}", dstEmployeeValidateAt)}");

			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 6;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;
			rowH.Cells[7].Shading.Color = Color.FromRgb(255, 255, 255);

			// primitor //


			rowH = table.AddRow();
			rowH.Cells[0].Format.Font.Size = 8;
			rowH.Cells[0].MergeRight = 8;
			rowH.Height = "0.5cm";

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.6cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(4, 50, 125);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("DETALII TRANSFER");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 8;

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(100, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("ACTUAL");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 3;

			rowH.Cells[4].AddParagraph("DESTINATIE");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 4;

			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.5cm";
			rowH.Borders.Visible = true;
			rowH.Shading.Color = Color.FromRgb(100, 145, 217);
			rowH.Format.Font.Color = Color.FromRgb(255, 255, 255);

			rowH.Cells[0].AddParagraph("Centru de cost");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 7;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 1;

			rowH.Cells[2].AddParagraph("Departament");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 7;
			rowH.Cells[2].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[3].AddParagraph("Business Unit");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 7;
			rowH.Cells[3].Format.Font.Bold = true;
			//rowH.Cells[3].MergeRight = 1;


			rowH.Cells[4].AddParagraph("Centru de cost");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 7;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph("Departament");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 7;
			rowH.Cells[6].Format.Font.Bold = true;
			//rowH.Cells[0].MergeRight = 1;

			rowH.Cells[7].AddParagraph("Business Unit");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 7;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;


			rowH = table.AddRow();
			rowH.HeightRule = RowHeightRule.Exactly;
			rowH.Height = "0.8cm";
			rowH.Borders.Visible = true;

			rowH.Cells[0].AddParagraph($"{srcCostCenter.Code}");
			rowH.Cells[0].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[0].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[0].Format.Font.Size = 6;
			rowH.Cells[0].Format.Font.Bold = true;
			rowH.Cells[0].MergeRight = 1;

			rowH.Cells[2].AddParagraph($"{srcDivision.Name}");
			rowH.Cells[2].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[2].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[2].Format.Font.Size = 6;
			rowH.Cells[2].Format.Font.Bold = true;
			//rowH.Cells[2].MergeRight = 1;

			rowH.Cells[3].AddParagraph($"{srcDepartment.Name}");
			rowH.Cells[3].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[3].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[3].Format.Font.Size = 6;
			rowH.Cells[3].Format.Font.Bold = true;
			//rowH.Cells[2].MergeRight = 1;


			rowH.Cells[4].AddParagraph($"{dstCostCenter.Code}");
			rowH.Cells[4].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[4].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[4].Format.Font.Size = 6;
			rowH.Cells[4].Format.Font.Bold = true;
			rowH.Cells[4].MergeRight = 1;

			rowH.Cells[6].AddParagraph($"{dstDivision.Name}");
			rowH.Cells[6].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[6].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[6].Format.Font.Size = 6;
			rowH.Cells[6].Format.Font.Bold = true;
			//rowH.Cells[2].MergeRight = 1;

			rowH.Cells[7].AddParagraph($"{dstDepartment.Name}");
			rowH.Cells[7].Format.Alignment = ParagraphAlignment.Center;
			rowH.Cells[7].VerticalAlignment = VerticalAlignment.Center;
			rowH.Cells[7].Format.Font.Size = 6;
			rowH.Cells[7].Format.Font.Bold = true;
			rowH.Cells[7].MergeRight = 1;



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

		//private void AddEPFooter2(Section section, Document document)
		//{

		//	var pageNumber = section.Footers.Primary.AddParagraph();

		//	DocumentRenderer docRenderer = new DocumentRenderer(document);
		//	docRenderer.PrepareDocument();
		//	int pageCount = docRenderer.FormattedDocument.PageCount;

		//	HeaderFooter header = section.Footers.Primary;

		//	var tableLogo = header.AddTable();
		//	tableLogo.AddColumn("8cm");
		//	tableLogo.AddColumn("4cm");
		//	tableLogo.AddColumn("6cm");
		//	tableLogo.Borders.Visible = false;
		//	var rowLogo = tableLogo.AddRow();
		//	rowLogo.HeightRule = RowHeightRule.Exactly;
		//	rowLogo.Height = "1.0cm";
		//	var paragraph = rowLogo.Cells[0].AddParagraph("Informatiile continute in prezentul document reprezinta SECRET PROFESIONAL IN DOMENIUL BANCAR, conform dispozitiilor OUG nr. 99 / 2006, iar nerespectarea acestor dispozitii constituie o incalcare atat a legii civile cat si a legii penale");
		//	paragraph.Format.Alignment = ParagraphAlignment.Center;
		//	rowLogo.Cells[0].Format.Font.Color = Colors.Gray;
		//	rowLogo.Cells[0].MergeRight = 2;
		//	rowLogo.VerticalAlignment = VerticalAlignment.Center;
		//	paragraph.Format.Font.Size = 7;
		//}
	}
}
