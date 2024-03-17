using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/entityfiles")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class EntityFilesController : GenericApiController<Model.EntityFile, Dto.EntityFile>
    {
        private readonly string _basePath;
        private readonly string _resourcesFolder;
        private readonly string _resourcesPath;
		private readonly IRequestsService _requestsService;
		private readonly IOrdersService _ordersService;
		private readonly UserManager<Model.ApplicationUser> _userManager;

		public EntityFilesController(ApplicationDbContext context, IEntityFilesRepository itemsRepository, IMapper mapper, IConfiguration configuration, IRequestsService requestsService, IOrdersService ordersService, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
            this._resourcesFolder = configuration.GetSection("BasePath").GetValue<string>("Resources");
            this._resourcesPath = $"{this._basePath}{this._resourcesFolder}";
			this._requestsService = requestsService;
			this._ordersService = ordersService;
			this._userManager = userManager;
		}

        /*[HttpGet]
        [Route("inuse", Order = -1)]
        public virtual async Task<IActionResult> GetInUse(int page, int pageSize, string colDefFilter, string filter, string sortColumn, string sortDirection, string includes, string jsonFilter, string columnFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            ColumnAssetFilter colAssetFilters = null;
            List<GenericFilter> genericFilters = new List<GenericFilter>();
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;
            int? costCenterId = null;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
            // genericFilter = colDefFilter != null ? JsonConvert.DeserializeObject<List<GenericFilter>>(colDefFilter) : new GenericFilter();
            colAssetFilters = columnFilter != null ? JsonConvert.DeserializeObject<ColumnAssetFilter>(columnFilter) : new ColumnAssetFilter();

            //         if (colDefFilter != null)
            //{
            //             genericFilters = JsonConvert.DeserializeObject<List<GenericFilter>>(colDefFilter);

            //         }

            includes = includes + ",Asset.AppState";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();
                //userName = "alexandru.ciudin";
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);

                    employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;
                    costCenterId = _context.Set<Model.Employee>().Where(e => e.Id == int.Parse(employeeId)).FirstOrDefault().CostCenterId;

                    assetFilter.EmpCostCenterIds = null;
                    assetFilter.EmpCostCenterIds = new List<int?>();
                    assetFilter.EmpCostCenterIds.Add(costCenterId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("-1"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("-1"));
            }

            if (filter != null && filter != "")
            {
                assetFilter.Filter = filter;
            }

            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInUse(assetFilter, colAssetFilters, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);
        }*/

        //[HttpGet]
        //[Route("api/apidetail/filtered")]
        [HttpGet("filtered")]
        public virtual IActionResult GetByEntity(string entityTypeCode, int entityId, Guid? guid, int? partnerId)
        {
            var items = 
                (_itemsRepository as IEntityFilesRepository)
                .GetByEntity(entityTypeCode, entityId, guid, partnerId)
                .Select(i => _mapper.Map<Dto.EntityFile>(i));

            return Ok(items);
        }

        [HttpGet("getByCostCenterID/{costCenterID}/{inventoryID}")]
        public virtual IActionResult GetByCostCenter(int costCenterID, int inventoryID)
        {
            string inventoryName =  _context.Set<Model.Inventory>().Where(i => i.Id == inventoryID).Select(i => i.Description).FirstOrDefault();
            var items = _context.EntityFiles
                                  .Where(e => e.CostCenterId == costCenterID && e.IsDeleted==false && e.Info == inventoryName)
                                  .Include(e => e.EntityType)
                                  .ToList();
            return Ok(items);
        }

        [HttpGet("getByRequestBudgetForecastId/{requestBudgetForecastId}")]
        public virtual IActionResult GetByRequestId(int requestBudgetForecastId)
        {
            var items = _context.EntityFiles
                                  .Where(e => e.RequestBudgetForecastId == requestBudgetForecastId && e.IsDeleted == false)
                                  .Include(e => e.EntityType)
                                  .ToList();
            return Ok(items);
        }

        [HttpGet("download/{entityFileId}")]
        public IActionResult Download(int entityFileId)
        {
            Model.EntityFile entityFile = _context.Set<Model.EntityFile>().Where(e => e.Id == entityFileId).Single();
            Model.EntityType entityType = _context.Set<Model.EntityType>().Where(e => e.Id == entityFile.EntityTypeId).Single();

            string storedAs = entityFile.StoredAs;
            string uploadFolder = entityType.UploadFolder;
            string filePath = uploadFolder + "\\" + storedAs;

            //using (var stream = new FileStream(filePath, FileMode.Open))
            //{
            //    var response = File(stream, "application/octet-stream"); // FileStreamResult
            //    return response;
            //}

            filePath = @"..\OFAAPI\upload" + "\\" + storedAs;

            if (!System.IO.File.Exists(filePath))
            {
                filePath = @"..\OFAAPI\upload" + "\\" + storedAs;

				if (!System.IO.File.Exists(filePath))
				{
					filePath = @".\upload" + "\\" + storedAs;

                    if (!System.IO.File.Exists(filePath))
                    {
                        filePath = @".\reception" + "\\" + storedAs;

                        if (!System.IO.File.Exists(filePath))
                        {
                            filePath = @".\request" + "\\" + storedAs;

                            if (!System.IO.File.Exists(filePath))
                            {
                                filePath = @".\order" + "\\" + storedAs;
                            }
                        }
                    }
                }
			}

            //using (var sr = new StreamReader())

            //var response = File(filePath, "application/octet-stream"); // FileStreamResult
            //return response;

            HttpContext.Response.ContentType = entityFile.FileType;
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), entityFile.FileType)
            {
                FileDownloadName = entityFile.Name
            };

            return result;
        }

        [HttpGet("downloadpdf/{entityFileId}")]
        public IActionResult DownloadPdf(int entityFileId)
        {
            Model.EntityFile entityFile = _context.Set<Model.EntityFile>().Where(e => e.Id == entityFileId).Single();
            Model.EntityType entityType = _context.Set<Model.EntityType>().Where(e => e.Id == entityFile.EntityTypeId).Single();

            //Model.EmailType emailType = _context.Set<Model.EmailType>().Where(e => e.Code.ToUpper() == "SCAN_LOCATION").Single();


            var path = "\\bonuri\\2022\\";
            string storedAs = entityFile.StoredAs;
            //string uploadFolder = entityType.UploadFolder;
            string uploadFolder = entityFile.Name;
            //string uploadFolder = emailType.UploadFolder;

            string filePath = $"{this._basePath}{path}{storedAs}";


            //string filePath = uploadFolder + "\\" + storedAs;



            //HttpContext.Response.ContentType = entityFile.FileType;
            //FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), entityFile.FileType)
            //{
            //    FileDownloadName = entityFile.Name
            //};

            //return result;


            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", "");
        }

        [HttpGet("downloadValidateAsset/{entityFileId}")]
        public IActionResult DownloadValidateAsset(int entityFileId)
        {
            Model.EntityFile entityFile = _context.Set<Model.EntityFile>().Where(e => e.Id == entityFileId).Single();
            Model.EntityType entityType = _context.Set<Model.EntityType>().Where(e => e.Id == entityFile.EntityTypeId).Single();

            string storedAs = entityFile.StoredAs;
            string uploadFolder = entityType.UploadFolder;
            string filePath = uploadFolder + "\\" + storedAs;

            filePath = @"..\OFAAPI\upload" + "\\" + storedAs;

            if (!System.IO.File.Exists(filePath))
            {
                filePath = @"..\OFAAPI\upload" + "\\" + storedAs;

                if (!System.IO.File.Exists(filePath))
                {
                    filePath = @".\upload" + "\\" + storedAs;

                    if (!System.IO.File.Exists(filePath))
                    {
                        filePath = @".\reception" + "\\" + storedAs;

                        if (!System.IO.File.Exists(filePath))
                        {
                            filePath = @".\request" + "\\" + storedAs;

                            if (!System.IO.File.Exists(filePath))
                            {
                                filePath = @".\order" + "\\" + storedAs;

                                if (!System.IO.File.Exists(filePath))
                                {
                                    filePath = @".\requestbudgetforecast" + "\\" + storedAs;

                                    if (!System.IO.File.Exists(filePath))
                                    {
                                        filePath = @".\offerui" + "\\" + storedAs;

										if (!System.IO.File.Exists(filePath))
										{
											filePath = @".\inventorybook" + "\\" + storedAs;

											if (!System.IO.File.Exists(filePath))
											{
												filePath = @".\storno" + "\\" + storedAs;

												if (!System.IO.File.Exists(filePath))
												{
													filePath = @".\pre_reception" + "\\" + storedAs;
												}
											}
										}
									}
                                }
                            }
                        }
                    }
                }
            }

            //using (var sr = new StreamReader())

            //var response = File(filePath, "application/octet-stream"); // FileStreamResult
            //return response;

            HttpContext.Response.ContentType = entityFile.FileType;
            FileContentResult result = new FileContentResult(System.IO.File.ReadAllBytes(filePath), entityFile.FileType)
            {
                FileDownloadName = entityFile.Name
            };

            return result;
        }

        [AllowAnonymous]
        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file, int entityId, string entityTypeCode, string info)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");
            int documentId = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefault().DocumentId;
            Model.EntityType entityType = _context.Set<Model.EntityType>().Where(e => e.Code == entityTypeCode && e.Name == documentId.ToString()).Single();

            //string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string storedAs = Guid.NewGuid().ToString() + ".jpeg";
            string uploadFolder = entityType.UploadFolder;
            string filePath = uploadFolder + "\\" + storedAs;

            //if (!Directory.Exists(uploadFolder))
            //{
            //    var d = Directory.CreateDirectory(uploadFolder);
            //    var s = d.FullName;
            //}

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            Model.EntityFile entityFile = new Model.EntityFile()
            {
                EntityId = entityId,
                FileType = file.ContentType,
                EntityTypeId = entityType.Id,
                Info = info,
                Name = info,
                Size = file.Length,
                StoredAs = storedAs,
                IsDeleted = false
            };

            _context.Add(entityFile);
            _context.SaveChanges();


            var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityType.Id).ToList();

            return Ok(entityFile);
        }

        [AllowAnonymous]
        [HttpPost("uploadReport")]
        public IActionResult UploadReport(IFormFile file, string entityTypeCode, string info)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");
            Model.EntityType entityType = _context.Set<Model.EntityType>().Where(e => e.Code == entityTypeCode).Single();

            //string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string storedAs = Guid.NewGuid().ToString() + ".pdf";
            string uploadFolder = entityType.UploadFolder;
            string filePath = uploadFolder + "\\" + storedAs;

            //if (!Directory.Exists(uploadFolder))
            //{
            //    var d = Directory.CreateDirectory(uploadFolder);
            //    var s = d.FullName;
            //}

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string costCenterCode = info.Substring(0, info.IndexOf('_'));
            var costCenter = _context.CostCenters
                                 .Where(cc => cc.Code == costCenterCode)
                                 .Select(cc => cc.Id)
                                 .FirstOrDefault();

            Model.EntityFile entityFile = new Model.EntityFile()
            {
                EntityId = null,
                FileType = file.ContentType,
                EntityTypeId = entityType.Id,
                Info = info,
                Name = info,
                Size = file.Length,
                CostCenterId=costCenter,
                StoredAs = storedAs,
                IsDeleted = false
            };

            _context.Add(entityFile);
            _context.SaveChanges();


            //var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityType.Id).ToList();

            return Ok(entityFile);
        }

        [HttpPost("uploadReception")]
        public IActionResult UploadReception(IFormFile file, int entityId, int entityTypeId, string info)
        {
            Model.Asset asset = null;
            Model.AssetState assetState = null;
            Model.EntityType entityType = null;


			if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");
            int documentId = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefault().DocumentId;
            entityType = _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).Single();
			assetState = _context.Set<Model.AssetState>().Where(e => e.Code == "TOVALIDATE").Single();

			string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string uploadFolder = entityType.UploadFolder;
            string filePath = uploadFolder + "\\" + storedAs;

            //if (!Directory.Exists(uploadFolder))
            //{
            //    var d = Directory.CreateDirectory(uploadFolder);
            //    var s = d.FullName;
            //}

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            Model.EntityFile entityFile = new Model.EntityFile()
            {
                EntityId = entityId,
                FileType = file.ContentType,
                EntityTypeId = entityType.Id,
                Info = info,
                Name = file.FileName,
                Size = file.Length,
                StoredAs = storedAs,
                IsDeleted = false
            };

            asset = _context.Set<Model.Asset>().Include(a => a.AssetState).Where(e => e.Id == entityId).Single();

            asset.IsLocked = false;

            if(asset.AssetState.Code == "RECEPTION")
            {
                if(entityType.Code == "F")
                {
                    asset.AssetStateId = assetState.Id;

				}
            }
            _context.Update(asset);

            _context.Add(entityFile);
            _context.SaveChanges();

           

            //var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityType.Id).ToList();

            return Ok(entityFile);
        }

		[HttpPost("uploadPreReception")]
        [AllowAnonymous]
		public async Task<IActionResult> UploadPreReception(IFormFile file, string entityIds, int entityTypeId, string info, string docNo1)
        {
            Model.EntityType entityType = null;

            var assetIds = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(entityIds);

            int assetStateId = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "PRE_RECEPTION").Select(a => a.Id).SingleOrDefault();

            if (file == null) throw new Exception("File is null");
			if (file.Length == 0) throw new Exception("File is empty");
			//inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefaultAsync();
			entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();

			string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
			string uploadFolder = entityType.UploadFolder;
			string filePath = uploadFolder + "\\" + storedAs;

			//if (!Directory.Exists(uploadFolder))
			//{
			//    var d = Directory.CreateDirectory(uploadFolder);
			//    var s = d.FullName;
			//}

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

            bool canYouUploadDocument = ((docNo1 != null && docNo1 != "" && docNo1.Length > 2)) ? true : false;   

            List<Model.Asset> assets = await _context.Set<Model.Asset>()
                .Include(d => d.Document)
                .Where(a => assetIds.Contains(a.Id))
                .ToListAsync();

            foreach (Model.Asset asset in assets)
            {
                Model.EntityFile entityFile = new Model.EntityFile()
                {
                    EntityId = asset.Id,
                    FileType = file.ContentType,
                    EntityTypeId = entityType.Id,
                    Info = info,
                    Name = file.FileName,
                    Size = file.Length,
                    StoredAs = storedAs,
                    IsDeleted = false,
                };

                _context.Add(entityFile);

                if(canYouUploadDocument)
                {
                    asset.Document.DocNo1 = docNo1;
                    asset.Document.Details = info;
                    _context.Update(asset.Document);
                }
                
                _context.SaveChanges();
            }
            return Ok(StatusCode(200));
        }

		[HttpPost("uploadStorno")]
		public IActionResult UploadStorno(IFormFile file, int entityId, int entityTypeId, string info, int stornoValue)
		{
			Model.Asset asset = null;
			Model.AppState appState = null;
			Model.EntityType entityType = null;
            Model.AssetState assetState = null;

			if (file == null) throw new Exception("File is null");
			if (file.Length == 0) throw new Exception("File is empty");
			int documentId = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefault().DocumentId;
			entityType = _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).Single();
			appState = _context.Set<Model.AppState>().Where(e => e.Code == "STORNO").Single();
			int assetStateId = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "TOVALIDATE").Select(a => a.Id).Single();

			string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
			string uploadFolder = entityType.UploadFolder;
			string filePath = uploadFolder + "\\" + storedAs;

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			Model.EntityFile entityFile = new Model.EntityFile()
			{
				EntityId = entityId,
				FileType = file.ContentType,
				EntityTypeId = entityType.Id,
				Info = info,
				Name = file.FileName,
				Size = file.Length,
				StoredAs = storedAs,
				IsDeleted = false
			};

			asset = _context.Set<Model.Asset>().Include(a => a.AssetState).Where(e => e.Id == entityId).Single();

			asset.IsLocked = false;
            asset.AppStateId = appState.Id;

            if(stornoValue > 0)
            {
				asset.StornoValue = stornoValue;
				asset.StornoValueRon = stornoValue;
                asset.AssetStateId = assetStateId;
			}
            

			_context.Update(asset);

			_context.Add(entityFile);
			_context.SaveChanges();



			//var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityType.Id).ToList();

			return Ok(entityFile);
		}

		[HttpPost("uploadPublicStorno")]
		public IActionResult UploadPublicStorno(IFormFile file, int entityId, int entityTypeId, string info)
		{
			Model.AppState appState = null;
			Model.EntityType entityType = null;

			if (file == null) throw new Exception("File is null");
			if (file.Length == 0) throw new Exception("File is empty");
			int documentId = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefault().DocumentId;
			entityType = _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).Single();
			appState = _context.Set<Model.AppState>().Where(e => e.Code == "STORNO_PUBLIC").Single();

			string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
			string uploadFolder = entityType.UploadFolder;
			string filePath = uploadFolder + "\\" + storedAs;

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			Model.EntityFile entityFile = new Model.EntityFile()
			{
				EntityId = entityId,
				FileType = file.ContentType,
				EntityTypeId = entityType.Id,
				Info = info,
				Name = file.FileName,
				Size = file.Length,
				StoredAs = storedAs,
				IsDeleted = false
			};

			_context.Add(entityFile);
			_context.SaveChanges();

			return Ok(entityFile);
		}

		[HttpPost("uploadRequest")]
        public async Task<EntityFileResult> UploadRequest(IFormFile file, int entityId, int entityTypeId, string info, int count)
        {
            Model.Request request = null;
            Model.AppState appState = null;
			Model.EntityType entityType = null;
            List<Model.RequestBudgetForecast> requestBudgetForecasts = new List<Model.RequestBudgetForecast>();
			Model.AppState appStateNewStatus = null;

            //_context.UserId = "99a93c5c-0f65-4487-9478-a16e4c60baec";

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);


                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if (user != null)
                {
                    _context.UserId = user.Id.ToString();

                    if (file == null) throw new Exception("File is null");
                    if (file.Length == 0) throw new Exception("File is empty");

                    int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(a => a.DocumentId).SingleAsync();
                    entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();

                    appState = await _context.Set<Model.AppState>().Where(a => a.Code == "NEED_BUDGET" && a.IsDeleted == false).SingleAsync();


					string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string uploadFolder = entityType.UploadFolder;
                    string filePath = uploadFolder + "\\" + storedAs;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Model.EntityFile entityFile = new Model.EntityFile()
                    {
                        EntityId = null,
                        FileType = file.ContentType,
                        EntityTypeId = entityType.Id,
                        Info = info,
                        Name = file.FileName,
                        Size = file.Length,
                        StoredAs = storedAs,
                        IsDeleted = false,
                        RequestId = entityId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId
                    };

                    request = await _context.Set<Model.Request>().Include(a => a.AppState).Include(d => d.Division).ThenInclude(d => d.Department).Where(e => e.Id == entityId).SingleAsync();

                    _context.Add(entityFile);
                    _context.SaveChanges();

                    List<int> countBooks = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.RequestId == request.Id && e.IsDeleted == false && e.EntityType.Code == "REQUEST_BOOK").Select(e => e.Id).ToListAsync();

                    if (countBooks.Count == count)
                    {
                        try
                        {
							if (request.AppState.Code == "ORDER_BOOK")
							{

								requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>().Where(r => r.RequestId == request.Id && r.IsDeleted == false).ToListAsync();
								appStateNewStatus = await _context.Set<Model.AppState>().Where(a => a.Code == "NEED_PLUS_BUDGET" && a.IsDeleted == false).SingleAsync();

								for (int i = 0; i < requestBudgetForecasts.Count; i++)
								{
									var res = await _requestsService.SendRequestBudgetForecastNeedBudget(requestBudgetForecasts[i].Id);

									if (res)
									{
										request.AppStateId = appStateNewStatus.Id;
										_context.Update(request);
									}
								}

								_context.SaveChanges();
								return new Model.EntityFileResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!", EntityFileId = entityId };
							}
							else if (request.AppState.Code == "REQUEST_BOOK")
							{
								// NEW BUDGET //

								Model.BudgetBase budget = null;
								// Model.BudgetOp budgetOp = null;
								Model.Document document = null;
								Model.DocumentType documentType = null;
								Model.EntityType entityTypeBGT = null;
								Model.Company company = null;
								Model.Country country = null;
								Model.Project project = null;
								Model.Activity activity = null;
								Model.AdmCenter admCenter = null;
								Model.Region region = null;
								Model.AssetType assetType = null;
								Model.ProjectType projectType = null;
								Model.AppState appStateBGT = null;
								// Model.Inventory inventory = null;
								Model.Inventory inventory = null;
								//Model.BudgetType budgetType = null;
								Model.BudgetType lastBudgetType = null;
								Model.BudgetType budgetTypeTotal = null;
								//Model.AccMonth accMonth = null;
								Model.AccMonth startAccMonth = null;
								Model.Department department = null;
								Model.Division division = null;
								Model.Uom uom = null;
								Model.Employee employee = null;

								Model.BudgetMonthBase budgetMonth1 = null;
								Model.BudgetMonthBase budgetMonth2 = null;
								Model.BudgetMonthBase budgetMonth3 = null;
								Model.BudgetMonthBase budgetMonth4 = null;
								Model.BudgetMonthBase budgetMonth5 = null;
								Model.BudgetMonthBase budgetMonth6 = null;
								Model.BudgetMonthBase budgetMonth7 = null;
								Model.BudgetMonthBase budgetMonth8 = null;
								Model.BudgetMonthBase budgetMonth9 = null;
								Model.BudgetMonthBase budgetMonth10 = null;
								Model.BudgetMonthBase budgetMonth11 = null;
								Model.BudgetMonthBase budgetMonth12 = null;

								Model.BudgetForecast budgetForecast = null;
								Model.BudgetMonthBase budgetMonthBase = null;
								//Model.BudgetMonth budgetTotal = null;

								Model.RequestBudgetForecast requestBudgetForecast = null;


								_context.UserId = _context.UserId;
								//int lastBgtType = 0;

								//accMonth = await _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).FirstOrDefaultAsync();
								lastBudgetType = await _context.Set<Model.BudgetForecast>()
									.Where(i => i.IsDeleted == false && i.IsLast == true)
									.Select(a => a.BudgetType)
									.AsNoTracking()
									.LastOrDefaultAsync();

								inventory = await _context.Set<Model.Inventory>().Where(i => i.Active == true && i.IsDeleted == false).FirstOrDefaultAsync();

								uom = await _context.Set<Model.Uom>().Where(i => i.Code == "RON" && i.IsDeleted == false).FirstOrDefaultAsync();

								//var ddd = lastBudgetType.Code.Substring(1, 1);

								//lastBgtType = int.Parse(ddd);
								//lastBgtType++;

								//budgetType = new Model.BudgetType
								//{
								//    Code = "V" + lastBgtType,
								//    Name = ((accMonth.Month.ToString().Length == 1 ? "0" + accMonth.Month.ToString() : accMonth.Month.ToString()) + (accMonth.Year.ToString())),
								//    IsDeleted = false
								//};

								//_context.Set<Model.BudgetType>().Add(budgetType);

								company = await _context.Set<Model.Company>().Where(c => c.Code == "RO10" && c.IsDeleted == false).FirstOrDefaultAsync();
								employee = await _context.Set<Model.Employee>().Where(c => c.Id == request.EmployeeId).FirstOrDefaultAsync();



								//country = await _context.Set<Model.Country>().Where(c => c.Id == request.CountryId).Single();
								//activity = await _context.Set<Model.Activity>().Where(c => c.Id == request.ActivityId).Single();
								if (request.Division != null)
								{
									division = await _context.Set<Model.Division>().Where(c => c.Id == request.DivisionId).FirstOrDefaultAsync();

									if (request.Division.Department != null)
									{
										department = await _context.Set<Model.Department>().Where(c => c.Id == request.Division.DepartmentId).FirstOrDefaultAsync();
									}
								}

								//admCenter = await _context.Set<Model.AdmCenter>().Where(c => c.Id == request.AdmCenterId).Single();
								//region = _context.Set<Model.Region>().Where(c => c.Id == budgetDto.request).Single();

								projectType = await _context.Set<Model.ProjectType>().Where(c => c.Id == request.ProjectTypeId).FirstOrDefaultAsync();
								assetType = await _context.Set<Model.AssetType>().Where(c => c.Id == request.AssetTypeId).FirstOrDefaultAsync();
								appStateBGT = await _context.Set<Model.AppState>().Where(c => c.Id == request.AppStateId).FirstOrDefaultAsync();
								startAccMonth = await _context.Set<Model.AccMonth>().Where(i => i.Id == request.StartAccMonthId).FirstOrDefaultAsync();

								if (request.ProjectId != null)
								{
									project = await _context.Set<Model.Project>().Where(c => c.Id == request.ProjectId).FirstOrDefaultAsync();
								}
								//                  else
								//                  {

								//	project = await _context.Set<Model.Project>().Where(p => p.Code == (country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code) && p.IsDeleted == false).SingleOrDefaultAsync();

								//	if (project == null)
								//	{
								//		project = new Model.Project
								//		{
								//			Code = country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code,
								//			Name = country.Code + "_" + department.Code + "_" + division.Code + "_" + projectType.Code + "_" + assetType.Code,
								//			IsDeleted = false,
								//			ProjectTypeId = projectType.Id
								//		};
								//	}

								//	_context.Set<Model.Project>().Add(project);
								//}

								entityTypeBGT = await _context.Set<Model.EntityType>().Where(c => c.UploadFolder == "NEWBUDGET").FirstOrDefaultAsync();
								var newBudgetCode = string.Empty;

								var lastCode = int.Parse(entityTypeBGT.Name);

								if (lastCode.ToString().Length == 1)
								{
									newBudgetCode = "B20240000" + entityTypeBGT.Name;
								}
								else if (lastCode.ToString().Length == 2)
								{
									newBudgetCode = "B2024000" + entityTypeBGT.Name;
								}
								else if (lastCode.ToString().Length == 3)
								{
									newBudgetCode = "B202400" + entityTypeBGT.Name;
								}
								else if (lastCode.ToString().Length == 4)
								{
									newBudgetCode = "B20240" + entityTypeBGT.Name;
								}
								else if (lastCode.ToString().Length == 5)
								{
									newBudgetCode = "B2024" + entityTypeBGT.Name;
								}

								documentType = await _context.Set<Model.DocumentType>().AsNoTracking().Where(d => d.Code == "ADD_NEW_BUDGET").FirstOrDefaultAsync();

								document = new Model.Document()
								{
									Approved = true,
									CompanyId = company.Id,
									CostCenterId = null,
									CreatedAt = DateTime.Now,
									CreatedBy = _context.UserId,
									CreationDate = DateTime.Now,
									Details = string.Empty,
									DocNo1 = string.Empty,
									DocNo2 = string.Empty,
									DocumentDate = DateTime.Now,
									DocumentTypeId = documentType.Id,
									Exported = true,
									IsDeleted = false,
									ModifiedAt = DateTime.Now,
									ModifiedBy = _context.UserId,
									ParentDocumentId = null,
									PartnerId = null,
									RegisterDate = DateTime.Now,
									ValidationDate = DateTime.Now
								};

								_context.Add(document);

								budget = new Model.BudgetBase()
								{
									AccMonthId = inventory.AccMonthBudgetId,
									EmployeeId = request.OwnerId,
									Project = project,
									Country = country,
									Activity = activity,
									Department = department,
									AdmCenter = admCenter,
									Region = region,
									Division = division,
									ProjectType = projectType,
									Info = request.Info,
									AssetType = assetType,
									AppState = appStateBGT,
									StartMonth = null,
									DepPeriod = 0,
									DepPeriodRem = 0,
									Code = newBudgetCode,
									Company = company,
									CreatedAt = DateTime.Now,
									CreatedBy = _context.UserId,
									IsAccepted = true,
									IsDeleted = false,
									ModifiedAt = DateTime.Now,
									ModifiedBy = _context.UserId,
									Name = newBudgetCode,
									UserId = _context.UserId,
									Validated = true,
									ValueFin = 0,
									ValueIni = 0,
									Total = 0,
									Uom = uom,
									//BudgetForecast = budgetForecast,
									//BudgetMonthBase = budgetMonthBase,
									BudgetTypeId = lastBudgetType.Id,
									BudgetManagerId = inventory.BudgetManagerId

								};
								_context.Add(budget);


								var sumMonth1 = 0;
								var sumMonth2 = 0;
								var sumMonth3 = 0;
								var sumMonth4 = 0;
								var sumMonth5 = 0;
								var sumMonth6 = 0;
								var sumMonth7 = 0;
								var sumMonth8 = 0;
								var sumMonth9 = 0;
								var sumMonth10 = 0;
								var sumMonth11 = 0;
								var sumMonth12 = 0;

								var startMonth = 0;

								if (startAccMonth != null)
								{
									startMonth = startAccMonth.Month;
								}
								int month = 0;

								if (startMonth > 0)
								{
									DateTime date = Convert.ToDateTime(startMonth, CultureInfo.InvariantCulture);

									month = date.Month;

									if (month == 4)
									{
										month = 1;

									}
									else if (month == 5)
									{
										month = 2;
									}
									else if (month == 6)
									{
										month = 3;
									}
									else if (month == 7)
									{
										month = 4;
									}
									else if (month == 8)
									{
										month = 5;
									}
									else if (month == 9)
									{
										month = 6;
									}
									else if (month == 10)
									{
										month = 7;
									}
									else if (month == 11)
									{
										month = 8;
									}
									else if (month == 12)
									{
										month = 9;
									}
									else if (month == 1)
									{
										month = 10;
									}
									else if (month == 2)
									{
										month = 11;
									}
									else if (month == 3)
									{
										month = 12;
									}

									startAccMonth = _context.Set<Model.AccMonth>().Where(a => a.Month == month && a.Year == 2022).Single();
								}

								budgetMonthBase = new Model.BudgetMonthBase()
								{
									BudgetBaseId = budget.Id,
									BudgetManagerId = inventory.BudgetManagerId.Value,
									BudgetTypeId = lastBudgetType.Id,
									IsFirst = false,
									IsLast = true,
									April = 0,
									May = 0,
									June = 0,
									July = 0,
									August = 0,
									September = 0,
									Octomber = 0,
									November = 0,
									December = 0,
									January = 0,
									February = 0,
									March = 0,
									AccMonthId = inventory.AccMonthBudgetId.Value
								};

								_context.Add(budgetMonthBase);


								budgetForecast = new Model.BudgetForecast()
								{
									BudgetBaseId = budget.Id,
									BudgetManagerId = inventory.BudgetManagerId.Value,
									BudgetTypeId = lastBudgetType.Id,
									IsFirst = false,
									IsLast = true,
									April = 0,
									May = 0,
									June = 0,
									July = 0,
									August = 0,
									September = 0,
									Octomber = 0,
									November = 0,
									December = 0,
									January = 0,
									February = 0,
									March = 0,
									AccMonthId = inventory.AccMonthBudgetId.Value
								};

								_context.Add(budgetForecast);


								if (month > 0)
								{
									if (month == 2)
									{
										budgetForecast.January = 0;

									}
									else if (month == 3)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;

									}
									else if (month == 4)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;

									}
									else if (month == 5)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;

									}
									else if (month == 6)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;

									}
									else if (month == 7)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;
										budgetForecast.June = 0;

									}
									else if (month == 8)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;
										budgetForecast.June = 0;
										budgetForecast.July = 0;

									}
									else if (month == 9)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;
										budgetForecast.June = 0;
										budgetForecast.July = 0;
										budgetForecast.August = 0;

									}
									else if (month == 10)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;
										budgetForecast.June = 0;
										budgetForecast.July = 0;
										budgetForecast.August = 0;
										budgetForecast.September = 0;

									}
									else if (month == 11)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;
										budgetForecast.June = 0;
										budgetForecast.July = 0;
										budgetForecast.August = 0;
										budgetForecast.September = 0;
										budgetForecast.Octomber = 0;

									}
									else if (month == 12)
									{
										budgetForecast.January = 0;
										budgetForecast.February = 0;
										budgetForecast.March = 0;
										budgetForecast.April = 0;
										budgetForecast.May = 0;
										budgetForecast.June = 0;
										budgetForecast.July = 0;
										budgetForecast.August = 0;
										budgetForecast.September = 0;
										budgetForecast.Octomber = 0;
										budgetForecast.November = 0;

									}

								}

								budget.StartMonth = startAccMonth;

								entityTypeBGT.Name = (int.Parse(entityTypeBGT.Name) + 1).ToString();
								_context.Update(entityTypeBGT);

								request.BudgetBase = budget;
								request.BudgetForecast = budgetForecast;

								_context.Update(request);


								requestBudgetForecast = new Model.RequestBudgetForecast()
								{
									RequestId = request.Id,
									BudgetForecast = budgetForecast,
									NeedBudget = false,
									NeedBudgetValue = request.BudgetValueNeed,
									AccMonthId = inventory.AccMonthBudgetId.Value,
									BudgetManagerId = inventory.BudgetManagerId.Value,
									Guid = Guid.NewGuid(),
                                    AppStateId = appState.Id
								};

								_context.Add(requestBudgetForecast);

								_context.SaveChanges();

								// NEW BUDGET //

								var res = await _requestsService.SendRequestNeedBudget(request.Id);

								if (res)
								{
                                    request.AppStateId = appState.Id;
                                    _context.Update(request);
                                    _context.SaveChanges();

									return new Model.EntityFileResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!", EntityFileId = entityId };
								}
								else
								{
									return new Model.EntityFileResult { Success = false, Message = $"Eroare trimitere notificare!", EntityFileId = entityId };
								}
							}

							return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = entityId };
						}
                        catch (Exception ex)
                        {
                            for (int i = 0; i < countBooks.Count; i++)
                            {
                                Model.EntityFile entityFileToDelete = await _context.Set<Model.EntityFile>().Where(e => e.Id == countBooks[i]).SingleAsync();

                                entityFileToDelete.IsDeleted = true;
                                _context.Update(entityFileToDelete);
                                _context.SaveChanges();

							}

							List<Model.RequestBudgetForecast> reqBFs = await _context.Set<Model.RequestBudgetForecast>().Where(e => e.RequestId == request.Id).ToListAsync();


							for (int i = 0; i < reqBFs.Count; i++)
							{
                                reqBFs[i].IsDeleted = true;
								_context.Update(reqBFs[i]);
								_context.SaveChanges();

							}

							return new Model.EntityFileResult { Success = false, Message = ex.Message, EntityFileId = entityId };
                        }


            }
            else
            {
                return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = entityId };
            }
                }
                else
                {
                    return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
                }
            }
            else
            {
                return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
            }
        }

        [HttpPost("deleteRequest/{entityId}")]
        public async Task<EntityFileResult> DeleteRequest(int entityId)
        {
            Model.EntityFile entityFile = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if(user == null)
                {
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

                if (user != null)
                {
                    _context.UserId = user.Id.ToString();

                    entityFile = await _context.Set<Model.EntityFile>().Where(e => e.Id == entityId).SingleAsync();

                    entityFile.IsDeleted = true;
                    entityFile.ModifiedAt = DateTime.Now;

                    _context.Update(entityFile);
                    _context.SaveChanges();

                    return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost sters cu succes!", EntityFileId = entityId };
                }
                else
                {
                    return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
                }
            }
            else
            {
                return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
            }
        }

        [HttpPost("uploadOfferUI")]
        public async Task<EntityFileResult> UploadOfferUI(IFormFile file, int entityId, int entityTypeId, string info, Guid guid, int quantity, int? requestId, int count)
        {
            Model.EntityType entityType = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if(user == null)
                {
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

                if (user != null)
                {
                    _context.UserId = user.Id.ToString();

                    if (file == null) throw new Exception("File is null");
                    if (file.Length == 0) throw new Exception("File is empty");

                    int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(a => a.DocumentId).SingleAsync();
                    entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();

                    string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string uploadFolder = entityType.UploadFolder;
                    string filePath = uploadFolder + "\\" + storedAs;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Model.EntityFile entityFile = new Model.EntityFile()
                    {
                        EntityId = null,
                        FileType = file.ContentType,
                        EntityTypeId = entityType.Id,
                        Info = info,
                        Name = file.FileName,
                        Size = file.Length,
                        StoredAs = storedAs,
                        IsDeleted = false,
                        PartnerId = entityId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        Guid = guid,
                        GuidAll = guid,
                        Quantity = quantity,
                        RequestId = requestId != null && requestId > 0 ? requestId : null
                    };

                    _context.Add(entityFile);
                    _context.SaveChanges();

                    return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = entityId };
                }
                else
                {
                    return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
                }
            }
            else
            {
                return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
            }
        }

        [HttpPost("uploadOrder")]
        public async Task<EntityFileResult> UploadOrder(IFormFile file, int orderId, int reqBFId, int entityTypeId, string info, int count)
        {
            Model.RequestBudgetForecast reqBF = null;
            Model.Order order = null;
            Model.AppState appState = null;
            Model.EntityType entityType = null;
            Model.EmailOrderStatus emailOrderStatus = null;
            List<Model.EmailOrderStatus> emailOrderStatuses = null;

            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(a => a.DocumentId).SingleAsync();
            entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();

            appState = await _context.Set<Model.AppState>().Where(a => a.Code == "NEED_BUDGET" && a.IsDeleted == false).SingleAsync();

            string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string uploadFolder = entityType.UploadFolder;
            string filePath = uploadFolder + "\\" + storedAs;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            Model.EntityFile entityFile = new Model.EntityFile()
            {
                EntityId = null,
                FileType = file.ContentType,
                EntityTypeId = entityType.Id,
                Info = info,
                Name = file.FileName,
                Size = file.Length,
                StoredAs = storedAs,
                IsDeleted = false,
                OrderId = orderId,
                RequestBudgetForecastId = reqBFId
            };

            order = await _context.Set<Model.Order>().Where(e => e.Id == orderId).SingleAsync();
            reqBF = await _context.Set<Model.RequestBudgetForecast>().Where(e => e.Id == reqBFId).SingleAsync();
            emailOrderStatus = await _context.Set<Model.EmailOrderStatus>().Where(e => e.RequestBudgetForecastId == reqBFId).SingleAsync();

            reqBF.AppStateId = appState.Id;
            _context.Update(reqBF);
            _context.Add(entityFile);
            _context.SaveChanges();

            List<int> countBooks = await _context.Set<Model.EntityFile>().Where(e => e.OrderId == order.Id && e.RequestBudgetForecastId == reqBFId).Select(e => e.Id).ToListAsync();

            if (countBooks.Count == count)
            {
                //await _ordersService.SendOrderNeedBudget(order.Id);

                emailOrderStatus.NeedBudgetEmailSend = false;
                emailOrderStatus.NotNeedBudgetSync = true;
                emailOrderStatus.SyncNeedBudgetErrorCount = 0;
                emailOrderStatus.AppStateId = appState.Id;
                _context.Update(emailOrderStatus);
                _context.SaveChanges();


                emailOrderStatuses = await _context.Set<Model.EmailOrderStatus>().Where(r => r.OrderId == orderId && r.AppStateId == 56 && r.EmailTypeId == 20).ToListAsync();

                if (emailOrderStatuses.Count == 0)
                {
                    order.AppStateId = appState.Id;
                    _context.Update(order);
                    _context.SaveChanges();
                }

                return new Model.EntityFileResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!", EntityFileId = reqBFId };

            }
            else
            {
                return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = reqBFId };
            }


            //var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityType.Id).ToList();
        }


        [AllowAnonymous]
        [HttpPost("uploadMobile")]
        public IActionResult UploadMobile(IFormFile file, int entityId, string entityTypeCode, string info, string userId, int roomId)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            Model.EntityType entityType = _context.Set<Model.EntityType>().AsNoTracking().Where(e => e.Code == entityTypeCode && e.Name == DateTime.Now.Year.ToString()).Single();

            string storedAs = Guid.NewGuid().ToString() + ".jpg";// + file.FileName.Substring(file.FileName.LastIndexOf("."));
            //string uploadFolder = entityType != null ? entityType.UploadFolder : "upload";
            string uploadFolder = entityType != null ? entityType.UploadFolder : "upload"; // original
            //string uploadFolder = "upload";
            string filePath = uploadFolder + "\\" + storedAs;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
                //stream.Write(file, 0, file.Length);
            }

            Model.EntityFile entityFile = new()
            {
                EntityId = entityId,
                FileType = file.ContentType,
                EntityTypeId = entityType.Id,
                Info = userId,
                Name = storedAs, //file.FileName,
                Size = file.Length,
                StoredAs = storedAs,
                IsDeleted = false,
                RoomId = roomId,
                CreatedBy = userId
            };

            if (entityTypeCode == "ASSET")
            {
                Model.Asset asset = _context.Assets.Single(a => a.Id == entityId);
                Model.Inventory inventory = _context.Set<Model.Inventory>().AsNoTracking().Single(a => a.Active == true);
                Model.InventoryAsset inventoryAsset = _context.Set<Model.InventoryAsset>().Single(a => a.InventoryId == inventory.Id && a.AssetId == entityId);
                inventoryAsset.ImageCount++;
                asset.ImageCount++;
                _context.Update(inventoryAsset);
                _context.Update(asset);
            }

            if (entityTypeCode == "LOCATION")
            {
                Model.Room room = _context.Rooms.Include(l => l.Location).Single(a => a.Id == entityId);

                room.Location.ImageCount = room.Location.ImageCount + 1;
                _context.Update(room.Location);
            }

            _context.Add(entityFile);
            _context.SaveChanges();

            return Ok(entityFile.Id);
        }

        [AllowAnonymous]
        [HttpDelete("deleteMobile/{id}")]
        public new void Delete(int id)
        {
            bool updateFlag = false;
            string entityTypeCode = "";
            int documentId = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefault().DocumentId;
            int entityTypeIdAsset = _context.EntityTypes.Single(e => e.Code == "ASSET" && e.Name == documentId.ToString()).Id;
            var entityFile = _context.EntityFiles.Include(e=> e.EntityType).Single(e => e.Id == id);
            entityTypeCode = entityFile.EntityType.Code;
            if (entityFile.EntityTypeId == entityTypeIdAsset) updateFlag = true;

            entityFile.IsDeleted = true;
            _itemsRepository.Update(entityFile);
            _context.SaveChanges();

            var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityTypeCode).ToList();

            //if (updateFlag)
            //{
            //    var asset = _context.Assets.Single(a => a.Id == entityFile.EntityId);

            //    if (asset != null)
            //    {
            //        asset.ImageCount = asset.ImageCount > 0 ? asset.ImageCount - 1 : 0;


            //        Model.Inventory inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(e => e.Active == true).Single();

            //        if (inventory != null)
            //        {
            //            Model.InventoryAsset invAsset = _context.Set<Model.InventoryAsset>().Where(e => e.AssetId == entityFile.EntityId && e.InventoryId == inventory.Id).Single();

            //            if (invAsset != null)
            //            {
            //                invAsset.ImageCount = invAsset.ImageCount > 0 ? invAsset.ImageCount - 1 : 0;
            //                _context.Update(invAsset);
            //            }
            //        }

            //        _context.Update(asset);
            //        _context.SaveChanges();
            //    }
            //}
        }


        //[Route("validate/{documentId}/{invNo}/{isManagerTransfer}")]
        //public async virtual Task<IActionResult> AssetOpProcess(int documentId, string invNo, int isManagerTransfer)
        //{

        //    Model.Asset assetPrev = null;
        //    Model.InvState invState = null;
        //    Model.AssetState assetState = null;
        //    Model.AccMonth accMonth = null;
        //    Model.AssetOp assetOpPrev = null;
        //    Model.AssetAdmMD assetAdmMDPrev = null;
        //    IQueryable<Model.AssetOp> assetOps2 = null;
        //    IQueryable<Model.AssetOp> assetOps3 = null;
        //    int assetOpIdByDocumentId = _context.Set<Model.AssetOp>().Include(a => a.Asset).Where(a => a.DocumentId == documentId && a.Asset.InvNo == invNo).Select(o => o.Id).SingleOrDefault();
        //    List<int> assetOpIds = new List<int>();
        //    assetOpIds.Add(assetOpIdByDocumentId);
        //    //assetOpIds = assetOpIds.Distinct().ToArray();
        //    assetOps2 = _context.AssetOps.AsNoTracking();
        //    assetOps3 = _context.AssetOps.AsNoTracking();
        //    Model.ApplicationUser user = null;
        //    var htmlBodyCompany1 = "";
        //    var htmlBodyCompany2 = "";
        //    var htmlBodyCompany3 = "";
        //    var htmlBodyCompany4 = "";
        //    var htmlBodyEnd2 = "";
        //    var htmlBodyEnd3 = "";

        //    foreach (var assetOpId in assetOpIds)
        //    {

        //        accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).Single();
        //        assetOpPrev = _context.Set<Model.AssetOp>().Include(d => d.Document).ThenInclude(d => d.DocumentType).Where(a => a.Id == assetOpId).SingleOrDefault();
        //        assetPrev = _context.Set<Model.Asset>().Include(c => c.Company).Where(a => a.Id == assetOpPrev.AssetId).SingleOrDefault();
        //        assetAdmMDPrev = _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assetOpPrev.AssetId && a.AccMonthId == accMonth.Id).SingleOrDefault();
        //        invState = _context.Set<Model.InvState>().Where(a => a.Id == assetOpPrev.InvStateIdFinal).SingleOrDefault();
        //        assetState = _context.Set<Model.AssetState>().Where(a => a.Id == assetOpPrev.AssetStateIdFinal).SingleOrDefault();

        //        if (assetOpPrev.AssetOpStateId == isManagerTransfer)
        //        {
        //            if (assetOpPrev.AssetOpStateId < 5)
        //            {


        //                string userName = HttpContext.User.Identity.Name;





        //                switch (assetOpPrev.AssetOpStateId)
        //                {
        //                    case 1:
        //                        assetOpPrev.ReleaseConfAt = DateTime.Now;
        //                        assetOpPrev.ReleaseConfBy = user.Id;
        //                        break;
        //                    case 2:


        //                        Model.AssetOp assetOp2 = null;
        //                        Model.Employee employeeIni2 = null;
        //                        Model.Employee employeeFin2 = null;
        //                        Model.Room roomIni2 = null;
        //                        Model.Room roomFin2 = null;
        //                        Model.Location locationIni2 = null;
        //                        Model.Location locationFin2 = null;

        //                        var roomNameIni2 = "";
        //                        var roomNameFin2 = "";
        //                        var roomCodeIni2 = "";
        //                        var roomCodeFin2 = "";
        //                        var employeeNameIni2 = "Nu exista";
        //                        var employeeNameFin2 = "Nu exista";

        //                        if (assetOpPrev.RoomIdInitial != null)
        //                        {
        //                            roomIni2 = _context.Set<Model.Room>().Where(a => a.Id == assetOpPrev.RoomIdInitial).FirstOrDefault();

        //                            if (roomIni2 != null)
        //                            {
        //                                locationIni2 = _context.Set<Model.Location>().Where(a => a.Id == roomIni2.LocationId).FirstOrDefault();

        //                                if (locationIni2 != null)
        //                                {
        //                                    employeeIni2 = _context.Set<Model.Employee>().Include(d => d.Division).Where(a => a.Id == locationIni2.EmployeeId).FirstOrDefault();
        //                                }
        //                            }
        //                        }

        //                        if (assetOpPrev.RoomIdFinal != null)
        //                        {
        //                            roomFin2 = _context.Set<Model.Room>().Where(a => a.Id == assetOpPrev.RoomIdFinal).FirstOrDefault();

        //                            if (roomFin2 != null)
        //                            {
        //                                locationFin2 = _context.Set<Model.Location>().Where(a => a.Id == roomFin2.LocationId).FirstOrDefault();

        //                                if (locationFin2 != null)
        //                                {
        //                                    employeeFin2 = _context.Set<Model.Employee>().Include(d => d.Division).Where(a => a.Id == locationFin2.EmployeeId).FirstOrDefault();
        //                                }
        //                            }
        //                        }

        //                        if (userName != null)
        //                        {
        //                            user = _context.Users.Where(u => u.UserName == userName).Single();
        //                        }
        //                        else
        //                        {
        //                            if (employeeIni2 != null && employeeIni2.Division != null)
        //                            {
        //                                user = _context.Users.Where(u => u.UserName == employeeIni2.Division.Name).FirstOrDefault();

        //                                if (user == null)
        //                                {
        //                                    user = _context.Users.Where(u => u.PhoneNumber == employeeIni2.Division.Name).FirstOrDefault();
        //                                }
        //                            }

        //                        }


        //                        //assetOpPrev.SrcConfAt = DateTime.Now;
        //                        //assetOpPrev.SrcConfBy = user != null ? user.Id : "";


        //                        assetOps2 = assetOps2
        //                              .Include(i => i.Asset)
        //                                  .ThenInclude(i => i.AssetInv)
        //                              .AsQueryable();

        //                        assetOp2 = assetOps2.Where(a => a.AssetId == assetOpPrev.AssetId).ToList().OrderByDescending(op => op.Id).Take(1).FirstOrDefault();

        //                        assetOpPrev.ReleaseConfAt = DateTime.Now;
        //                        assetOpPrev.ReleaseConfBy = user != null ? user.Id : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F";

        //                        var assetId2 = assetOp2.AssetId;

        //                        assetPrev.IsWaitingValidation = false;
        //                        assetPrev.IsInTransfer = true;

        //                        var company1 = assetPrev.Company.Name;
        //                        var company2 = assetPrev.Company.Name;

        //                        var rIniCode2 = roomIni2 != null ? roomIni2.Code : roomCodeIni2;
        //                        var rFinCode2 = roomFin2 != null ? roomFin2.Code : roomCodeFin2;
        //                        var rIniName2 = roomIni2 != null ? roomIni2.Name : roomNameIni2;
        //                        var rFinName2 = roomFin2 != null ? roomFin2.Name : roomNameFin2;

        //                        var eIni2 = employeeIni2 != null ? employeeIni2.FirstName : employeeNameIni2;
        //                        var eFin2 = employeeIni2 != null ? employeeIni2.Division != null ? employeeIni2.Division.Code : employeeNameFin2 : employeeNameFin2;

        //                        var emailIni21 = employeeIni2 != null ? employeeIni2.Email != null ? employeeIni2.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailIni22 = employeeIni2 != null ? employeeIni2.Division != null ? employeeIni2.Division.Name : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailFin21 = employeeFin2 != null ? employeeFin2.Email != null ? employeeFin2.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailFin22 = employeeFin2 != null ? employeeFin2.Division != null ? employeeFin2.Division.Name : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailCC2 = "adrian.cirnaru@optima.ro";



        //                        var link1 = "https://service.inventare.ro/FaisFortuna/api/entityfiles/validatedest/" + assetOp2.Id + "/3";
        //                        var link2 = "https://service.inventare.ro/FaisFortuna/api/entityfiles/notvalidatedest/" + assetOp2.Id;

        //                        // var subject2 = "Articole validate de manager";
        //                        var subject2 = assetOpPrev.Document.DocumentType.Code == "STATE_CHANGE" ? "Propuneri casari aprobate de manager" : "Articole aprobate de manager";

        //                        string htmlBody21 = @"

        //                                                <tr>
        //                                                    <th class=""red"">Gestiune Expeditor</th>
        //                                                    <th class=""red"">Cod Articol</th>
        //                                                    <th class=""red"">Denumire Articol</th>
        //                                                    <th class=""red"">Gestiune Destinatar</th>
        //                                                    <th class=""red"" colspan=""2"">Validare</th>

        //                                                </tr>
        //                                            </thead>
        //                                            <tbody>

        //                                               <tr>
        //                                                    <td class=""description"">" + rIniCode2 + @" </ td >
        //                                                    <td class=""description"">" + assetOp2.Asset.InvNo + @" </ td >
        //                                                    <td class=""description"">" + assetOp2.Asset.Name + @" </ td >
        //                                                    <td class=""description"">" + rFinCode2 + @" </ td >

        //                                ";

        //                        string htmlBody21Cass = @"

        //                                                <tr>
        //                                                    <th class=""red"">Gestiune</th>
        //                                                    <th class=""red"">Cod Articol</th>
        //                                                    <th class=""red"">Denumire Articol</th>
        //                                                    <th class=""red"" colspan=""2"">Validare</th>

        //                                                </tr>
        //                                            </thead>
        //                                            <tbody>

        //                                               <tr>
        //                                                    <td class=""description"">" + rIniCode2 + @" </ td >
        //                                                    <td class=""description"">" + assetOp2.Asset.InvNo + @" </ td >
        //                                                    <td class=""description"">" + assetOp2.Asset.Name + @" </ td >

        //                                ";

        //                        string htmlBody22 = @"
        //                                                    <td class=""description""><a href='" + link1 + "'" + "' >DA</a>" + @" </ td >
        //                                                    <td class=""description""><a href='" + link2 + "'" + "' >NU</a>" + @" </ td >
        //                                                </tr>

        //                                ";

        //                        htmlBodyEnd2 = htmlBodyEnd2 + @"</tbody>
        //                                        </table>
        //                                            <h3> Pentru validarea acestora, acceseaza urmatorul link  https://service.inventare.ro/BOFO/#/operations
        //                                            <br>
        //                                            <h3> Multumesc, </ h3 >
        //                                            <br>
        //                                            <h3> Manager " + eFin2 + @" </ h3 >
        //                                            <h3> Referent " + eIni2 + @" </ h3 >

        //                                    </body>
        //                                </html> ";

        //                        if (assetOpPrev.Document.DocumentType.Code == "STATE_CHANGE")
        //                        {
        //                            htmlBodyCompany1 = htmlBodyCompany1 + @"    <tr>
        //                                                    <th class=""red"" colspan=""5"">" + company1;

        //                            //htmlBodyCompany2 = htmlBodyCompany2 + @"</th>
        //                            //                        <th class=""red"" colspan=""3"">" + company2 + "</th></tr>"
        //                            //      ;
        //                        }
        //                        else
        //                        {
        //                            htmlBodyCompany1 = htmlBodyCompany1 + @"    <tr>
        //                                                    <th class=""red"" colspan=""3"">" + company1;

        //                            htmlBodyCompany2 = htmlBodyCompany2 + @"</th>
        //                                                    <th class=""red"" colspan=""3"">" + company2 + "</th></tr>"
        //                          ;
        //                        }




        //                        var htmlBody2 = @"
        //                                <html lang=""en"">
        //                                    <head>    
        //                                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
        //                                        <title>
        //                                            Upcoming topics
        //                                        </title>
        //                                        <style type=""text/css"">
        //                                            HTML{background-color: #e8e8e8;}
        //                                            .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
        //                                            .courses-table .description{color: #505050;}
        //                                            .courses-table td{border: 1px solid #05050A; background-color: #F9DF02; text-align: center; padding: 8px;}
        //                                            .courses-table th{border: 1px solid #424242; color: #030804;text-align: center; padding: 8px;}
        //                                            .red{background-color: #FFDD04;}
        //                                            .green{background-color: #6B9852;}
        //                                        </style>
        //                                    </head>
        //                                    <body>
        //                            <h2>Buna ziua,</h2>

        //                <br>
        //                                        <h2>Urmatoarele articole au fost validate de managerul expeditor si transferate catre managerul destinatar:</h2>
        //                                        <table class=""courses-table"">
        //                                            <thead>

        //                                ";

        //                        var htmlBody2Cass = @"
        //                                <html lang=""en"">
        //                                    <head>    
        //                                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
        //                                        <title>
        //                                            Upcoming topics
        //                                        </title>
        //                                        <style type=""text/css"">
        //                                            HTML{background-color: #e8e8e8;}
        //                                            .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
        //                                            .courses-table .description{color: #505050;}
        //                                            .courses-table td{border: 1px solid #05050A; background-color: #F9DF02; text-align: center; padding: 8px;}
        //                                            .courses-table th{border: 1px solid #424242; color: #030804;text-align: center; padding: 8px;}
        //                                            .red{background-color: #FFDD04;}
        //                                            .green{background-color: #6B9852;}
        //                                        </style>
        //                                    </head>
        //                                    <body>
        //                            <h2>Buna ziua,</h2>

        //                <br>
        //                                        <h2>Urmatoarele propuneri de casari au fost aprobate de manager si trimise catre validare!</h2>
        //                                        <table class=""courses-table"">
        //                                            <thead>

        //                                ";

        //                        if (assetOpPrev.Document.DocumentType.Code == "STATE_CHANGE")
        //                        {
        //                            htmlBody21 = htmlBody21Cass;
        //                            htmlBody2 = htmlBody2Cass;
        //                        }

        //                        htmlBody2 = htmlBody2 + htmlBodyCompany1 + htmlBodyCompany2 + htmlBody21 + htmlBody22 + htmlBodyEnd2;


        //                        var emailMessage2 = new MimeMessage();

        //                        //emailMessage2.From.Add(new MailboxAddress("Adrian Cirnaru", "adrian.cirnaru@optima.ro"));
        //                        emailMessage2.From.Add(new MailboxAddress("Transferuri OPTIMA", "ro.optima.transfer@fortunagroup.eu"));
        //                        emailMessage2.Cc.Add(new MailboxAddress("", emailIni21));
        //                        emailMessage2.Cc.Add(new MailboxAddress("", emailIni22));
        //                        emailMessage2.Cc.Add(new MailboxAddress("", emailFin21));
        //                        emailMessage2.To.Add(new MailboxAddress("", emailFin22));
        //                        emailMessage2.To.Add(new MailboxAddress("", emailCC2));

        //                        emailMessage2.Subject = subject2;
        //                        //emailMessage.Body = new TextPart("plain") { Text = message };

        //                        var builder2 = new BodyBuilder { TextBody = htmlBody2, HtmlBody = htmlBody2 };
        //                        //  builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
        //                        emailMessage2.Body = builder2.ToMessageBody();

        //                        using (var client = new SmtpClient())
        //                        {

        //                            await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
        //                            client.AuthenticationMechanisms.Remove("XOAUTH2");

        //                            //await client.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
        //                            await client.AuthenticateAsync("ro.optima.transfer@fortunagroup.eu", "opt32!TRA");
        //                            await client.SendAsync(emailMessage2).ConfigureAwait(false);
        //                            await client.DisconnectAsync(true).ConfigureAwait(false);


        //                        }


        //                        break;
        //                    case 4:
        //                        assetOpPrev.RegisterConfAt = DateTime.Now;
        //                        assetOpPrev.RegisterConfBy = user.Id;
        //                        break;
        //                    default:
        //                        break;
        //                }

        //                assetOpPrev.AssetOpStateId += 1;
        //                _context.UserName = userName;
        //                _context.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            return Redirect("https://service.inventare.ro/BOFO/#/alreadyvalidate");
        //        }


        //    }

        //    return Redirect("https://service.inventare.ro/BOFO/#/validate");
        //}

        [Route("notvalidate/{documentId?}")]
        public async virtual Task<IActionResult> DeleteAssetTransfer(int documentId)
        {

            var assetOp =  _context.Set<Model.AssetOp>().Where(a => a.DocumentId == documentId).Single();

            if (assetOp != null)
            {
                assetOp.IsDeleted = true;

                _context.Update(assetOp);


                var asset = _context.Set<Model.Asset>().Where(a => a.Id == assetOp.AssetId).Single();

                if (asset != null)
                {
                    asset.IsInTransfer = false;
                    asset.IsAccepted = false;
                    _context.Update(asset);
                }
            }

            _context.SaveChanges();

			//return Redirect("http://localhost:3100/#/notvalidate");
			return Redirect("https://service.inventare.ro/SocgenDemo/#/notvalidate");

        }

        //[Route("validatedest/{assetOperationId?}/{isManagerTransfer}")]
        //public async virtual Task<IActionResult> AssetOpProcessDest(int assetOperationId, int isManagerTransfer)
        //{

        //    Model.Asset assetPrev = null;
        //    Model.InvState invState = null;
        //    Model.AssetState assetState = null;
        //    Model.AccMonth accMonth = null;
        //    Model.AssetOp assetOpPrev = null;
        //    Model.AssetAdmMD assetAdmMDPrev = null;
        //    Model.TransfersPerMonth transfersPerMonth = null;
        //    Model.AccMonth accMonthTransfer = null;
        //    IQueryable<Model.AssetOp> assetOps2 = null;
        //    IQueryable<Model.AssetOp> assetOps3 = null;
        //    int assetOpIdById = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOperationId).Select(o => o.Id).SingleOrDefault();
        //    List<int> assetOpIds = new List<int>();
        //    assetOpIds.Add(assetOpIdById);
        //    //assetOpIds = assetOpIds.Distinct().ToArray();
        //    assetOps2 = _context.AssetOps.AsNoTracking();
        //    assetOps3 = _context.AssetOps.AsNoTracking();
        //    Model.ApplicationUser user = null;
        //    var htmlBodyCompany1 = "";
        //    var htmlBodyCompany2 = "";
        //    var htmlBodyCompany3 = "";
        //    var htmlBodyCompany4 = "";
        //    var htmlBodyEnd2 = "";
        //    var htmlBodyEnd3 = "";

        //    foreach (var assetOpId in assetOpIds)
        //    {

        //        accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).Single();
        //        assetOpPrev = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpId).SingleOrDefault();
        //        assetPrev = _context.Set<Model.Asset>().Include(c => c.Company).Include(d => d.Document).Where(a => a.Id == assetOpPrev.AssetId).SingleOrDefault();
        //        assetAdmMDPrev = _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == assetOpPrev.AssetId && a.AccMonthId == accMonth.Id).SingleOrDefault();
        //        invState = _context.Set<Model.InvState>().Where(a => a.Id == assetOpPrev.InvStateIdFinal).SingleOrDefault();
        //        assetState = _context.Set<Model.AssetState>().Where(a => a.Id == assetOpPrev.AssetStateIdFinal).SingleOrDefault();

        //        if (assetOpPrev.AssetOpStateId == isManagerTransfer)
        //        {
        //            if (assetOpPrev.AssetOpStateId < 5)
        //            {


        //                string userName = HttpContext.User.Identity.Name;

        //                switch (assetOpPrev.AssetOpStateId)
        //                {
        //                    case 1:
        //                        assetOpPrev.ReleaseConfAt = DateTime.Now;
        //                        assetOpPrev.ReleaseConfBy = user.Id;
        //                        break;
        //                    case 3:

        //                        Model.AssetOp assetOp3 = null;
        //                        Model.Employee employeeIni3 = null;
        //                        Model.Employee employeeFin3 = null;
        //                        Model.Room roomIni3 = null;
        //                        Model.Room roomFin3 = null;
        //                        Model.Location locationIni3 = null;
        //                        Model.Location locationFin3 = null;

        //                        var roomNameIni3 = "";
        //                        var roomNameFin3 = "";
        //                        var roomCodeIni3 = "";
        //                        var roomCodeFin3 = "";
        //                        var employeeNameIni3 = "Nu exista";
        //                        var employeeNameFin3 = "Nu exista";

        //                        if (assetOpPrev.RoomIdInitial != null)
        //                        {
        //                            roomIni3 = _context.Set<Model.Room>().Where(a => a.Id == assetOpPrev.RoomIdInitial).FirstOrDefault();

        //                            if (roomIni3 != null)
        //                            {
        //                                locationIni3 = _context.Set<Model.Location>().Where(a => a.Id == roomIni3.LocationId).FirstOrDefault();

        //                                if (locationIni3 != null)
        //                                {
        //                                    employeeIni3 = _context.Set<Model.Employee>().Include(d => d.Division).Where(a => a.Id == locationIni3.EmployeeId).FirstOrDefault();
        //                                }
        //                            }
        //                        }

        //                        if (assetOpPrev.RoomIdFinal != null)
        //                        {
        //                            roomFin3 = _context.Set<Model.Room>().Where(a => a.Id == assetOpPrev.RoomIdFinal).FirstOrDefault();

        //                            if (roomFin3 != null)
        //                            {
        //                                locationFin3 = _context.Set<Model.Location>().Where(a => a.Id == roomFin3.LocationId).FirstOrDefault();

        //                                if (locationFin3 != null)
        //                                {
        //                                    employeeFin3 = _context.Set<Model.Employee>().Include(d => d.Division).Where(a => a.Id == locationFin3.EmployeeId).FirstOrDefault();
        //                                }
        //                            }
        //                        }

        //                        if (userName != null)
        //                        {
        //                            user = _context.Users.Where(u => u.UserName == userName).Single();
        //                        }
        //                        else
        //                        {
        //                            if (employeeFin3 != null && employeeFin3.Division != null)
        //                            {
        //                                user = _context.Users.Where(u => u.UserName == employeeFin3.Division.Name).FirstOrDefault();

        //                                if (user == null)
        //                                {
        //                                    user = _context.Users.Where(u => u.PhoneNumber == employeeFin3.Division.Name).FirstOrDefault();
        //                                }
        //                            }

        //                        }



        //                        assetOps3 = assetOps3
        //                              .Include(i => i.Asset)
        //                                  .ThenInclude(i => i.AssetInv)
        //                              .AsQueryable();


        //                        assetOp3 = assetOps3.Where(a => a.AssetId == assetOpPrev.AssetId).ToList().OrderByDescending(op => op.Id).Take(1).FirstOrDefault();
        //                        int? employeeId = null;

        //                        assetOpPrev.DstConfAt = DateTime.Now;
        //                        assetOpPrev.DstConfBy = user != null ? user.Id : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F";
        //                        assetPrev.RoomId = roomFin3.Id;
        //                        assetPrev.EmployeeId = employeeFin3 != null ? employeeFin3.Id : employeeId;
        //                        assetPrev.AssetStateId = assetState.Id;
        //                        assetPrev.InvStateId = assetOpPrev.InvStateIdFinal;
        //                        assetPrev.IsInTransfer = false;
        //                        assetPrev.AssetStateId = 20;
        //                        assetPrev.Document.ValidationDate = DateTime.Now;

        //                        var company3 = assetPrev.Company.Name;
        //                        var company4 = assetPrev.Company.Name;

        //                        assetAdmMDPrev.RoomId = roomFin3.Id;
        //                        assetAdmMDPrev.EmployeeId = employeeFin3 != null ? employeeFin3.Id : employeeId;
        //                        assetAdmMDPrev.AssetStateId = 20;

        //                        var assetId3 = assetOp3.AssetId;

        //                        var rIniCode3 = roomIni3 != null ? roomIni3.Code : roomCodeIni3;
        //                        var rFinCode3 = roomFin3 != null ? roomFin3.Code : roomCodeFin3;
        //                        var rIniName3 = roomIni3 != null ? roomIni3.Name : roomNameIni3;
        //                        var rFinName3 = roomFin3 != null ? roomFin3.Name : roomNameFin3;

        //                        var eIni31 = employeeIni3 != null ? employeeIni3.FirstName : employeeNameIni3;
        //                        var eIni32 = employeeIni3 != null ? employeeIni3.Division != null ? employeeIni3.Division.Code : employeeNameIni3 : employeeNameIni3;
        //                        var eFin31 = employeeFin3 != null ? employeeFin3.FirstName : employeeNameFin3;
        //                        var eFin32 = employeeFin3 != null ? employeeFin3.Division != null ? employeeFin3.Division.Code : employeeNameFin3 : employeeNameFin3;

        //                        var emailIni31 = employeeIni3 != null ? employeeIni3.Email != null ? employeeIni3.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailIni32 = employeeIni3 != null ? employeeIni3.Division != null ? employeeIni3.Division.Name : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailFin31 = employeeFin3 != null ? employeeFin3.Email != null ? employeeFin3.Email : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailFin32 = employeeFin3 != null ? employeeFin3.Division != null ? employeeFin3.Division.Name : "adrian.cirnaru@optima.ro" : "adrian.cirnaru@optima.ro";
        //                        var emailCC3 = "adrian.cirnaru@optima.ro";



        //                        //var link3 = "https://service.inventare.ro/FaisFortuna/api/entityfiles/validatedest/" + assetOp3.Id + "/3";
        //                        //var link4 = "https://service.inventare.ro/FaisFortuna/api/entityfiles/notvalidatedest/" + assetOp3.Id;

        //                        //var subject3 = "Articole validate de managerul destinatar";
        //                        var subject3 = assetOpPrev.Document.DocumentType.Code == "STATE_CHANGE" ? "Propuneri casari validate" : "Articole validate de managerul destinatar";

        //                        string htmlBody31 = @"

        //                                                <tr>
        //                                                    <th class=""red"">Gestiune Expeditor</th>
        //                                                    <th class=""red"">Cod Articol</th>
        //                                                    <th class=""red"">Denumire Articol</th>
        //                                                    <th class=""red"">Gestiune Destinatar</th>


        //                                                </tr>
        //                                            </thead>
        //                                            <tbody>

        //                                               <tr>
        //                                                    <td class=""description"">" + rIniCode3 + @" </ td >
        //                                                    <td class=""description"">" + assetOp3.Asset.InvNo + @" </ td >
        //                                                    <td class=""description"">" + assetOp3.Asset.Name + @" </ td >
        //                                                    <td class=""description"">" + rFinCode3 + @" </ td >

        //                                ";

        //                        string htmlBody31Cass = @"

        //                                                <tr>
        //                                                    <th class=""red"">Gestiune</th>
        //                                                    <th class=""red"">Cod Articol</th>
        //                                                    <th class=""red"">Denumire Articol</th>


        //                                                </tr>
        //                                            </thead>
        //                                            <tbody>

        //                                               <tr>
        //                                                    <td class=""description"">" + rIniCode3 + @" </ td >
        //                                                    <td class=""description"">" + assetOp3.Asset.InvNo + @" </ td >
        //                                                    <td class=""description"">" + assetOp3.Asset.Name + @" </ td >

        //                                ";

        //                        string htmlBody32 = @"

        //                                                </tr>

        //                                ";



        //                        htmlBodyEnd3 = htmlBodyEnd3 + @"</tbody>
        //                                        </table>
        //                                            <h3> Pentru validarea acestora, acceseaza urmatorul link  https://service.inventare.ro/BOFO/#/operations
        //                                            <br>
        //                                            <h3> Multumesc, </ h3 >
        //                                            <br>
        //                                            <h3> Manager Expeditor:  " + eIni32 + @" </ h3 >
        //                                            <h3> Referent Expeditor: " + eIni31 + @" </ h3 >
        //                                            <h3> Manager Destinatar:  " + eFin32 + @" </ h3 >
        //                                            <h3> Referent Destinatar:  " + eFin31 + @" </ h3 >

        //                                    </body>
        //                                </html> ";

        //                        if (assetOpPrev.Document.DocumentType.Code == "STATE_CHANGE")
        //                        {
        //                            htmlBodyCompany3 = htmlBodyCompany3 + @"    <tr>
        //                                                    <th class=""red"" colspan=""5"">" + company3;

        //                            //htmlBodyCompany4 = htmlBodyCompany4 + @"</th>
        //                            //                        <th class=""red"" colspan=""3"">" + company4 + "</th></tr>"
        //                            //      ;
        //                        }
        //                        else
        //                        {
        //                            htmlBodyCompany3 = htmlBodyCompany3 + @"    <tr>
        //                                                    <th class=""red"" colspan=""3"">" + company3;

        //                            htmlBodyCompany4 = htmlBodyCompany4 + @"</th>
        //                                                    <th class=""red"" colspan=""3"">" + company4 + "</th></tr>"
        //                                  ;

        //                        }




        //                        var htmlBody3 = @"
        //                                <html lang=""en"">
        //                                    <head>    
        //                                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
        //                                        <title>
        //                                            Upcoming topics
        //                                        </title>
        //                                        <style type=""text/css"">
        //                                            HTML{background-color: #e8e8e8;}
        //                                            .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
        //                                            .courses-table .description{color: #505050;}
        //                                            .courses-table td{border: 1px solid #05050A; background-color: #F9DF02; text-align: center; padding: 8px;}
        //                                            .courses-table th{border: 1px solid #424242; color: #030804;text-align: center; padding: 8px;}
        //                                            .red{background-color: #FFDD04;}
        //                                            .green{background-color: #6B9852;}
        //                                        </style>
        //                                    </head>
        //                                    <body>
        //                            <h2>Buna ziua,</h2>

        //                <br>
        //                                        <h2>Urmatoarele articole au fost validate de managerul destinatar:</h2>
        //                                        <table class=""courses-table"">
        //                                            <thead>

        //                                ";

        //                        var htmlBody3Cass = @"
        //                                <html lang=""en"">
        //                                    <head>    
        //                                        <meta content=""text/html; charset=utf-8"" http-equiv=""Content-Type"">
        //                                        <title>
        //                                            Upcoming topics
        //                                        </title>
        //                                        <style type=""text/css"">
        //                                            HTML{background-color: #e8e8e8;}
        //                                            .courses-table{font-size: 12px; padding: 3px; border-collapse: collapse; border-spacing: 0;}
        //                                            .courses-table .description{color: #505050;}
        //                                            .courses-table td{border: 1px solid #05050A; background-color: #F9DF02; text-align: center; padding: 8px;}
        //                                            .courses-table th{border: 1px solid #424242; color: #030804;text-align: center; padding: 8px;}
        //                                            .red{background-color: #FFDD04;}
        //                                            .green{background-color: #6B9852;}
        //                                        </style>
        //                                    </head>
        //                                    <body>
        //                            <h2>Buna ziua,</h2>

        //                <br>
        //                                        <h2>Urmatoarele propuneri de casari au fost validate de manager!</h2>
        //                                        <table class=""courses-table"">
        //                                            <thead>

        //                                ";

        //                        if (assetOpPrev.Document.DocumentType.Code == "STATE_CHANGE")
        //                        {
        //                            htmlBody31 = htmlBody31Cass;
        //                            htmlBody3 = htmlBody3Cass;
        //                        }

        //                        htmlBody3 = htmlBody3 + htmlBodyCompany3 + htmlBodyCompany4 + htmlBody31 + htmlBody32 + htmlBodyEnd3;

        //                        var emailMessage3 = new MimeMessage();

        //                        //emailMessage3.From.Add(new MailboxAddress("Adrian Cirnaru", "adrian.cirnaru@optima.ro"));
        //                        emailMessage3.From.Add(new MailboxAddress("Transferuri OPTIMA", "ro.optima.transfer@fortunagroup.eu"));
        //                        emailMessage3.Cc.Add(new MailboxAddress("", emailIni31));
        //                        emailMessage3.To.Add(new MailboxAddress("", emailIni32));
        //                        emailMessage3.Cc.Add(new MailboxAddress("", emailFin31));
        //                        emailMessage3.Cc.Add(new MailboxAddress("", emailFin32));
        //                        emailMessage3.Cc.Add(new MailboxAddress("", emailCC3));

        //                        emailMessage3.Subject = subject3;

        //                        var builder3 = new BodyBuilder { TextBody = htmlBody3, HtmlBody = htmlBody3 };
        //                        emailMessage3.Body = builder3.ToMessageBody();

        //                        using (var client = new SmtpClient())
        //                        {

        //                            await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
        //                            client.AuthenticationMechanisms.Remove("XOAUTH2");

        //                            //await client.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
        //                            await client.AuthenticateAsync("ro.optima.transfer@fortunagroup.eu", "opt32!TRA");
        //                            await client.SendAsync(emailMessage3).ConfigureAwait(false);
        //                            await client.DisconnectAsync(true).ConfigureAwait(false);


        //                        }

        //                        if (assetOpPrev.RoomIdInitial == 2964 || assetOpPrev.RoomIdInitial == 2965 || assetOpPrev.RoomIdInitial == 2966 || assetOpPrev.RoomIdInitial == 2967 || assetOpPrev.RoomIdInitial == 2968 || assetOpPrev.RoomIdInitial == 2969)
        //                        {
        //                            accMonthTransfer = _context.Set<Model.AccMonth>().Where(acc => acc.Year == DateTime.UtcNow.Year && acc.Month == DateTime.UtcNow.Month).Single();
        //                            transfersPerMonth = _context.Set<Model.TransfersPerMonth>().Where(a => a.AssetId == assetOpPrev.AssetId && a.AccMonthId == accMonthTransfer.Id).SingleOrDefault();

        //                            if (transfersPerMonth == null)
        //                            {
        //                                transfersPerMonth = new Model.TransfersPerMonth
        //                                {
        //                                    AssetOpId = assetOpPrev.Id,
        //                                    AssetId = assetOpPrev.AssetId,
        //                                    AccMonthId = accMonthTransfer.Id,
        //                                    CreatedAt = DateTime.UtcNow,
        //                                    CreatedBy = user != null ? user.Id : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F",
        //                                    ModifiedAt = DateTime.UtcNow,
        //                                    ModifiedBy = user != null ? user.Id : "92E74C4F-A79A-4C83-A7D0-A3202BD2507F"
        //                                };

        //                                _context.Add(transfersPerMonth);
        //                            }
        //                        }


        //                        break;
        //                    case 4:
        //                        assetOpPrev.RegisterConfAt = DateTime.Now;
        //                        assetOpPrev.RegisterConfBy = user.Id;
        //                        break;
        //                    default:
        //                        break;
        //                }

        //                assetOpPrev.AssetOpStateId += 1;
        //                _context.UserName = userName;
        //                _context.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            return Redirect("https://service.inventare.ro/BOFO/#/alreadyvalidatedest");
        //        }


        //    }

        //    return Redirect("https://service.inventare.ro/BOFO/#/validatedest");
        //}

        //[Route("notvalidatedest/{assetOpId?}")]
        //public async virtual Task<IActionResult> DeleteAssetTransferDest(int assetOpId)
        //{

        //    var assetOp = _context.Set<Model.AssetOp>().Where(a => a.Id == assetOpId).Single();

        //    if (assetOp != null)
        //    {
        //        assetOp.IsDeleted = true;

        //        _context.Update(assetOp);


        //        var asset = _context.Set<Model.Asset>().Where(a => a.Id == assetOp.AssetId).Single();

        //        if (asset != null)
        //        {
        //            asset.IsInTransfer = false;
        //            asset.IsWaitingValidation = false;
        //            _context.Update(asset);
        //        }
        //    }

        //    _context.SaveChanges();

        //    return Redirect("http://localhost:3100/#/notvalidatedest");

        //}


        [HttpPost("uploadNewAsset")]
        public IActionResult UploadInitial(IFormFile[] file, Guid guid, string entityTypeCode)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            Model.EntityType entityType = null;

            if (entityTypeCode == "NEWASSET")
            {
                entityType = _context.Set<Model.EntityType>().Where(e => e.Code == entityTypeCode).Single();
            }

            for (int i = 0; i < file.Length; i++)
            {
                string storedAs = Guid.NewGuid().ToString() + file[i].FileName.Substring(file[i].FileName.LastIndexOf("."));
                string uploadFolder = entityType.UploadFolder;
                string filePath = uploadFolder + "\\" + storedAs;


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file[i].CopyTo(stream);
                }

                Model.EntityFile entityFile = new()
                {
                    EntityId = 251615,
                    FileType = file[i].ContentType,
                    EntityTypeId = entityType.Id,
                    Info = "",
                    Name = file[i].FileName,
                    Size = file.Length,
                    StoredAs = storedAs,
                    IsDeleted = false,
                    Guid = Guid.NewGuid(),
                    GuidAll = guid,
                    IsEmailSend = false
                };

                _context.Add(entityFile);
                _context.SaveChanges();


            }

            return Ok(251615);
        }

        [HttpPost("uploadNewOffer")]
        public IActionResult UploadNewOffer(IFormFile[] file, Guid guid, string entityTypeCode, int partnerId)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            Model.EntityType entityType = null;

            if (entityTypeCode == "NEWOFFER")
            {
                entityType = _context.Set<Model.EntityType>().Where(e => e.Code == entityTypeCode).Single();
            }

            for (int i = 0; i < file.Length; i++)
            {
                string storedAs = Guid.NewGuid().ToString() + file[i].FileName.Substring(file[i].FileName.LastIndexOf("."));
                string uploadFolder = entityType.UploadFolder;
                string filePath = uploadFolder + "\\" + storedAs;


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file[i].CopyTo(stream);
                }

                Model.EntityFile entityFile = new()
                {
                    EntityId = 251615,
                    FileType = file[i].ContentType,
                    EntityTypeId = entityType.Id,
                    Info = "",
                    Name = file[i].FileName,
                    Size = file.Length,
                    StoredAs = storedAs,
                    IsDeleted = false,
                    Guid = Guid.NewGuid(),
                    GuidAll = guid,
                    PartnerId = partnerId,
                    IsEmailSend = false
                };

                _context.Add(entityFile);
                _context.SaveChanges();


            }

            return Ok(251615);
        }

        [HttpPost("uploadRequestDocument")]
        public async Task<EntityFileResult> UploadRequestDocument(IFormFile file, int entityId, int entityTypeId, string info, decimal quantity, int count)
        {
            Model.Request request = null;
            //Model.AppState appState = null;
            Model.EntityType entityType = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

				if (user != null)
                {
                    _context.UserId = user.Id.ToString();

                    if (file == null) throw new Exception("File is null");
                    if (file.Length == 0) throw new Exception("File is empty");

                    int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(a => a.DocumentId).SingleAsync();
                    entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();

                    //appState = await _context.Set<Model.AppState>().Where(a => a.Code == "NEED_BUDGET" && a.IsDeleted == false).SingleAsync();

                    string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string uploadFolder = entityType.UploadFolder;
                    string filePath = uploadFolder + "\\" + storedAs;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Model.EntityFile entityFile = new Model.EntityFile()
                    {
                        EntityId = null,
                        FileType = file.ContentType,
                        EntityTypeId = entityType.Id,
                        Info = info,
                        Name = file.FileName,
                        Size = file.Length,
                        StoredAs = storedAs,
                        IsDeleted = false,
                        RequestId = entityId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        Quantity = quantity
                    };

                    //request = await _context.Set<Model.Request>().Where(e => e.Id == entityId).SingleAsync();

                    //request.AppStateId = appState.Id;
                    //_context.Update(request);

                    _context.Add(entityFile);
                    _context.SaveChanges();

                    return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = entityId };
                }
                else
                {
                    return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
                }
            }
            else
            {
                return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
            }
        }

        [HttpPost("uploadRequestBudgetForecastDocument")]
        public async Task<EntityFileResult> UploadRequestBudgetForecastDocument(IFormFile file, int entityId, int entityTypeId, string info, decimal quantity, int count)
        {
            Model.RequestBudgetForecast requestBudgetForecast = null;
            Model.EntityType entityType = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if(user == null)
                {
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

                if (user != null)
                {
                    _context.UserId = user.Id.ToString();

                    if (file == null) throw new Exception("File is null");
                    if (file.Length == 0) throw new Exception("File is empty");

                    int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(a => a.DocumentId).SingleAsync();
                    entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();
                    requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>().Where(e => e.Id == entityId).SingleAsync();


                    string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string uploadFolder = entityType.UploadFolder;
                    string filePath = uploadFolder + "\\" + storedAs;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Model.EntityFile entityFile = new Model.EntityFile()
                    {
                        EntityId = null,
                        FileType = file.ContentType,
                        EntityTypeId = entityType.Id,
                        Info = info,
                        Name = file.FileName,
                        Size = file.Length,
                        StoredAs = storedAs,
                        IsDeleted = false,
                        RequestId = requestBudgetForecast.RequestId,
                        RequestBudgetForecastId = entityId,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        Quantity = quantity,
                        SkipEmail = false
                    };


                    _context.Add(entityFile);
                    _context.SaveChanges();

                    return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = entityId };
                }
                else
                {
                    return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
                }
            }
            else
            {
                return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
            }
        }

		[HttpPost("uploadRequestTRansferBudgetForecast")]
		public async Task<EntityFileResult> UploadRequestTransferBudgetForecast(IFormFile file, int entityId, int budgetBaseOpId, int entityTypeId, string info, decimal quantity, int count)
		{
			Model.BudgetForecast budgetForecast = null;
			Model.EntityType entityType = null;
            Model.Employee employee = null;

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user != null)
				{
					_context.UserId = user.Id.ToString();

					if (file == null) throw new Exception("File is null");
					if (file.Length == 0) throw new Exception("File is empty");

					int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(a => a.DocumentId).SingleAsync();
					entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();
					budgetForecast = await _context.Set<Model.BudgetForecast>().Where(e => e.Id == entityId).SingleAsync();
					employee = await _context.Set<Model.Employee>().Where(e => e.Id == user.EmployeeId).FirstOrDefaultAsync();
                    if (employee == null) return new EntityFileResult {  Success = false, Message = "Userul nu are alocat un angajat!"};


					string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
					string uploadFolder = entityType.UploadFolder;
					string filePath = uploadFolder + "\\" + storedAs;

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						file.CopyTo(stream);
					}

					Model.EntityFile entityFile = new Model.EntityFile()
					{
						EntityId = null,
						FileType = file.ContentType,
						EntityTypeId = entityType.Id,
						Info = info,
						Name = file.FileName,
						Size = file.Length,
						StoredAs = storedAs,
						IsDeleted = false,
						RequestId = null,
						RequestBudgetForecastId = null,
                        BudgetForecastId = entityId,
                        BudgetBaseOpId = budgetBaseOpId,
						CreatedAt = DateTime.Now,
						CreatedBy = _context.UserId,
						ModifiedAt = DateTime.Now,
						ModifiedBy = _context.UserId,
						Quantity = quantity,
						SkipEmail = false
					};


					_context.Add(entityFile);
                    _context.SaveChanges();

					List<int> countBooks = await _context.Set<Model.EntityFile>().Where(e => e.BudgetForecastId == entityId && e.BudgetBaseOpId == budgetBaseOpId).Select(e => e.Id).ToListAsync();

                    if(countBooks.Count == count)
                    {
						var result = await this._requestsService.SendRequestTransferBudget(budgetBaseOpId, employee.Email);

						if (result)
						{
							return new Model.EntityFileResult { Success = true, Message = $"Notificarea a fost trimisa cu succes!", EntityFileId = entityId };
						}
					}

					return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = entityId };
				}
				else
				{
					return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
				}
			}
			else
			{
				return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
			}
		}

		[AllowAnonymous]
        [HttpPost("updateSkipEntityFile/{id}/{skipEmail}")]
        public async Task<EntityFileResult> UpdateChecked(int id, bool skipEmail)
        {
            Model.EntityFile entityFile = await _context.EntityFiles.SingleAsync(e => e.Id == id);

            entityFile.SkipEmail = skipEmail;
            _itemsRepository.Update(entityFile);
            _context.SaveChanges();

            return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost actualizat cu succes!", EntityFileId = id };

        }

        [HttpPost("updatePartnerEntityFile")]
        public async Task<EntityFileResult> updatePartnerEntityFile([FromBody] Dto.PartnerEntityFile partnerEntityFile)
        {
            //Model.EntityFile entityFile = await _context.EntityFiles.SingleAsync(e => e.Id == id);

            //entityFile.SkipEmail = skipEmail;
            //_itemsRepository.Update(entityFile);
            //_context.SaveChanges();

            return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost actualizat cu succes!", EntityFileId = 3 };

        }

		[HttpPost("uploadInventoryList")]
		public async Task<IActionResult> UploadInventoryList(IFormFile file, int entityId, int entityTypeId, int inventoryId, string info)
		{
			Model.CostCenter costCenter = null;
			Model.EntityType entityType = null;

			if (file == null) throw new Exception("File is null");
			if (file.Length == 0) throw new Exception("File is empty");
			int documentId = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Select(d => d.DocumentId).SingleAsync();

            string inventoryName = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Id == inventoryId).Select(i => i.Description).SingleAsync();

            entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();

			string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
			string uploadFolder = entityType.UploadFolder;
			string filePath = uploadFolder + "\\" + storedAs;

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

            var count = _context.EntityFiles
                .Where(e => e.EntityTypeId==entityTypeId && e.IsDeleted==false && e.CostCenterId==entityId)
                .Select(e => e.EntityId)
                .Count();

            string selectedCostCenterCode = await _context.Set<Model.CostCenter>().Where(c => c.Id == entityId).Select(c => c.Code).SingleAsync();
            string fileCostCenterCode = file.FileName.Substring(file.FileName.IndexOf('_') + 1, file.FileName.IndexOf('_', file.FileName.IndexOf('_') + 1) - file.FileName.IndexOf('_') - 1);

            if (count!=0)
            {
                return new StatusCodeResult(418);
            }
            else 
                if(selectedCostCenterCode != fileCostCenterCode)
                    {
                        return new StatusCodeResult(419);
                    }
                else
                {
                Model.EntityFile entityFile = new Model.EntityFile()
                {
                    EntityId = null,
                    FileType = file.ContentType,
                    EntityTypeId = entityType.Id,
                    CostCenterId = entityId,
                    Info = inventoryName,
                    Name = file.FileName,
                    Size = file.Length,
                    StoredAs = storedAs,
                    IsDeleted = false,
                    OrderId = null,
                    ModifiedAt = DateTime.Now
            };

                    _context.Add(entityFile);
                    _context.SaveChanges();


                    costCenter = await _context.Set<Model.CostCenter>().Where(c => c.Id == entityId).SingleAsync();

                    costCenter.AllowLabelList = false;
                    costCenter.InventoryList = false;
                    costCenter.BookBefore = false;
                    costCenter.BookAfter = false;
                    costCenter.PvBook = false;

                    List<Model.EntityFile> entityFiles1 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "ALLOW_LABEL_LIST").ToListAsync();
                    List<Model.EntityFile> entityFiles2 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "INVENTORY_LIST").ToListAsync();
                    List<Model.EntityFile> entityFiles3 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "BOOK_BEFORE").ToListAsync();
                    List<Model.EntityFile> entityFiles4 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "BOOK_AFTER").ToListAsync();
                    List<Model.EntityFile> entityFiles5 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "PV_BOOK").ToListAsync();

                    if (entityFiles1.Count > 0)
                    {
                        costCenter.AllowLabelList = true;
                    }

                    if (entityFiles2.Count > 0)
                    {
                        costCenter.InventoryList = true;
                    }

                    if (entityFiles3.Count > 0)
                    {
                        costCenter.BookBefore = true;
                    }

                    if (entityFiles4.Count > 0)
                    {
                        costCenter.BookAfter = true;
                    }

                    if (entityFiles5.Count > 0)
                    {
                        costCenter.PvBook = true;
                    }

                    _context.Update(costCenter);
                    _context.SaveChanges();

                    // await _ordersService.SendOrderNeedBudget(order.Id);
                    //var count = _context.Set<Model.RecordCount>().FromSql("UpdateImagesCount {0}", entityType.Id).ToList();

                    return Ok(entityFile);
                }
		}

		[HttpPost("deleteInventoryList/{entityId}")]
		public async Task<EntityFileResult> DeleteInventoryList(int entityId)
		{
			Model.EntityFile entityFile = null;
            Model.CostCenter costCenter = null;

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if(user == null)
                {
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

				if (user != null)
				{
					_context.UserId = user.Id.ToString();

					entityFile = await _context.Set<Model.EntityFile>().Where(e => e.Id == entityId).SingleAsync();

					entityFile.IsDeleted = true;
					entityFile.ModifiedAt = DateTime.Now;

					_context.Update(entityFile);
					_context.SaveChanges();

					costCenter = await _context.Set<Model.CostCenter>().Where(c => c.Id == entityFile.CostCenterId).FirstOrDefaultAsync();

                    if (costCenter != null)
                    {
                        costCenter.AllowLabelList = false;
                        costCenter.InventoryList = false;
                        costCenter.BookBefore = false;
                        costCenter.BookAfter = false;
                        costCenter.PvBook = false;

                        List<Model.EntityFile> entityFiles1 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "ALLOW_LABEL_LIST").ToListAsync();
                        List<Model.EntityFile> entityFiles2 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "INVENTORY_LIST").ToListAsync();
                        List<Model.EntityFile> entityFiles3 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "BOOK_BEFORE").ToListAsync();
                        List<Model.EntityFile> entityFiles4 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "BOOK_AFTER").ToListAsync();
                        List<Model.EntityFile> entityFiles5 = await _context.Set<Model.EntityFile>().Include(e => e.EntityType).Where(e => e.IsDeleted == false && e.CostCenterId == entityId && e.EntityType.Code == "PV_BOOK").ToListAsync();

                        if (entityFiles1.Count > 0)
                        {
                            costCenter.AllowLabelList = true;
                        }

                        if (entityFiles2.Count > 0)
                        {
                            costCenter.InventoryList = true;
                        }

                        if (entityFiles3.Count > 0)
                        {
                            costCenter.BookBefore = true;
                        }

                        if (entityFiles4.Count > 0)
                        {
                            costCenter.BookAfter = true;
                        }

                        if (entityFiles5.Count > 0)
                        {
                            costCenter.PvBook = true;
                        }

                        _context.Update(costCenter);
                        _context.SaveChanges();
                    }

					return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost sters cu succes!", EntityFileId = entityId };
				}
				else
				{
					return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
				}
			}
			else
			{
				return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
			}
		}

		[HttpPost("deleteList/{entityId}")]
		public async Task<EntityFileResult> DeleteList(int entityId)
		{
			Model.EntityFile entityFile = null;

			if (HttpContext.User.Identity.Name != null)
			{
				var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

				if (user == null)
				{
					user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				}

				if (user != null)
				{
					_context.UserId = user.Id.ToString();

					entityFile = await _context.Set<Model.EntityFile>().Where(e => e.Id == entityId).SingleAsync();

					entityFile.IsDeleted = true;
					entityFile.ModifiedAt = DateTime.Now;

					_context.Update(entityFile);
					_context.SaveChanges();

					return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost sters cu succes!", EntityFileId = entityId };
				}
				else
				{
					return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
				}
			}
			else
			{
				return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
			}
		}

        [HttpPost("uploadpodocument")]
        public async Task<EntityFileResult> UploadPODocument(IFormFile file, int orderId, int entityTypeId, string info)
        {
            Model.Order order = null;
            Model.EntityType entityType = null;

            if (HttpContext.User.Identity.Name != null)
            {
                var user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);

                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                }

                if (user != null)
                {
                    _context.UserId = user.Id.ToString();

                    if (file == null) throw new Exception("File is null");
                    if (file.Length == 0) throw new Exception("File is empty");

                    entityType = await _context.Set<Model.EntityType>().Where(e => e.Id == entityTypeId).SingleAsync();
                    order = await _context.Set<Model.Order>().Where(o => o.Id == orderId).SingleAsync();

                    string storedAs = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    string uploadFolder = entityType.UploadFolder;
                    string filePath = uploadFolder + "\\" + storedAs;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    Model.EntityFile entityFile = new Model.EntityFile()
                    {
                        EntityId = null,
                        FileType = file.ContentType,
                        EntityTypeId = entityType.Id,
                        Info = info,
                        Name = file.FileName,
                        Size = file.Length,
                        StoredAs = storedAs,
                        IsDeleted = false,
                        RequestId = null,
                        OrderId = orderId,
                        RequestBudgetForecastId = null, 
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        Quantity = 0,
                        SkipEmail = false
                    };

                    _context.Add(entityFile);
                    _context.SaveChanges();

                    return new Model.EntityFileResult { Success = true, Message = $"Fisierul a fost incarcat cu succes!", EntityFileId = orderId };
                }
                else
                {
                    return new Model.EntityFileResult { Success = false, Message = $"Userul nu exista!", EntityFileId = 0 };
                }
            }
            else
            {
                return new Model.EntityFileResult { Success = false, Message = $"Va rugam sa va autentificati!", EntityFileId = 0 };
            }
        }
    }
}
