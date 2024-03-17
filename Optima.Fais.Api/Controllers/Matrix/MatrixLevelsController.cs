using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/matrixlevels")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class MatrixLevelsController : GenericApiController<Model.MatrixLevel, Dto.MatrixLevel>
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public MatrixLevelsController(ApplicationDbContext context, IMatrixLevelsRepository itemsRepository, IMapper mapper, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string matrixIds, string costCenterIds, string subCategoryIds, string includes)
        {
            List<Model.MatrixLevel> items = null;
            IEnumerable<Dto.MatrixLevel> itemsResult = null;
            List<int?> cIds = null;
            List<int?> eIds = null;
            List<int?> catIds = null;

            includes = "Matrix,Level,Uom,Employee";


			//if (matrixIds != null && !matrixIds.StartsWith("["))
			//{
   //             matrixIds = "[" + matrixIds + "]";
			//}

			if (costCenterIds != null && !costCenterIds.StartsWith("["))
			{
                costCenterIds = "[" + costCenterIds + "]";
			}

            if (subCategoryIds != null && !subCategoryIds.StartsWith("["))
            {
                subCategoryIds = "[" + subCategoryIds + "]";
            }

            if ((matrixIds != null) && (matrixIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(matrixIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((costCenterIds != null) && (costCenterIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();
            if ((subCategoryIds != null) && (subCategoryIds.Length > 0)) catIds = JsonConvert.DeserializeObject<string[]>(subCategoryIds).ToList().Select(i => i != null ? (int?)int.Parse(i) : null).ToList();

            items = (_itemsRepository as IMatrixLevelsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, cIds, eIds, catIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.MatrixLevel>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IMatrixLevelsRepository).GetCountByFilters(filter, cIds, eIds, catIds);
                var pagedResult = new Dto.PagedResult<Dto.MatrixLevel>(itemsResult, new Dto.PagingInfo()
                {
                    TotalItems = totalItems,
                    CurrentPage = page.Value,
                    PageSize = pageSize.Value
                });
                return Ok(pagedResult);
            }
            else
            {
                return Ok(itemsResult);
            }
        }

        //[HttpPost]
        //[Route("add")]
        //public async virtual Task<IActionResult> AddOfferMaterial([FromBody] Dto.MatrixLevelAdd offerMatAdd)
        //{
        //    Model.MatrixLevelAdd employeeCostCenter = null;

        //    for (int i = 0; i < offerMatAdd.LevelIds.Length; i++)
        //    {
        //        var material = await _context.Set<Model.Level>().Where(u => u.Id == offerMatAdd.LevelIds[i]).FirstOrDefaultAsync();


        //        employeeCostCenter = new Model.MatrixLevel()
        //        {
        //            OfferId = offerMatAdd.OfferId,
        //            MaterialId = offerMatAdd.MaterialIds[i],
        //            EmailManagerId = offerMatAdd.EmailManagerId,
        //            AppStateId = 6,
        //            Value = material.Price * material.Quantity,
        //            Price = material.Price,
        //            Quantity = material.Quantity
        //        };

        //        _context.Add(employeeCostCenter);

        //        _context.SaveChanges();
        //    }

        //    return Ok(StatusCode(200));
        //}

        //[HttpDelete("remove/{id}")]
        //public virtual IActionResult DeleteOfferMaterial(int id)
        //{

        //    Model.OfferMaterial offerMaterial = _context.Set<Model.OfferMaterial>().Where(a => a.Id == id).Single();

        //    if (offerMaterial != null)
        //    {

        //        offerMaterial.IsDeleted = true;
        //        offerMaterial.ModifiedAt = DateTime.Now;
        //        _context.Update(offerMaterial);
        //        _context.SaveChanges();
        //    }
        //    else
        //    {
        //        return Ok(StatusCode(404));
        //    }

        //    return Ok(StatusCode(200));
        //}
    }
}
