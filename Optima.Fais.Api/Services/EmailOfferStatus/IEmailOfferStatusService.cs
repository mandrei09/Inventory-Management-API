using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IEmailOfferStatusService
	{
		Task<bool> SendNotification(int documentNumber);
	}
}
