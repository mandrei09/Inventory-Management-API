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
	public class ModelsRepository : Repository<Model.Model>, IModelsRepository
	{
		public ModelsRepository(ApplicationDbContext context)
			: base(context, (filter) => { return (a) => (a.Code.Contains(filter) || a.Name.Contains(filter) || a.Brand.Name.Contains(filter) || a.Brand.DictionaryItem.Name.Contains(filter)); })
		{ }

		private Expression<Func<Model.Model, bool>> GetFiltersPredicate(string filter, List<int> brandIds, bool showWFH)
		{
			Expression<Func<Model.Model, bool>> predicate = null;
			if ((filter != null) && (filter.Length > 0)) predicate = ExpressionHelper.GetAllPartsFilter(filter, _filterPredicate);

			if ((brandIds != null) && (brandIds.Count > 0))
			{
				predicate = predicate != null
					? ExpressionHelper.And<Model.Model>(predicate, r => brandIds.Contains(r.BrandId.Value) || r.Code == "00")
					: r => brandIds.Contains(r.BrandId.Value) || r.Code == "00";
			}

            if (showWFH)
            {
                predicate = predicate != null
                    ? ExpressionHelper.And<Model.Model>(predicate, r => ((r.Brand.DictionaryItem.DictionaryTypeId == 3) || (r.Code == "00")))
                    : r => ((r.Brand.DictionaryItem.DictionaryTypeId == 3) || (r.Code == "00"));
            }

            return predicate;
		}

		public IEnumerable<Model.Model> GetByFilters(string filter, string includes, string sortColumn, string sortDirection, int? page, int? pageSize, List<int> brandIds, bool showWFH)
		{
			var predicate = GetFiltersPredicate(filter, brandIds, showWFH);

			return GetQueryable(predicate, includes, sortColumn, sortDirection, page, pageSize).ToList();
		}

		public int GetCountByFilters(string filter, List<int> brandIds, bool showWFH)
		{
			var predicate = GetFiltersPredicate(filter, brandIds, showWFH);

			return GetQueryable(predicate).Count();
		}

        public async Task<ImportITModelResult> ImportModel(ImportModel import)
        {
            Model.DictionaryItem dictionaryItem = null;
            Model.Brand brand = null;
            Model.Model model = null;

            if (import == null) return new ImportITModelResult { Success = false, Message = $"Eroare fisier import!" };

            if ((import.DictionaryItem == null) || (import.DictionaryItem.Trim().Length < 1)) return new ImportITModelResult { Success = false, Message = $"Lipsa tip pentru {import.Model}!" };
            if (import.Brand == null || (import.Brand.Trim().Length < 1)) return new ImportITModelResult { Success = false, Message = $"Lipsa brand pentru {import.Model}!" };
            if (import.Model == null || (import.Model.Trim().Length < 1)) return new ImportITModelResult { Success = false, Message = $"Lipsa model pentru {import.Brand}!" };
            if (import.SNLength < 0)
            {
                import.SNLength = 0;
            }
            if (import.IMEILength < 0)
            {
                import.IMEILength = 0;
            }

            dictionaryItem = await _context.Set<Model.DictionaryItem>().Where(a => a.Name.Trim() == import.DictionaryItem.Trim() && a.IsDeleted == false).FirstOrDefaultAsync();

            if(dictionaryItem == null)
            {
                dictionaryItem = await _context.Set<Model.DictionaryItem>().Where(a => a.Name.Trim() == import.DictionaryItem.Trim() && a.IsDeleted == true).FirstOrDefaultAsync();
            }
           
            if (dictionaryItem == null) return new ImportITModelResult { Success = false, Message = $"Tip - ul {import.DictionaryItem} nu exista!" };

            if(import.Brand.Trim().ToUpper()== "ALTELE")
            {
                brand = await _context.Set<Model.Brand>().Where(a => a.Name.Trim() == import.Brand.Trim()).FirstOrDefaultAsync();
            }
            else
            {
                brand = await _context.Set<Model.Brand>().Where(a => a.Name.Trim() == import.Brand.Trim() && a.IsDeleted == false && a.DictionaryItemId == dictionaryItem.Id).FirstOrDefaultAsync();
            }

            

            if (brand == null)
            {
                brand = await _context.Set<Model.Brand>().Where(a => a.Name.Trim() == import.Brand.Trim() && a.IsDeleted == true && a.DictionaryItemId == dictionaryItem.Id).FirstOrDefaultAsync();

                if(brand == null)
                {
                    brand = new Model.Brand()
                    {
                        Code = import.Brand.Substring(0, 1),
                        Name = import.Brand.Trim(),
                        DictionaryItemId = dictionaryItem.Id,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId
                    };

                    _context.Add(brand);
                }
                else
                {
                    brand.DictionaryItemId = dictionaryItem.Id;
                    brand.ModifiedAt = DateTime.Now;
                    brand.ModifiedBy = _context.UserId;
                    brand.IsDeleted = false;

                    _context.Update(brand);
                }

            }
            else
            {
                brand.DictionaryItemId = dictionaryItem.Id;
                brand.ModifiedAt = DateTime.Now;
                brand.ModifiedBy = _context.UserId;
                brand.IsDeleted = false;

                _context.Update(brand);
            }

            if (import.Model.Trim().ToUpper() == "ALTELE")
            {
                model = await _context.Set<Model.Model>().Where(a => a.Name.Trim() == import.Model.Trim()).FirstOrDefaultAsync();
            }
            else
            {
                model = await _context.Set<Model.Model>().Where(a => a.Name.Trim() == import.Model.Trim() && a.IsDeleted == false && a.BrandId == brand.Id).FirstOrDefaultAsync();
            }

            

            if(model == null)
            {
                model = await _context.Set<Model.Model>().Where(a => a.Name.Trim() == import.Model.Trim() && a.IsDeleted == true && a.BrandId == brand.Id).FirstOrDefaultAsync();

                if(model == null)
                {
                    model = new Model.Model()
                    {
                        Code = import.Model.Substring(0, 1),
                        Name = import.Model.Trim(),
                        Brand = brand,
                        SNLength = import.SNLength,
                        IMEILength = import.IMEILength,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId
                    };

                    _context.Add(model);
                }
                else
                {
                    model.SNLength = import.SNLength;
                    model.IMEILength = import.IMEILength;
                    model.ModifiedAt = DateTime.Now;
                    model.ModifiedBy = _context.UserId;
                    model.IsDeleted = false;

                    if (import.Active.ToUpper() == "NU")
                    {
                        model.IsDeleted = true;
                    }

                    _context.Update(model);
                }

            }
            else
            {
                model.SNLength = import.SNLength;
                model.IMEILength = import.IMEILength;
                model.ModifiedAt = DateTime.Now;
                model.ModifiedBy = _context.UserId;
                model.Brand = brand; 
                model.IsDeleted = false;

                if(import.Active.ToUpper() == "NU")
                {
                    model.IsDeleted = true;
                }

                _context.Update(model);
            }
           

            _context.SaveChanges();

            return new ImportITModelResult { Success = true, Message = "", Id = 0 };
        }
    }
}
