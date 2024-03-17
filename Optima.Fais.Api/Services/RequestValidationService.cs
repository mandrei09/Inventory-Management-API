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
    public class RequestValidationService : IRequestValidationService
	{

        public RequestValidationService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

		public async Task<Model.RequestValidationResult> SearchNewEmailNeedOrderResponseBudgetAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailRequestStatusService = scope.ServiceProvider.GetRequiredService<IEmailRequestStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAIL_NEED_REQUEST_RESPONSE").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				//Model.EmailType emailType = null;

				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAIL_NEED_REQUEST_RESPONSE",
						Name = "Sincronizare email suplimentare buget P.R. raspuns",
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
						//emailType = await dbContext.Set<Model.EmailType>().Where(c => c.Code == "ORDER_LEVEL4").SingleAsync();

						List<Dto.EmailRequestStatus> emails = dbContext.Set<Model.EmailRequestStatus>()
							.Where(com => com.IsDeleted == false && com.NotEmployeeL4Sync == true && com.EmployeeL4EmailSend == false && com.SyncEmployeeL4ErrorCount < 3).GroupBy(a => new { a.RequestBudgetForecastId, a.RequestId }).Select(a => new Dto.EmailRequestStatus()
							{
								RequestId = a.Key.RequestId,
								RequestBudgetForecastId = a.Key.RequestBudgetForecastId,
								DocumentNumber = a.FirstOrDefault().DocumentNumber
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailRequestStatusService.SendNeedBudgetResponseNotification(emails[i].RequestId, emails[i].RequestBudgetForecastId, emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.RequestValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.RequestValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.RequestValidationResult> SearchNewEmailNeedOrderBudgetAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailRequestStatusService = scope.ServiceProvider.GetRequiredService<IEmailRequestStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAIL_NEED_REQUEST_BUDGET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.EmailType emailType = null;

				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAIL_NEED_REQUEST_BUDGET",
						Name = "Sincronizare email suplimentare buget P.R.",
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

						List<Dto.EmailRequestStatus> emails = dbContext.Set<Model.EmailRequestStatus>()
							.Where(com => com.IsDeleted == false && com.NotNeedBudgetSync == true && com.NeedBudgetEmailSend == false && com.SyncNeedBudgetErrorCount < 3).GroupBy(a => new { a.RequestBudgetForecastId, a.RequestId }).Select(a => new Dto.EmailRequestStatus()
							{
								RequestId = a.Key.RequestId,
								RequestBudgetForecastId = a.Key.RequestBudgetForecastId,
								DocumentNumber = a.FirstOrDefault().DocumentNumber
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailRequestStatusService.SendNeedBudgetNotification(emails[i].RequestId, emails[i].RequestBudgetForecastId, emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.RequestValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.RequestValidationResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.RequestValidationResult> SearchNewEmailL4RequestAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailRequestStatusService = scope.ServiceProvider.GetRequiredService<IEmailRequestStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "EMAILREQUEST_L4").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "EMAILREQUEST_L4",
						Name = "Sincronizare email P.R. aprobare L4",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "NEW").SingleAsync();

						List<Dto.EmailRequestStatus> emails = dbContext.Set<Model.EmailRequestStatus>()
							.Where(
							com => com.AppStateId == appState.Id && 
							com.IsDeleted == false && 
							com.NotEmployeeL4Sync == true && 
							com.EmployeeL4EmailSkip == false && 
							com.EmployeeL4EmailSend == false && 
							com.SyncEmployeeL4ErrorCount < 3)
							.GroupBy(a => a.DocumentNumber)
							.Select(a => new Dto.EmailRequestStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailRequestStatusService.SendL4Notification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.RequestValidationResult { Success = true, ErrorMessage = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.RequestValidationResult { Success = false, ErrorMessage = "" };
		}

	}
}

