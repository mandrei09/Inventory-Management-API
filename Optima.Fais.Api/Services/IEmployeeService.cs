using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface IEmployeeService
    {
        Task<List<Model.EmployeeEmailResult>> GetEmployeesAsync();
        Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeTransferAsync();
		Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder1TransferAsync();
        Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder2TransferAsync();
        Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder3TransferAsync();
        Task<Model.EmployeeEmailStatusResult> SearchNewEmployeeReminder4TransferAsync();
        Task<Model.EmployeeEmailStatusResult> SearchNewManagerTransferAsync();
        Task<Model.EmployeeEmailStatusResult> SearchNewAppendixTransferAsync();
		Task<Model.EmployeeEmailStatusResult> SearchNewRejectedByStockTransferAsync();
		Task<Model.EmployeeEmailStatusResult> SearchNewRejectedAccountingValidationAsync();
	}
}
