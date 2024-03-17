using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Api.Controllers
{
    [Route("api/assetcomponents")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class AssetComponentsController : GenericApiController<Model.AssetComponent, Dto.AssetComponent>
    {
        public AssetComponentsController(ApplicationDbContext context, IAssetComponentsRepository itemsRepository, IMapper mapper)
            : base(context, itemsRepository, mapper)
        {
        }


        [HttpGet]
        [Route("", Order = -1)]
        public virtual IActionResult Get(int? page, int? pageSize, string sortColumn, string sortDirection, string filter, string assetIds, string employeeIds, string subTypeIds, string includes)
        {
            List<Model.AssetComponent> items = null;
            IEnumerable<Dto.AssetComponent> itemsResult = null;
            List<int> aIds = null;
            List<int> eIds = null;
            List<int> sIds = null;

            includes = includes ?? "Asset,Employee,SubType.Type.MasterType,InvState";

            if (assetIds != null && !assetIds.StartsWith("["))
            {
                assetIds = "[" + assetIds + "]";
            }

            if (employeeIds != null && !employeeIds.StartsWith("["))
            {
                employeeIds = "[" + employeeIds + "]";
            }

            if (subTypeIds != null && !subTypeIds.StartsWith("["))
            {
                subTypeIds = "[" + subTypeIds + "]";
            }

            if ((assetIds != null) && (assetIds.Length > 0)) aIds = JsonConvert.DeserializeObject<string[]>(assetIds).ToList().Select(int.Parse).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(int.Parse).ToList();
            if ((subTypeIds != null) && (subTypeIds.Length > 0)) sIds = JsonConvert.DeserializeObject<string[]>(subTypeIds).ToList().Select(int.Parse).ToList();

            items = (_itemsRepository as IAssetComponentsRepository).GetByFilters(filter, includes, sortColumn, sortDirection, page, pageSize, aIds, eIds, sIds).ToList();
            itemsResult = items.Select(i => _mapper.Map<Dto.AssetComponent>(i));



            if (page.HasValue && pageSize.HasValue)
            {
                int totalItems = (_itemsRepository as IAssetComponentsRepository).GetCountByFilters(filter, aIds, eIds, sIds);
                var pagedResult = new Dto.PagedResult<Dto.AssetComponent>(itemsResult, new Dto.PagingInfo()
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


        [HttpGet]
        [Route("details")]
        public virtual IActionResult GetOperationDetailsFull(int? page, int? pageSize, string sortColumn, string sortDirection, string includes, int? assetId, int? employeeId)
        {
            // AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            // string employeeId = string.Empty;
            string admCenterId = string.Empty;

            // assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset";


            List<Model.AssetComponent> items = (_itemsRepository as IAssetComponentsRepository)
                .GetFiltered(includes, assetId, employeeId, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.AssetComponent>, List<Dto.AssetComponent>>(items);
            var result = new PagedResult<Dto.AssetComponent>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }


        [HttpGet]
        [Route("detailUI")]
        public virtual IActionResult GetDetailUI(int? page, int? pageSize, string sortColumn, string sortDirection, string includes, int? assetId, int? employeeId)
        {
            // AssetFilter assetFilter = null;
            int totalItems = 0;
            string userName = string.Empty;
            // string employeeId = string.Empty;
            string admCenterId = string.Empty;

            // assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset";


            List<Model.AssetComponent> items = (_itemsRepository as IAssetComponentsRepository)
                .GetFilteredDetailUI(includes, assetId, employeeId, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.AssetComponent>, List<Dto.AssetComponent>>(items);
            var result = new PagedResult<Dto.AssetComponent>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }


        [HttpPost]
        [Route("add")]
        public virtual IActionResult AddComponent([FromBody] Dto.AssetComponentAdd assetComponentAdd)
        {
            Model.AssetComponent assetComponent = null;
            Model.AssetComponentOp assetComponentOp = null;
            Model.AssetComponentOp assetComponentOpLast = null;
            Model.Document document = null;
            string documentTypeCode = "ADD";

            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

            document = new Model.Document
            {
                Approved = true,
                DocumentType = documentType,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Partner = null
            };

            _context.Add(document);

            for (int i = 0; i < assetComponentAdd.AssetComponentIds.Length; i++)
            {
                assetComponent = _context.Set<Model.AssetComponent>().SingleOrDefault(a => a.Id == assetComponentAdd.AssetComponentIds[i]);
                

                if (assetComponent != null)
                {
                    assetComponentOpLast = _context.Set<Model.AssetComponentOp>().Include(d => d.Document).Where(a => a.AssetComponentId == assetComponent.Id && a.Document.DocumentTypeId == 16).OrderByDescending(op => op.Id).Take(1).SingleOrDefault();

                    assetComponent.EmployeeId = assetComponentAdd.EmployeeId;
                    _context.Update(assetComponent);

                    assetComponentOp = new Model.AssetComponentOp()
                    {
                        AssetComponent = assetComponent,
                        DocumentId = document.Id,
                        EmployeeIdInitial = assetComponentOpLast != null ? assetComponentOpLast.EmployeeIdFinal : null,
                        EmployeeIdFinal = assetComponentAdd.EmployeeId,
                        Quantity = 1,
                        ModifiedAt = DateTime.Now
                    };

                    _context.Add(assetComponentOp);

                    _context.SaveChanges();
                }
                else
                {
                    return Ok(StatusCode(404));
                }
            }

            return Ok(StatusCode(200));
        }


        [HttpDelete("remove/{assetComponentId}")]
        public virtual IActionResult DeleteAsset(int assetComponentId)
        {
            Model.AssetComponentOp assetComponentOp = null;
            Model.AssetComponentOp assetComponentOpLast = null;
            Model.Document document = null;
            string documentTypeCode = "REMOVE";

            var assetComponent = _context.Set<Model.AssetComponent>().Where(a => a.Id == assetComponentId).Single();

            if (assetComponent != null)
            {

                var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

                document = new Model.Document
                {
                    Approved = true,
                    DocumentType = documentType,
                    DocNo1 = string.Empty,
                    DocNo2 = string.Empty,
                    DocumentDate = DateTime.Now,
                    RegisterDate = DateTime.Now,
                    Partner = null
                };

                _context.Add(document);


                assetComponentOpLast = _context.Set<Model.AssetComponentOp>().Include(d => d.Document).Where(a => a.AssetComponentId == assetComponent.Id && a.Document.DocumentTypeId == 16).OrderByDescending(op => op.Id).Take(1).SingleOrDefault();


                //if (assetComponentOpLast != null)
                //{
                //    assetComponentOpLast.IsDeleted = true;
                //    _context.Update(assetComponentOpLast);
                //}

                assetComponentOp = new Model.AssetComponentOp()
                {
                    AssetComponent = assetComponent,
                    DocumentId = document.Id,
                    EmployeeIdInitial = assetComponentOpLast != null ? assetComponentOpLast.EmployeeIdFinal : null,
                    EmployeeIdFinal = assetComponentOpLast != null ? assetComponentOpLast.EmployeeIdFinal : null,
                    Quantity = 1,
                    ModifiedAt = DateTime.Now
                };

                _context.Add(assetComponentOp);


                assetComponent.EmployeeId = null;
                _context.Update(assetComponent);
                _context.SaveChanges();
            }
            else
            {
                return Ok(StatusCode(404));
            }

            return Ok(StatusCode(200));
        }

        [HttpPost]
        [Route("addTransfer")]
        public virtual IActionResult AddTransfer([FromBody] Dto.AssetComponentTransferAdd assetComponentTransferAdd)
        {
            Model.AssetComponent assetComponent = null;
            Model.AssetInv assetInv = null;
            Model.AssetComponentOp assetComponentOpOld = null;
            Model.AssetComponentOp assetComponentOpNew = null;
            Model.Document documentAdd = null;
            Model.Document documentRemove = null;
            Model.Inventory inventory = null;

            string documentTypeAddCode = "ADD";
            string documentTypeRemoveCode = "REMOVE";

            var documentTypeAdd = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeAddCode).Single();
            var documentTypeRemove = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeRemoveCode).Single();
            inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefault();

            documentAdd = new Model.Document
            {
                Approved = true,
                DocumentType = documentTypeAdd,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Partner = null,
                ParentDocumentId = inventory.DocumentId
            };

            _context.Add(documentAdd);


            documentRemove = new Model.Document
            {
                Approved = true,
                DocumentType = documentTypeRemove,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Partner = null,
                ParentDocumentId = inventory.DocumentId
            };

            _context.Add(documentRemove);


            for (int i = 0; i < assetComponentTransferAdd.AssetComponentOldIds.Length; i++)
            {
                assetComponent = _context.Set<Model.AssetComponent>().SingleOrDefault(a => a.Id == assetComponentTransferAdd.AssetComponentOldIds[i]);

                if (assetComponent != null)
                {
                    assetComponent.EmployeeId = null;
                    assetComponent.InvStateId = assetComponentTransferAdd.InvStateId > 0 ? assetComponentTransferAdd.InvStateId : assetComponent.InvStateId;

                    assetComponentOpOld = new Model.AssetComponentOp()
                    {
                        AssetComponent = assetComponent,
                        Document = documentRemove,
                        CreatedAt = DateTime.Now,
                        EmployeeIdInitial = assetComponent.EmployeeId,
                        EmployeeIdFinal = assetComponentTransferAdd.EmployeeId,
                        IsDeleted = false,
                        Quantity = 1,
                        ModifiedAt = DateTime.Now,
                        InvStateId = assetComponentTransferAdd.InvStateId > 0 ? assetComponentTransferAdd.InvStateId : assetComponent.InvStateId
                    };

                    _context.Add(assetComponentOpOld);
                    _context.Update(assetComponent);

                    _context.SaveChanges();
                }
                else
                {
                    return Ok(StatusCode(404));
                }
            }

            for (int i = 0; i < assetComponentTransferAdd.AssetComponentNewIds.Length; i++)
            {
                assetComponent = _context.Set<Model.AssetComponent>().SingleOrDefault(a => a.Id == assetComponentTransferAdd.AssetComponentNewIds[i]);

                if (assetComponent != null)
                {
                    assetComponent.EmployeeId = assetComponentTransferAdd.EmployeeId;

                    assetComponentOpNew = new Model.AssetComponentOp()
                    {
                        AssetComponent = assetComponent,
                        Document = documentAdd,
                        CreatedAt = DateTime.Now,
                        EmployeeIdInitial = assetComponent.EmployeeId,
                        EmployeeIdFinal = assetComponentTransferAdd.EmployeeId,
                        IsDeleted = false,
                        Quantity = 1,
                        ModifiedAt = DateTime.Now
                    };

                    _context.Add(assetComponentOpNew);
                    _context.Update(assetComponent);

                    _context.SaveChanges();
                }
                else
                {
                    return Ok(StatusCode(404));
                }
            }

            return Ok(StatusCode(200));
        }
    }
}
