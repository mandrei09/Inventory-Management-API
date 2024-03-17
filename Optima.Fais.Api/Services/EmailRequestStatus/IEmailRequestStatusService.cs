using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IEmailRequestStatusService
	{
		Task<bool> SendL4Notification(int documentNumber);
		Task<bool> SendNeedBudgetNotification(int requestId, int? requestBudgetForecastId, int documentNumber);
		Task<bool> SendNeedBudgetResponseNotification(int requestId, int? requestBudgetForecastId, int documentNumber);
	}
}
