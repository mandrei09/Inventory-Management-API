using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IRepository<T> : IReadOnlyRepository<T> where T : class, IEntity
    {
        void Create(T item, string createdBy = null);

        void Update(T item, string modifiedBy = null);

        void Delete(int id, string modifiedBy = null);
    }
}
