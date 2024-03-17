using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IReadOnlyRepository<T> where T : class, IEntity
    {
        IEnumerable<T> GetAll(string includes, string sortColumn, string sortDirection);
        Task<IEnumerable<T>> GetAllAsync(string includes, string sortColumn, string sortDirection);

        IEnumerable<T> Get(Expression<Func<T, bool>> predicate, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);

        IEnumerable<T> GetByFilter(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);
        Task<IEnumerable<T>> GetByFilterAsync(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize);

        T GetById(int id);
        Task<T> GetByIdAsync(int id);

        int GetCount(Expression<Func<T, bool>> predicate);
        Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);

        int GetCountByFilter(string filter);
        Task<int> GetCountByFilterAsync(string filter);
    }
}
