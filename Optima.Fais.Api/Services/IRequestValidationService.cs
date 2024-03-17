using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface IRequestValidationService
    {
		Task<Model.RequestValidationResult> SearchNewEmailL4RequestAsync();
		Task<Model.RequestValidationResult> SearchNewEmailNeedOrderBudgetAsync();
		Task<Model.RequestValidationResult> SearchNewEmailNeedOrderResponseBudgetAsync();
	}
}
