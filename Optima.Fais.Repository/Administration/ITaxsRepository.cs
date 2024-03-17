using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
	public interface ITaxsRepository : IRepository<Tax>
	{
		IEnumerable<Tax> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
		int GetCountByFilters(string filter);
	}
}
