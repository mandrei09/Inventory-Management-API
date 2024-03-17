using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IRequestsService
	{
		Task<bool> SendRequestNeedBudget(int requestId);
		Task<bool> SendRequestResponseNeedBudget(int requestId, int budgetBaseId);

		Task<bool> SendRequestBudgetForecastNeedBudget(int requestBudgetForecastId);
		Task<bool> SendRequestTransferBudget(int budgetBaseOpId, string requester);
		Task<bool> SendRequestResponseTransferBudget(int budgetBaseOpId, string requester);
	}
}
