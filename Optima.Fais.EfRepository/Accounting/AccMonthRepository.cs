using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace Optima.Fais.EfRepository
{
    public class AccMonthsRepository : Repository<AccMonth>, IAccMonthsRepository
    {
        public AccMonthsRepository(ApplicationDbContext context)
            : base(context, null)
        { }

        public AccMonth GetAccMonth(int month, int year)
        {
            Expression<Func<AccMonth, bool>> predicate = null;

            predicate = a => ((a.Month == month) && (a.Year == year));

            return GetQueryable(predicate, null, null, null, null, null).SingleOrDefault();
        }

        public int CreateAccMonth(Dto.AccMonth accMonth)
        {

            Model.AccMonth accMonthSave = null;
            Model.AccMonth accMonthOld = null;

            accMonthOld = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

            if(accMonthOld != null)
			{
                accMonthOld.IsActive = false;
                accMonthOld.ModifiedAt = DateTime.Now;
                _context.Update(accMonthOld);
                _context.SaveChanges();
            }

           
            if(accMonth != null && accMonth.Id > 0)
			{
                var accMth = _context.Set<Model.AccMonth>().Where(a => a.Id == accMonth.Id).Single();
                accMth.IsActive = accMonth.IsActive;
                accMth.ModifiedAt = DateTime.Now;
                _context.Update(accMth);
                _context.SaveChanges();

                return accMonth.Id;
            }
			else
			{
                accMonthSave = new Model.AccMonth()
                {
                    ModifiedAt = DateTime.Now
                };

                _context.Add(accMonthSave);
                _context.SaveChanges();

                return accMonthSave.Id;
            }
        }
    }
}
