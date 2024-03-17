
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class SFTPService : BackgroundService
	{

		public SFTPService(IServiceProvider services)
		{
			Services = services;
		}

		public IServiceProvider Services { get; }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

					if (!Directory.Exists(path))
					{
						StringHelpers.SFTP_Connect_And_Download_Sample();
					}


					//ImportDimensionEmployee();
					//ImportDimensionProject(); // OK
					
					// ImportDimensionBudgetManager(); // OK
					// ImportDimensionBudgeLine();  // OK
					// ImportDimensionRunChange(); // OK
					// ImportDimensionInterCompany(); // OK
					// ImportDimensionAnalysisCenter(); // OK
					// ImportDimensionItem(); // OK
					// ImportDimensionPartner(); // OK
					// ImportDimensionAddressCode();



					//ImportDimensionPartnerLocation();


					//ImportCRDimensionAddressCode_v2();

					//ImportDimensionSite();


				}
				catch (Exception ex)
				{
					string s = ex.ToString();
				}

				await Task.Delay(3000000, stoppingToken);
			}


		}


		public void ImportDimensionProject()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionProject.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionProject.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Project").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 5)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateProject {0}, {1}, {2}, {3}", lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}

		public void ImportDimensionBudgetManager()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionBudgetManager.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionBudgetManager.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "BudgetManager").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									

									if (lineToInsert.Length == 4 || lineToInsert.Length == 5)
									{
										var vvv = lineToInsert[2].Trim();

									
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateBudgetManager {0}, {1}, {2}, {3}", lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3]).Single();
									}

									
								}
							}
						}
					}
				}
			}
			return;

		}


		public void ImportDimensionBudgeLine()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionBugetLine.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionBugetLine.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var IdAdministration = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Administration").Single();
							var IdDivision = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Division").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 5)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateBudgetLine {0}, {1}, {2}, {3}", lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3]).Single();
									}


								}
							}
						}
					}
				}
			}
			return;

		}


		public void ImportDimensionEmployee()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionEmployee.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionEmployee.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var IdEmployee = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Employee").Single();
							var IdDepartment = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Department").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 9)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateEmployee {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", 
											lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3], lineToInsert[4], lineToInsert[5], lineToInsert[6], lineToInsert[7]).Single();
									}


								}
							}
						}
					}
				}
			}
			return;

		}


		public void ImportDimensionInterCompany()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\Dimension" + "InterCompanyCode.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\Dimension" + "InterCompanyCode.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "InterCompany").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 4)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateInterCompany {0}, {1}, {2}",
											lineToInsert[0], lineToInsert[1], lineToInsert[2]).Single();
									}


								}
							}
						}
					}
				}
			}
			return;

		}

		public void ImportDimensionRunChange()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionRunChange.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionRunChange.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Dimension").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 4)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateRunChange {0}, {1}, {2}", lineToInsert[0], lineToInsert[1], lineToInsert[2]).Single();
									}


								}
							}
						}
					}
				}
			}
			return;

		}

		public void ImportDimensionPartnerLocation()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\Partner.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\Partner.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 11)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdatePartnerLocation {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
											lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3], lineToInsert[4], lineToInsert[5], lineToInsert[6], lineToInsert[7], lineToInsert[8], lineToInsert[9]).Single();
									}


								}
							}
						}
					}
				}
			}
			return;

		}

		public void ImportDimensionItem()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\Item.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\Item.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id1 = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "DictionaryItem").Single();
							var Id2 = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "DictionaryType").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 10)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateItem {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
											lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3], lineToInsert[4], lineToInsert[5], lineToInsert[6], lineToInsert[7], lineToInsert[8]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}


		public void ImportDimensionPartner()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\Partner.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\Partner.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Partner").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 10 || lineToInsert.Length == 11)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdatePartner {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
											lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3], lineToInsert[4], lineToInsert[5], lineToInsert[6], lineToInsert[7], lineToInsert[8], lineToInsert[9]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}

		public void ImportDimensionAnalysisCenter()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\Dimension" + "AnalysisCenter.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\Dimension" + "AnalysisCenter.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "CostCenter").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 5)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateAnalysisCenter {0}, {1}, {2}, {3}", lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}



		public void ImportCRDimensionAddressCode_v2()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\CRDimensionAddressCode_v2.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\CRDimensionAddressCode_v2.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 8)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateCRDimensionAddressCode_v2 {0}, {1}, {2}, {3}, {4}, {5}, {6}", 
											lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3], lineToInsert[4], lineToInsert[5], lineToInsert[6]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}


		public void ImportDimensionAddressCode()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\GetDimensionAddressCode.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\GetDimensionAddressCode.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{
							var Id = dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Partner").Single();

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 10)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateDimensionAddressCode {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
											lineToInsert[0], lineToInsert[1], lineToInsert[2], lineToInsert[3], lineToInsert[4], lineToInsert[5], lineToInsert[6], lineToInsert[7], lineToInsert[8]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}


		public void ImportDimensionSite()
		{

			var path = Path.Combine("download", DateTime.UtcNow.ToString("yyyyMMdd"));

			if (Directory.Exists(path))
			{
				if (File.Exists(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionSite.txt"))
				{
					FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\" + path + @"\DimensionSite.txt", FileMode.Open);


					using (var scope = Services.CreateScope())
					{
						var dbContext =
						   scope.ServiceProvider
							   .GetRequiredService<ApplicationDbContext>();

						using (StreamReader reader = new StreamReader(fileStream))
						{

							while (!reader.EndOfStream)
							{
								string line = reader.ReadLine();

								if (line != "")
								{
									string[] lineToInsert = line.Split(";");

									if (lineToInsert.Length == 3)
									{
										var ProjectId = dbContext.Set<Model.DimensionERP>().FromSql("AddOrUpdateDimensionSite {0}, {1}, {2}",
											lineToInsert[0], lineToInsert[1], lineToInsert[2]).Single();
									}
								}
							}
						}
					}
				}
			}
			return;

		}


	}
}
