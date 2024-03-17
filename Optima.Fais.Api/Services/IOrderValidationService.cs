using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface IOrderValidationService
    {
		Task<Model.OrderValidationResult> SearchNewEmailB1OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailL4OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailL3OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailL2OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailL1OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailS3OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailS2OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailS1OrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailAcceptedOrderAsync();
		Task<Model.OrderValidationResult> SearchNewEmailNeedOrderBudgetAsync();
	}
}
