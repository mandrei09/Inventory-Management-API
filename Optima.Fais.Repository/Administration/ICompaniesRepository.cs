using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
	public interface ICompaniesRepository : IRepository<Company>
	{
		IEnumerable<Company> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int?> exceptEmployeeIds);
		int GetCountByFilters(string filter, List<int?> exceptEmployeeIds);
		IEnumerable<Company> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
	}
}
