using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Repository
{
    public interface IBudgetBaseOpsRepository : IRepository<Model.BudgetBaseOp>
    {
        IEnumerable<Model.BudgetBaseOp> GetFiltered(AssetFilter assetFilter, string includes, int? budgetForecastId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);
		IEnumerable<Model.BudgetBaseOp> GetValidationFiltered(AssetFilter assetFilter, string includes, int? budgetForecastId, string documentTypeCode, string assetOpState, DateTime startDate, DateTime endDate, string sortColumn, string sortDirection, int? page, int? pageSize, out int count);

	}
}