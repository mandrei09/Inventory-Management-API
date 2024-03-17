using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface IOfferEmailService
    {
        Task<List<Model.EmployeeEmailResult>> GetEmployeesAsync();
        Task<Model.CreateAssetSAPResult> SearchNewEmployeeTransferAsync();
        Task<Model.CreateAssetSAPResult> SearchNewManagerTransferAsync();
        Task<Model.CreateAssetSAPResult> SearchNewAppendixTransferAsync();
    }
}
