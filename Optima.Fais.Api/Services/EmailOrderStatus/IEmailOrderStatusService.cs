using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IEmailOrderStatusService
	{
		Task<bool> SendB1Notification(int documentNumber);
		Task<bool> SendL4Notification(int documentNumber);
		Task<bool> SendL3Notification(int documentNumber);
		Task<bool> SendL2Notification(int documentNumber);
		Task<bool> SendL1Notification(int documentNumber);
		Task<bool> SendS3Notification(int documentNumber);
		Task<bool> SendS2Notification(int documentNumber);
		Task<bool> SendS1Notification(int documentNumber);
		Task<bool> SendAcceptedNotification(int documentNumber);
		Task<bool> SendNeedBudgetNotification(int orderId, int? requestBudgetForecastId);
		Task<bool> SendL4RequesterNotification(int documentNumber);
		Task<bool> SendL3RequesterNotification(int documentNumber);
		Task<bool> SendL2RequesterNotification(int documentNumber);
		Task<bool> SendL1RequesterNotification(int documentNumber);
		Task<bool> SendS3RequesterNotification(int documentNumber);
		Task<bool> SendS2RequesterNotification(int documentNumber);
		Task<bool> SendS1RequesterNotification(int documentNumber);
	}
}
