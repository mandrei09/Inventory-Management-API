using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class EntityTypesRepository : Repository<EntityType>, IEntityTypesRepository
    {
        public EntityTypesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }

        private Expression<Func<EntityType, bool>> GetFiltersPredicate(string filter, string uploadFolder)
        {
            Expression<Func<EntityType, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            if ((uploadFolder != null) && (uploadFolder.Length > 0))
            {
                predicate = d => d.UploadFolder == uploadFolder;
            }


            return predicate;
        }

        public IEnumerable<EntityType> GetByFilters(string filter, string uploadFolder, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter, uploadFolder);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filters, string uploadFolder)
        {
            var predicate = GetFiltersPredicate(filters, uploadFolder);

            return GetQueryable(predicate).Count();
        }
    }
}
