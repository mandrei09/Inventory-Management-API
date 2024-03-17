using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class, IEntity
    {
        protected ApplicationDbContext _context = null;
        protected Func<string, Expression<Func<T, bool>>> _filterPredicate = null;

        public ReadOnlyRepository(ApplicationDbContext context, Func<string, Expression<Func<T, bool>>> filterPredicate)
        {
            _context = context;
            _filterPredicate = filterPredicate;
        }

        public ReadOnlyRepository(ApplicationDbContext context)
            : this(context, null)
        { }

        protected virtual IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate = null, string includes = null, 
            string sortColumn = null, string sortDirection = null, int? page = null, int? pageSize = null, bool deleted = false)
        {
            IQueryable<T> query = _context.Set<T>().Where(i => i.IsDeleted == deleted);
            //IQueryable<T> query = _context.Set<T>();
            //if (deleted) query = query.Where(i => i.IsDeleted == deleted);

            includes = includes ?? string.Empty;

            if (predicate != null) query = query.Where(predicate);

            foreach (var includeProperty in includes.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if ((sortColumn != null) && (sortColumn.Length > 0) && (sortDirection != null) && (sortDirection.Length > 0))
            {
                query = sortDirection.ToLower() == "asc"
                    ? query.OrderBy(ExpressionHelper.GenericEvaluateOrderBy<T>(sortColumn))
                    : query.OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<T>(sortColumn));
            }

            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        public IEnumerable<T> GetAll(string includes, string sortColumn, string sortDirection)
        {
            return GetQueryable(null, includes, sortColumn, sortDirection).ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string includes, string sortColumn, string sortDirection)
        {
            return await GetQueryable(null, includes, sortColumn, sortDirection).ToListAsync();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate, string includes,
            string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, string includes,
            string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            return await GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToListAsync();
        }

        public IEnumerable<T> GetByFilter(string filter, string includes, string sortColumn, 
            string sortDirection, int? page, int? pageSize)
        {
            Expression<Func<T, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public async Task<IEnumerable<T>> GetByFilterAsync(string filter, string includes, string sortColumn, 
            string sortDirection, int? page, int? pageSize)
        {
            Expression<Func<T, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
            return await GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToListAsync();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public int GetCount(Expression<Func<T, bool>> predicate)
        {
            return GetQueryable(predicate).Count();
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetQueryable(predicate).CountAsync();
        }

        public int GetCountByFilter(string filter)
        {
            Expression<Func<T, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
            return GetQueryable(predicate).Count();
        }

        public async Task<int> GetCountByFilterAsync(string filter)
        {
            Expression<Func<T, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);
            return await GetQueryable(predicate).CountAsync();
        }
    }
}
