using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class DocumentTypesRepository : Repository<DocumentType>, IDocumentTypesRepository
    {
        public DocumentTypesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (d) => (d.Code.Contains(filter) || d.Name.Contains(filter)); })
        { }

        public IEnumerable<DocumentType> GetDocumentTypesByFilters(string parentCode)
        {
            Expression<Func<DocumentType, bool>> predicate = null;

            if ((parentCode != null) && (parentCode.Length > 0))
            {
                predicate = d => d.ParentCode == parentCode;
            }

            return Get(predicate, null, null, null, null, null);
        }

        public IEnumerable<DocumentType> GetDocumentTypesByPrefix(string prefix)
        {
            Expression<Func<DocumentType, bool>> predicate = null;

            if ((prefix != null) && (prefix.Length > 0))
            {
                predicate = d => d.Prefix == prefix;
            }

            return Get(predicate, null, null, null, null, null);
        }

        public async Task<DocumentType> GetByCodeAsync(string code)
        {
            return await GetQueryable(p => p.Code == code).SingleOrDefaultAsync();
        }
    }
}
