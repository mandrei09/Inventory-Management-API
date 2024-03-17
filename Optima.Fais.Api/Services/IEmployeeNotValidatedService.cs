using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
    public interface IEmployeeNotValidatedService
    {
        Task<List<Model.EmployeeNotValidatedEmailResult>> GetEmployeesNotValidatedAsync();
    }
}
