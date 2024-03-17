//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Optima.Fais.Repository;
//using System.Collections.Generic;
//using System;
//using Optima.Fais.Data;
//using Optima.Fais.Repository;
//using System.Linq;

//namespace Optima.Fais.Api.Controllers
//{
//	[Route("api/inventoryassets")]
//	[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
//	public partial class InventoryAssetsController : GenericApiController<Model.InventoryAsset, Dto.InventoryAsset>
//	{
//		public InventoryAssetsController(ApplicationDbContext context, IInventoryAssetsRepository itemsRepository, IMapper mapper)
//		   : base(context, itemsRepository, mapper)
//		{
//		}

//		[HttpGet]
//		[Route("sync")]
//		public virtual IActionResult Sync(int pageSize, int? lastId, DateTime? modifiedAt)
//		{
//			int totalItems = 0;
//			List<Model.InventoryAsset> items = (_itemsRepository as IInventoryAssetsRepository).GetSync(pageSize, lastId, modifiedAt, out totalItems).ToList();
//			var itemsResult = items.Select(i => this._mapper.Map<Dto.InventoryAssetSync>(i));
//			var pagedResult = new Dto.PagedResult<Dto.InventoryAssetSync>(itemsResult, new Dto.PagingInfo()
//			{
//				TotalItems = totalItems,
//				CurrentPage = 1,
//				PageSize = pageSize
//			});
//			return Ok(pagedResult);
//		}
//	}
//}
