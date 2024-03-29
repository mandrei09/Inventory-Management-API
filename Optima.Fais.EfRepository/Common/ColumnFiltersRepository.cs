﻿using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class ColumnFiltersRepository : Repository<ColumnFilter>, IColumnFiltersRepository
	{
        public ColumnFiltersRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Property.Contains(filter) || a.Type.Contains(filter)); })
        { }

        private Expression<Func<ColumnFilter, bool>> GetFiltersPredicate(string filter)
        {
            Expression<Func<ColumnFilter, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);


            return predicate;
        }

        public IEnumerable<ColumnFilter> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filters)
        {
            var predicate = GetFiltersPredicate(filters);

            return GetQueryable(predicate).Count();
        }
    }
}
