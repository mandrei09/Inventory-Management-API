using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
	public interface IInsuranceCategoriesRepository : IRepository<InsuranceCategory>
	{
		IEnumerable<InsuranceCategory> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
		int GetCountByFilters(string filter);
		IEnumerable<InsuranceCategory> GetSync(int pageSize, int? lastId, DateTime? lastModifiedAt, out int totalItems);
	}
}
