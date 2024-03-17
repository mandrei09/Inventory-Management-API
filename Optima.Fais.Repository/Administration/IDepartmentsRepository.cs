using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IDepartmentsRepository : IRepository<Model.Department>
    {
        IEnumerable<Dto.DepartmentDetail> GetDetailsByFilters(int? teamLeaderId, string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
        IEnumerable<Model.Department> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
    }
}
