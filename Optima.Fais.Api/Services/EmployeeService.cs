using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public class EmployeeService : IEmployeeService
    {

        public EmployeeService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        public async Task<List<Model.EmployeeEmailResult>> GetEmployeesAsync()
        {

            using (var scope = Services.CreateScope())
            {
                var dbContext =
                   scope.ServiceProvider
                       .GetRequiredService<ApplicationDbContext>();



                List<Model.EmployeeEmailResult> employees = await dbContext.Set<Model.EmployeeEmailResult>().FromSql("GetEmployeeNewAssetListEmail").ToListAsync();
				//List<Model.EmployeeEmailResult> employees = await dbContext.Set<Model.EmployeeEmailResult>().FromSql("GetEmployeeListEmailNew").ToListAsync();

				return employees;
            }
  
        }

		public async Task<Model.EmployeeEmailStatusResult> SearchNewAppendixTransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "APPENDIXTRANSFERASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "APPENDIXTRANSFERASSET",
						Name = "Sincronizare bonuri PDF noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;
					Model.AppState appState = null;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "FINAL_VALIDATE").FirstOrDefaultAsync();

						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(
							com => 
							com.IsDeleted == false && 
							com.NotSync == true && 
							com.NotCompletedSync == true && 
							com.SyncErrorCount < 3 &&
							com.GenerateBookErrorCount < 3 &&
							com.AppStateId == appState.Id)
							.GroupBy(a => a.DocumentNumber)
							.Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							try
							{
								var res = await emailStatusService.GenerateAppendixAsync(emails[i].DocumentNumber);

								if (res)
								{
									var rslt = await emailStatusService.SendFinalNotification(emails[i].DocumentNumber);

									if (rslt)
									{
										return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
									}
								}
							}
							catch (Exception ex)
							{

								using (var errorfile = System.IO.File.CreateText("DOCUMENT-" + DateTime.Now.Ticks + ".txt"))
								{
									errorfile.WriteLine(ex.StackTrace);
									errorfile.WriteLine(ex.ToString());
								};


							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewManagerTransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "MANAGERTRANSFERASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "MANAGERTRANSFERASSET",
						Name = "Sincronizare transferuri manager noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(com => com.IsDeleted == false && com.NotDstManagerSync == true && com.DstManagerEmailSend == false && com.SyncDstManagerErrorCount < 5).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailStatusService.SendDstManagerNotification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeTransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMPLOYEETRANSFERASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMPLOYEETRANSFERASSET",
						Name = "Sincronizare transferuri angajati noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(com => com.IsDeleted == false && com.NotDstEmployeeSync == true && com.DstEmployeeEmailSend == false && com.SyncErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STOCKEMAILSTATUS").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}
							
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder1TransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMPLOYEETRANSFERREMINDER1").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMPLOYEETRANSFERREMINDER1",
						Name = "Sincronizare transferuri angajati noi - reminder 1",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						DateTime reminderDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0).AddDays(1);

						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(com => com.IsDeleted == false && com.NotDstEmployeeSync == false && com.DstEmployeeEmailSend == true && com.DstEmployeeReminder1EmailSend == false && (reminderDate < com.CreatedAt) && com.SyncErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendDstEmployeeReminder1Notification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STOCKEMAILSTATUS").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeReminder1ErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder2TransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMPLOYEETRANSFERREMINDER2").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMPLOYEETRANSFERREMINDER2",
						Name = "Sincronizare transferuri angajati noi - reminder 2",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(com => com.IsDeleted == false && com.NotDstEmployeeSync == true && com.DstEmployeeEmailSend == false && com.NotDstEmployeeReminder2Sync == true && com.SyncErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendDstEmployeeReminder2Notification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STOCKEMAILSTATUS").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeReminder2ErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder3TransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMPLOYEETRANSFERREMINDER3").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMPLOYEETRANSFERREMINDER3",
						Name = "Sincronizare transferuri angajati noi - reminder 3",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(com => com.IsDeleted == false && com.NotDstEmployeeSync == true && com.DstEmployeeEmailSend == false && com.NotDstEmployeeReminder3Sync == true && com.SyncErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendDstEmployeeReminder3Notification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STOCKEMAILSTATUS").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeReminder3ErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder4TransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMPLOYEETRANSFERREMINDER4").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMPLOYEETRANSFERREMINDER4",
						Name = "Sincronizare transferuri angajati noi - reminder 4",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(com => com.IsDeleted == false && com.NotDstEmployeeSync == true && com.DstEmployeeEmailSend == false && com.NotDstEmployeeReminder4Sync == true && com.SyncErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendDstEmployeeReminder4Notification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STOCKEMAILSTATUS").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeReminder4ErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewRejectedByStockTransferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMP-TR-STOCK-REJECTED").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMP-TR-STOCK-REJECTED",
						Name = "Sincronizare transferuri refuzate de stock IT",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;
					Model.AppState appState = null;

					appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_DECLINED").FirstOrDefaultAsync();

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(
							com => com.IsDeleted == false && 
							com.NotCompletedSync == true && 
							com.NotSync == true && 
							com.EmailSend == false && 
							com.AppStateId == appState.Id && 
							com.SyncErrorCount < 3)
							.GroupBy(a => a.DocumentNumber)
							.Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendRejectedNotification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STOCKEMAILSTATUS").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.EmployeeEmailStatusResult> SearchNewRejectedAccountingValidationAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailStatusService = scope.ServiceProvider.GetRequiredService<IEmailStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "ACCOUNTING-REJECTED").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "ACCOUNTING-REJECTED",
						Name = "Sincronizare validari refuzate de contabilitate",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;
					List<Model.EmailStatus> emailStatuses = null;
					Model.Error error = null;
					Model.ErrorType errorType = null;
					Model.AppState appState = null;

					appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "REJECTASSET").FirstOrDefaultAsync();

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.EmailStatus> emails = dbContext.Set<Model.EmailStatus>()
							.Where(
							com => com.IsDeleted == false && 
							com.NotCompletedSync == true && 
							com.NotSync == true && 
							com.EmailSend == false && 
							com.AppStateId == appState.Id && 
							com.SyncErrorCount < 3)
							.GroupBy(a => a.DocumentNumber)
							.Select(a => new Dto.EmailStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							//var rslt = await emailStatusService.SendDstEmployeeNotification(emails[i].DocumentNumber);
							//if (rslt)
							//{
							//	return new Model.EmployeeEmailStatusResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							//}

							try
							{
								await emailStatusService.SendRejectedAccountingNotification(emails[i].DocumentNumber);
							}
							catch (Exception ex)
							{
								emailStatuses = await dbContext.Set<Model.EmailStatus>().Where(a => a.DocumentNumber == emails[i].DocumentNumber && a.IsDeleted == false).ToListAsync();
								errorType = await dbContext.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "REJECTASSET").FirstOrDefaultAsync();

								for (int e = 0; e < emailStatuses.Count; e++)
								{
									error = new Model.Error()
									{
										Code = errorType.Code,
										CreatedAt = DateTime.Now,
										CreatedBy = null,
										ModifiedAt = DateTime.Now,
										ModifiedBy = null,
										ErrorTypeId = errorType.Id,
										IsDeleted = false,
										Name = ex.StackTrace.ToString()
									};

									emailStatuses[e].Error = error;
									emailStatuses[e].SyncDstEmployeeErrorCount++;
									dbContext.Update(emailStatuses[e]);
									dbContext.Add(error);
								}

								dbContext.SaveChanges();
							}

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.EmployeeEmailStatusResult { Success = false, ErrorMessage = "" };
		}
	}
}

