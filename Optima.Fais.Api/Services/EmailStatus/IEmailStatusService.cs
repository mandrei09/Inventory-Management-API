using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IEmailStatusService
	{
		Task<bool> SendDstEmployeeNotification(int documentNumber);
		Task<bool> SendDstEmployeeReminder1Notification(int documentNumber);
		Task<bool> SendDstEmployeeReminder2Notification(int documentNumber);
		Task<bool> SendDstEmployeeReminder3Notification(int documentNumber);
		Task<bool> SendDstEmployeeReminder4Notification(int documentNumber);
		Task<bool> SendDstManagerNotification(int documentNumber);
		Task<bool> SendFinalNotification(int documentNumber);
		Task<bool> GenerateAppendixAsync(int documentNumber);
		Task<bool> SendRejectedNotification(int documentNumber);
		Task<bool> SendRejectedAccountingNotification(int documentNumber);
	}
}
