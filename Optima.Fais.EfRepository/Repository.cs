using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public class Repository<T> : ReadOnlyRepository<T>, IRepository<T> where T : class, IEntity
    {
        public Repository(ApplicationDbContext context, Func<string, Expression<Func<T, bool>>> filterPredicate)
            : base(context, filterPredicate)
        { }

        public Repository(ApplicationDbContext context)
            : base(context)
        { }

        

        public virtual void Create(T item, string createdBy = null)
        {
            //item.CreatedAt = DateTime.UtcNow;
            item.CreatedBy = createdBy;
            _context.Set<T>().Add(item);
        }

        public void Delete(int id, string modifiedBy = null)
        {
            var item = _context.Set<T>().Where(i => i.Id == id).Single();
            //_context.Set<T>().Remove(item);
            //item.ModifiedAt = DateTime.UtcNow;
            item.ModifiedBy = modifiedBy;
            item.IsDeleted = true;
            _context.Entry(item).State = EntityState.Modified;
        }

        public void Update(T item, string modifiedBy = null)
        {
            //item.ModifiedAt = DateTime.UtcNow;
            item.ModifiedBy = modifiedBy;
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
