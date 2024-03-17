using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IOrdersService
	{
		Task<bool> SendOrderNeedBudget(int orderId);
		Task<bool> SendOrderResponseNeedBudget(int orderId, int budgetBaseId);
	}
}
