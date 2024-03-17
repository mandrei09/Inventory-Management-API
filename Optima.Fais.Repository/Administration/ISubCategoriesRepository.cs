using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
	public interface ISubCategoriesRepository : IRepository<SubCategory>
	{
		IEnumerable<SubCategory> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> categoryIds, List<int> assetTypeIds, List<int> subIds);
		int GetCountByFilters(string filter, List<int> categoryIds, List<int> assetTypeIds, List<int> subIds);
	}
}
