using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
	public interface IModelsRepository : IRepository<Model.Model>
	{
		IEnumerable<Model.Model> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> brandIds, bool showWFH = false);
		int GetCountByFilters(string filter, List<int> brandIds, bool showWFH = false);
        Task<Model.ImportITModelResult> ImportModel(Dto.ImportModel import);
    }
}
