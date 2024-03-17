using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Optima.Fais.EfRepository
{
    public class MobilePhonesRepository : Repository<Model.MobilePhone>, IMobilePhonesRepository
	{
        public MobilePhonesRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter)); })
        { }


        private Expression<Func<Model.MobilePhone, bool>> GetFiltersPredicate(string filter)
        {
            Expression<Func<Model.MobilePhone, bool>> predicate = null;
            if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

            return predicate;
        }

        public IEnumerable<Model.MobilePhone> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
        }

        public int GetCountByFilters(string filter)
        {
            var predicate = GetFiltersPredicate(filter);

            return GetQueryable(predicate).Count();
        }

        public async Task<ImportITModelResult> Import(ImportMobilePhone import)
        {
            Model.MobilePhone mobilePhone = null;

            if (import == null) return new ImportITModelResult { Success = false, Message = $"Eroare fisier import!" };

            if (import.PhoneNumber == null || (import.PhoneNumber.Trim().Length != 10)) return new ImportITModelResult { Success = false, Message = $"Lipsa numar telefon!" };

            mobilePhone = await _context.Set<Model.MobilePhone>().Where(a => a.Name.Trim() == import.PhoneNumber.Trim() && a.IsDeleted == false).FirstOrDefaultAsync();

            if (mobilePhone == null)
            {
                mobilePhone = await _context.Set<Model.MobilePhone>().Where(a => a.Name.Trim() == import.PhoneNumber.Trim() && a.IsDeleted == true).FirstOrDefaultAsync();

                if(mobilePhone == null)
                {
                    mobilePhone = new Model.MobilePhone()
                    {
                        Code = import.PhoneNumber.Substring(0, 4),
                        Name = import.PhoneNumber.Trim(),
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId
                    };

                    _context.Add(mobilePhone);
                }
                else
                {
                    mobilePhone.ModifiedAt = DateTime.Now;
                    mobilePhone.ModifiedBy = _context.UserId;
                    mobilePhone.IsDeleted = false;

                    if (import.Active.ToUpper() == "NU")
                    {
                        mobilePhone.IsDeleted = true;
                    }

                    _context.Update(mobilePhone);
                }

            }
            else
            {
                mobilePhone.ModifiedAt = DateTime.Now;
                mobilePhone.ModifiedBy = _context.UserId;
                mobilePhone.IsDeleted = false;

                if(import.Active.ToUpper() == "NU")
                {
                    mobilePhone.IsDeleted = true;
                }

                _context.Update(mobilePhone);
            }

            _context.SaveChanges();

            return new ImportITModelResult { Success = true, Message = "", Id = 0 };
        }
    }
}
