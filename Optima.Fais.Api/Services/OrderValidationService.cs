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
    public class OrderValidationService : IOrderValidationService
	{

        public OrderValidationService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

		public async Task<Model.OrderValidationResult> SearchNewEmailNeedOrderBudgetAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAIL_NEED_ORDER_BUDGET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.EmailType emailType = null;

				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAIL_NEED_ORDER_BUDGET",
						Name = "Sincronizare email lipsa buget",
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
						emailType = await dbContext.Set<Model.EmailType>().Where(c => c.Code == "NEED_BUDGET").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotNeedBudgetSync == true && com.NeedBudgetEmailSend == false && com.SyncNeedBudgetErrorCount < 3).GroupBy(a => new { a.RequestBudgetForecastId, a.OrderId }).Select(a => new Dto.EmailOrderStatus()
							{
								OrderId = a.Key.OrderId,
								RequestBudgetForecastId = a.Key.RequestBudgetForecastId,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendNeedBudgetNotification(emails[i].OrderId, emails[i].RequestBudgetForecastId);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailB1OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_B1").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_B1",
						Name = "Sincronizare email aprobare B1",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVELB1").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.AppStateId == appState.Id && com.IsDeleted == false && com.NotEmployeeB1Sync == true && com.EmployeeB1EmailSkip == false && com.EmployeeB1EmailSend == false && com.SyncEmployeeB1ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendB1Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailL4OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_L4").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_L4",
						Name = "Sincronizare email aprobare L4",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVEL4").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.AppStateId == appState.Id && com.IsDeleted == false && com.NotEmployeeL4Sync == true && com.EmployeeL4EmailSkip == false && com.EmployeeL4EmailSend == false && com.SyncEmployeeL4ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendL4Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailL3OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_L3").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_L3",
						Name = "Sincronizare email aprobare L3",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVEL3").SingleAsync();
						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeL3Sync == true && com.EmployeeL3EmailSkip == false && com.EmployeeL3EmailSend == false && com.SyncEmployeeL3ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendL3Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailL2OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_L2").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_L2",
						Name = "Sincronizare email aprobare L2",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVEL2").SingleAsync();
						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeL2Sync == true && com.EmployeeL2EmailSkip == false && com.EmployeeL2EmailSend == false && com.SyncEmployeeL2ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendL2Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailL1OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_L1").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_L1",
						Name = "Sincronizare email aprobare L1",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVEL1").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeL1Sync == true && com.EmployeeL1EmailSkip == false && com.EmployeeL1EmailSend == false && com.SyncEmployeeL1ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendL1Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailS3OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_S3").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_S3",
						Name = "Sincronizare email aprobare S3",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVELS3").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeS3Sync == true && com.EmployeeS3EmailSkip == false && com.EmployeeS3EmailSend == false && com.SyncEmployeeS3ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendS3Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailS2OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_S2").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_S2",
						Name = "Sincronizare email aprobare S2",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVELS2").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeS2Sync == true && com.EmployeeS2EmailSkip == false && com.EmployeeS2EmailSend == false && com.SyncEmployeeS2ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendS2Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailS1OrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILORDER_S1").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILORDER_S1",
						Name = "Sincronizare email aprobare S1",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ORDER_LEVELS1").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeS1Sync == true && com.EmployeeS1EmailSkip == false && com.EmployeeS1EmailSend == false && com.SyncEmployeeS1ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendS1Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.OrderValidationResult> SearchNewEmailAcceptedOrderAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOrderStatusService = scope.ServiceProvider.GetRequiredService<IEmailOrderStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "ACCEPTED").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "ACCEPTED",
						Name = "Sincronizare email flux finalizat",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "ACCEPTED").SingleAsync();

						List<Dto.EmailOrderStatus> emails = dbContext.Set<Model.EmailOrderStatus>()
							.Where(com => com.IsDeleted == false && com.NotCompletedSync == true && com.EmailSend == false && com.AppStateId == appState.Id && com.SyncEmployeeS1ErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOrderStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOrderStatusService.SendAcceptedNotification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OrderValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OrderValidationResult { Success = false, ErrorMessage = "" };
		}

	}
}

