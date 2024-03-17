using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;

namespace Optima.Fais.EfRepository
{
    public class EntityFilesRepository : Repository<EntityFile>, IEntityFilesRepository
    {
        public EntityFilesRepository(ApplicationDbContext context)
            : base(context, null)
        { }

        public IEnumerable<Model.EntityFile> GetByEntity(string entityTypeCode, int entityId, Guid? guid, int? partnerId)
        {
            List<Model.EntityFile> entityFiles = new List<EntityFile>();
            if(entityTypeCode == "NEWASSET")
			{
                entityFiles = _context.Set<Model.EntityFile>()
                    .Include(p => p.EntityType)
                    .Include(r => r.Partner)
                    .Include(r => r.Request)
                    .Include(r => r.RequestBudgetForecast).ThenInclude(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
                    .Where(e => (e.EntityType.UploadFolder == "RECEPTION" || e.EntityType.UploadFolder == "PRE_RECEPTION" || e.EntityType.Code == "STORNO") && e.EntityId == entityId && e.IsDeleted == false)
                    .ToList();
			}else if (entityTypeCode == "INVENTORYBOOK")
			{
				entityFiles = _context.Set<Model.EntityFile>()
					.Include(p => p.EntityType)
					.Include(r => r.Partner)
					.Include(r => r.Request)
					.Where(e => (
                    (e.EntityType.UploadFolder.ToUpper() == "INVENTORYBOOK")) && e.CostCenterId == entityId && e.IsDeleted == false)
					.ToList();
			}
			else if (entityTypeCode == "REQUEST_BUDGET_FORECAST")
            {
				if (partnerId != null && partnerId > 0)
				{
                    entityFiles = _context.Set<Model.EntityFile>()
                       .Include(p => p.EntityType)
                       .Include(r => r.Partner)
                       .Include(r => r.Request)
                       .Include(r => r.RequestBudgetForecast).ThenInclude(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
                       .Where(e => (((e.EntityType.UploadFolder == "REQUESTBUDGETFORECAST") || (e.EntityType.UploadFolder == "OFFERUI" && e.PartnerId == partnerId)) && e.RequestId == entityId && e.IsDeleted == false) || 
                        (e.EntityType.UploadFolder == "OFFERUI" && e.Guid == guid && e.PartnerId == partnerId && e.IsDeleted == false))
                       .ToList();
				}
				else
				{
					if (guid != null && guid.HasValue)
					{
                        entityFiles = _context.Set<Model.EntityFile>().Include(p => p.EntityType)
                      .Include(r => r.Partner)
                      .Include(r => r.Request)
                      .Include(r => r.RequestBudgetForecast).ThenInclude(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
                      .Where(e => (((e.EntityType.UploadFolder == "REQUESTBUDGETFORECAST" || e.EntityType.UploadFolder == "OFFERUI") && (e.RequestId == entityId || e.RequestBudgetForecastId == entityId) && e.IsDeleted == false)) ||
                       (e.EntityType.UploadFolder == "OFFERUI" && e.Guid == guid && e.IsDeleted == false))
                      .ToList();
					}
					else
					{
						entityFiles = _context.Set<Model.EntityFile>().Include(p => p.EntityType)
					  .Include(r => r.Partner)
					  .Include(r => r.Request)
					  .Include(r => r.RequestBudgetForecast).ThenInclude(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
					  .Where(e => ((e.EntityType.UploadFolder == "REQUESTBUDGETFORECAST" && (e.RequestId == entityId || e.RequestBudgetForecastId == entityId) && e.IsDeleted == false)) ||
					   (e.EntityType.UploadFolder == "OFFERUI" && (e.RequestId == entityId || e.RequestBudgetForecastId == entityId) && e.IsDeleted == false))
					  .ToList();
					}
                   
                }
               
            }
            else if (entityTypeCode == "OFFERUI_DOCUMENT")
            {
                entityFiles = _context.Set<Model.EntityFile>().Include(p => p.EntityType)
                .Include(r => r.Partner)
                .Include(r => r.Request)
                .Include(r => r.RequestBudgetForecast).ThenInclude(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
                .Where(e => (e.EntityType.UploadFolder == "OFFERUI") && e.Guid == guid && e.PartnerId == partnerId && e.IsDeleted == false)
                .ToList();
            }
			else if (entityTypeCode == "EDIT_PANEL")
			{
				entityFiles = _context.Set<Model.EntityFile>().Include(p => p.EntityType)
				.Where(e => e.EntityType.Code == "REQUEST_BUDGET_FORECAST" && e.RequestId== entityId && e.IsDeleted == false)
				.ToList();
			}
			else if (entityTypeCode == "EDIT_ORDER_PANEL")
			{
                List<int?> assetIds = _context.Set<Model.Asset>().Where(a => a.IsDeleted == false && a.OrderId == entityId).Select(a => (int?)a.Id).ToList();
                entityFiles = new List<EntityFile>();

                //if (assetIds.Count > 0 || assetIds == null)
                //{
                    entityFiles = _context.Set<Model.EntityFile>().Include(p => p.EntityType)
				        .Where(e => e.IsDeleted == false && (assetIds.Contains(e.EntityId) || (e.OrderId == entityId)))
				        .ToList();
                //}
            }
			else
			{
                entityFiles = _context.Set<Model.EntityFile>().Include(p => p.EntityType)
                .Include(r => r.Partner)
                .Include(r => r.Request)
                .Include(r => r.RequestBudgetForecast).ThenInclude(r => r.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(p => p.Project)
                .Where(e => (e.EntityType.Code == entityTypeCode || e.EntityType.UploadFolder == entityTypeCode) && e.EntityId == entityId && e.IsDeleted == false)
                .ToList();
            }
           

            return entityFiles;
        }
    }
}
