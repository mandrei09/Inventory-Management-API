using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
	public interface IBrandsRepository : IRepository<Brand>
	{
		IEnumerable<Brand> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> dictionaryItemIds);
		int GetCountByFilters(string filter, List<int> dictionaryItemIds);
	}
}
