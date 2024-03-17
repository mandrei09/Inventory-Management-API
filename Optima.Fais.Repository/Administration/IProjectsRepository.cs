using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
	public interface IProjectsRepository : IRepository<Project>
	{
		IEnumerable<Project> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
		int GetCountByFilters(string filter);
	}
}
