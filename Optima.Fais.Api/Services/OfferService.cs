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
    public class OfferService : IOfferService
	{

        public OfferService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

		public async Task<Model.OfferResult> SearchNewEmailOfferAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				var emailOfferStatusService = scope.ServiceProvider.GetRequiredService<IEmailOfferStatusService>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "OFFERUI_DOCUMENT").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				Model.AppState appState = null;
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "OFFERUI_DOCUMENT",
						Name = "Sincronizare oferta furnizori",
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
						appState = await dbContext.Set<Model.AppState>().Where(a => a.Code == "OFFERUI_DOCUMENT").SingleAsync();

						List<Dto.EmailOfferStatus> emails = dbContext.Set<Model.EmailOfferStatus>()
							.Where(com => com.AppStateId == appState.Id && com.IsDeleted == false && com.NotSync == true && com.EmailSkip == false && com.EmailSend == false && com.SyncErrorCount < 3).GroupBy(a => a.DocumentNumber).Select(a => new Dto.EmailOfferStatus()
							{
								DocumentNumber = a.Key,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < emails.Count; i++)
						{
							var rslt = await emailOfferStatusService.SendNotification(emails[i].DocumentNumber);

							if (rslt)
							{
								return new Model.OfferResult { Success = true, Message = "Notificarea a fost trimisa cu succes!" };
							}
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.OfferResult { Success = false, Message = "" };
		}

	}
}

