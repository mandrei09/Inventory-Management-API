//using Microsoft.EntityFrameworkCore;
//using Optima.Fais.Data;
//using Optima.Fais.Repository;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Optima.Fais.EfRepository
//{
//	public class InventoryAssetsRepository : Repository<Model.InventoryAsset>, IInventoryAssetsRepository
//	{
//		public InventoryAssetsRepository(ApplicationDbContext context)
//			: base(context, (filter) => { return (c) => (c.Asset.InvNo.Contains(filter) || c.Asset.Name.Contains(filter)); })
//		{ }

//		public IEnumerable<Model.InventoryAsset> GetSync(int pageSize, int? lastId, DateTime? lastUpdatedAt, out int totalItems)
//		{
//			var query = _context.InventoryAssets.Where(a => a.InventoryId == 14 && a.InInventory == true).AsNoTracking();

//			if (lastId.HasValue)
//			{
//				query = query
//					.Where(r => (((r.UpdatedAt == lastUpdatedAt) && (r.Id > lastId)) || (r.UpdatedAt > lastUpdatedAt)));
//				totalItems = query.Count();
//				query = query
//					.OrderBy(a => a.UpdatedAt)
//					.ThenBy(a => a.Id)
//					.Take(pageSize);
//			}
//			else
//			{
//				totalItems = query.Count();
//				query = query
//					.OrderBy(a => a.UpdatedAt)
//					.ThenBy(a => a.Id)
//					.Take(pageSize);
//			}

//			return query.ToList();
//		}
//	}
//}
