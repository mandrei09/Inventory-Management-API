using Optima.Fais.Dto;
using Optima.Fais.Model;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.Flow
{
    public interface IOrderFlowService
    {
        Task<OrderResult> DeleteOrder(OrderDelete order);
    }
}
