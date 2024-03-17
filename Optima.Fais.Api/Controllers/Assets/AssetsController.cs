using AutoMapper;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using MigraDoc.DocumentObjectModel.Tables;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Utilities;
using OfficeOpenXml.Style;
using Optima.Fais.Api.Helpers;
using Optima.Fais.Api.Services;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Dto.Common;
using Optima.Fais.Dto.Sync;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using Optima.Fais.Repository;
using PdfSharp.Pdf.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.Linq;
using Optima.Fais.Api.Services.Flow;
using Optima.Fais.Api.Identity;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/assets")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    //[ResponseCache(Location = ResponseCacheLocation.Client, NoStore = false, Duration = 60)]
    public partial class AssetsController : GenericApiController<Model.Asset, Dto.Asset>
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<Model.ApplicationUser> userManager;
        private readonly INotifyService _notifyService = null;
		private readonly IAssetService _assetService = null;
		private readonly IConfiguration _configuration;
        private readonly IOrderFlowService _orderFlowService;
        private readonly string _BASEURL;
        private readonly string _TOKEN;

        //public AssetsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, 
        //    IAssetsRepository itemsRepository, IMapper mapper)
        //    : base(userManager, context, itemsRepository, mapper)
        //{
        //}

        public AssetsController(ApplicationDbContext context,
            IAssetsRepository itemsRepository, IMapper mapper, 
            IWebHostEnvironment hostingEnvironment,  
            UserManager<Model.ApplicationUser> userManager, 
            INotifyService notifyService,
			IAssetService assetService,
			IConfiguration configuration, IOrderFlowService orderFlowService)
            : base(context, itemsRepository, mapper)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this._notifyService = notifyService;
            this._assetService = assetService;
			_configuration = configuration;
            _orderFlowService = orderFlowService;

            this._BASEURL = configuration.GetSection("SAP").GetValue<string>("URL");
            this._TOKEN = configuration.GetSection("SAP").GetValue<string>("SAP-PROXY-AUTH-TOKEN");
        }

        [HttpPost("add")]
        public async Task<CreateAssetSAPResult> AddAsset([FromBody] AddAsset asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).CreateAssetSAP(asset);

                if (createAssetSAPResult.Success)
                {
					await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
				}

               
                return createAssetSAPResult;
			}
			else
			{
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }

        }

		[HttpPost("addPreReception")]
		public async Task<CreateAssetSAPResult> AddPreReceptionAsset([FromBody] AddAsset asset)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).CreatePreReceptionAssetSAP(asset);

				if (createAssetSAPResult.Success)
				{
					await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
				}


				return createAssetSAPResult;
			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}

		}

		[HttpPost("approvePreReception")]
		public async Task<CreateAssetSAPResult> ApprovePreReceptionAsset([FromBody] int assetId)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).ApprovePreReceptionAssetSAP(assetId);

				if (createAssetSAPResult.Success)
				{
					await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
				}


				return createAssetSAPResult;
			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}

		}

		[HttpPost("addAssetInvPlus")]
        public async Task<CreateAssetSAPResult> AddAssetInvPlus([FromBody] AddAssetInvPlus asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).CreateAssetInvPlusSAP(asset);

                await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }

        }

        [HttpPost("addStock")]
        public async Task<CreateAssetSAPResult> AddAssetStock([FromBody] AddStockAsset asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).AddAssetStock(asset);

                await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }

        }

        [HttpPost("change")]
        public async Task<IActionResult> AssetChange([FromBody] AssetChangeDTO asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).AssetChange(asset, out List<Dto.AssetChangeSAP> newAsset);
            AssetChangeResult createAssetResult = null;

            if (assetId > 0)
            {

                using (var errorfile = System.IO.File.CreateText("validate-" + DateTime.Now.Ticks + ".txt"))
                {
                    errorfile.WriteLine(newAsset);

                };


                var result = await this.AssetChangeAsync(newAsset);

                if (result != "")
                {
                    string errorFilePath = System.IO.Path.Combine("errors", "error-validate" + DateTime.Now.Ticks + ".txt");

                    using (var errorfile = System.IO.File.CreateText(errorFilePath))
                    {
                        errorfile.WriteLine(result);

                    };

                    try
                    {
                        createAssetResult = JsonConvert.DeserializeObject<AssetChangeResult>(result);

                        if (createAssetResult.Data != null && createAssetResult.Data.Return_Code == "1")
                        {
                            Model.Asset assetUpdate = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

                            assetUpdate.InvNo = createAssetResult.Data.Asset;
                            assetUpdate.ModifiedAt = DateTime.Now;
                            assetUpdate.AssetStateId = 1;
                            _context.Update(assetUpdate);
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();
                        
                        assetToDelete.IsDeleted = true;
                        _context.Update(assetToDelete);
                        _context.SaveChanges();

                        using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                        {
                            errorfile.WriteLine(ex.StackTrace);
                            errorfile.WriteLine(ex.ToString());

                        };
                    }



                    return Ok(createAssetResult);
                }
                else
                {

                    var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

                    assetToDelete.IsDeleted = true;
                    _context.Update(assetToDelete);
                    _context.SaveChanges();

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(result);
                        errorfile.WriteLine(result);

                    };

                    return Ok(result);
                }


            }


            return Ok(assetId);
        }

        [HttpPost("newTransferAsset")]
        public async Task<CreateAssetSAPResult> TransferAsset([FromBody] SaveAssetTransfer newAssetTransferSAP)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).TransferAssetSAP(newAssetTransferSAP);

                await this._notifyService.NotifyDataTransferAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("newTransferCloneAsset")]
        public async Task<CreateAssetSAPResult> TransferCloneAsset([FromBody] SaveAssetCloneTransfer newAssetTransferSAP)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).TransferCloneAssetSAP(newAssetTransferSAP);

                await this._notifyService.NotifyDataTransferAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("newRetireAsset")]
        public async Task<CreateAssetSAPResult> RetireAsset([FromBody] SaveRetireAsset newRetireAssetSAP)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var retireAssetSAPResult = (_itemsRepository as IAssetsRepository).RetireAsset(newRetireAssetSAP);

                 await this._notifyService.NotifyDataRetireAssetAsync(retireAssetSAPResult);

                return retireAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

		[HttpPost("publicRetireAsset")]
		public async Task<CreateAssetSAPResult> PublicRetireAsset([FromBody] SaveRetireAsset newRetireAssetSAP)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				var retireAssetSAPResult = await (_itemsRepository as IAssetsRepository).PublicRetireAsset(newRetireAssetSAP);

				await this._notifyService.NotifyDataRetireAssetAsync(retireAssetSAPResult);

				return retireAssetSAPResult;
			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("newStornoAsset")]
        public async Task<CreateAssetSAPResult> StornoAsset([FromBody] SaveStornoAsset stornoAssetSAP)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var stornoAssetSAPResult = (_itemsRepository as IAssetsRepository).StornoAsset(stornoAssetSAP);

                await this._notifyService.NotifyDataStornoAssetAsync(stornoAssetSAPResult);

                return stornoAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("newStornoAssetMFX")]
        public async Task<CreateAssetSAPResult> StornoAssetMFX([FromBody] SaveStornoAssetMFX stornoAssetSAP)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var stornoAssetSAPResult = await (_itemsRepository as IAssetsRepository).StornoAssetMFX(stornoAssetSAP);

                await this._notifyService.NotifyDataStornoAssetAsync(stornoAssetSAPResult);

                Model.Asset selectedAsset = _context.Set<Model.Asset>().Where(a => a.Id == stornoAssetSAP.AssetId).FirstOrDefault();
                selectedAsset.IsInTransfer = true;
                _context.Update(selectedAsset);
                _context.SaveChanges();

                Model.Stock selectedStock = _context.Set<Model.Stock>().Where(s => s.Id == selectedAsset.StockId).FirstOrDefault();
                selectedStock.Imported = false;
                _context.Update(selectedStock);
                _context.SaveChanges();

                return stornoAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("newAssetAcquisitionStorno")]
        public async Task<CreateAssetSAPResult> StornoAcqAsset([FromBody] SaveAssetAcquisitionStorno stornoAcqAssetSAP)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var stornoAssetSAPResult = (_itemsRepository as IAssetsRepository).StornoAcquisitionAsset(stornoAcqAssetSAP);

                await this._notifyService.NotifyDataStornoAcquisitionAssetAsync(stornoAssetSAPResult);

                return stornoAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("newAssetInvPlus")]
        public async Task<Model.CreateAssetSAPResult> AssetInvPlus([FromBody] SaveAssetInvPlus assetInvPlus)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).AssetInvPlus(assetInvPlus);

                await this._notifyService.NotifyDataAssetInvPlusAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("newAssetInvMinus")]
        public async Task<Model.CreateAssetSAPResult> AssetInvMinus([FromBody] SaveAssetInvMinus assetInvMinus)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).AssetInvMinus(assetInvMinus);

                await this._notifyService.NotifyDataAssetInvMinusAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("acquisition")]
        public async Task<CreateAssetSAPResult> AcquisitionAsset([FromBody] AssetAcquisition asset)
        {

            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

				try
				{
                    var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).AcquisitionAssetSAP(asset);
                    // await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
                    return createAssetSAPResult;
                }
                catch (Exception ex)
				{

                    return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = ex.Message };
                }

            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("stornoacquisition")]
		public async Task<CreateAssetSAPResult> StornoAcquisitionAsset([FromBody] AssetAcquisition asset)
		{
			Model.Asset assetToValidate = null;
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				try
				{
					assetToValidate = await _context.Set<Model.Asset>().Include(o => o.Order).Where(a => a.Id == asset.Id).SingleAsync();
					if (assetToValidate.OrderId == null)
                    {
						var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).StornoAcquisitionNoPOAssetSAP(asset);
						// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
						return createAssetSAPResult;
                    }
                    else
                    {
						var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).StornoAcquisitionAssetSAP(asset);
						// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
						return createAssetSAPResult;
					}
						
				}
				catch (Exception ex)
				{

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = ex.Message };
				}

			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("validateacquisition")]
		public async Task<CreateAssetSAPResult> ValidateAcquisitionAsset([FromBody] AssetAcquisition asset)
		{

			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				try
				{
					var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).ValidateAcquisitionAssetSAP(asset);
					// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
					return createAssetSAPResult;
				}
				catch (Exception ex)
				{

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = ex.Message };
				}

			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("validatestornoacquisition")]
		public async Task<CreateAssetSAPResult> ValidateStornoAcquisitionAsset([FromBody] AssetAcquisition asset)
		{
            Model.Asset assetToValidate = null;
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				try
				{
                    assetToValidate = await _context.Set<Model.Asset>().Include(o => o.Order).Where(a => a.Id == asset.Id).SingleAsync();
                    if(assetToValidate.OrderId == null)
                    {
						var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).ValidateStornoNoPOAcquisitionAssetSAP(asset);
						// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
						return createAssetSAPResult;
					}
                    else
                    {
						var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).ValidateStornoAcquisitionAssetSAP(asset);
						// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
						return createAssetSAPResult;
					}
					
				}
				catch (Exception ex)
				{

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = ex.Message };
				}

			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("validatestornonopoacquisition")]
		public async Task<CreateAssetSAPResult> ValidateStornoNoPOAcquisitionAsset([FromBody] AssetAcquisition asset)
		{

			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				try
				{
					var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).ValidateStornoNoPOAcquisitionAssetSAP(asset);
					// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);
					return createAssetSAPResult;
				}
				catch (Exception ex)
				{

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = ex.Message };
				}

			}
			else
			{
				return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("rejectAsset/{assetId}/{reason}")]
        public async Task<RejectResult> RejectAsset(int assetId, string reason)
        {
			Model.Asset asset = null;
			List<Model.Asset> assets = null;
            Model.EmailStatus emailStatus = null;
            Model.Inventory inventory = null;
			Model.EmailType emailType = null;
			Model.EntityType entityType = null;
            Model.ApplicationUser receptionUser = null;
			int documentNumber = 0;
            int appStateId = 0;
            string createdBy = "";
			string modifiedBy = "";

			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				try
				{
					asset = await _context.Set<Model.Asset>().Include(d => d.Document).Where(a => a.Id == assetId).SingleAsync();
					appStateId = await _context.Set<Model.AppState>().AsNoTracking().Where(a => a.Code == "REJECTASSET").Select(a => a.Id).FirstOrDefaultAsync();

                    createdBy = asset.CreatedBy;
                    modifiedBy = asset.ModifiedBy;

					if (asset.Document.DocNo1 != "" && asset.Document.DocNo1 != null && asset.Document.DocNo1.Trim().Length > 2)
                    {
						assets = await _context.Set<Model.Asset>().Include(d => d.Document).Where(a => a.Document.DocNo1 == asset.Document.DocNo1).ToListAsync();

						int assetStateId = await _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "Rejected").Select(a => a.Id).FirstOrDefaultAsync();

						for (int i = 0; i < assets.Count; i++)
                        {
                            assets[i].Info = reason;
							assets[i].AppStateId = appStateId;
							assets[i].AssetStateId = assetStateId;
							assets[i].ModifiedAt = DateTime.Now;
							assets[i].ModifiedBy = _context.UserId;

                            //Model.Order order = await _context.Set<Model.Order>().AsNoTracking().Where(or => or.Id == asset.OrderId).FirstOrDefaultAsync();
                            //OrderDelete orderDelete = new OrderDelete(order.Id, reason, order.UserId);
                            //await this._orderFlowService.DeleteOrder(orderDelete);

                            _context.Update(assets[i]);

                            _context.SaveChanges();
						}

					}

					inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).FirstOrDefaultAsync();
					emailType = await _context.Set<Model.EmailType>().AsNoTracking().Where(a => a.Code == "REJECTASSET").FirstOrDefaultAsync();
					entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "REJECTASSET").FirstOrDefaultAsync();

					Guid guid = Guid.NewGuid();
					Guid guidAll = Guid.NewGuid();

					documentNumber = int.Parse(entityType.Name);

					documentNumber++;

                    if(createdBy != null && createdBy != "")
                    {
						receptionUser = await userManager.FindByIdAsync(asset.CreatedBy);
					}
                    else if (modifiedBy != null && modifiedBy != "")
                    {
						receptionUser = await userManager.FindByIdAsync(asset.ModifiedBy);
                    }
                    else
                    {
						receptionUser = await userManager.FindByIdAsync("92E74C4F-A79A-4C83-A7D0-A3202BD2507F");
					}


					emailStatus = new Model.EmailStatus()
					{
						AppStateId = appStateId,
						AssetId = asset.Id,
						AssetOp = null,
						BudgetBaseId = asset.BudgetBaseId,
						CompanyId = asset.CompanyId,
						Completed = false,
						CostCenterIdFinal = asset.CostCenterId,
						CostCenterIdInitial = asset.CostCenterId,
						CreatedAt = DateTime.Now,
						CreatedBy = _context.UserId,
						DocumentNumber = documentNumber,
						DstEmployeeEmailSend = false,
						DstEmployeeValidateAt = null,
						DstEmployeeValidateBy = null,
						DstManagerEmailSend = false,
						DstManagerValidateAt = null,
						DstManagerValidateBy = null,
						EmailSend = false,
						EmailTypeId = emailType.Id,
						EmployeeIdFinal = receptionUser.EmployeeId,
						EmployeeIdInitial = user.EmployeeId,
						ErrorId = null,
						Exported = false,
						FinalValidateAt = null,
						FinalValidateBy = null,
						Guid = guid,
						GuidAll = guidAll,
						Info = reason,
						IsAccepted = false,
						IsDeleted = false,
						ModifiedAt = DateTime.Now,
						ModifiedBy = _context.UserId,
						NotCompletedSync = true,
						NotDstEmployeeSync = false,
						NotDstManagerSync = false,
						NotSrcEmployeeSync = false,
						NotSrcManagerSync = false,
						NotSync = true,
						OfferId = asset.Order != null ? asset.Order.OfferId : null,
						OrderId = asset.OrderId,
						PartnerId = asset.Document.PartnerId,
						RequestId = asset.RequestId,
						SameEmployee = true,
						SameManager = false,
						Skip = false,
						SkipDstEmployee = false,
						SkipDstManager = false,
						SkipSrcEmployee = false,
						SkipSrcManager = false,
						SrcEmployeeEmailSend = false,
						SrcEmployeeValidateAt = DateTime.Now,
						SrcEmployeeValidateBy = _context.UserId,
						SrcManagerEmailSend = false,
						SrcManagerValidateAt = DateTime.Now,
						SrcManagerValidateBy = _context.UserId,
						StockId = asset.StockId,
						SyncCompletedErrorCount = 0,
						SyncDstEmployeeErrorCount = 0,
						SyncDstManagerErrorCount = 0,
						SyncErrorCount = 0,
						SyncSrcEmployeeErrorCount = 0,
						SyncSrcManagerErrorCount = 0,

					};

					entityType.Name = documentNumber.ToString();

                    _context.Update(entityType);
                    _context.Add(emailStatus);

                    _context.SaveChanges();

					return new Model.RejectResult { Success = true, Message = $"Numarul de inventar {asset.InvNo}/{asset.SubNo} a fost refuzat." };
				}
				catch (Exception ex)
				{

					return new Model.RejectResult { Success = false, Message = ex.Message };
				}

			}
			else
			{
				return new Model.RejectResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

        [HttpPost("clone")]
        public async Task<IActionResult> PostCloneDetail([FromBody] AssetClone asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).CloneAsset(asset);

            return Ok(assetId);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> PostDetail([FromBody] AssetSave asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).CreateOrUpdateAsset(asset);

            return Ok(assetId);
        }

        [HttpPut("detail")]
        public virtual IActionResult PutDetail([FromBody] AssetSave asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).CreateOrUpdateAsset(asset);

           // return Ok(_itemsRepository.GetById(assetId));
            return Ok();
        }

        [HttpPut("assetSapValidation")]
        public virtual IActionResult PutDetail([FromBody] AssetSapValidationSave asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).UpdateAssetSapValidation(asset);

            return Ok();
        }

        [HttpPost("updateAcquisition")]
        public async Task<UpdateAssetSAPResult> PutAcquisition([FromBody] AssetAcquisition asset)
        {
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).UpdateAcquisition(asset);

				await this._notifyService.NotifyDataEditAssetAsync(createAssetSAPResult);

				return createAssetSAPResult;
			}
			else
			{
				return new Model.UpdateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
			}
		}

        [HttpPost("updatePreAcquisition")]
        public async Task<UpdateAssetSAPResult> PutPreAcquisition([FromBody] AssetPreAcquisition asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).UpdatePreAcquisition(asset);

                await this._notifyService.NotifyDataEditAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.UpdateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("updateParentNumber")]
        public async Task<UpdateAssetSAPResult> UpdateParentNumber([FromBody] AssetPreAcquisition asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).UpdateParentNumber(asset);

                await this._notifyService.NotifyDataEditAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.UpdateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("updateAssetChange")]
        public async Task<UpdateAssetSAPResult> PostAssetChange([FromBody] AssetEditChange asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IAssetsRepository).UpdateAssetChange(asset);

                await this._notifyService.NotifyDataEditAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.UpdateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

       


        [HttpPost("deleteOrderItem")]
        public async Task<AssetResult> DeleteOrderItem([FromBody] int assetId)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var result = (_itemsRepository as IAssetsRepository).DeleteOrderItem(assetId);

				if (result.Success)
				{
                    DeleteAssetResult deleteResult = null;
                    var response =  await this.DeleteAssetAsync(assetId);

                    if (response != "")
					{
						try
						{
                            deleteResult = JsonConvert.DeserializeObject<DeleteAssetResult>(response);

							if (deleteResult.Data != null && deleteResult.Data.Return_Code == "1")
							{
								Model.Asset asset = null;
								asset = _context.Set<Model.Asset>().Where(c => c.Id == assetId).Single();

								asset.ModifiedAt = DateTime.Now;
                                asset.IsDeleted = true;
                                _context.Update(asset);
                                _context.SaveChanges();

                                return new Model.AssetResult { Success = true, ErrorMessage = $"Operatia a fost finalizata cu success!" };
                            }
						}
						catch (Exception ex)
						{
							var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

							assetToDelete.IsDeleted = false;
							_context.Update(assetToDelete);
							_context.SaveChanges();

                            return new Model.AssetResult { Success = false, ErrorMessage = $"{ex.Message}" };
                        }

					}
					else
					{

						var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

						assetToDelete.IsDeleted = false;
						_context.Update(assetToDelete);
						_context.SaveChanges();

                        return new Model.AssetResult { Success = false, ErrorMessage = $"Eroare server!" };
                    }

				}

                await this._notifyService.NotifyDataOrderItemDeleteAsync(result);

                return result;
            }
            else
            {
                return new Model.AssetResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("updateAssetInvPlus")]
        public async Task<CreateAssetSAPResult> UpdateAssetInvPlus([FromBody] UpdateAssetInvPlus asset)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IAssetsRepository).UpdateAssetInvPlus(asset);

                // await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpGet("detail/{id:int}")]
        //[Authorize(Policy = "Asset.Edit")]
        public virtual async Task<IActionResult> GetDetail(int id, string includes)
        {
            int accMonthId = await _context.Set<Model.Inventory>().Where(a => a.Active == true).Select(a => a.AccMonthId.Value).SingleAsync();
			int errorTypeId = await _context.Set<Model.ErrorType>().Where(a => a.Code == "ACQUISITIONASSET").Select(a => a.Id).SingleAsync();
			var questionnaire = _context.Assets
                    .Include(a => a.Document)
                        .ThenInclude(d => d.DocumentType)
                    .Include(a => a.Document)
                            .ThenInclude(d => d.Partner)
                                .ThenInclude(d => d.PartnerLocation)
                    .Include(a => a.Account)
                    .Include(a => a.EmployeeTransfer)
                    .Include(a => a.ExpAccount)
                    .Include(a => a.AssetCategory)
                    .Include(a => a.Article)
                    .Include(a => a.Uom)
                    .Include(a => a.CostCenter)
                        .ThenInclude(c => c.Division)
                            .ThenInclude(c => c.Department)
                    .Include(a => a.CostCenter)
                        .ThenInclude(c => c.Room)
                            .ThenInclude(r => r.Location)
                                .ThenInclude(c => c.City)
                                    .ThenInclude(c => c.County)
                                        .ThenInclude(c => c.Country)
                    .Include(a => a.CostCenter)
                        .ThenInclude(c => c.Administration)
                    .Include(a => a.BudgetManager)
                    .Include(a => a.AssetNature)
                    .Include(a => a.SubType)
                        .ThenInclude(a => a.Type)
                    .Include(a => a.Employee)
                    .Include(a => a.Material)
                    .Include(a => a.Company)
                    .Include(a => a.CostCenter)
                        .ThenInclude(c => c.AdmCenter)
                   .Include(a => a.CostCenter)
                        .ThenInclude(c => c.Region)
                    .Include(a => a.InsuranceCategory)
                    .Include(a => a.AssetType)
                    .Include(a => a.Project)
                    .Include(a => a.Order)
                    .Include(a => a.InvState)
                    .Include(a => a.AssetState)
                    .Include(a => a.Tax)
                    .Include(a => a.Rate).ThenInclude(u => u.Uom)
                    .Include(a => a.SubCategory)
                    .Include(a => a.Request)
                    .Include(a => a.DictionaryItem)
                    .Include(a => a.BudgetForecast).ThenInclude(b => b.BudgetBase)
                    .Include(a => a.AssetDeps)
					//.Include(a => a.Error)
					.FirstOrDefault(qn => qn.Id == id);

            questionnaire.AssetDepMDs = _context.Entry(questionnaire)
             .Collection(b => b.AssetDepMDs)
             .Query()
             .Where(a => a.AccMonthId == accMonthId).ToList();

			//questionnaire.Error = _context.Entry(questionnaire)
		 //  .Collection(b => b.Error)
		 //  .Query()
		 //  .Where(a => a.IsDeleted == false && a.ErrorTypeId == errorTypeId).ToList();

			//var asset = (_itemsRepository as IAssetsRepository).GetDetailsById(id, includes);
			var result = _mapper.Map<Dto.Asset>(questionnaire);

            return  Ok(result);
        }

        //public async Task SendStocksMail(string stocks)
        //{

        //    var email = "adrian.cirnaru@optima.ro";

        //    var subject = "Stocks information";
        //    var message = @"
        //    Dear Administrator, 

        //        " + string.Format(@"{0}", stocks) + @" have dropped below 20 units

        //        This is an automated message – Please do not replay directly to this email!

                
        //        ";

        //    var emailMessage = new MimeMessage();

        //    //mailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "transferuri@piraeusbank.ro"));
        //    emailMessage.From.Add(new MailboxAddress("Adrian Cirnaru", "adrian.cirnaru@optima.ro"));
        //    emailMessage.To.Add(new MailboxAddress("", email));

        //    emailMessage.Subject = subject;
        //    //emailMessage.Body = new TextPart("plain") { Text = message };

        //    var builder = new BodyBuilder { TextBody = message };
        //    //  builder.Attachments.Add(attachmentName, attachment, ContentType.Parse(attachmentType));
        //    emailMessage.Body = builder.ToMessageBody();

        //    using (var client = new SmtpClient())
        //    {

        //        //await client.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
        //        //client.AuthenticationMechanisms.Remove("XOAUTH2");

        //        //await client.AuthenticateAsync("adrian.cirnaru@optima.ro", "Adcr3386");
        //        //await client.SendAsync(emailMessage).ConfigureAwait(false);
        //        //await client.DisconnectAsync(true).ConfigureAwait(false);

        //    }

        //}

        [HttpDelete("detail/{id}")]
        public void DeleteDetail(int id)
        {
            _itemsRepository.Delete(id);
            _context.SaveChanges();
        }

        [HttpDelete("deleteAssetOp/{id}, {inventoryId}")]
        public void DeleteAsset(int id, int inventoryId)
        {
           
            var assetId = (_itemsRepository as IAssetsRepository).DeleteAssetOp(id, inventoryId);
            _context.SaveChanges();
        }

        [HttpPost("importv1")]
        public virtual IActionResult PostImportDetailV1([FromBody] AssetImportV1 asset)
        {
            int assetId = 0;
            assetId = (_itemsRepository as IAssetsRepository).AssetImportV1(asset);

            return Ok(asset);
        }

        //[HttpGet]
        //[Route("Export")]
        //public string Export()
        //{
        //    string sWebRootFolder = hostingEnvironment.WebRootPath;
        //    string sFileName = @"demo.xlsx";
        //    string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    if (file.Exists)
        //    {
        //        file.Delete();
        //        file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    }
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        // add a new worksheet to the empty workbook
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employee");
        //        //First add the headers
        //        worksheet.Cells[1, 1].Value = "ID";
        //        worksheet.Cells[1, 2].Value = "Name";
        //        worksheet.Cells[1, 3].Value = "Gender";
        //        worksheet.Cells[1, 4].Value = "Salary (in $)";

        //        //Add values
        //        worksheet.Cells["A2"].Value = 1000;
        //        worksheet.Cells["B2"].Value = "Jon";
        //        worksheet.Cells["C2"].Value = "M";
        //        worksheet.Cells["D2"].Value = 5000;

        //        worksheet.Cells["A3"].Value = 1001;
        //        worksheet.Cells["B3"].Value = "Graham";
        //        worksheet.Cells["C3"].Value = "M";
        //        worksheet.Cells["D3"].Value = 10000;

        //        worksheet.Cells["A4"].Value = 1002;
        //        worksheet.Cells["B4"].Value = "Jenny";
        //        worksheet.Cells["C4"].Value = "F";
        //        worksheet.Cells["D4"].Value = 5000;

        //        using (var cells = worksheet.Cells[1, 1, 1, 4])
        //        {
        //            cells.Style.Font.Bold = true;
        //            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        //        }

        //        package.Save(); //Save the workbook.
        //    }

        //    return URL;
        //}

        [HttpGet]
        [Route("Import")]
        public string Import()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV1> assets = new List<AssetImportV1>();
            int lineIndex = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                  
                   
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                   
                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV1 asset = new AssetImportV1();

                        asset.InvNo1 = worksheet.Cells[i, 1].Text.ToString();
                        asset.InvNo2 = worksheet.Cells[i, 3].Text.ToString();
                        asset.InvNo3 = worksheet.Cells[i, 4].Text.ToString();
                        asset.AssetCategoryCode = worksheet.Cells[i, 7].Text.ToString();
                        asset.AssetCategoryName = worksheet.Cells[i, 29].Text.ToString();
                        asset.Quantity = int.Parse(worksheet.Cells[i, 5].Text);
                        asset.LocationCode = worksheet.Cells[i, 10].Text.ToString();
                        asset.CostCenterCode = worksheet.Cells[i, 11].Text.ToString();
                        asset.AssetName = worksheet.Cells[i, 27].Text.ToString();
                        asset.AssetState = worksheet.Cells[i, 28].Text.ToString();


                        //DateTime datevalue = (Convert.ToDateTime(DateTime.Parse(worksheet.Cells[i, 9].Text).ToString()));

                        //String day = datevalue.Day.ToString();
                        //String mm = datevalue.Month.ToString();
                        //String yy = datevalue.Year.ToString();


                        //asset.PurchaseDate = yy + "-" + mm + "-" + day;

                        string initialDate = worksheet.Cells[i, 21].Text.ToString();
                        string[] strDate = initialDate.Split('/');
                       
                            int month = int.Parse(strDate[0]);
                            int day = int.Parse(strDate[1]);
                            int year = int.Parse(strDate[2]);

                            asset.PurchaseDate = new DateTime(year, month, day);
                        

                        //string strDate = worksheet.Cells[i, 21].Text.ToString();

                        //if(strDate.Length > 9)
                        //{
                        //    int month = int.Parse(strDate.Substring(0, 2));
                        //    int day = int.Parse(strDate.Substring(3, 2));
                        //    int year = int.Parse(strDate.Substring(6, 4));

                        //    asset.PurchaseDate = new DateTime(year, month, day);
                        //}
                        //else
                        //{
                        //    if()
                        //    int month = int.Parse(strDate.Substring(0, 1));

                        //    if(strDate.Length <= 9)
                        //    {
                        //        int day = int.Parse(strDate.Substring(2, 1));
                        //        int year = int.Parse(strDate.Substring(4, 4));

                        //        asset.PurchaseDate = new DateTime(year, month, day);
                        //    }
                        //    else
                        //    {
                        //        int day = int.Parse(strDate.Substring(2, 2));
                        //        int year = int.Parse(strDate.Substring(5, 4));

                        //        asset.PurchaseDate = new DateTime(year, month, day);
                        //    }

                        //}




                        if (worksheet.Cells[i, 16].Text.Trim() == "-" || worksheet.Cells[i, 16].Text.Trim().StartsWith("("))
                        {
                            asset.ValueInv = 0;
                        }else
                        {
                            asset.ValueInv = decimal.Parse(worksheet.Cells[i, 16].Text);
                        }
                       
                        asset.PartnerName = worksheet.Cells[i, 32].Text.ToString();
                        asset.FiscalCode = worksheet.Cells[i, 33].Text.ToString();
                        asset.DocNo1 = worksheet.Cells[i, 34].Text.ToString();
                        asset.SerialNumber = worksheet.Cells[i, 35].Text.ToString();
                       
                        if (worksheet.Cells[i, 20].Text.Trim() == "-" || worksheet.Cells[i, 20].Text.Trim().StartsWith("("))
                        {
                            asset.ValueRem = 0;
                        }
                        else
                        {
                            asset.ValueRem = decimal.Parse(worksheet.Cells[i, 20].Text);
                        }
                        asset.AssetType = worksheet.Cells[i, 12].Text.ToString();


                        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(asset))
                        {
                            string name = descriptor.Name;
                            object value = descriptor.GetValue(asset);
                            Console.WriteLine("{0}={1}", name, value);
                        }

                        assets.Add(asset);
                        
                    }

                    
                }

                
                foreach (AssetImportV1 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    int assetId = repo.AssetImportV1(asset);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        [HttpGet]
        [Route("ImportVodafone")]
        public string ImportVodafone()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"demo.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV4> assets = new List<AssetImportV4>();
            int lineIndex = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {


                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV4 asset = new AssetImportV4();


                        //DateTime datevalue = (Convert.ToDateTime(DateTime.Parse(worksheet.Cells[i, 4].Text).ToString()));

                        //String mn = datevalue.Day.ToString();
                        //String day = datevalue.Month.ToString();
                        //String yy = datevalue.Year.ToString();


                        asset.AssetName = worksheet.Cells[i, 2].Text.ToString();
                        asset.InvNo = worksheet.Cells[i, 3].Text.ToString();
                        //asset.PurchaseDate = new DateTime (int.Parse(yy), int.Parse(mn), int.Parse(day));
                        asset.LocationName = worksheet.Cells[i, 5].Text.ToString();
                        asset.EmployeeFullName = worksheet.Cells[i, 6].Text.ToString();
                        asset.SerialNumber = worksheet.Cells[i, 7].Text.ToString();
                        asset.Model = worksheet.Cells[i, 8].Text.ToString();
                        asset.Quantity = int.Parse(worksheet.Cells[i, 9].Text.ToString());
       

                        assets.Add(asset);

                    }


                }


                foreach (AssetImportV4 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    int assetId = repo.AssetImportV4(asset);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        [HttpGet]
        [Route("ImportAllianzTehnology")]
        public string ImportAllianzTehnology()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"ImportAllianzTehnology.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV6> assets = new List<AssetImportV6>();
            int lineIndex = 0;
            string invNoUpdated = "";
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {


                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV6 asset = new AssetImportV6();


                        asset.InvNo = worksheet.Cells[i, 1].Text.ToString();
                        asset.AssetName = worksheet.Cells[i, 2].Text.ToString();
                        asset.Model = worksheet.Cells[i, 3].Text.ToString();
                        asset.Model1 = worksheet.Cells[i, 4].Text.ToString();
                        asset.CostCenter = worksheet.Cells[i, 5].Text.ToString();
                        asset.Quantity = float.Parse(worksheet.Cells[i, 6].Text.ToString());
                        asset.Uom = worksheet.Cells[i, 7].Text.ToString();
                        asset.ValueInv = decimal.Parse(worksheet.Cells[i, 8].Text.ToString());
                        asset.AssetCategory = worksheet.Cells[i, 9].Text.ToString();
                        asset.FiscalCode = worksheet.Cells[i, 10].Text.ToString();
                        asset.SupplierName = worksheet.Cells[i, 11].Text.ToString();


                        assets.Add(asset);

                    }


                }


                foreach (AssetImportV6 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    string assetId = repo.AssetImportV6(asset, out invNoUpdated);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        [HttpGet]
        [Route("ImportAllianzAsigurari")]
        public string ImportAllianzAsigurari()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"import.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV7> assets = new List<AssetImportV7>();
            int lineIndex = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {


                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV7 asset = new AssetImportV7();


                        //DateTime datevalue = (Convert.ToDateTime(DateTime.Parse(worksheet.Cells[i, 4].Text).ToString()));

                        //String mn = datevalue.Day.ToString();
                        //String day = datevalue.Month.ToString();
                        //String yy = datevalue.Year.ToString();


                   

                        asset.AssetName = worksheet.Cells[i, 1].Text.ToString();
                        asset.InvNo = worksheet.Cells[i, 2].Text.ToString();
                        asset.SerialNumber = worksheet.Cells[i, 3].Text.ToString();
                        asset.AssetCategory = worksheet.Cells[i, 4].Text.ToString();
                        asset.AssetState = worksheet.Cells[i, 5].Text.ToString();
                        asset.ValueInv = decimal.Parse(worksheet.Cells[i, 6].Text.ToString());
                        asset.ValueDep = decimal.Parse(worksheet.Cells[i, 7].Text.ToString());
                        //asset.PurchaseDate = DateTime.Parse(worksheet.Cells[i, 8].Text.ToString());
                        asset.EmployeeFullName = worksheet.Cells[i, 9].Text.ToString();
                        asset.CostCenter= worksheet.Cells[i, 10].Text.ToString();
                        asset.Room = worksheet.Cells[i, 11].Text.ToString();
                        asset.Location = worksheet.Cells[i, 13].Text.ToString();
                        asset.Region = worksheet.Cells[i, 12].Text.ToString();


                        assets.Add(asset);

                    }


                }


                foreach (AssetImportV7 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    int assetId = repo.AssetImportV7(asset);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        [HttpGet]
        [Route("ImportBnr")]
        public string ImportBnr()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"importBnr.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV8> assets = new List<AssetImportV8>();
            int lineIndex = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {


                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV8 asset = new AssetImportV8();

                        DateTime date = DateTime.Now;

                        asset.LocationName = "Directia Servicii Informatice";
                        asset.AssetState = "Functiune";

                        asset.InvNo = worksheet.Cells[i, 1].Text.ToString();
                        asset.SerialNumber = worksheet.Cells[i, 2].Text.ToString();
                        asset.Name = worksheet.Cells[i, 3].Text.ToString();
                        asset.Quantity = float.Parse(worksheet.Cells[i, 4].Text.ToString());
                        asset.ValueInv = decimal.Parse(worksheet.Cells[i, 6].Text.ToString());
                        asset.ValueDep = decimal.Parse(worksheet.Cells[i, 5].Text.ToString());
                        asset.ValueCassation = decimal.Parse(worksheet.Cells[i, 8].Text.ToString());
                        asset.MonthCassationRate = decimal.Parse(worksheet.Cells[i, 9].Text.ToString());

                        string strDate = worksheet.Cells[i, 10].Text.ToString();

                        int month = int.Parse(strDate.Substring(0, 2));
                        int day = int.Parse(strDate.Substring(3, 2));
                        int year = int.Parse(strDate.Substring(6, 4));

                        asset.PurchaseDate = new DateTime(year, month, day);
                        asset.RegionCode = worksheet.Cells[i, 11].Text.ToString();
                        asset.RegionName = worksheet.Cells[i, 12].Text.ToString();
                        asset.LocationCode = worksheet.Cells[i, 13].Text.ToString();
                        asset.LocationName = worksheet.Cells[i, 14].Text.ToString();
                        asset.RoomName = worksheet.Cells[i, 15].Text.ToString();
                        asset.InternalCode = worksheet.Cells[i, 17].Text.ToString();
                        asset.LastName = worksheet.Cells[i, 18].Text.ToString();
                        asset.FirstName = worksheet.Cells[i, 19].Text.ToString();
                        asset.AssetCategoryCode = worksheet.Cells[i, 20].Text.ToString();
                        asset.AssetCategoryName = worksheet.Cells[i, 22].Text.ToString(); ;
                        asset.AssetClassCode = worksheet.Cells[i, 23].Text.ToString();
                        asset.AssetClassName = worksheet.Cells[i, 24].Text.ToString();
                        asset.Years = decimal.Parse(worksheet.Cells[i, 25].Text.ToString());
                        asset.Months = decimal.Parse(worksheet.Cells[i, 26].Text.ToString());
                        asset.InvNoParent = worksheet.Cells[i, 27].Text.ToString();
                        asset.CostCenterCode = worksheet.Cells[i, 29].Text.ToString();
                        asset.CostCenterName = worksheet.Cells[i, 30].Text.ToString();
                        asset.AssetType = worksheet.Cells[i, 16].Text.ToString();
                        asset.BookRate = worksheet.Cells[i, 31].Text.ToString();

                        string strDate1 = worksheet.Cells[i, 32].Text.ToString();

                        int month1 = int.Parse(strDate.Substring(0, 2));
                        int day1 = int.Parse(strDate.Substring(3, 2));
                        int year1 = int.Parse(strDate.Substring(6, 4));
                        asset.ChangeDate = new DateTime(year1, month1, day1);
                        asset.TransactionTypeCode = worksheet.Cells[i, 33].Text.ToString();
                        asset.TransactionType = worksheet.Cells[i, 34].Text.ToString();
                      //  asset.QuantityChange = int.Parse(worksheet.Cells[i, 35].Text.ToString());
                      //  asset.ValueChange = decimal.Parse(worksheet.Cells[i, 36].Text.ToString());






                        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(asset))
                        {
                            string name = descriptor.Name;
                            object value = descriptor.GetValue(asset);
                            Console.WriteLine("{0}={1}", name, value);
                        }

                        //asset.AssetState = values[2];
                        //asset.LocationCode = values[7];
                        //asset.LocationName = values[8];
                        //asset.SupplierCode = values[15];
                        //asset.SupplierName = values[16];

                        assets.Add(asset);

                    }


                }


                foreach (AssetImportV8 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    int assetId = repo.AssetImportV8(asset);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        [HttpGet]
        [Route("ImportBnrCasari")]
        public string ImportBnrCassation()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"importBnrCassation.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV11> assets = new List<AssetImportV11>();
            int lineIndex = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {


                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV11 asset = new AssetImportV11();

                        DateTime date = DateTime.Now;

                        asset.InvNo = worksheet.Cells[i, 2].Text.ToString();
                        asset.SerialNumber = worksheet.Cells[i, 3].Text.ToString();
                        asset.Name = worksheet.Cells[i, 4].Text.ToString();
                        asset.Quantity = float.Parse(worksheet.Cells[i, 5].Text.ToString());
                        asset.AssetType = worksheet.Cells[i, 6].Text.ToString();

                        string strDate = worksheet.Cells[i, 7].Text.ToString();

                        int month = int.Parse(strDate.Substring(0, 2));
                        int day = int.Parse(strDate.Substring(3, 2));
                        int year = int.Parse(strDate.Substring(6, 4));
                        asset.RetiredDate = new DateTime(year, month, day);
                        asset.TransactionTypeCode = worksheet.Cells[i, 8].Text.ToString();
                        asset.TransactionType = worksheet.Cells[i, 9].Text.ToString();
                        asset.RetiredQuantity = float.Parse(worksheet.Cells[i, 10].Text.ToString());
                        asset.RetiredValue = decimal.Parse(worksheet.Cells[i, 11].Text.ToString());

                        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(asset))
                        {
                            string name = descriptor.Name;
                            object value = descriptor.GetValue(asset);
                            Console.WriteLine("{0}={1}", name, value);
                        }

                     

                        assets.Add(asset);

                    }


                }


                foreach (AssetImportV11 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    int assetId = repo.AssetImportV11(asset);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        [HttpGet]
        [Route("ImportOtp")]
        public string ImportOtp()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            string sFileName = @"importOtpTest.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            List<AssetImportV10> assets = new List<AssetImportV10>();
            int lineIndex = 0;
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {


                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


                    int totalRows = worksheet.Dimension.End.Row;
                    for (int i = 2; i <= totalRows; i++)
                    {
                        AssetImportV10 asset = new AssetImportV10();

                        DateTime date = DateTime.Now;

                       
                        asset.InvNo = worksheet.Cells[i, 1].Text.ToString();
                        asset.InvNoParent = worksheet.Cells[i, 2].Text.ToString();
                        asset.IsParent = worksheet.Cells[i, 3].Text.ToString();
                        asset.Description = worksheet.Cells[i, 4].Text.ToString();
                        asset.SerialNumber = worksheet.Cells[i, 5].Text.ToString();
                        asset.PurchaseDate = date;
                        asset.ValueInv = 0;
                        asset.QuantityInitial = float.Parse(worksheet.Cells[i, 8].Text.ToString());
                        asset.QuantityFinal = 0;
                        asset.CostCenterCode = worksheet.Cells[i, 10].Text.ToString();
                        asset.CostCenterName = worksheet.Cells[i, 11].Text.ToString();
                        asset.LocationCodeInitial = worksheet.Cells[i, 14].Text.ToString();
                        asset.LocationNameInitial = worksheet.Cells[i, 15].Text.ToString();
                       


                        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(asset))
                        {
                            string name = descriptor.Name;
                            object value = descriptor.GetValue(asset);
                            Console.WriteLine("{0}={1}", name, value);
                        }

                       

                        assets.Add(asset);

                    }


                }


                foreach (AssetImportV10 asset in assets)
                {
                    ApplicationDbContext context = new ApplicationDbContext();
                    AssetsRepository repo = new AssetsRepository(context, null);


                    int assetId = repo.AssetImportV10(asset);
                    lineIndex++;

                    Console.WriteLine(lineIndex);
                }
            }
            catch (Exception ex)
            {
                return "Some error occured while importing." + ex.Message;
            }

            return "";
        }

        //[HttpGet]
        //[Route("ImportRingier")]
        //public string ImportRingier()
        //{
        //    string sWebRootFolder = hostingEnvironment.WebRootPath;
        //    string sFileName = @"ImportRingier.xlsx";
        //    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        //    List<AssetImportV9> assets = new List<AssetImportV9>();
        //    int lineIndex = 0;
        //    try
        //    {
        //        using (ExcelPackage package = new ExcelPackage(file))
        //        {


        //            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


        //            int totalRows = worksheet.Dimension.End.Row;
        //            for (int i = 2; i <= totalRows; i++)
        //            {
        //                AssetImportV9 asset = new AssetImportV9();

        //                DateTime date = DateTime.Now;

        //                asset.ERPCode = worksheet.Cells[i, 1].Text.ToString();
        //                asset.InvNo = worksheet.Cells[i, 2].Text.ToString();
        //                asset.Name = worksheet.Cells[i, 3].Text.ToString();
        //                asset.AssetCategory = worksheet.Cells[i, 4].Text.ToString();
        //                asset.LocationCode = worksheet.Cells[i, 5].Text.ToString();
        //                asset.LocationName = worksheet.Cells[i, 6].Text.ToString();
        //                asset.RoomCode = worksheet.Cells[i, 7].Text.ToString();
        //                asset.RoomName = worksheet.Cells[i, 8].Text.ToString();
        //                asset.InternalCode = worksheet.Cells[i, 9].Text.ToString();
        //                asset.EmployeeName = worksheet.Cells[i, 10].Text.ToString();
        //                asset.DocNo = worksheet.Cells[i, 11].Text.ToString();

        //                string initialDate = worksheet.Cells[i, 12].Text.ToString();
        //                string[] strDate = initialDate.Split('/');

        //                int month = int.Parse(strDate[0]);
        //                int day = int.Parse(strDate[1]);
        //                int year = int.Parse(strDate[2]);

        //                asset.PurchaseDate = new DateTime(year, month, day);

        //                asset.ValueInv = decimal.Parse(worksheet.Cells[i, 13].Text.ToString());
        //                asset.SerialNumber = worksheet.Cells[i, 14].Text.ToString();
                            

        //                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(asset))
        //                {
        //                    string name = descriptor.Name;
        //                    object value = descriptor.GetValue(asset);
        //                    Console.WriteLine("{0}={1}", name, value);
        //                }

        //                //asset.AssetState = values[2];
        //                //asset.LocationCode = values[7];
        //                //asset.LocationName = values[8];
        //                //asset.SupplierCode = values[15];
        //                //asset.SupplierName = values[16];

        //                assets.Add(asset);

        //            }


        //        }


        //        foreach (AssetImportV9 asset in assets)
        //        {
        //            ApplicationDbContext context = new ApplicationDbContext();
        //            AssetsRepository repo = new AssetsRepository(context, null);


        //            int assetId = repo.AssetImportV9(asset);
        //            lineIndex++;

        //            Console.WriteLine(lineIndex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Some error occured while importing." + ex.Message;
        //    }

        //    return "";
        //}

        [HttpPost("importv2")]
        public virtual IActionResult PostImportDetailV2([FromBody] AssetImportV2 asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).AssetImportV2(asset);

            return Ok(asset);
        }

 
        [HttpPost]
        [Route("importthales")]
        public virtual IActionResult PostImportThales([FromBody] ImportThales asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).ImportThales(asset);

            return Ok(asset);
        }


        [HttpPost]
        [Route("importitthales")]
        public virtual IActionResult PostImportITThales([FromBody] ImportITThales asset)
        {
            int assetId = 0;

            assetId = (_itemsRepository as IAssetsRepository).ImportITThales(asset);

            return Ok(asset);
        }

        [HttpPost]
        [Route("importitmfx")]
        public async Task<ImportITMFXResult> PostImportITMFX([FromBody] ImportITMFX data)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await userManager.FindByEmailAsync(userName);

                if (user == null)
                {
                    user = await userManager.FindByNameAsync(userName);
                }

                _context.UserId = user.Id.ToString();
                data.EmployeeId = user.EmployeeId;

                return await (_itemsRepository as IAssetsRepository).ImportITMFX(data);
            }
            else
            {
                return new Model.ImportITMFXResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

		[HttpPost]
		[Route("importprintlabel")]
		public async Task<ImportPrintLabelResult> PostImportPrintLabel([FromBody] ImportPrintLabel data)
		{
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				_context.UserId = user.Id.ToString();
				data.EmployeeId = user.EmployeeId;

				return await (_itemsRepository as IAssetsRepository).ImportPrintLabel(data);
			}
			else
			{
				return new Model.ImportPrintLabelResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}


		[HttpGet]
        [Route("details")]
        public virtual IActionResult GetDetails(int? page, int? pageSize, string sortColumn, string sortDirection,
            string filter, string assetCategoryIds, string assetTypeIds, string partnerIds,
            string departmentIds, string employeeIds, string locationIds, string roomIds, string costCenterIds, bool? custody)
        {
            //int totalItems = 0;
            List<int> acIds = null;
            List<int> atIds = null;
            List<int> pIds = null;
            List<int> dIds = null;
            List<int> eIds = null;
            List<int> lIds = null;
            List<int> rIds = null;
            List<int> cIds = null;


            if ((assetCategoryIds != null) && (assetCategoryIds.Length > 0)) acIds = JsonConvert.DeserializeObject<string[]>(assetCategoryIds).ToList().Select(int.Parse).ToList();
            if ((assetTypeIds != null) && (assetTypeIds.Length > 0)) atIds = JsonConvert.DeserializeObject<string[]>(assetTypeIds).ToList().Select(int.Parse).ToList();
            if ((partnerIds != null) && (partnerIds.Length > 0)) pIds = JsonConvert.DeserializeObject<string[]>(partnerIds).ToList().Select(int.Parse).ToList();

            if ((departmentIds != null) && (departmentIds.Length > 0)) dIds = JsonConvert.DeserializeObject<string[]>(departmentIds).ToList().Select(int.Parse).ToList();
            if ((employeeIds != null) && (employeeIds.Length > 0)) eIds = JsonConvert.DeserializeObject<string[]>(employeeIds).ToList().Select(int.Parse).ToList();
            if ((locationIds != null) && (locationIds.Length > 0)) lIds = JsonConvert.DeserializeObject<string[]>(locationIds).ToList().Select(int.Parse).ToList();
            if ((roomIds != null) && (roomIds.Length > 0)) rIds = JsonConvert.DeserializeObject<string[]>(roomIds).ToList().Select(int.Parse).ToList();
            if ((costCenterIds != null) && (costCenterIds.Length > 0)) cIds = JsonConvert.DeserializeObject<string[]>(costCenterIds).ToList().Select(int.Parse).ToList();


            //List<Model.Asset> items = (_itemsRepository as IAssetsRepository).GetDetailsByFilters(filter, acIds, atIds, pIds, dIds, eIds, lIds, rIds, cIds, custody,
            //    sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            //var result = new PagedResult<Dto.AssetInvDetail>(items, new PagingInfo()
            //{
            //    TotalItems = totalItems,
            //    CurrentPage = page.Value,
            //    PageSize = pageSize.Value
            //});

            object result = null;
            return Ok(result);
        }

        [Route("invdetails")]
        [HttpGet]
        //[Route("", Order = -1)]
        public virtual IActionResult GetInvDetails(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter)
        {
            int totalItems = 0;
            int count = 0;
            decimal sumValueInv = 0;
            decimal sumValueRem = 0;
            AssetDepTotal assetDepTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
            //assetFilter.Includes = includes;


            //List<Model.Asset> items = (_itemsRepository as IAssetsRepository)
            //    .GetDetails(paging, sorting, assetFilter, /*out assetDepTotal*/ out count, out sumValueInv, out sumValueRem).ToList();

            List<Model.Asset> items = (_itemsRepository as IAssetsRepository)
                .GetDetails(assetFilter, includes, paging, sorting, out totalItems).ToList();
            var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.Asset>>(items);

            //var result = new PagedResult<Dto.Asset>(itemsResource, new PagingInfo()
            //{
            //    TotalItems = count,
            //    CurrentPage = page,
            //    PageSize = pageSize,
            //    AssetDepDetails = assetDepTotal,
            //    sumValueInv = sumValueInv,
            //    sumValueRem = sumValueRem

            //});

            var result = new PagedResult<Dto.Asset>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize
                 
            });


            return Ok(result);
        }

        [HttpGet]
        [Route("", Order = -1)]
        public virtual async Task<IActionResult> GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId= string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

				if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
			} 
            else
			{
				assetFilter.EmployeeIds = null;
				assetFilter.EmployeeIds = new List<int?>();
				assetFilter.EmployeeIds.Add(int.Parse("100000000"));
			}



			var items = (_itemsRepository as IAssetsRepository)
                .GetMonth(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
                {
                    TotalItems = depTotal.Count,
                    CurrentPage = page,
                    PageSize = pageSize
                }, depTotal);

            return Ok(result);
            
           

            
        }

        [HttpGet]
        [Route("acquisition", Order = -1)]
        public virtual async Task<IActionResult> GetAcquisitionDetails(int page, int pageSize, string sortColumn, string sortDirection, string includes, string docNo1, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            includes += ",Tax";

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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

            includes += "Asset.Order.Offer";

            var items = (_itemsRepository as IAssetsRepository)
                .GetAcquisitionMonth(assetFilter, docNo1, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("scrapping", Order = -1)]
        public virtual async Task<IActionResult> GetScrab(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthScrap(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("sold", Order = -1)]
        public virtual async Task<IActionResult> GetSold(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthSold(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }


        [HttpGet]
        [Route("closed", Order = -1)]
        public virtual async Task<IActionResult> GetClosed(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthClosed(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("wfh", Order = -1)]
        public virtual async Task<IActionResult> GetWFH(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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

            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthWFH(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("invplus", Order = -1)]
        public virtual async Task<IActionResult> GetInvPlus(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInvPlus(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }


        [HttpGet]
        [Route("suspended", Order = -1)]
        public virtual async Task<IActionResult> GetSuspended(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthSuspended(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("inuse", Order = -1)]
        public virtual async Task<IActionResult> GetInUse(int page, int pageSize, string colDefFilter, string filter, string sortColumn, string sortDirection, string includes, string jsonFilter, string columnFilter)
       {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            ColumnAssetFilter colAssetFilters = null;
            List<GenericFilter> genericFilters =  new List<GenericFilter>();
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
            this._context.Database.SetCommandTimeout(200);

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

            if(filter != null && filter != "")
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




        }

		[HttpGet]
		[Route("maptemps", Order = -1)]
		public virtual async Task<IActionResult> GetTemps(int page, int pageSize, string colDefFilter, string filter, string sortColumn, string sortDirection, string includes, string jsonFilter, string propertyFilters)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			List<PropertyFilter> propFilters = null;
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
			propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();

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
				.GetMonthTemps(assetFilter, propFilters, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);




		}

		[HttpGet]
		[Route("stockhistory", Order = -1)]
		public virtual async Task<IActionResult> GetStockHistory(int page, int pageSize, string colDefFilter, string filter, string sortColumn, string sortDirection, string includes, string jsonFilter, string propertyFilters, int? bfId, bool historyMeniu = false)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			List<PropertyFilter> propFilters = null;
			List<GenericFilter> genericFilters = new List<GenericFilter>();
			Paging paging = null;
			Sorting sorting = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			string userName = string.Empty;
			string role = string.Empty;

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
			// genericFilter = colDefFilter != null ? JsonConvert.DeserializeObject<List<GenericFilter>>(colDefFilter) : new GenericFilter();
			propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();

			//         if (colDefFilter != null)
			//{
			//             genericFilters = JsonConvert.DeserializeObject<List<GenericFilter>>(colDefFilter);

			//         }

			includes = includes + ",Asset.AppState";

			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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
				.GetMonthStockHistory(assetFilter, bfId, historyMeniu, propFilters, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);




		}

		[HttpGet]
		[Route("budgetforecastcorrection", Order = -1)]
		public virtual async Task<IActionResult> GetBudgetForecastCorrection(int page, int pageSize, string colDefFilter, string filter, string sortColumn, string sortDirection, string includes, string jsonFilter, string propertyFilters, int? bfId, bool historyMeniu = false)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			List<PropertyFilter> propFilters = null;
			List<GenericFilter> genericFilters = new List<GenericFilter>();
			Paging paging = null;
			Sorting sorting = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			string userName = string.Empty;
			string role = string.Empty;

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
			// genericFilter = colDefFilter != null ? JsonConvert.DeserializeObject<List<GenericFilter>>(colDefFilter) : new GenericFilter();
			propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();

			//         if (colDefFilter != null)
			//{
			//             genericFilters = JsonConvert.DeserializeObject<List<GenericFilter>>(colDefFilter);

			//         }

			includes = includes + ",Asset.AppState";

			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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
				.GetMonthBudgetForecastCorrection(assetFilter, bfId, historyMeniu, propFilters, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);




		}
        
		[HttpGet]
        [Route("employeepersonel", Order = -1)]
        public virtual async Task<IActionResult> GetInEmployeePersonel(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState,Asset.EmployeeTransfer";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInEmployeePersonel(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

		[HttpGet]
		[Route("employeewfh", Order = -1)]
		public virtual async Task<IActionResult> GetInEmployeeWFH(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			Paging paging = null;
			Sorting sorting = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			string userName = string.Empty;
			string role = string.Empty;

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

			includes = includes + ",Asset.AppState,Asset.EmployeeTransfer";

			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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



			var items = (_itemsRepository as IAssetsRepository)
				.GetMonthInEmployeeWFH(assetFilter, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);




		}

		[HttpGet]
        [Route("employeepersonelvalidate", Order = -1)]
        public virtual async Task<IActionResult> GetInEmployeePersonelValidate(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState,Asset.EmployeeTransfer";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                //assetFilter.UserName = "Tudor.Mihailescu";
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInEmployeePersonelValidate(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("employeemanager", Order = -1)]
        public virtual async Task<IActionResult> GetInEmployeeManager(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState,Asset.EmployeeTransfer";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                // assetFilter.UserName = "ioana.cristea";
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInEmployeeManager(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("employeemanagervalidate", Order = -1)]
        public virtual async Task<IActionResult> GetInEmployeeManagerValidate(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState,Asset.EmployeeTransfer";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
				//assetFilter.UserName = "ioana.cristea";
				assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInEmployeeManagerValidate(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("reception", Order = -1)]
        public virtual async Task<IActionResult> GetInReception(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState,Tax";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInReception(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);
        }

		[HttpGet]
		[Route("prereception", Order = -1)]
		public virtual async Task<IActionResult> GetInPreReception(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter,string columnFilter)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			Paging paging = null;
			Sorting sorting = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			string userName = string.Empty;
			string role = string.Empty;
            ColumnAssetFilter colAssetFilters = null;
            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

			includes = includes + ",Asset.AppState,Asset.Order.Offer";
            colAssetFilters = columnFilter != null ? JsonConvert.DeserializeObject<ColumnAssetFilter>(columnFilter) : new ColumnAssetFilter();

			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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



			var items = (_itemsRepository as IAssetsRepository)
				.GetMonthInPreReception(assetFilter, colAssetFilters,includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);
		}

        [HttpGet("exportprereception")]
        public async Task<IActionResult> ExportPrereceptionAsync(AssetFilter filter, string includes)
        {
            var fileContent = await (_itemsRepository as IAssetsRepository).ExportPrereceptionAsync(filter, includes);
            string fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            HttpContext.Response.ContentType = fileType;
            FileContentResult result = new FileContentResult(fileContent, fileType)
            {
                FileDownloadName = "pre-receptie.xlsx"
            };
            return result;
        }

        [HttpGet]
		[Route("receptionhistory", Order = -1)]
		public virtual async Task<IActionResult> GetInReceptionHistory(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			Paging paging = null;
			Sorting sorting = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			string userName = string.Empty;
			string role = string.Empty;

			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

			includes = includes + ",Asset.AppState";

			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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



			var items = (_itemsRepository as IAssetsRepository)
				.GetMonthInReceptionHistory(assetFilter, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);
		}

		[HttpGet]
        [Route("stockit", Order = -1)]
        public virtual async Task<IActionResult> GetStockIT(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockIT(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("stockitmfx", Order = -1)]
        public virtual async Task<IActionResult> GetStockITMFX(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockITMFX(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("stockittovalidate", Order = -1)]
        public virtual async Task<IActionResult> GetStockITToValidate(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockITToValidate(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("stockittovalidateemployee", Order = -1)]
        public virtual async Task<IActionResult> GetStockITToValidateEmployee(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockITToValidateEmployee(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("rejected", Order = -1)]
        public virtual async Task<IActionResult> GetRejection(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            includes = includes + ",Asset.AppState";

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthRejection(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

        [HttpGet]
        [Route("tovalidate", Order = -1)]
        public virtual async Task<IActionResult> GetValidate(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Paging paging = null;
            Sorting sorting = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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



            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthValidate(assetFilter, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);
        }

        [HttpGet]
        [Route("components")]
        public virtual IActionResult GetComponents(int page, int pageSize, string sortColumn, string sortDirection, string includes, string filter)
        {
            AssetDepTotal depTotal = null;
            Paging paging = null;
            Sorting sorting = null;
            AssetFilter assetFilter = null;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;
            List<int> divisionIds = new List<int>();

            includes = includes ?? ",Asset";


            sortColumn = sortColumn ?? "asset.invNo";
            sortDirection = sortDirection ?? "asc";
            if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
                || (page <= 0) || (pageSize <= 0))
                return BadRequest();

            paging = new Paging() { Page = page, PageSize = pageSize };
            sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };

            assetFilter = new AssetFilter();
            assetFilter.Filter = filter;
            assetFilter.AccSystemId = 3;

            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInUse(assetFilter, null, includes, sorting, paging, out depTotal).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items).Select(a => new Dto.Asset
            {
                Id = a.Id,
                InvNo = a.InvNo,
                Name = a.Name,
                SerialNumber = a.SerialNumber,
                Adm = a.Adm
            });

            var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
            {
                TotalItems = depTotal.Count,
                CurrentPage = page,
                PageSize = pageSize
            }, depTotal);

            return Ok(result);




        }

		[HttpGet]
		[Route("addnewAsset")]
		public virtual IActionResult AddAssetValidate(int page, int pageSize, string sortColumn, string sortDirection, string includes, string filter)
		{
			AssetDepTotal depTotal = null;
			Paging paging = null;
			Sorting sorting = null;
			string userName = string.Empty;
			string userId = null;
			string employeeId = string.Empty;
			List<int> divisionIds = new List<int>();


			sortColumn = sortColumn ?? "asset.invNo";
			sortDirection = sortDirection ?? "asc";
			if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
				|| (page <= 0) || (pageSize <= 0))
				return BadRequest();

			paging = new Paging() { Page = page, PageSize = pageSize };
			sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };

			var items = (_itemsRepository as IAssetsRepository)
				.AddNewAssetValidate(includes, sorting, paging, filter, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items).Select(a => new Dto.Asset
			{
				Id = a.Id,
				InvNo = a.InvNo,
				Name = a.Name,
				SerialNumber = a.SerialNumber,
				Adm = a.Adm,
				// Model = a.Model,
				// Brand = a.Brand
			});

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);

			return Ok(result);




		}


		[HttpGet]
		[Route("reco", Order = -1)]
		public virtual async Task<IActionResult> GetRecoDepDetails(int page, int pageSize, string colDefFilter, string filter, string sortColumn, string sortDirection, string includes, string jsonFilter, string propertyFilters)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			List<PropertyFilter> propFilters = null;
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
			propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();

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
				.GetMonthReco(assetFilter, includes, sorting, paging, out depTotal).ToList();
			var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);

			var result = new AssetDepPagedResult(itemsResource, new PagingInfo()
			{
				TotalItems = depTotal.Count,
				CurrentPage = page,
				PageSize = pageSize
			}, depTotal);


			//var assetCategoryDeskPhone = result.AssetCategoryTotals.AssetCategoryDeskPhone;
			//var assetCategoryMonitor = result.AssetCategoryTotals.AssetCategoryMonitor;
			//var assetCategoryThinClient = result.AssetCategoryTotals.AssetCategoryThinClient;
			//assetCategoryDeskPhone = 19;
			//if (((assetCategoryDeskPhone < 20) || (assetCategoryMonitor < 20) || (assetCategoryThinClient < 20)))
			//{
			//    if (company.ModifiedAt.GetValueOrDefault().AddDays(3) <= DateTime.Now)
			//    {

			//        if (assetCategoryDeskPhone < 20)
			//        {
			//            await SendStocksMail("Desk Phone");
			//        }
			//        if (assetCategoryMonitor < 20)
			//        {
			//            await SendStocksMail("Monitor");
			//        }
			//        if (assetCategoryThinClient < 20)
			//        {
			//            await SendStocksMail("Thin Client");
			//        }

			//        company.ModifiedAt = DateTime.Now;
			//        _context.Set<Model.Company>().Update(company);
			//        _context.SaveChanges();
			//    }


			//}


			return Ok(result);




		}


		//[HttpGet]
		//[Route("depdetails")]
		//public virtual IActionResult GetDepDetails(int page, int pageSize, string sortColumn, string sortDirection,
		//    string jsonFilter)
		//{
		//    AssetDepTotal assetDepTotal = null;

		//    AssetFilter assetFilter = null;
		//    Paging paging = null;
		//    Sorting sorting = null;

		//    if ((sortColumn == null) || (sortColumn.Length == 0) || (sortDirection == null) || (sortDirection.Length == 0)
		//        || (page <= 0) || (pageSize <= 0))
		//        return BadRequest();

		//    paging = new Paging() { Page = page, PageSize = pageSize };
		//    sorting = new Sorting() { Column = sortColumn, Direction = sortDirection };
		//    assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

		//    List<Dto.AssetDepDetail> items = (_itemsRepository as IAssetsRepository)
		//        .GetDepDetails(paging, sorting, assetFilter, out assetDepTotal).ToList();

		//    var result = new AssetDepPagedResult(items, new PagingInfo()
		//    {
		//        TotalItems = assetDepTotal.Count,
		//        CurrentPage = page,
		//        PageSize = pageSize
		//    }, assetDepTotal);

		//    return Ok(result);
		//}

		[HttpGet]
        [Route("inventory")]
        public virtual IActionResult GetInventoryDetails(int? page, int? pageSize, string sortColumn, string sortDirection, string reportType, string assetState, 
            int inventoryId, string includes, string filter, 
            string assetCategoryIds, string assetTypeIds, string partnerIds, 
            string administrationIdsIni, string administrationIdsFin, 
            string divisionIdsIni, string divisionIdsFin,
            string invStateIdsIni, string invStateIdsFin, string invStateIdsAll, string userIds,
            string regionIdsIni, string costCenterIdsIni, string admCenterIdsIni, string departmentIdsIni, string employeeIdsIni, string locationIdsIni, string roomIdsIni,
            string regionIdsFin, string costCenterIdsFin, string admCenterIdsFin, string departmentIdsFin, string employeeIdsFin, string locationIdsFin, string roomIdsFin, 
            string admCenterIdsAll, string employeeIdsAll, string locationIdsAll, string regionIdsAll, string costCenterIdsAll, string divisionIdsAll, string departmentIdsAll, string administrationIdsAll, string roomIdsAll, bool? custody, bool? reconcile)
        {
            int totalItems = 0;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;
            includes = includes + ",AssetRecoState";


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (employeeId != null)
                {
                    employeeIdsAll = "[" + employeeId + "]";
                }
                else
                {
                    employeeIdsAll = "[" + -1 + "]";
                }
            }
            else
            {
                employeeIdsAll = "[" + -1 + "]";
            }


            //if (HttpContext.User.Identity.Name != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    Model.ApplicationUser userClaim = _context.Set<Model.ApplicationUser>().Include(c => c.Claims).Where(u => u.UserName == userName).Single();

            //    if (userClaim != null)
            //    {
            //        role = userClaim.Claims.Where(c => c.ClaimType == "role").Select(c => c.ClaimValue).Single();
            //        //role = "user";
            //        employeeId = userClaim.Claims.Where(c => c.ClaimType == "employeeId").Select(c => c.ClaimValue).Single();

            //        if (employeeId != null)
            //        {
            //            employeeIdsAll = "[" + employeeId + "]";
            //        }
            //        else
            //        {
            //            employeeIdsAll = "[" + 100000000 + "]";
            //        }
            //    }
            //}
            //else
            //{
            //    employeeIdsAll = "[" + 100000000 + "]";
            //}



            List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).GetInventoryAssetsByFilters2(
                inventoryId, includes, filter, reportType, assetState, custody, reconcile, false, userName, role, employeeId,
                assetCategoryIds.TryToIntArray(), assetTypeIds.TryToIntArray(), partnerIds.TryToIntArray(), 
                administrationIdsIni.TryToIntArray(), administrationIdsFin.TryToIntArray(), divisionIdsIni.TryToIntArray(), divisionIdsFin.TryToIntArray(),
                invStateIdsIni.TryToIntArray(), invStateIdsFin.TryToIntArray(), invStateIdsAll.TryToIntArray(), userIds.TryToStringtArray(),
                regionIdsIni.TryToIntArray(), costCenterIdsIni.TryToIntArray(), admCenterIdsIni.TryToIntArray(), departmentIdsIni.TryToIntArray(),
                employeeIdsIni.TryToIntArray(), locationIdsIni.TryToIntArray(), roomIdsIni.TryToIntArray(),
                regionIdsFin.TryToIntArray(), costCenterIdsFin.TryToIntArray(), admCenterIdsFin.TryToIntArray(), departmentIdsFin.TryToIntArray(),
                employeeIdsFin.TryToIntArray(), locationIdsFin.TryToIntArray(), roomIdsFin.TryToIntArray(), admCenterIdsAll.TryToIntArray(), 
                employeeIdsAll.TryToIntArray(), locationIdsAll.TryToIntArray(), regionIdsAll.TryToIntArray(), costCenterIdsAll.TryToIntArray(), divisionIdsAll.TryToIntArray(), departmentIdsAll.TryToIntArray(), administrationIdsAll.TryToIntArray(), roomIdsAll.TryToIntArray(),
                sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.InventoryAsset>, List<Dto.InventoryAssetResource>>(items);
            
            var result = new PagedResult<Dto.InventoryAssetResource>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });

            return Ok(result);
        }

		[HttpGet]
		[Route("inventoryemail")]
		public virtual IActionResult GetInventoryEmailDetails(int? page, int? pageSize, string sortColumn, string sortDirection,
			int inventoryId, int appStateId, string includes, string filter,
			string invStateIdsAll, string employeeIdsAll, string locationIdsAll, string regionIdsAll, string roomIdsAll, string companyIds, bool? custody)
		{
			int totalItems = 0;


			List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).GetInventoryEmail(
				inventoryId, appStateId, includes, filter, custody, invStateIdsAll.TryToIntArray(),
				employeeIdsAll.TryToIntArray(), locationIdsAll.TryToIntArray(), regionIdsAll.TryToIntArray(), roomIdsAll.TryToIntArray(), companyIds.TryToIntArray(),
				sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

			var itemsResource = _mapper.Map<List<Model.InventoryAsset>, List<Dto.Reporting.InventoryListEmail>>(items);

			var result = new PagedResult<Dto.Reporting.InventoryListEmail>(itemsResource, new PagingInfo()
			{
				TotalItems = totalItems,
				CurrentPage = page.Value,
				PageSize = pageSize.Value
			});

			return Ok(result);
		}


		[HttpGet]
        [Route("temp")]
        public virtual IActionResult GetInventoryTempDetails(int? page, int? pageSize, string sortColumn, string sortDirection, string reportType,
          int inventoryId, string includes, string filters, string conditionType,
          string regionIds, string costCenterIds, //string admCenterIds, string departmentIds, 
          string employeeIds,
          string countyIds, string cityIds, string locationIds,
          string invStateIds, string userIds, string companyIds, string divisionIds, string departmentIds, string uomIds, string dimensionIds,
          string roomIds,
          bool? custody)
        {
            int totalItems = 0;
            string userName = string.Empty;
            string userId = null;
			string role = string.Empty;
			string employeeId = string.Empty;
            //string admCenterId = string.Empty;
            //List<string> claimStrings = null;

            List<string> fs = null;

            if ((filters != null) && (filters.Length > 0)) fs = JsonConvert.DeserializeObject<string[]>(filters).ToList();

			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				if (employeeId != null)
				{
					employeeIds = "[" + employeeId + "]";
				}
				else
				{
					employeeIds = "[" + -1 + "]";
				}
			}
			else
			{
				employeeIds = "[" + -1 + "]";
			}

			List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).GetInventoryTempAssetsByFilters2(
                inventoryId, includes, fs, conditionType, reportType, custody, userName, role, employeeId,
				regionIds.TryToIntArray(), 
                costCenterIds.TryToIntArray(), //admCenterIds.TryToIntArray(), departmentIds.TryToIntArray(),
                employeeIds.TryToIntArray(),
                countyIds.TryToIntArray(), cityIds.TryToIntArray(), locationIds.TryToIntArray(),
                invStateIds.TryToIntArray(), userIds.TryToStringtArray(), companyIds.TryToIntArray(), divisionIds.TryToIntArray(), departmentIds.TryToIntArray(), uomIds.TryToIntArray(), dimensionIds.TryToIntArray(),
                roomIds.TryToIntArray(),
                sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.InventoryAsset>, List<Dto.InventoryAssetResource>>(items);

            var result = new PagedResult<Dto.InventoryAssetResource>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });

            return Ok(result);
        }


        [HttpGet]
        [Route("inventoryChart/{locationId}")]
        public virtual IActionResult GetInventoryChartDetails(int locationId)
        {
            List<Model.InventoryChartProcentage> items = _context.Set<Model.InventoryChartProcentage>().FromSql("InventoryReportByAdmCenter {0}", 8).ToList();

            return Ok(items);
        }

        [HttpGet]
        [Route("auditChart/{locationId}")]
        public virtual IActionResult GetAuditInventoryChartDetails(int locationId)
        {
            List<Model.AuditInventoryV1T1> items = _context.Set<Model.AuditInventoryV1T1>().FromSql("CustomReport_V1_AuditInventory1Demo {0}, {1}", 8, locationId).ToList();

            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("sync")]
        public virtual IActionResult GetSyncDetails(string includes, int pageSize, int? lastId, DateTime? modifiedAt)
        {
            int totalItems = 0;
            List<Model.Asset> items = (_itemsRepository as IAssetsRepository).GetSync(includes, pageSize, lastId, modifiedAt, out totalItems).ToList();
            var itemsResult = items.Select(i => _mapper.Map<Dto.AssetSync>(i));
            var pagedResult = new PagedResult<Dto.AssetSync>(itemsResult, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = 1,
                PageSize = pageSize
            });
            return Ok(pagedResult);
        }

        [HttpGet]
        [Route("directimport")]
        public virtual IActionResult GetImportResult()
        {
            string filePath = "import\\import.csv";
            int lineIndex = 0;
            List<Dto.AssetImportV2> assets = new List<AssetImportV2>();

            //FileStream fileStream = new FileStream(filePath, FileMode.Open);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string line = reader.ReadLine();

                    while (line != null)
                    {
                        string[] values = line.Split(",".ToCharArray());

                        Dto.AssetImportV2 asset = new AssetImportV2();

                        asset.InvNo = values[0];
                        asset.Name = values[1];
                        asset.SerialNumber = values[2];
                        asset.CostCenterCode = values[3];
                        asset.AdmCenterName = values[4];
                        asset.RoomName = values[5];
                        asset.PurchaseDate = values[6];
                        asset.ValueInv = decimal.Parse(values[7]);
                        asset.ValueDep = decimal.Parse(values[8]);
                        asset.Uom = values[9];
                        asset.Quantity = float.Parse(values[10]);
                        asset.Custody = values[11];
                        asset.InternalCode = values[12];
                        asset.EmployeeFullName = values[13];

                        assets.Add(asset);

                        line = reader.ReadLine();
                        lineIndex++;
                    }
                }
            }

            lineIndex = 0;
            foreach(Dto.AssetImportV2 asset in assets)
            {
                //int assetId = (_itemsRepository as IAssetsRepository).AssetImportV2(asset);

                ApplicationDbContext context = new ApplicationDbContext();
                EfRepository.AssetsRepository repo = new EfRepository.AssetsRepository(context, null);
                int assetId = repo.AssetImportV2(asset);

                lineIndex++;
            }

            return Ok("success");
        }


        [HttpGet("exportIn")]
        public IActionResult ExportIn(int page, int pageSize, string sortColumn, string sortDirection,
          string includes, string jsonFilter)
        {

            AssetFilter assetFilter = null;
            int totalItems = 0;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            List<Model.Asset> items = (_itemsRepository as IAssetsRepository).GetDetails(assetFilter, null, null, null, out totalItems).ToList();
            var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.Asset>>(items);

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Intrari");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. crt";
                worksheet.Cells[1, 2].Value = "Tip op";
                worksheet.Cells[1, 3].Value = "Luna";
                worksheet.Cells[1, 4].Value = "Cat Cont";
                worksheet.Cells[1, 5].Value = "Nr inv";
                worksheet.Cells[1, 6].Value = "Descriere";
                worksheet.Cells[1, 7].Value = "Responsabil";
                worksheet.Cells[1, 8].Value = "Numar marca";
                worksheet.Cells[1, 9].Value = "Locatie";
                worksheet.Cells[1, 10].Value = "Structura responsabila (1)";
                worksheet.Cells[1, 11].Value = "Structura responsabila (2)";
                worksheet.Cells[1, 12].Value = "Data intrare";
                worksheet.Cells[1, 13].Value = "Unitati";
                worksheet.Cells[1, 14].Value = "Valoare";
                worksheet.Cells[1, 15].Value = "Amortizare cumulata";


                int recordIndex = 2;
                foreach (var item in itemsResource)
                {
                    // worksheet.Cells[recordIndex, 1].Value = item.Id;
                    worksheet.Cells[recordIndex, 2].Value = "Intrari";
                    worksheet.Cells[recordIndex, 3].Value = item.PurchaseDate.Value.ToString("MMMM", new CultureInfo("ro-RO"));
                    worksheet.Cells[recordIndex, 4].Value = item.Adm.AssetCategory.Code;
                    worksheet.Cells[recordIndex, 5].Value = item.InvNo;
                    worksheet.Cells[recordIndex, 6].Value = item.Name;
                    worksheet.Cells[recordIndex, 7].Value = item.Adm.Employee.LastName + " " + item.Adm.Employee.FirstName;
                    worksheet.Cells[recordIndex, 8].Value = item.Adm.Employee.InternalCode;
                    worksheet.Cells[recordIndex, 9].Value = item.Adm.Region.Code + "." + item.Adm.Location.Code + "." + item.Adm.Room.Code;
                    worksheet.Cells[recordIndex, 10].Value = item.Adm.Region.Name;
                    worksheet.Cells[recordIndex, 11].Value = item.Adm.Location.Name;
                    worksheet.Cells[recordIndex, 12].Value = item.PurchaseDate.Value.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                    worksheet.Cells[recordIndex, 13].Value = item.Quantity;
                    worksheet.Cells[recordIndex, 14].Value = item.ValueInv;
                    worksheet.Cells[recordIndex, 15].Value = item.Dep.CurrentAPC;
                    recordIndex++;
                }

               




                //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
             //   worksheet.Column(12).Style.Numberformat.Format = "yyyy-mm-dd";



               

                using (var cells = worksheet.Cells[1, 1, 1, 15])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);
                    
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Intrari.xlsx"
                };

                return result;

            }
        }

        [HttpGet("exportOut")]
        public IActionResult ExportOut(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter)
        {
      
            AssetFilter assetFilter = null;
            int totalItems = 0;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            List<Model.Asset> items = (_itemsRepository as IAssetsRepository)
           .GetAssetsOut(assetFilter, null, null, null, out totalItems).ToList();
            var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.Asset>>(items);

            // add a new worksheet to the empty workbook
            //  ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Intrari");
            //First add the headers

            using (ExcelPackage package = new ExcelPackage())
            {

                Dictionary<string, int> MonthIndexes = new Dictionary<string, int>();
                Dictionary<string, int> MonthNames = new Dictionary<string, int>();

                int recordIndex = 0;
                foreach (var item in itemsResource)
                {
                    DateTime myDate = DateTime.ParseExact(item.CreatedBy, "yyyy-MM-dd", new CultureInfo("ro-RO"));
                    string sheetName = myDate.ToString("MMMM", new CultureInfo("ro-RO"));

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[sheetName];

                    worksheet = package.Workbook.Worksheets[sheetName];
                   
                    if(worksheet == null)
                    {
                        recordIndex = 2;
                        worksheet = package.Workbook.Worksheets.Add(sheetName);
                        MonthIndexes.Add(sheetName, recordIndex);
                        MonthNames.Add(item.Document.DocumentDate.ToString("MMMM", new CultureInfo("ro-RO")), recordIndex);

                        worksheet.Cells[1, 1].Value = "Nr. crt";
                        worksheet.Cells[1, 2].Value = "Tip op";
                        worksheet.Cells[1, 3].Value = "Luna";
                        worksheet.Cells[1, 4].Value = "Cat Cont";
                        worksheet.Cells[1, 5].Value = "Nr inv";
                        worksheet.Cells[1, 6].Value = "Descriere";
                        worksheet.Cells[1, 7].Value = "Locatie";
                        worksheet.Cells[1, 8].Value = "Structura responsabila (1)";
                        worksheet.Cells[1, 9].Value = "Structura responsabila (2)";
                        worksheet.Cells[1, 10].Value = "Centrul Cost";
                        worksheet.Cells[1, 11].Value = "Responsabil";
                        worksheet.Cells[1, 12].Value = "Numar marca";
                        worksheet.Cells[1, 13].Value = "Data punere in functiune";
                        worksheet.Cells[1, 14].Value = "Data scoatere din functiune";
                        worksheet.Cells[1, 15].Value = "Unitati";
                        worksheet.Cells[1, 16].Value = "Valoare";
                        worksheet.Cells[1, 17].Value = "Amortizare cumulata";
                        worksheet.Cells[1, 18].Value = "Profit/  Pierdere";

                        worksheet.Cells[recordIndex, 2].Value = "Iesire";
                        worksheet.Cells[recordIndex, 3].Value = myDate.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                        worksheet.Cells[recordIndex, 4].Value = item.Adm.AssetType.Name;
                        worksheet.Cells[recordIndex, 5].Value = item.InvNo;
                        worksheet.Cells[recordIndex, 6].Value = item.Name;
                        worksheet.Cells[recordIndex, 7].Value = item.Adm.Region.Code + "." + item.Adm.Location.Code + "." + item.Adm.Room.Code;
                        worksheet.Cells[recordIndex, 8].Value = item.Adm.Region.Name;
                        worksheet.Cells[recordIndex, 9].Value = item.Adm.Location.Name;
                        worksheet.Cells[recordIndex, 10].Value = item.Adm.CostCenter.Code;
                        worksheet.Cells[recordIndex, 11].Value = item.Adm.Employee.LastName + " " + item.Adm.Employee.FirstName;
                        worksheet.Cells[recordIndex, 12].Value = item.Adm.Employee.InternalCode;
                        worksheet.Cells[recordIndex, 13].Value = item.PurchaseDate.Value.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                        worksheet.Cells[recordIndex, 14].Value = myDate.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                        worksheet.Cells[recordIndex, 15].Value = item.Quantity;
                        worksheet.Cells[recordIndex, 16].Value = item.ValueInv;
                        worksheet.Cells[recordIndex, 17].Value = item.Dep.CurrentAPC;
                        worksheet.Cells[recordIndex, 18].Value = item.ValueInv - item.Dep.CurrentAPC;
                        
                    }
                    else
                    {
                        recordIndex = MonthIndexes[sheetName];
                       
                        worksheet.Cells[recordIndex, 2].Value = "Iesire";
                        worksheet.Cells[recordIndex, 3].Value = myDate.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                        worksheet.Cells[recordIndex, 4].Value = item.Adm.AssetType.Name;
                        worksheet.Cells[recordIndex, 5].Value = item.InvNo;
                        worksheet.Cells[recordIndex, 6].Value = item.Name;
                        worksheet.Cells[recordIndex, 7].Value = item.Adm.Region.Code + "." + item.Adm.Location.Code + "." + item.Adm.Room.Code;
                        worksheet.Cells[recordIndex, 8].Value = item.Adm.Region.Name;
                        worksheet.Cells[recordIndex, 9].Value = item.Adm.Location.Name;
                        worksheet.Cells[recordIndex, 10].Value = item.Adm.CostCenter.Code;
                        worksheet.Cells[recordIndex, 11].Value = item.Adm.Employee.LastName + " " + item.Adm.Employee.FirstName;
                        worksheet.Cells[recordIndex, 12].Value = item.Adm.Employee.InternalCode;
                        worksheet.Cells[recordIndex, 13].Value = item.PurchaseDate.Value.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                        worksheet.Cells[recordIndex, 14].Value = myDate.ToString("dd-MMMM-yyyy", new CultureInfo("ro-RO"));
                        worksheet.Cells[recordIndex, 15].Value = item.Quantity;
                        worksheet.Cells[recordIndex, 16].Value = item.ValueInv;
                        worksheet.Cells[recordIndex, 17].Value = item.Dep.CurrentAPC;
                        worksheet.Cells[recordIndex, 18].Value = item.ValueInv - item.Dep.CurrentAPC;
                        
                       
                    }


                    //worksheet.Column(1).AutoFit();
                    //worksheet.Column(2).AutoFit();
                    //worksheet.Column(3).AutoFit();
                    //worksheet.Column(4).AutoFit();
                    //worksheet.Column(5).AutoFit();
                    //worksheet.Column(6).AutoFit();
                    //worksheet.Column(7).AutoFit();
                    //worksheet.Column(8).AutoFit();
                    //worksheet.Column(9).AutoFit();
                    //worksheet.Column(10).AutoFit();
                    //worksheet.Column(11).AutoFit();
                    //worksheet.Column(12).AutoFit();
                    //worksheet.Column(13).AutoFit();
                    //worksheet.Column(14).AutoFit();
                    //worksheet.Column(15).AutoFit();
                    //worksheet.Column(16).AutoFit();
                    //worksheet.Column(17).AutoFit();
                    //worksheet.Column(18).AutoFit();



                    //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
                    //  worksheet.Column(13).Style.Numberformat.Format = "yyyy-mm-dd";
                    //  worksheet.Column(14).Style.Numberformat.Format = "yyyy-MM-dd";


                    //  worksheet.Cells.AutoFitColumns();

                    using (var cells = worksheet.Cells[1, 1, 1, 18])
                    {
                        cells.Style.Font.Bold = true;
                        cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);
                        cells.AutoFitColumns();
                       
                    }

                    recordIndex++;
                    MonthIndexes[sheetName] = recordIndex;
                }

              

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Iesiri.xlsx"
                };

                return result;

            }
        }

        [HttpGet("export")]
        public IActionResult Export(int page, int pageSize, string sortColumn, string sortDirection,
            string includes, string jsonFilter)
        {

            AssetFilter assetFilter = null;
            string userName = string.Empty;
            string userId = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            // if (HttpContext != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    var user = _context.Users.Where(u => u.UserName == userName).SingleOrDefault();
            //    userId = user != null ? user.Id : null;
            //    employeeId = user != null ? user.EmployeeId.ToString() : null;
            //    admCenterId = user != null ? user.AdmCenterId.ToString() : null;
            //    _context.UserId = userId;
            //    if (employeeId != null && employeeId != "")
            //    {
            //        if ((assetFilter).EmployeeIds == null) assetFilter.EmployeeIds = new List<int?>();
            //        assetFilter.EmployeeIds.Add(int.Parse(employeeId));
            //    }

            //    if (admCenterId != null && admCenterId != "")
            //    {
            //        if ((assetFilter).AdmCenterIds == null) assetFilter.AdmCenterIds = new List<int?>();
            //        assetFilter.AdmCenterIds.Add(int.Parse(admCenterId));
            //    }

            //}

            //if (HttpContext != null)
            //{
            //    userName = HttpContext.User.Identity.Name;
            //    var user = _context.Users.Include(r => r.Claims).Where(u => u.UserName == userName).SingleOrDefault();


            //    userId = user != null ? user.Id : null;
            //    employeeId = user != null ? user.EmployeeId.ToString() : null;
            //    admCenterId = user != null ? user.AdmCenterId.ToString() : null;
            //    _context.UserId = userId;

            //    if (user != null)
            //    {
            //        var claims = user.Claims.ToList();
            //        if (claims[2].ClaimValue == "administrator")
            //        {
            //            employeeId = null;
            //            admCenterId = null;
            //        }


            //    }

            //    if (employeeId != null && employeeId != "")
            //    {
            //        if (assetFilter.EmployeeIds.Count() > 0)
            //        {
            //            var claimss = user.Claims.ToList();
            //            if (claimss[2].ClaimValue == "user")
            //            {

            //                assetFilter.EmployeeIds = null;
            //                assetFilter.EmployeeIds = new List<int?>();
            //                assetFilter.EmployeeIds.Add(int.Parse(employeeId));
            //            }
            //            else
            //            {
            //                employeeId = null;
            //            }

            //        }
            //        else
            //        {
            //            if ((assetFilter).EmployeeIds == null) assetFilter.EmployeeIds = new List<int?>();
            //            assetFilter.EmployeeIds.Add(int.Parse(employeeId));
            //        }

            //    }

            //    if (admCenterId != null && admCenterId != "")
            //    {
            //        if ((assetFilter).AdmCenterIds == null) assetFilter.AdmCenterIds = new List<int?>();
            //        assetFilter.AdmCenterIds.Add(int.Parse(admCenterId));

            //        if (employeeId != null && employeeId != "")
            //        {
            //            assetFilter.EmployeeIds = null;
            //        }

            //    }

            //    if (user == null)
            //    {
            //        assetFilter.EmployeeIds = null;
            //        assetFilter.EmployeeIds = new List<int?>();
            //        assetFilter.EmployeeIds.Add(int.Parse("10000000000000000"));
            //    }
            //}



            //List<Model.Asset> items = (_itemsRepository as IAssetsRepository).GetDetails(assetFilter, null, null, null, out totalItems).ToList();
            //var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.Asset>>(items);

            // if (assetFilter.AccMonthId.GetValueOrDefault() == 0) assetFilter.AccMonthId = 9;





            includes = @"Asset.Uom,Adm.AssetCategory,Asset.Document.Partner,Asset.Department,Asset.AssetInv,Adm.AssetType,Adm.SubType.Type.MasterType,Adm.SubType.Type,Adm.SubType,Adm.InsuranceCategory,Adm.AssetState,Asset.InvState,Asset.Document,Dep.AccSystem,Dep,Adm.Employee,Adm.Room.Location.Region,Adm.Room.Location,Adm.Room,Adm.CostCenter,Adm.AssetNature,Adm.BudgetManager,Asset.Dimension,Adm.Project,Asset.Company,Adm.InterCompany,";

            List<Model.AssetMonthDetail> items = (_itemsRepository as IAssetsRepository)
               .GetMonthExport(assetFilter, includes).ToList();
            var itemsResource = _mapper.Map<List<Model.AssetMonthDetail>, List<Dto.Asset>>(items);



            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Export");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. crt";
                worksheet.Cells[1, 2].Value = "OPTIMA RO";
                worksheet.Cells[1, 3].Value = "ERPCODE";
                worksheet.Cells[1, 4].Value = "Serial number";
                worksheet.Cells[1, 5].Value = "Asset Type";
                worksheet.Cells[1, 6].Value = "Master Type";
                worksheet.Cells[1, 7].Value = "Type";
                worksheet.Cells[1, 8].Value = "Sub Type";
                worksheet.Cells[1, 9].Value = "Insurance Category";
                worksheet.Cells[1, 10].Value = "Description";
                worksheet.Cells[1, 11].Value = "Model";
                worksheet.Cells[1, 12].Value = "Brand";
                worksheet.Cells[1, 13].Value = "Operation Type";
                worksheet.Cells[1, 14].Value = "Asset State";
                worksheet.Cells[1, 15].Value = "Furnizor";
                worksheet.Cells[1, 16].Value = "ReceptionDate";
                worksheet.Cells[1, 17].Value = "PO Date";
                worksheet.Cells[1, 18].Value = "Invoice Date";
                worksheet.Cells[1, 19].Value = "Removal Date";
                worksheet.Cells[1, 20].Value = "Invoice No";
                worksheet.Cells[1, 21].Value = "PO No";
                worksheet.Cells[1, 22].Value = "Cost";
                worksheet.Cells[1, 23].Value = "Currency";
                worksheet.Cells[1, 24].Value = "Depreciation";
                worksheet.Cells[1, 25].Value = "Carring amount";
                worksheet.Cells[1, 26].Value = "Depreciation period (months)";
                worksheet.Cells[1, 27].Value = "Employee Code";
                worksheet.Cells[1, 28].Value = "Employee Last Name";
                worksheet.Cells[1, 29].Value = "Employee First Name";
                worksheet.Cells[1, 30].Value = "Location";
                worksheet.Cells[1, 31].Value = "Building";
                worksheet.Cells[1, 32].Value = "Room";
                worksheet.Cells[1, 33].Value = "N+1";
                worksheet.Cells[1, 34].Value = "Employee Status";
                worksheet.Cells[1, 35].Value = "Cost center";
                worksheet.Cells[1, 36].Value = "Budget line";
                worksheet.Cells[1, 37].Value = "Budget manager";
                worksheet.Cells[1, 38].Value = "Run change";
                worksheet.Cells[1, 39].Value = "Project";
                worksheet.Cells[1, 40].Value = "Client";
                worksheet.Cells[1, 41].Value = "Inter Company";
                worksheet.Cells[1, 42].Value = "IT/NON-IT";
                worksheet.Cells[1, 43].Value = "Asset Category";



                int recordIndex = 2;
                foreach (var item in itemsResource)
                {
                    rowNumber++;
                    worksheet.Cells[recordIndex, 1].Value = rowNumber;
                    worksheet.Cells[recordIndex, 2].Value = item.InvNo;
                    worksheet.Cells[recordIndex, 3].Value = item.ERPCode;
                    worksheet.Cells[recordIndex, 4].Value = item.SerialNumber;
                    worksheet.Cells[recordIndex, 5].Value = item.Adm.AssetType != null ? item.Adm.AssetType.Name : "";
                    worksheet.Cells[recordIndex, 6].Value = item.Adm.MasterType != null ?  item.Adm.MasterType.Name : "";
                    worksheet.Cells[recordIndex, 7].Value = item.Adm.Type != null ? item.Adm.Type.Name : "";
                    worksheet.Cells[recordIndex, 8].Value = item.Adm.SubType != null ? item.Adm.SubType.Name : "";
                    worksheet.Cells[recordIndex, 9].Value = item.Adm.InsuranceCategory != null ? item.Adm.InsuranceCategory.Name : "";
                    worksheet.Cells[recordIndex, 10].Value = item.Name;
                    worksheet.Cells[recordIndex, 11].Value = item.ModelIni;
                    worksheet.Cells[recordIndex, 12].Value = string.Empty;
                    worksheet.Cells[recordIndex, 13].Value = item.Adm.AssetState != null ? item.Adm.AssetState.Name : "";
                    worksheet.Cells[recordIndex, 14].Value = item.InvState != null ? item.InvState.Name : "";
                    worksheet.Cells[recordIndex, 15].Value = item.Partner != null ? item.Partner.Name : "";
                    worksheet.Cells[recordIndex, 16].Value = item.ReceptionDate != null ? item.ReceptionDate.ToString("dd-MM-yyyy", new CultureInfo("ro-RO")) : "";
                    worksheet.Cells[recordIndex, 17].Value = item.PODate != null ? item.PODate.ToString("dd-MM-yyyy", new CultureInfo("ro-RO")) : "";
                    worksheet.Cells[recordIndex, 18].Value = item.InvoiceDate != null ? item.InvoiceDate.ToString("dd-MM-yyyy", new CultureInfo("ro-RO")) : "";
                    worksheet.Cells[recordIndex, 19].Value = item.RemovalDate != null ? item.RemovalDate.ToString("dd-MM-yyyy", new CultureInfo("ro-RO")) : "";
                    worksheet.Cells[recordIndex, 20].Value = item.DocNo1;
                    worksheet.Cells[recordIndex, 21].Value = item.DocNo2;
                    worksheet.Cells[recordIndex, 22].Value = item.Dep.DepForYear;
                    worksheet.Cells[recordIndex, 23].Value = item.Dep.AccSystem.Name;
                    worksheet.Cells[recordIndex, 24].Value = item.Dep.AccumulDep;
                    worksheet.Cells[recordIndex, 25].Value = item.Dep.BkValFYStart;
                    worksheet.Cells[recordIndex, 26].Value = item.Dep.RemLifeInPeriods;
                    worksheet.Cells[recordIndex, 27].Value = item.Adm.Employee != null ? item.Adm.Employee.InternalCode : "";
                    worksheet.Cells[recordIndex, 28].Value = item.Adm.Employee != null ? item.Adm.Employee.FirstName : "";
                    worksheet.Cells[recordIndex, 29].Value = item.Adm.Employee != null ? item.Adm.Employee.LastName : "";
                    worksheet.Cells[recordIndex, 30].Value = item.Adm.Region != null ? item.Adm.Region.Code.Contains("Bucuresti") ? item.Adm.Region.Code + " - " + item.Adm.Region.Name : item.Adm.Region.Name : "";
                    worksheet.Cells[recordIndex, 31].Value = item.Adm.Location != null ? item.Adm.Location.Code : "";
                    worksheet.Cells[recordIndex, 32].Value = item.Adm.Room != null ? item.Adm.Room.Name : "";
                    worksheet.Cells[recordIndex, 33].Value = item.Adm.Employee != null ? item.Adm.Employee.ErpCode : "";
                    worksheet.Cells[recordIndex, 34].Value = item.Adm.Employee != null ? item.Adm.Employee.IsDeleted == true ? "Inactive" : "Active" : "";
                    worksheet.Cells[recordIndex, 35].Value = item.Adm.CostCenter != null ? item.Adm.CostCenter.Name : "";
                    worksheet.Cells[recordIndex, 36].Value = item.Adm.AssetNature != null ? item.Adm.AssetNature.Name : "";
                    worksheet.Cells[recordIndex, 37].Value = item.Adm.BudgetManager != null ? item.Adm.BudgetManager.Name : "";
                    worksheet.Cells[recordIndex, 38].Value = item.Dimension != null ? item.Dimension.Length : "";
                    worksheet.Cells[recordIndex, 39].Value = item.Adm.Project != null ? item.Adm.Project.Name : "";
                    worksheet.Cells[recordIndex, 40].Value = item.Uom != null ? item.Uom.Name : "";
                    worksheet.Cells[recordIndex, 41].Value = item.Adm.InterCompany != null ? item.Adm.InterCompany.Name : "";
					worksheet.Cells[recordIndex, 42].Value = item.Company != null ? item.Company.Name : "";
					worksheet.Cells[recordIndex, 43].Value = item.Adm.AssetCategory != null ? item.Adm.AssetCategory.Name : "";



                    recordIndex++;
                }






                //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
                //   worksheet.Column(12).Style.Numberformat.Format = "yyyy-mm-dd";





                using (var cells = worksheet.Cells[1, 1, 1, 43])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

        [HttpGet("exportOtp")]
        public IActionResult ExportOtp(int page, int pageSize, string sortColumn, string sortDirection,
        string includes, string jsonFilter)
        {

            AssetFilter assetFilter = null;
            int totalItems = 0;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

            List<Model.Asset> items = (_itemsRepository as IAssetsRepository).GetDetails(assetFilter, null, null, null, out totalItems).ToList();
            var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.Asset>>(items);

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("OTP");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr. crt";
                worksheet.Cells[1, 2].Value = "Nr inv";
                worksheet.Cells[1, 3].Value = "Descriere";
                worksheet.Cells[1, 4].Value = "Serial number";
                worksheet.Cells[1, 5].Value = "Valoare";
                worksheet.Cells[1, 6].Value = "Cantitate initiala";
                worksheet.Cells[1, 7].Value = "Cod centru de cost";
                worksheet.Cells[1, 8].Value = "Denumire Centru de cost";
                worksheet.Cells[1, 9].Value = "Cod cladire";
                worksheet.Cells[1, 10].Value = "Denumire cladire";
              


                int recordIndex = 2;
                foreach (var item in itemsResource)
                {
                    worksheet.Cells[recordIndex, 2].Value = item.InvNo;
                    worksheet.Cells[recordIndex, 3].Value = item.Name;
                    worksheet.Cells[recordIndex, 4].Value = item.SerialNumber;
                    worksheet.Cells[recordIndex, 5].Value = item.ValueInv;
                    worksheet.Cells[recordIndex, 6].Value = item.Quantity;
                    worksheet.Cells[recordIndex, 7].Value = item.Adm.Room.Code;
                    worksheet.Cells[recordIndex, 8].Value = item.Adm.Room.Name;
                    worksheet.Cells[recordIndex, 9].Value = item.Adm.Location.Code;
                    worksheet.Cells[recordIndex, 10].Value = item.Adm.Location.Name;
                    recordIndex++;
                }






                //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
                //   worksheet.Column(12).Style.Numberformat.Format = "yyyy-mm-dd";





                using (var cells = worksheet.Cells[1, 1, 1, 15])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.Khaki);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export OTP.xlsx"
                };

                return result;

            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            MemoryStream ms = null;
            //  ApplicationDbContext context = new ApplicationDbContext();
            //  EfRepository.AssetOpsRepository repo = new EfRepository.AssetOpsRepository(context);

            string invNoUpdated = "";

            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            ms = new MemoryStream();
            file.CopyTo(ms);

            await ImportEmag(ms);

            if (invNoUpdated != "")
            {
                return Ok(StatusCode(200));
            }
            else
            {
                return Ok(StatusCode(400));
            }
        }

        private async Task<IActionResult> ImportEmag(MemoryStream ms, string operationType = "Import")
        {
			Model.AssetClass assetClass = null;
			Model.AssetClassType assetClassType = null;
			Model.AccSystem accSystem = null;
			Model.Administration administration = null;
			Model.AssetType assetType = null;
			Model.Company company = null;
			Model.Uom uom = null;
			Model.AssetState assetState = null;
			Model.InvState invState = null;
			Model.Dimension dimension = null;
			Model.Inventory inventory = null;

			string assetClassTypeDefault = "-";
			string assetTypeDefault = "-";
			string assetClassDefault = "-";
			string accSystemDefault = "RON";
			string codeDefault = "NSP";
			string nameDefault = "Nespecificat";
			string uomDefault = "RON";

			List<Dto.AssetImportEmag> assets = null;
            assets = new List<AssetImportEmag>();
            int lineIndex = 0;

            using (ExcelPackage package = new ExcelPackage(ms))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int rows = worksheet.Dimension.End.Row;

				assetClassType = await _context.Set<Model.AssetClassType>().Where(a => a.Code == assetClassTypeDefault).SingleAsync();

                /*
				//if (assetClassType == null)
				//{
				//	assetClassType = new Model.AssetClassType
				//	{
				//		Code = assetClassTypeDefault,
				//		Name = assetClassTypeDefault,
				//		IsDeleted = false
				//	};
				//	_context.Set<Model.AssetClassType>().Add(assetClassType);
				//}
                */

				assetClass = await _context.Set<Model.AssetClass>().Where(a => a.Id == 22).SingleAsync();

                /*
                //if (assetClass == null)
                //{
                //    assetClass = new Model.AssetClass
                //    {
                //        Code = assetImport.AssetClass.Trim(),
                //        Name = assetImport.AssetClass.Trim(),
                //        IsDeleted = false,
                //        AssetClassType = assetClassType
                //    };
                //    _context.Set<Model.AssetClass>().Add(assetClass);
                //}
                */

                accSystem = await _context.Set<Model.AccSystem>().Where(a => a.Code == accSystemDefault).SingleAsync();

                /*
                //if (accSystem == null)
                //{
                //	accSystem = new Model.AccSystem
                //	{
                //		AssetClassType = assetClassType,
                //		Code = accSystemDefault.Trim(),
                //		Name = accSystemDefault.Trim(),
                //		IsDeleted = false
                //	};
                //	_context.Set<Model.AccSystem>().Add(accSystem);
                //}

                //if (administration == null)
                //{
                //    administration = new Model.Administration
                //    {
                //        Code = assetImport.Administration.Trim(),
                //        Name = assetImport.Administration.Trim(),
                //        IsDeleted = false
                //    };

                //    _context.Set<Model.Administration>().Add(administration);
                //}
                */

                assetType = await _context.Set<Model.AssetType>().Where(a => a.Code.Trim() == assetTypeDefault).SingleAsync();

                /*
                //if (assetType == null)
                //{
                //	assetType = new Model.AssetType
                //	{
                //		Code = assetTypeDefault.Trim(),
                //		Name = assetTypeDefault.Trim(),
                //		IsDeleted = true
                //	};
                //	_context.Set<Model.AssetType>().Add(assetType);
                //}

                //interCompany = _context.Set<Model.InterCompany>().Where(a => a.Code == assetImport.InterCompany.Trim()).SingleOrDefault();

                //if (interCompany == null)
                //{
                //    interCompany = new Model.InterCompany
                //    {
                //        Code = assetImport.InterCompany.Trim(),
                //        Name = assetImport.InterCompany.Trim(),
                //        IsDeleted = false
                //    };
                //    _context.Set<Model.InterCompany>().Add(interCompany);
                //}
                */

                company = await _context.Set<Model.Company>().Where(a => a.Code == "RO10").SingleAsync();

                /*
                //if (company == null)
                //{
                //	company = new Model.Company
                //	{
                //		Code = codeDefault.Trim(),
                //		Name = nameDefault.Trim(),
                //		IsDeleted = false
                //	};
                //	_context.Set<Model.Company>().Add(company);
                //}
                */

                var documentType = await _context.Set<Model.DocumentType>().Where(d => d.Code == "PURCHASE".ToUpper()).SingleAsync();

				uom = await _context.Set<Model.Uom>().Where(a => a.Code == uomDefault).SingleAsync();

                /*
                //if (uom == null)
                //{
                //	uom = new Model.Uom
                //	{
                //		Code = uomDefault,
                //		Name = uomDefault,
                //		IsDeleted = false
                //	};
                //	_context.Set<Model.Uom>().Add(uom);
                //}
                */

                dimension = await _context.Set<Model.Dimension>().Where(a => a.Length == codeDefault).SingleAsync();

                /*
                //if (dimension == null)
                //{
                //	dimension = new Model.Dimension
                //	{
                //		Length = codeDefault,
                //		Width = codeDefault,
                //		Height = codeDefault,
                //		IsDeleted = false
                //	};
                //	_context.Set<Model.Dimension>().Add(dimension);
                //} 
                */

                invState = await _context.Set<Model.InvState>().Where(a => a.Code == "F".Trim()).SingleAsync();

                /*
                //if (invState == null)
                //{
                //	invState = new Model.InvState
                //	{
                //		Code = codeDefault,
                //		Name = nameDefault,
                //		IsDeleted = false
                //	};
                //	_context.Set<Model.InvState>().Add(invState);
                //} 
                */

                assetState = await _context.Set<Model.AssetState>().Where(a => a.Code == "IN_USE".Trim()).SingleAsync();

                /*
                //if (assetState == null)
                //{
                //	assetState = new Model.AssetState
                //	{
                //		Code = codeDefault,
                //		Name = nameDefault,
                //		IsDeleted = false
                //	};
                //	_context.Set<Model.AssetState>().Add(assetState);
                //} 
                */

                inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleAsync();

				for (int i = 2; i <= rows; i++)
                {
                    int columnIndex = 0;
                    if (worksheet.Cells[i, 1].Text != "")
                    {
                        Dto.AssetImportEmag asset = new AssetImportEmag();

                        asset.InvNo = worksheet.Cells[i, ++columnIndex].Text.ToString();
                        asset.SubNo = worksheet.Cells[i, ++columnIndex].Text.ToString(); ++columnIndex;

                        if (asset.SubNo.Length == 1)
                        {
                            asset.SubNo = "000" + asset.SubNo;
                        }
                        else if (asset.SubNo.Length == 2)
                        {
                            asset.SubNo = "00" + asset.SubNo;
                        }
                        else if (asset.SubNo.Length == 3)
                        {
                            asset.SubNo = "0" + asset.SubNo;
                        }

                        Model.Asset dbAsset = await _context.Set<Model.Asset>().Where(a => a.InvNo == asset.InvNo.Trim() && a.SubNo == asset.SubNo && a.IsDeleted == false).FirstOrDefaultAsync();

                        if (dbAsset == null) //nu exista in bd, asset nou
                        {
                            asset.Quantity = 1;
                            asset.Account = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.ExpAccount = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.AssetCategory = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.Description = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.SAPCode = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            //asset.License = worksheet.Cells[i, 8].Text.ToString();
                            asset.SerialNumber = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.CUI = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.Partner = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            //asset.AgreementNo = worksheet.Cells[i, 11].Text.ToString();
                            asset.ManufacturerNo = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.Article = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.CostCenterCode = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.CostCenterName = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            //asset.CostCenterDescription = worksheet.Cells[i, 41].Text.ToString();

                            string PIFDate = worksheet.Cells[i, ++columnIndex].Text.ToString();

                            if (PIFDate.Length > 0)
                            {
                                string[] strDate = PIFDate.Split('.');

                                int day = int.Parse(strDate[0]);
                                int month = int.Parse(strDate[1]);
                                int year = int.Parse(strDate[2]);

                                asset.PIFDate = new DateTime(year, month, day);
                            }
                            else
                            {
                                asset.PIFDate = null;
                            }

                            string UsageStartDate = worksheet.Cells[i, ++columnIndex].Text.ToString();

                            if (UsageStartDate.Length > 0)
                            {
                                string[] strDate = UsageStartDate.Split('.');

                                int day = int.Parse(strDate[0]);
                                int month = int.Parse(strDate[1]);
                                int year = int.Parse(strDate[2]);

                                asset.UsageStartDate = new DateTime(year, month, day);
                            }
                            else
                            {
                                asset.UsageStartDate = null;
                            }

                            string RemovalDate = worksheet.Cells[i, ++columnIndex].Text.ToString();


                            if (RemovalDate.Length > 0 && RemovalDate.Length > 5)
                            {
                                string[] strDate = RemovalDate.Split('.');

                                int month = 0;
                                int day = 0;
                                int year = 0;

                                if (int.Parse(strDate[1]) > 12)
                                {
                                    month = int.Parse(strDate[0]);
                                    day = int.Parse(strDate[1]);
                                    year = int.Parse(strDate[2]);
                                }
                                else
                                {
                                    day = int.Parse(strDate[0]);
                                    month = int.Parse(strDate[1]);
                                    year = int.Parse(strDate[2]);
                                }


                                asset.RemovalDate = new DateTime(year, month, day);
                            }
                            else
                            {

                                asset.RemovalDate = null;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.UsefulLife = int.Parse(worksheet.Cells[i, columnIndex].Text.ToString());
                            }
                            else
                            {
                                asset.UsefulLife = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.TotLifeInPeriods = int.Parse(worksheet.Cells[i, columnIndex].Text.ToString());
                            }
                            else
                            {
                                asset.TotLifeInPeriods = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.ExpLifeInPeriods = int.Parse(worksheet.Cells[i, columnIndex].Text.ToString());
                            }
                            else
                            {
                                asset.ExpLifeInPeriods = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.RemLifeInPeriods = int.Parse(worksheet.Cells[i, columnIndex].Text.ToString());
                            }
                            else
                            {
                                asset.RemLifeInPeriods = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                //asset.ValueInvIn = decimal.Parse(worksheet.Cells[i, 24].Text.TrimStart().ToString(), CultureInfo.InvariantCulture);
                                asset.APCFYStart = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                            }
                            else
                            {
                                asset.APCFYStart = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.DepFYStart = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.ToString());
                                //asset.ValueDepIn = decimal.Parse(worksheet.Cells[i, 25].Text.ToString());
                            }
                            else
                            {
                                asset.DepFYStart = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.BkValFYStart = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueDepPU = decimal.Parse(worksheet.Cells[i, 26].Text.ToString());
                            }
                            else
                            {
                                asset.BkValFYStart = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.Acquisition = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueDepYTDIn = decimal.Parse(worksheet.Cells[i, 27].Text.ToString());
                            }
                            else
                            {
                                asset.Acquisition = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.DepForYear = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueDepYTD = decimal.Parse(worksheet.Cells[i, 28].Text.ToString());
                            }
                            else
                            {
                                asset.DepForYear = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.Retirement = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueRet = decimal.Parse(worksheet.Cells[i, 29].Text.ToString());
                            }
                            else
                            {
                                asset.Retirement = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.DepRetirement = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueRetIn = decimal.Parse(worksheet.Cells[i, 30].Text.ToString());
                            }
                            else
                            {
                                asset.DepRetirement = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.CurrBkValue = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueDepPUIn = decimal.Parse(worksheet.Cells[i, 31].Text.ToString());
                            }
                            else
                            {
                                asset.CurrBkValue = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.Transfer = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueTr = decimal.Parse(worksheet.Cells[i, 32].Text.ToString());
                            }
                            else
                            {
                                asset.Transfer = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.DepTransfer = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueTrIn = decimal.Parse(worksheet.Cells[i, 33].Text.ToString());
                            }
                            else
                            {
                                asset.DepTransfer = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.PosCap = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueRem = decimal.Parse(worksheet.Cells[i, 34].Text.ToString());
                            }
                            else
                            {
                                asset.PosCap = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.DepPostCap = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueRemIn = decimal.Parse(worksheet.Cells[i, 35].Text.ToString());
                            }
                            else
                            {
                                asset.DepPostCap = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.InvestSupport = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.InvestSupport = decimal.Parse(worksheet.Cells[i, 36].Text.ToString());
                            }
                            else
                            {
                                asset.InvestSupport = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.WriteUps = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.WriteUps = decimal.Parse(worksheet.Cells[i, 37].Text.ToString());
                            }
                            else
                            {
                                asset.WriteUps = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.CurrentAPC = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueInv = decimal.Parse(worksheet.Cells[i, 38].Text.ToString());
                            }
                            else
                            {
                                asset.CurrentAPC = 0;
                            }

                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.AccumulDep = NormalizeAndParse(worksheet.Cells[i, columnIndex].Text.TrimStart().ToString());
                                //asset.ValueDep = decimal.Parse(worksheet.Cells[i, 39].Text.ToString());
                            }
                            else
                            {
                                asset.AccumulDep = 0;
                            }


                            asset.IdentityNumber = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.PersonnelNumber = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            asset.ProfitCenter = worksheet.Cells[i, ++columnIndex].Text.ToString();
                            if (worksheet.Cells[i, ++columnIndex].Text.ToString().Length > 0)
                            {
                                asset.UsefullLifeInPeriods = int.Parse(worksheet.Cells[i, columnIndex].Text.ToString());
                            }
                            else
                            {
                                asset.UsefullLifeInPeriods = 0;
                            }

                            asset.InventoryId = inventory.Id;
                            asset.AccMonthId = inventory.AccMonthId.Value;
                            asset.DocumentId = inventory.DocumentId;
                            asset.BudgetManagerId = inventory.BudgetManagerId.Value;
                            asset.AssetStateId = assetState.Id;
                            asset.InvStateId = invState.Id;
                            asset.DimensionId = dimension.Id;
                            asset.UomId = uom.Id;
                            asset.DocumentTypeId = documentType.Id;
                            asset.CompanyId = company.Id;
                            asset.AssetTypeId = assetType.Id;

                            columnIndex++; administration = await _context.Set<Model.Administration>().Where(a => a.Name == (worksheet.Cells[i, columnIndex].Text.ToString())).SingleAsync();

                            asset.AdministrationId = administration.Id;
                            asset.AccSystemId = accSystem.Id;
                            asset.AssetClassId = assetClass.Id;
                            asset.AssetClassTypeId = assetClassType.Id;

                            /*
                             //string ScanDate = worksheet.Cells[i, 42].Text.ToString();

                            //if (ScanDate.Length > 0)
                            //{
                            //    string[] strDate = ScanDate.Split('/');

                            //    int month = int.Parse(strDate[0]);
                            //    int day = int.Parse(strDate[1]);
                            //    int year = int.Parse(strDate[2]);

                            //    asset.ScanDate = new DateTime(year, month, day);
                            //}
                            //else
                            //{
                            //    asset.ScanDate = null;
                            //}

                            //asset.ScanUser = worksheet.Cells[i, 43].Text.ToString();
                            //asset.Department = worksheet.Cells[i, 42].Text.ToString();
                            //asset.Division = worksheet.Cells[i, 43].Text.ToString();
                            //asset.Room = worksheet.Cells[i, 44].Text.ToString();
                            //asset.Administration = worksheet.Cells[i, 45].Text.ToString();
                            //asset.Country = worksheet.Cells[i, 46].Text.ToString();
                            //asset.BudgetManager = worksheet.Cells[i, 47].Text.ToString();
                            //asset.AssetNature = worksheet.Cells[i, 48].Text.ToString();
                            //asset.Type = worksheet.Cells[i, 49].Text.ToString();
                            //asset.Employee = worksheet.Cells[i, 50].Text.ToString();
                            //asset.MaterialCode = worksheet.Cells[i, 51].Text.ToString();
                            //asset.Material = worksheet.Cells[i, 52].Text.ToString();
                            //asset.InterCompany = worksheet.Cells[i, 53].Text.ToString();

                            //asset.SubType = worksheet.Cells[i, 62].Text.ToString();

                            //asset.ERPCode = worksheet.Cells[i, 64].Text.ToString();
                            //asset.AssetClass = worksheet.Cells[i, 65].Text.ToString();
                            //asset.AdmCenter = worksheet.Cells[i, 66].Text.ToString();
                            //asset.Region = worksheet.Cells[i, 67].Text.ToString();
                            //asset.InsuranceCategory = worksheet.Cells[i, 68].Text.ToString();
                            //asset.AssetType = worksheet.Cells[i, 69].Text.ToString();
                            //asset.Project = worksheet.Cells[i, 70].Text.ToString();
                            */

                            Console.WriteLine(i);

                            assets.Add(asset);
                        }
                    }
                }
            }

            switch (operationType)
            {
                case "Import":
                    foreach (AssetImportEmag asset in assets)
                    {
                        ApplicationDbContext context = new ApplicationDbContext();
                        AssetsRepository repo = new AssetsRepository(context, null);

                        string invNo = await repo.AssetImportEmag(asset);
                        lineIndex++;

                        Console.WriteLine(lineIndex);
                    }
                    break;
                //case "Update":
                //    foreach (AssetImportEmag asset in assets)
                //    {
                //        ApplicationDbContext context = new ApplicationDbContext();
                //        AssetsRepository repo = new AssetsRepository(context, null);

                //        string invNo = await repo.AssetImportEmagUpdate(asset);
                //        lineIndex++;

                //        Console.WriteLine(lineIndex);
                //    }
                //    break;
            }

            return Ok(StatusCode(200));
        }

		[HttpGet("checkUnique/{invNo}")]
        public IActionResult CheckUniqueAsset(string invNo)
        {
            Model.Asset asset = null;
            asset =   _context.Set<Model.Asset>().Where(a => a.InvNo == invNo && a.IsDeleted == false).FirstOrDefault();
            
            if(asset != null)
            {
                return StatusCode(201);
            }
            else
            {
                return StatusCode(200);
            }
        }

        [HttpGet("checkUniqueSerialNumber/{serialNumber}")]
        public async Task<bool> CheckUniqueSerialNumber(string serialNumber)
        {
            Model.Asset asset = null;
            var found = false;

            if(serialNumber != "" && serialNumber != null && serialNumber.Trim().Length > 3)
			{
                asset = await _context.Set<Model.Asset>().Where(a => a.SerialNumber == serialNumber && a.IsDeleted == false).FirstOrDefaultAsync();
            }

           
            if (asset != null)
            {
                found = true;
                return found;
            }
            else
            {
                return found;
            }
        }

        [HttpGet("checkUniqueDocument/{document}")]
        public async Task<bool> CheckUniqueDocument(string document)
        {
            List<Model.Asset> assets = null;
            List<int?> res = null;
            var found = false;

            if (document != "" && document != null && document.Trim().Length > 3)
            {
                assets = await _context.Set<Model.Asset>().Include(a => a.Document).Where(a => a.Document.DocNo1.Equals(document) && a.IsDeleted == false).ToListAsync();


                res = assets
                   .GroupBy(c => new { c.Document.PartnerId, c.BudgetManagerId })
                   .Where(g => g.Count() > 1)
                   .Select(g => g.Key.PartnerId)
                   .ToList();
            }


            if (res.Count > 0)
            {
                found = true;
                return found;
            }
            else
            {
                return found;
            }
        }

        [HttpGet("checkUniqueDocumentByPartnerAndYear/{document}/{partnerId}/{budgetManagerId}")]
        public async Task<bool> CheckUniqueDocumentAndPartner(string document, int partnerId, int budgetManagerId)
        {
            List<Model.Asset> assets = null;
            List<int?> res = null;
            var found = false;

            if (document != "" && document != null && document.Trim().Length > 3)
            {
                assets = await _context.Set<Model.Asset>().Include(a => a.Document).Where(a => a.Document.DocNo1.Equals(document) && a.IsDeleted == false && a.Validated == true && a.Document.PartnerId == partnerId && a.BudgetManagerId == budgetManagerId).ToListAsync();


                res = assets
                   .GroupBy(c => new { c.Document.PartnerId, c.BudgetManagerId })
                   .Where(g => g.Count() > 1)
                   .Select(g => g.Key.PartnerId)
                   .ToList();
            }


            if (res.Count > 0)
            {
                found = true;
                return found;
            }
            else
            {
                return found;
            }
        }

        [HttpGet("getLastInvNo")]
        public async Task<string> GetLastInvNo()
        {
            Model.Uom uom = null;
			uom = _context.Set<Model.Uom>().Where(a => a.Code == "LastInvNo").FirstOrDefault();

            if (uom != null)
            {
                long lastInvNo = Convert.ToInt64(uom.Name.Substring(2));
                long newInvNo = lastInvNo + 1;

                //if(newInvNo.ToString().Length == 4)
                //{
                   
                //    return "T00" + newInvNo.ToString();
                //}
                //else if(newInvNo.ToString().Length == 5)
                //{
                //    return "T0" + newInvNo.ToString();
                //}
                //else
                //{
                //    return "T" + newInvNo.ToString();
                //}
                return newInvNo.ToString();
            }
            else
            {
                return "Eroare";
            }
        }

        [HttpPost("multiple/asset")]
        public int MultipleAssetDetail([FromBody]  Dto.MultipleAssetUpload multipleAssetUpload)
        {

            Model.Asset asset = null;
            Model.Asset assetBlank = null;

            Model.AssetCategory assetCategory = null;
            Model.CostCenter costCenter = null;
            Model.Location location = null;
            Model.Document document = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.AssetOp assetOp = null;
            Model.Inventory inventory = null;
            Model.Partner partner = null;
            Model.AdmCenter admCenter = null;
            Model.Employee employee = null;
            Model.AssetType assetType = null;
            Model.AssetState assetState = null;
            Model.AssetClass assetClass = null;
            Model.AssetClassType assetClassType = null;
            Model.Room room = null;
            Model.Administration administration = null;
            Model.Division division = null;
            Model.AssetAC assetAc = null;
            Model.AssetDep assetDep = null;
            Model.AssetInv assetInv = null;
            Model.AccSystem accSystem = null;
            Model.AccMonth accMonth = null;
            Model.AssetDepMD assetDepMD = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.Uom uom = null;
            Model.InvState invState = null;
            Model.DocumentType documentType = null;
            Model.Department department = null;
            Model.Company company = null;

            string assetClassTypeDefault = "RAS";
            string locationCode = string.Empty;
            string costCenterCode = string.Empty;
            string costCenterName = string.Empty;
            string invNo = string.Empty;
            string disposition = string.Empty;

            if (multipleAssetUpload.AssetTypeId != null)
            {
                assetType = _context.Set<Model.AssetType>().Where(a => (a.Id == multipleAssetUpload.AssetTypeId)).FirstOrDefault();
            }


            company = _context.Set<Model.Company>().Where(a => (a.Code == "-".Trim())).FirstOrDefault();
            accSystem = _context.Set<Model.AccSystem>().Where(a => (a.Code == assetClassTypeDefault.Trim())).FirstOrDefault();
            assetClassType = _context.Set<Model.AssetClassType>().Where(a => (a.Code == assetClassTypeDefault)).FirstOrDefault();
            assetClass = _context.Set<Model.AssetClass>().Where(a => (a.Code == assetClassTypeDefault.Trim())).FirstOrDefault();


            if (multipleAssetUpload.AssetStateId != null)
            {
                assetState = _context.Set<Model.AssetState>().Where(a => (a.Id == multipleAssetUpload.AssetStateId)).FirstOrDefault();
            }

            if (multipleAssetUpload.InvStateId != null)
            {
                invState = _context.Set<Model.InvState>().Where(a => (a.Id == multipleAssetUpload.InvStateId)).FirstOrDefault();
            }

            documentType = _context.Set<Model.DocumentType>().Where(a => a.Name.ToUpper() == "Inventariere".Trim()).FirstOrDefault();

            if (multipleAssetUpload.AssetCategoryId != null)
            {
                assetCategory = _context.Set<Model.AssetCategory>().Where(a => a.Id == multipleAssetUpload.AssetCategoryId).FirstOrDefault();
            }


            if (multipleAssetUpload.DepartmentId != null)
            {
                department = _context.Set<Model.Department>().Where(a => a.Id == multipleAssetUpload.DepartmentId).FirstOrDefault();
            }

            uom = _context.Set<Model.Uom>().Where(a => a.Code == "-".Trim() && a.Name == "-".Trim()).FirstOrDefault();


            costCenterCode = "-".Trim();
            costCenterName = "-".Trim();
            costCenter = _context.Set<Model.CostCenter>().Where(a => a.Code == costCenterCode).FirstOrDefault();

            if (multipleAssetUpload.RoomId != null)
            {
                room = _context.Set<Model.Room>().Where(r => r.Id == multipleAssetUpload.RoomId).FirstOrDefault();

                if (room != null)
                {
                    location = _context.Set<Model.Location>().Where(l => (l.Id == room.LocationId)).FirstOrDefault();

                    if (location != null)
                    {
                        admCenter = _context.Set<Model.AdmCenter>().Where(r => (r.Id == location.AdmCenterId)).FirstOrDefault();

                        if (admCenter != null)
                        {
                            employee = _context.Set<Model.Employee>().Where(r => (r.Id == admCenter.EmployeeId)).FirstOrDefault();
                        }
                    }
                }


            }


            division = _context.Set<Model.Division>().Where(a => (a.Code == "-")).FirstOrDefault();
            administration = _context.Set<Model.Administration>().Where(a => (a.Code == "-")).FirstOrDefault();
            partner = _context.Set<Model.Partner>().Where(a => a.Name == "-").FirstOrDefault();


            inventory = _context.Set<Model.Inventory>().Where(i => !i.IsDeleted && i.Active).OrderByDescending(i => i.Id).FirstOrDefault();

            for (int i = 0; i < multipleAssetUpload.Quantity; i++)
            {
                var invNoNew = 0;
                var invNew = "";
                if (i == 0)
                {

                    invNoNew = int.Parse(multipleAssetUpload.InvNoInitial);

                    switch (invNoNew.ToString().Length)
                    {
                        case 1:
                            invNew = "000000" + invNoNew.ToString();
                            break;
                        case 2:
                            invNew = "00000" + invNoNew.ToString();
                            break;
                        case 3:
                            invNew = "0000" + invNoNew.ToString();
                            break;
                        case 4:
                            invNew = "000" + invNoNew.ToString();
                            break;
                        case 5:
                            invNew = "00" + invNoNew.ToString();
                            break;
                        case 6:
                            invNew = "0" + invNoNew.ToString();
                            break;
                        default:
                            break;
                    }

                    assetBlank = _context.Set<Model.Asset>().Where(a => a.InvNo == invNoNew.ToString() && a.IsDeleted == false).FirstOrDefault();

                    if (assetBlank != null)
                    {
                        assetBlank.IsDeleted = true;
                        assetBlank.Validated = false;

                        _context.Update(assetBlank);

                    }
                }
                else
                {
                    invNoNew = int.Parse(multipleAssetUpload.InvNoInitial) + i;


                    switch (invNoNew.ToString().Length)
                    {
                        case 1:
                            invNew = "000000" + invNoNew.ToString();
                            break;
                        case 2:
                            invNew = "00000" + invNoNew.ToString();
                            break;
                        case 3:
                            invNew = "0000" + invNoNew.ToString();
                            break;
                        case 4:
                            invNew = "000" + invNoNew.ToString();
                            break;
                        case 5:
                            invNew = "00" + invNoNew.ToString();
                            break;
                        case 6:
                            invNew = "0" + invNoNew.ToString();
                            break;
                        default:
                            break;
                    }

                    assetBlank = _context.Set<Model.Asset>().Where(a => a.InvNo == invNoNew.ToString() && a.IsDeleted == false).FirstOrDefault();

                    if (assetBlank != null)
                    {
                        assetBlank.IsDeleted = true;
                        assetBlank.Validated = false;

                        _context.Update(assetBlank);

                    }
                }


                document = new Model.Document
                {
                    Approved = true,
                    DocumentType = documentType,
                    DocNo1 = string.Empty,
                    DocNo2 = string.Empty,
                    DocumentDate = DateTime.Now,
                    RegisterDate = DateTime.Now,
                    Partner = partner,
                    CreationDate = DateTime.Now

                };

                _context.Add(document);


                asset = new Model.Asset()
                {
                    InvNo = invNew,
                    Document = document,
                    Employee = employee,
                    Room = room,
                    AssetTypeId = assetType != null ? assetType.Id : 26,
                    PurchaseDate = null,
                    Quantity = 1,
                    Validated = true,
                    InvStateId = invState != null ? invState.Id : 20,
                    Administration = administration,
                    CostCenter = costCenter,
                    AssetStateId = assetState != null ? assetState.Id : 20,
                    Uom = uom,
                    Department = department,
                    ERPCode = string.Empty,
                    Custody = false,
                    Company = company,
                    SAPCode = string.Empty,
                    SerialNumber = multipleAssetUpload.SerialNumber


                };


                _context.Add(asset);

                assetOp = new Model.AssetOp()
                {
                    Asset = asset,
                    Document = document,
                    RoomInitial = room,
                    EmployeeInitial = employee,
                    CostCenterInitial = costCenter,
                    AssetCategoryInitial = assetCategory,
                    InvStateIdInitial = invState != null ? invState.Id : 20,
                    InvStateIdFinal = null,
                    AdministrationInitial = administration,
                    AccSystem = accSystem,
                    Info = multipleAssetUpload.Info.Trim(),
                    AssetStateInitial = assetState,
                    DepartmentIdInitial = department != null ? department.Id : 3,
                    AllowLabel = true
                    


                };

                _context.Add(assetOp);

                inventory = _context.Set<Model.Inventory>().OrderByDescending(j => j.Id).FirstOrDefault();

                if (inventory != null)
                {

                    inventoryAsset = new Model.InventoryAsset()
                    {
                        Asset = asset,
                        Inventory = inventory,
                        QInitial = 1,
                        QFinal = 0,
                        EmployeeInitial = employee,
                        RoomInitial = room,
                        CostCenterInitial = costCenter,
                        StateIdInitial = assetState != null ? invState.Id : 20,
                        StateIdFinal = null,
                        SerialNumber = multipleAssetUpload.SerialNumber,
                        Info = multipleAssetUpload.Info.Trim(),
                        AdministrationInitial = administration

                    };

                    _context.Add(inventoryAsset);


                    assetAc = new Model.AssetAC
                    {
                        AssetClassType = assetClassType,
                        Asset = asset,
                        AssetClass = assetClass,
                        AssetClassIn = assetClass
                    };


                    _context.Set<Model.AssetAC>().Add(assetAc);

                    var monthSum = 1;


                    assetDep = new Model.AssetDep
                    {
                        AccSystem = accSystem,
                        Asset = asset,
                        DepPeriod = (int)monthSum,
                        DepPeriodIn = (int)monthSum,
                        DepPeriodMonth = 1,
                        DepPeriodMonthIn = 1,
                        DepPeriodRem = 1,
                        DepPeriodRemIn = 1,
                        UsageStartDate = null,
                        ValueDep = multipleAssetUpload.ValueInv,
                        ValueDepIn = multipleAssetUpload.ValueInv,
                        ValueDepPU = multipleAssetUpload.ValueInv,
                        ValueDepPUIn = multipleAssetUpload.ValueInv,
                        ValueDepYTD = multipleAssetUpload.ValueInv,
                        ValueDepYTDIn = multipleAssetUpload.ValueInv,
                        ValueInv = multipleAssetUpload.ValueInv,
                        ValueInvIn = multipleAssetUpload.ValueInv,
                        ValueRem = multipleAssetUpload.ValueInv,
                        ValueRemIn = multipleAssetUpload.ValueInv
                    };

                    _context.Set<Model.AssetDep>().Add(assetDep);


                    assetInv = new Model.AssetInv
                    {
                        Asset = asset,
                        AllowLabel = true,
                        Barcode = multipleAssetUpload.InvNoInitial,
                        Info = multipleAssetUpload.Info.Trim(),
                        InvName = multipleAssetUpload.Name,
                        InvNoOld = string.Empty,
                        Model = assetType != null ? assetType.Name.Trim() : "",
                        Producer = multipleAssetUpload.Producer.Trim(),
                        InvState = invState

                    };

                    _context.Set<Model.AssetInv>().Add(assetInv);

                    accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).FirstOrDefault();

                    assetDepMD = new Model.AssetDepMD
                    {
                        AccMonth = accMonth,
                        AccSystem = accSystem,
                        Asset = asset,
                        UsefulLife = 1,
                        TotLifeInpPeriods = 1,
                        RemLifeInPeriods = 1,
                        AccumulDep = 1,
                        BkValFYStart = 1,
                        DepForYear = 1,
                        CurrentAPC = multipleAssetUpload.ValueInv,
                        PosCap = multipleAssetUpload.ValueInv
                    };

                    _context.Set<Model.AssetDepMD>().Add(assetDepMD);

                    assetAdmMD = new Model.AssetAdmMD
                    {
                        AccMonth = accMonth,
                        Asset = asset,
                        Administration = administration,
                        AssetCategory = assetCategory,
                        AssetState = assetState,
                        AssetType = assetType,
                        CostCenter = costCenter,
                        Department = department,
                        Employee = employee,
                        Room = room,
                        AssetStateId = assetState != null ? assetState.Id : 20,
                    };

                    _context.Set<Model.AssetAdmMD>().Add(assetAdmMD);

                }

                asset.Name = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(multipleAssetUpload.Name));
                asset.CostCenter = costCenter;
                asset.AssetCategory = assetCategory;
                asset.AssetType = assetType;
                asset.ValueInv = multipleAssetUpload.ValueInv;
                asset.ValueRem = multipleAssetUpload.ValueInv;
                asset.PurchaseDate = multipleAssetUpload.PurchaseDate;
                asset.SerialNumber = multipleAssetUpload.SerialNumber;
                asset.Company = company;

                _context.SaveChanges();



            }

            return asset.Id;

        }

        //[HttpPost]
        //[Route("filters/reco")]
        //public virtual IActionResult SaveReco([FromBody] Dto.AssetRecoSave reco)
        //{
        //    Model.Asset assetReco = null;
        //    Model.Asset asset = null;
        //    Model.AssetOp assetOp = null;
        //    Model.Document document = null;
        //    Model.AccMonth accMonth = null;
        //    Model.AssetAdmMD assetAdmMD = null;
        //    Model.AssetDepMD assetDepMD = null;
        //    Model.AssetDep assetDep = null;

        //    string documentTypeCode = "RECONCILIATION";

        //    accMonth = _context.Set<Model.AccMonth>().Where(a => a.IsActive == true).SingleOrDefault();

        //    assetAdmMD = _context.Set<Model.AssetAdmMD>().Where(a => a.AccMonthId == accMonth.Id && a.AssetId == reco.AssetId).SingleOrDefault();
        //    assetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AccMonthId == accMonth.Id && a.AssetId == reco.AssetId).SingleOrDefault();
        //    assetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == reco.AssetId).SingleOrDefault();

        //    assetReco = _context.Set<Model.Asset>().SingleOrDefault(a => a.Id == reco.AssetRecoId);
        //    asset = _context.Set<Model.Asset>().Where(a => a.Id == reco.AssetId).SingleOrDefault();

        //    var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

        //    document = new Model.Document();
        //    document.Approved = true;
        //    document.DocumentType = documentType;
        //    document.DocNo1 = string.Empty;
        //    document.DocNo2 = string.Empty;
        //    document.DocumentDate = DateTime.Now;
        //    document.RegisterDate = DateTime.Now;
        //    document.Partner = null;

        //    _context.Add(document);

        //    assetOp = new Model.AssetOp()
        //    {
        //        Asset = asset,
        //        DocumentId = document.Id,
        //        RoomIdInitial = asset.RoomId,
        //        RoomIdFinal = assetReco.RoomId,
        //        EmployeeIdInitial = asset.EmployeeId,
        //        EmployeeIdFinal = assetReco.EmployeeId,
        //        CostCenterIdInitial = asset.CostCenterId,
        //        CostCenterIdFinal = asset.CostCenterId,
        //        AssetCategoryIdInitial = asset.AssetCategoryId,
        //        AssetCategoryIdFinal = asset.AssetCategoryId,
        //        InvStateIdInitial = asset.InvStateId,
        //        InvStateIdFinal = assetReco.InvStateId,
        //        AdministrationIdInitial = asset.AdministrationId,
        //        AdministrationIdFinal = asset.AdministrationId,
        //        AccSystemId = 1
        //    };

        //    _context.Add(assetOp);

        //    assetReco.AdministrationId = asset.AdministrationId;
        //    assetReco.AssetCategoryId = asset.AssetCategoryId;
        //    assetReco.AssetStateId = asset.AssetStateId;
        //    assetReco.AssetTypeId = asset.AssetTypeId;
        //    assetReco.CompanyId = asset.CompanyId;
        //    assetReco.CostCenterId = asset.CostCenterId;
        //    assetReco.DepartmentId = asset.DepartmentId;
        //    assetReco.DocumentId = asset.DocumentId;
        //    assetReco.ERPCode = asset.ERPCode;
        //    assetReco.EmployeeId = asset.EmployeeId;
        //    assetReco.InvNo = asset.InvNo;
        //    assetReco.InvStateId = asset.InvStateId;
        //    assetReco.IsDeleted = asset.IsDeleted;
        //    assetReco.Name = asset.Name;
        //    assetReco.PurchaseDate = asset.PurchaseDate;
        //    assetReco.Quantity = asset.Quantity;
        //    assetReco.RoomId = asset.RoomId;
        //    assetReco.SerialNumber = asset.SerialNumber;
        //    assetReco.UomId = asset.UomId;
        //    assetReco.Validated = asset.Validated;
        //    assetReco.ValueInv = asset.ValueInv;
        //    assetReco.ValueRem = asset.ValueRem;
        //    assetReco.ArticleId = asset.ArticleId;
        //    assetReco.BudgetManagerId = asset.BudgetManagerId;
        //    assetReco.AssetNatureId = asset.AssetNatureId;
        //    assetReco.AccountId = asset.AccountId;
        //    assetReco.ExpAccountId = asset.ExpAccountId;

        //    assetReco.TempReco = asset.TempReco;
        //    assetReco.TempName = asset.TempName;
        //    assetReco.SAPCode = asset.SAPCode;
        //    assetReco.IsInTransfer = asset.IsInTransfer;
        //    assetReco.SubTypeId = asset.SubTypeId;
        //    assetReco.InsuranceCategoryId = asset.InsuranceCategoryId;
        //    assetReco.BrandId = asset.BrandId;
        //    assetReco.ModelId = asset.ModelId;
        //    assetReco.InterCompanyId = asset.InterCompanyId;
        //    assetReco.ProjectId = asset.ProjectId;
        //    assetReco.InvoiceDate = asset.InvoiceDate;
        //    assetReco.PODate = asset.PODate;

        //    assetReco.ReceptionDate = asset.ReceptionDate;
        //    assetReco.RemovalDate = asset.RemovalDate;

        //    _context.Update(assetReco);


        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AssetCategoryId = asset.AssetCategoryId;
        //    //assetAdmMD.AssetStateId = asset.AssetStateId;
        //    //assetAdmMD.AssetTypeId = asset.AssetTypeId;
        //    //assetAdmMD.CostCenterId = asset.CostCenterId;
        //    //assetAdmMD.DepartmentId = asset.DepartmentId;
        //    //assetAdmMD.EmployeeId = asset.EmployeeId;
        //    //assetAdmMD.RoomId = asset.RoomId;
        //    //assetAdmMD.AssetClassId = asset.;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;
        //    //assetAdmMD.AdministrationId = asset.AdministrationId;

        //    _context.SaveChanges();

        //    return Ok(reco);
        //}

        [HttpGet("exportInventoryEmag")]
        public IActionResult ExportSocGen(int? page, int? pageSize, string sortColumn, string sortDirection, string reportType, string assetState,
            int inventoryId, string includes, string filter,
            string assetCategoryIds, string assetTypeIds, string partnerIds,
            string administrationIdsIni, string administrationIdsFin,
            string divisionIdsIni, string divisionIdsFin,
            string invStateIdsIni, string invStateIdsFin, string invStateIdsAll, string userIds,
            string regionIdsIni, string costCenterIdsIni, string admCenterIdsIni, string departmentIdsIni, string employeeIdsIni, string locationIdsIni, string roomIdsIni,
            string regionIdsFin, string costCenterIdsFin, string admCenterIdsFin, string departmentIdsFin, string employeeIdsFin, string locationIdsFin, string roomIdsFin,
            string admCenterIdsAll, string employeeIdsAll, string locationIdsAll, string regionIdsAll, string costCenterIdsAll, string divisionIdsAll, string departmentIdsAll, string administrationIdsAll, string roomIdsAll, bool? custody, bool? reconcile)
        {
            int totalItems = 0;
            string userName = string.Empty;
            string role = string.Empty;
            string employeeId = string.Empty;

            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (employeeId != null)
                {
                    employeeIdsAll = "[" + employeeId + "]";
                }
                else
                {
                    employeeIdsAll = "[" + 100000000 + "]";
                }
            }
            else
            {
                employeeIdsAll = "[" + 100000000 + "]";
            }

            List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).GetInventoryAssetsByFilters2(
               inventoryId, includes, filter, reportType, assetState, custody, reconcile, true, userName, role, employeeId,
               assetCategoryIds.TryToIntArray(), assetTypeIds.TryToIntArray(), partnerIds.TryToIntArray(), administrationIdsIni.TryToIntArray(), administrationIdsFin.TryToIntArray(), divisionIdsIni.TryToIntArray(), divisionIdsFin.TryToIntArray(),
               invStateIdsIni.TryToIntArray(), invStateIdsFin.TryToIntArray(), invStateIdsAll.TryToIntArray(), userIds.TryToStringtArray(),
               regionIdsIni.TryToIntArray(), costCenterIdsIni.TryToIntArray(), admCenterIdsIni.TryToIntArray(), departmentIdsIni.TryToIntArray(),
               employeeIdsIni.TryToIntArray(), locationIdsIni.TryToIntArray(), roomIdsIni.TryToIntArray(),
               regionIdsFin.TryToIntArray(), costCenterIdsFin.TryToIntArray(), admCenterIdsFin.TryToIntArray(), departmentIdsFin.TryToIntArray(),
               employeeIdsFin.TryToIntArray(), locationIdsFin.TryToIntArray(), roomIdsFin.TryToIntArray(), admCenterIdsAll.TryToIntArray(), employeeIdsAll.TryToIntArray(), locationIdsAll.TryToIntArray(), regionIdsAll.TryToIntArray(), costCenterIdsAll.TryToIntArray(), divisionIdsAll.TryToIntArray(), departmentIdsAll.TryToIntArray(), administrationIdsAll.TryToIntArray(), roomIdsAll.TryToIntArray(),
               sortColumn, sortDirection, page, pageSize, out totalItems).ToList();


            if (items.Count > 0)
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    string sheetName = "Rezultat inventar";

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                    int recordIndex = 4;
                    int rowNumber = 0;
                    int count = items.Count();
                    Model.Employee employee = null;
                    foreach (var item in items)
                    {
                       
                        rowNumber++;
                        int diff = recordIndex - count;

                        if (diff > 0)
                        {
                            diff = 0;
                        }

                        var user = item.ModifiedByUser;
                        if (user != null)
                        {
                           var empUser = item.ModifiedByUser.EmployeeId.GetValueOrDefault();
                            employee = this._context.Set<Model.Employee>().Where(u => u.Id == empUser).FirstOrDefault();
                        }

                        worksheet.Cells[3, 1].Value = "1";
                        worksheet.Cells[3, 2].Value = "2";
                        worksheet.Cells[3, 3].Value = "3";
                        worksheet.Cells[3, 4].Value = "4";
                        worksheet.Cells[3, 5].Value = "5";
                        worksheet.Cells[3, 6].Value = "6";
                        worksheet.Cells[3, 7].Value = "7";
                        worksheet.Cells[3, 8].Value = "8";
                        worksheet.Cells[3, 9].Value = "9";
                        worksheet.Cells[3, 10].Value = "10";
                        worksheet.Cells[3, 11].Value = "11";
                        worksheet.Cells[3, 12].Value = "12";
                        worksheet.Cells[3, 13].Value = "13";
                        worksheet.Cells[3, 14].Value = "14";
                        worksheet.Cells[3, 15].Value = "15";
                        worksheet.Cells[3, 16].Value = "16";
                        worksheet.Cells[3, 17].Value = "17";
                        worksheet.Cells[3, 18].Value = "18";
                        worksheet.Cells[3, 19].Value = "19";
                        worksheet.Cells[3, 20].Value = "20";
                        worksheet.Cells[3, 21].Value = "21";
                        worksheet.Cells[3, 22].Value = "22";
                        worksheet.Cells[3, 23].Value = "23";
                        worksheet.Cells[3, 24].Value = "24";
                        worksheet.Cells[3, 25].Value = "25";
                        worksheet.Cells[3, 26].Value = "26";
                        worksheet.Cells[3, 27].Value = "27";
                        worksheet.Cells[3, 28].Value = "28";
                        worksheet.Cells[3, 29].Value = "29";
                        worksheet.Cells[3, 30].Value = "30";
                        worksheet.Cells[3, 31].Value = "31";
                        worksheet.Cells[3, 32].Value = "32";
                        worksheet.Cells[3, 33].Value = "33";
                        worksheet.Cells[3, 34].Value = "34";
                        worksheet.Cells[3, 35].Value = "35";
                        worksheet.Cells[3, 36].Value = "36";
                        worksheet.Cells[3, 37].Value = "37";
                        worksheet.Cells[3, 38].Value = "38";
                        worksheet.Cells[3, 39].Value = "39";
                        worksheet.Cells[3, 40].Value = "40";
                        worksheet.Cells[3, 41].Value = "41";
                        //                  worksheet.Cells[3, 42].Value = "42";
                        //                  worksheet.Cells[3, 43].Value = "43";
                        //                  worksheet.Cells[3, 44].Value = "44";
                        //                  worksheet.Cells[3, 45].Value = "45";
                        //worksheet.Cells[3, 46].Value = "46";

                        worksheet.Cells[recordIndex, 1].Value = rowNumber;
                        worksheet.Cells[recordIndex, 2].Value = item.Asset.InvNo;
                        worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
                        worksheet.Cells[recordIndex, 4].Value = item.QInitial;
                        worksheet.Cells[recordIndex, 5].Value = item.Asset.Name;
                        worksheet.Cells[recordIndex, 6].Value = item.Asset.SAPCode;
                        worksheet.Cells[recordIndex, 7].Value = item.SNInitial;
                        worksheet.Cells[recordIndex, 8].Value = item.Asset.PurchaseDate;
						worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "mm/dd/yyyy";
						worksheet.Cells[recordIndex, 9].Value = item.Asset.InvoiceDate;
						worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "mm/dd/yyyy";
                        worksheet.Cells[recordIndex, 10].Value = item.EmployeeInitial != null ? item.EmployeeInitial.InternalCode : "";
                        worksheet.Cells[recordIndex, 11].Value = item.SerialNumber;
                        worksheet.Cells[recordIndex, 12].Value = item.Info;
                        worksheet.Cells[recordIndex, 13].Value = item.TempReco;
                        worksheet.Cells[recordIndex, 14].Value = item.TempName;
                        worksheet.Cells[recordIndex, 15].Value = item.QInitial;
                        worksheet.Cells[recordIndex, 16].Value = item.QFinal;
                        worksheet.Cells[recordIndex, 17].Value = (item.QInitial > 0 && ((item.QInitial == item.QFinal) || (item.QFinal > item.QInitial))) ? 0 : (item.QInitial - item.QFinal) < 0 ? 0 : item.QInitial - item.QFinal;
                        worksheet.Cells[recordIndex, 18].Value = (item.QInitial > 0 && item.QFinal > 0 && item.QFinal > item.QInitial) ? item.QFinal - item.QInitial : item.Asset.InvNo.Substring(0, 1) == "T" ? item.QFinal - item.QInitial : 0;
                        worksheet.Cells[recordIndex, 19].Value = item.Asset.ERPCode;
                        worksheet.Cells[recordIndex, 20].Value = item.Asset.AllowLabel == true ? "DA" : "NU";
                        worksheet.Cells[recordIndex, 21].Value = item.StateInitial != null ? item.StateInitial.Name : "";

                        worksheet.Cells[recordIndex, 22].Value = item.CostCenterInitial != null && item.CostCenterInitial.AdmCenter != null ? item.CostCenterInitial.AdmCenter.Code : "";
                        worksheet.Cells[recordIndex, 23].Value = item.CostCenterInitial != null ? item.CostCenterInitial.Code : "";
                        worksheet.Cells[recordIndex, 24].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null ? item.CostCenterInitial.Division.Name : "";
                        worksheet.Cells[recordIndex, 25].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null && item.CostCenterInitial.Division.Department != null ? item.CostCenterInitial.Division.Department.Name : "";

                        worksheet.Cells[recordIndex, 26].Value = item.CostCenterFinal != null && item.CostCenterFinal.AdmCenter != null ? item.CostCenterFinal.AdmCenter.Code : "";
						worksheet.Cells[recordIndex, 27].Value = item.CostCenterFinal != null ? item.CostCenterFinal.Code : "";
						worksheet.Cells[recordIndex, 28].Value = item.CostCenterFinal != null && item.CostCenterFinal.Division != null ? item.CostCenterFinal.Division.Name : "";
						worksheet.Cells[recordIndex, 29].Value = item.CostCenterFinal != null && item.CostCenterFinal.Division != null && item.CostCenterFinal.Division.Department != null ? item.CostCenterFinal.Division.Department.Name : "";
						worksheet.Cells[recordIndex, 30].Value = item.StateFinal != null ? item.StateFinal.Name : "";

						worksheet.Cells[recordIndex, 31].Value = item.ModifiedByUser != null ? item.ModifiedByUser.Email : string.Empty;
						worksheet.Cells[recordIndex, 32].Value = item.ModifiedAt;
						worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "mm/dd/yyyy";
						worksheet.Cells[recordIndex, 33].Value = item.CurrBkValue;
						worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[recordIndex, 34].Value = item.AccumulDep;
						worksheet.Cells[recordIndex, 34].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[recordIndex, 35].Value = item.CurrentAPC;
						worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, 36].Value = item.AdministrationInitial != null ? item.AdministrationInitial.Name : "";
                        worksheet.Cells[recordIndex, 37].Value = item.AdministrationFinal != null ? item.AdministrationFinal.Name : "";
                        worksheet.Cells[recordIndex, 38].Value = item.Asset.AssetCategory != null && item.Asset.AssetCategory.Prefix != null && item.Asset.AssetCategory.Prefix != "" ? item.Asset.AssetCategory.Prefix : "";
						worksheet.Cells[recordIndex, 39].Value = employee != null ? employee.FirstName : string.Empty;
						worksheet.Cells[recordIndex, 40].Value = employee != null ? employee.LastName : string.Empty;
                        worksheet.Cells[recordIndex, 41].Value = employee != null ? employee.Email : string.Empty;

                        if (diff == 0)
                        {



                            for (int i = 1; i < 42; i++)
                            {
                                worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                                worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                            }

                            worksheet.Row(1).Height = 35.00;
                            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Row(2).Height = 35.00;
                            worksheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Row(3).Height = 15.00;
                            worksheet.Row(3).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.View.FreezePanes(4, 1);

                            using (var cells = worksheet.Cells[1, 1, items.Count() + 3, 41])
                            {
                                cells.Style.Font.Bold = false;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                            }




                            worksheet.Cells["A1:A2"].Merge = true;
                            worksheet.Cells["A1:A2"].Value = "Nr. Crt";
                            worksheet.Cells["B1:B2"].Merge = true;
                            worksheet.Cells["B1:B2"].Value = "Asset";
                            worksheet.Cells["C1:C2"].Merge = true;
                            worksheet.Cells["C1:C2"].Value = "SubNo";
                            worksheet.Cells["D1:D2"].Merge = true;
                            worksheet.Cells["D1:D2"].Value = "PCS";
                            worksheet.Cells["E1:E2"].Merge = true;
                            worksheet.Cells["E1:E2"].Value = "Descriere";
                            worksheet.Cells["F1:F2"].Merge = true;
                            worksheet.Cells["F1:F2"].Value = "Inventory number";
                            worksheet.Cells["G1:G2"].Merge = true;
                            worksheet.Cells["G1:G2"].Value = "Serial number initial";
                            worksheet.Cells["H1:H2"].Merge = true;
                            worksheet.Cells["H1:H2"].Value = "Capitalized on";
                            worksheet.Cells["I1:I2"].Merge = true;
                            worksheet.Cells["I1:I2"].Value = "Ord.dep.start date";
                            worksheet.Cells["J1:J2"].Merge = true;
                            worksheet.Cells["J1:J2"].Value = "Marca P.";
                            worksheet.Cells["K1:K2"].Merge = true;
                            worksheet.Cells["K1:K2"].Value = "Serie inventar";
                            worksheet.Cells["L1:L2"].Merge = true;
                            worksheet.Cells["L1:L2"].Value = "Observatii inventar";
                            worksheet.Cells["M1:M2"].Merge = true;
                            worksheet.Cells["M1:M2"].Value = "Reconciliat";
                            worksheet.Cells["N1:N2"].Merge = true;
                            worksheet.Cells["N1:N2"].Value = "Reconciliat Denumire";
                            worksheet.Cells["O1:O2"].Merge = true;
                            worksheet.Cells["O1:O2"].Value = "Scriptic";
                            worksheet.Cells["P1:P2"].Merge = true;
                            worksheet.Cells["P1:P2"].Value = "Faptic";
                            worksheet.Cells["Q1:Q2"].Merge = true;
                            worksheet.Cells["Q1:Q2"].Value = "Minus";
                            worksheet.Cells["R1:R2"].Merge = true;
                            worksheet.Cells["R1:R2"].Value = "Plus";
                            worksheet.Cells["S1:S2"].Merge = true;
                            worksheet.Cells["S1:S2"].Value = "FAR";
                            worksheet.Cells["T1:T2"].Merge = true;
                            worksheet.Cells["T1:T2"].Value = "Etichetabil?";
                            worksheet.Cells["U1:U2"].Merge = true;
                            worksheet.Cells["U1:U2"].Value = "Stare initial";
                            worksheet.Cells["V1:V2"].Merge = true;
                            worksheet.Cells["V1:V2"].Value = "PC Initial";
                            worksheet.Cells["W1:W2"].Merge = true;
                            worksheet.Cells["W1:W2"].Value = "CC initial";
                            worksheet.Cells["X1:X2"].Merge = true;
                            worksheet.Cells["X1:X2"].Value = "Departament initial";
                            worksheet.Cells["Y1:Y2"].Merge = true;
                            worksheet.Cells["Y1:Y2"].Value = "B.U. initial";
                            worksheet.Cells["Z1:Z2"].Merge = true;
                            worksheet.Cells["Z1:Z2"].Value = "PC inventar";
                            worksheet.Cells["AA1:AA2"].Merge = true;
                            worksheet.Cells["AA1:AA2"].Value = "CC inventar";

                            worksheet.Cells["AB1:AB2"].Merge = true;
                            worksheet.Cells["AB1:AB2"].Value = "Departament inventar";

                            worksheet.Cells["AC1:AC2"].Merge = true;
                            worksheet.Cells["AC1:AC2"].Value = "B.U. inventar";

                            worksheet.Cells["AD1:AD2"].Merge = true;
                            worksheet.Cells["AD1:AD2"].Value = "Stare inventar";

                            worksheet.Cells["AE1:AE2"].Merge = true;
                            worksheet.Cells["AE1:AE2"].Value = "Utilizator";
                            worksheet.Cells["AE1:AE2"].Style.WrapText = true;

                            worksheet.Cells["AF1:AF2"].Merge = true;
                            worksheet.Cells["AF1:AF2"].Value = "Data scanare";

                            worksheet.Cells["AG1:AG2"].Merge = true;
                            worksheet.Cells["AG1:AG2"].Value = "CurrBkValue";

                            worksheet.Cells["AH1:AH2"].Merge = true;
                            worksheet.Cells["AH1:AH2"].Value = "AccumulDep";

                            worksheet.Cells["AI1:AI2"].Merge = true;
                            worksheet.Cells["AI1:AI2"].Value = "CurrentAPC ";


                            worksheet.Cells["AJ1:AJ2"].Merge = true;
                            worksheet.Cells["AJ1:AJ2"].Value = "Locatie initial";

                            worksheet.Cells["AK1:AK2"].Merge = true;
                            worksheet.Cells["AK1:AK2"].Value = "Locatie inventar";

                            worksheet.Cells["AL1:AL2"].Merge = true;
                            worksheet.Cells["AL1:AL2"].Value = "IT/Facility";

                            worksheet.Cells["AM1:AM2"].Merge = true;
                            worksheet.Cells["AM1:AM2"].Value = "Angajat - nume inventar";

                            worksheet.Cells["AN1:AN2"].Merge = true;
                            worksheet.Cells["AN1:AN2"].Value = "Angajat - prenume inventar";

                            worksheet.Cells["AO1:AO2"].Merge = true;
                            worksheet.Cells["AO1:AO2"].Value = "Angajat - email inventar";

							//worksheet.Cells["AP1:AP2"].Merge = true;
							//worksheet.Cells["AP1:AP2"].Value = "";

							//worksheet.Cells["AQ1:AQ2"].Merge = true;
							//worksheet.Cells["AQ1:AQ2"].Value = "Type";

							//worksheet.Cells["AR1:AR2"].Merge = true;
							//worksheet.Cells["AR1:AR2"].Value = "WBS";

							//worksheet.Cells["AS1:AS2"].Merge = true;
							//worksheet.Cells["AS1:AS2"].Value = "";

							//worksheet.Cells["AT1:AT2"].Merge = true;
							//worksheet.Cells["AT1:AT2"].Value = "";

							//worksheet.Cells["AU1:AU2"].Merge = true;
							//worksheet.Cells["AU1:AU2"].Value = "Poze 3";


							using (var cells = worksheet.Cells[3, 1, 3, 41])
                            {
                                cells.Style.Font.Bold = true;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                                cells.Style.Font.Color.SetColor(Color.White);
                            }

                            using (var cells = worksheet.Cells[1, 1, 2, 41])
                            {
                                cells.Style.Font.Bold = true;
                            }

                            using (var cells = worksheet.Cells[4, 1, items.Count() + 5, 41])
                            {
                                cells.Style.Font.Bold = false;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                            }

                            using (var cells = worksheet.Cells[3, 1, items.Count() + 5, 41])
                            {
                                for (int i = 4; i < items.Count() + 4; i++)
                                {
                                    worksheet.Row(i).Height = 20.00;
                                    worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"I{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"J{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"K{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"L{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"M{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"N{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"O{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"P{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"Q{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"R{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"S{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"T{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"U{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"V{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"W{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"W{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"X{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"Y{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"Z{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AA{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AB{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AC{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AD{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AE{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AF{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AG{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AH{i}"].Style.WrapText = true;
									worksheet.Cells[$"AI{i}"].Style.WrapText = true;
									worksheet.Cells[$"AJ{i}"].Style.WrapText = true;
									worksheet.Cells[$"AK{i}"].Style.WrapText = true;
									worksheet.Cells[$"AL{i}"].Style.WrapText = true;
                                    worksheet.Cells[$"AM{i}"].Style.WrapText = true;
									worksheet.Cells[$"AN{i}"].Style.WrapText = true;
									worksheet.Cells[$"AO{i}"].Style.WrapText = true;
								}
                            }


                            worksheet.View.ShowGridLines = false;
                            worksheet.View.ZoomScale = 100;

                            for (int i = 1; i < 42; i++)
                            {
                                worksheet.Column(i).AutoFit();
                            }

                            worksheet.Column(1).Width = 15.00;
                            worksheet.Column(2).Width = 15.00;
                            worksheet.Column(3).Width = 10.00;
                            worksheet.Column(4).Width = 10.00;
                            worksheet.Column(5).Width = 60.00;
                            worksheet.Column(6).Width = 15.00;
                            worksheet.Column(7).Width = 20.00;
                            worksheet.Column(8).Width = 15.00;
                            worksheet.Column(9).Width = 15.00;
                            worksheet.Column(10).Width = 20.00;
                            worksheet.Column(11).Width = 15.00;
                            worksheet.Column(12).Width = 20.00;
                            worksheet.Column(13).Width = 20.00;
                            worksheet.Column(14).Width = 20.00;
                            worksheet.Column(15).Width = 15.00;
                            worksheet.Column(16).Width = 15.00;
                            worksheet.Column(17).Width = 15.00;
                            worksheet.Column(18).Width = 40.00;
                            worksheet.Column(19).Width = 40.00;
                            worksheet.Column(20).Width = 10.00;
                            worksheet.Column(21).Width = 60.00;
                            worksheet.Column(22).Width = 20.00;
                            worksheet.Column(23).Width = 20.00;
                            worksheet.Column(24).Width = 20.00;
                            worksheet.Column(25).Width = 20.00;
                            worksheet.Column(26).Width = 10.00;
                            worksheet.Column(27).Width = 10.00;
                            worksheet.Column(28).Width = 10.00;
                            worksheet.Column(29).Width = 10.00;
                            worksheet.Column(30).Width = 20.00;
                            worksheet.Column(31).Width = 20.00;
                            worksheet.Column(32).Width = 20.00;
                            worksheet.Column(33).Width = 20.00;
                            worksheet.Column(34).Width = 20.00;
                            worksheet.Column(35).Width = 20.00;
                            worksheet.Column(36).Width = 20.00;
                            worksheet.Column(37).Width = 20.00;
                            worksheet.Column(38).Width = 20.00;
							worksheet.Column(39).Width = 20.00;
							worksheet.Column(40).Width = 20.00;
							worksheet.Column(41).Width = 20.00;
							//worksheet.Column(39).Width = 20.00;
							//                     worksheet.Column(40).Width = 20.00;
							//                     worksheet.Column(41).Width = 20.00;
							//                     worksheet.Column(42).Width = 20.00;
							//                     worksheet.Column(44).Width = 20.00;
							//                     worksheet.Column(45).Width = 20.00;
							//worksheet.Column(46).Width = 20.00;
						}


                        //  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
                        //  worksheet.Column(13).Style.Numberformat.Format = "yyyy-mm-dd";
                        //  worksheet.Column(14).Style.Numberformat.Format = "yyyy-MM-dd";


                        //  worksheet.Cells.AutoFitColumns();


                        recordIndex++;
                    }




                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //HttpContext.Response.ContentType = entityFile.FileType;
                    HttpContext.Response.ContentType = contentType;
                    FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                    {
                        FileDownloadName = "Rezultat inventar.xlsx"
                    };

                    return result;

                }
            }

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nu exista scanari");

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Rezultat inventar.xlsx"
                };

                return result;

            }
        }

		[HttpGet("exportAllInventoryEmag")]
		public IActionResult ExportAllSocGen(int? page, int? pageSize, string sortColumn, string sortDirection, string reportType, string assetState,
			int inventoryId, string includes, string filter,
			string assetCategoryIds, string assetTypeIds, string partnerIds,
			string administrationIdsIni, string administrationIdsFin,
			string divisionIdsIni, string divisionIdsFin,
			string invStateIdsIni, string invStateIdsFin, string invStateIdsAll, string userIds,
			string regionIdsIni, string costCenterIdsIni, string admCenterIdsIni, string departmentIdsIni, string employeeIdsIni, string locationIdsIni, string roomIdsIni,
			string regionIdsFin, string costCenterIdsFin, string admCenterIdsFin, string departmentIdsFin, string employeeIdsFin, string locationIdsFin, string roomIdsFin,
			string admCenterIdsAll, string employeeIdsAll, string locationIdsAll, string regionIdsAll, string costCenterIdsAll, string divisionIdsAll, string departmentIdsAll, string administrationIdsAll, string roomIdsAll, bool? custody, bool? reconcile)
		{
			int totalItems = 0;
			string userName = string.Empty;
			string role = string.Empty;
			string employeeId = string.Empty;


			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				if (employeeId != null)
				{
					employeeIdsAll = "[" + employeeId + "]";
				}
				else
				{
					employeeIdsAll = "[" + 100000000 + "]";
				}
			}
			else
			{
				employeeIdsAll = "[" + 100000000 + "]";
			}

			List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).GetInventoryAllAssetsByFilters2(
			   inventoryId, includes, filter, reportType, assetState, custody, reconcile, true, userName, role, employeeId,
			   assetCategoryIds.TryToIntArray(), assetTypeIds.TryToIntArray(), partnerIds.TryToIntArray(), administrationIdsIni.TryToIntArray(), administrationIdsFin.TryToIntArray(), divisionIdsIni.TryToIntArray(), divisionIdsFin.TryToIntArray(),
			   invStateIdsIni.TryToIntArray(), invStateIdsFin.TryToIntArray(), invStateIdsAll.TryToIntArray(), userIds.TryToStringtArray(),
			   regionIdsIni.TryToIntArray(), costCenterIdsIni.TryToIntArray(), admCenterIdsIni.TryToIntArray(), departmentIdsIni.TryToIntArray(),
			   employeeIdsIni.TryToIntArray(), locationIdsIni.TryToIntArray(), roomIdsIni.TryToIntArray(),
			   regionIdsFin.TryToIntArray(), costCenterIdsFin.TryToIntArray(), admCenterIdsFin.TryToIntArray(), departmentIdsFin.TryToIntArray(),
			   employeeIdsFin.TryToIntArray(), locationIdsFin.TryToIntArray(), roomIdsFin.TryToIntArray(), admCenterIdsAll.TryToIntArray(), employeeIdsAll.TryToIntArray(), locationIdsAll.TryToIntArray(), regionIdsAll.TryToIntArray(), costCenterIdsAll.TryToIntArray(), divisionIdsAll.TryToIntArray(), departmentIdsAll.TryToIntArray(), administrationIdsAll.TryToIntArray(), roomIdsAll.TryToIntArray(),
			   sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

			if (items.Count > 0)
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					string sheetName = "Rezultat inventar";

					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

					int recordIndex = 4;
					int rowNumber = 0;
					int count = items.Count();

					foreach (var item in items)
					{

						rowNumber++;
						int diff = recordIndex - count;

						if (diff > 0)
						{
							diff = 0;
						}



						worksheet.Cells[3, 1].Value = "1";
						worksheet.Cells[3, 2].Value = "2";
						worksheet.Cells[3, 3].Value = "3";
						worksheet.Cells[3, 4].Value = "4";
						worksheet.Cells[3, 5].Value = "5";
						worksheet.Cells[3, 6].Value = "6";
						worksheet.Cells[3, 7].Value = "7";
						worksheet.Cells[3, 8].Value = "8";
						worksheet.Cells[3, 9].Value = "9";
						worksheet.Cells[3, 10].Value = "10";
						worksheet.Cells[3, 11].Value = "11";
						worksheet.Cells[3, 12].Value = "12";
						worksheet.Cells[3, 13].Value = "13";
						worksheet.Cells[3, 14].Value = "14";
						worksheet.Cells[3, 15].Value = "15";
						worksheet.Cells[3, 16].Value = "16";
						worksheet.Cells[3, 17].Value = "17";
						worksheet.Cells[3, 18].Value = "18";
						worksheet.Cells[3, 19].Value = "19";
						worksheet.Cells[3, 20].Value = "20";
						worksheet.Cells[3, 21].Value = "21";
						worksheet.Cells[3, 22].Value = "22";
						worksheet.Cells[3, 23].Value = "23";
						worksheet.Cells[3, 24].Value = "24";
						worksheet.Cells[3, 25].Value = "25";
						worksheet.Cells[3, 26].Value = "26";
						worksheet.Cells[3, 27].Value = "27";
						worksheet.Cells[3, 28].Value = "28";
						worksheet.Cells[3, 29].Value = "29";
						worksheet.Cells[3, 30].Value = "30";
						worksheet.Cells[3, 31].Value = "31";
						worksheet.Cells[3, 32].Value = "32";
						worksheet.Cells[3, 33].Value = "33";
						worksheet.Cells[3, 34].Value = "34";
						worksheet.Cells[3, 35].Value = "35";
						worksheet.Cells[3, 36].Value = "36";
						worksheet.Cells[3, 37].Value = "37";
						worksheet.Cells[3, 38].Value = "38";
						//worksheet.Cells[3, 39].Value = "39";
						//                  worksheet.Cells[3, 40].Value = "40";
						//                  worksheet.Cells[3, 41].Value = "41";
						//                  worksheet.Cells[3, 42].Value = "42";
						//                  worksheet.Cells[3, 43].Value = "43";
						//                  worksheet.Cells[3, 44].Value = "44";
						//                  worksheet.Cells[3, 45].Value = "45";
						//worksheet.Cells[3, 46].Value = "46";

						worksheet.Cells[recordIndex, 1].Value = rowNumber;
						worksheet.Cells[recordIndex, 2].Value = item.Asset.InvNo;
						worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
						worksheet.Cells[recordIndex, 4].Value = item.QInitial;
						worksheet.Cells[recordIndex, 5].Value = item.Asset.Name;
						worksheet.Cells[recordIndex, 6].Value = item.Asset.SAPCode;
						worksheet.Cells[recordIndex, 7].Value = item.SNInitial;
						worksheet.Cells[recordIndex, 8].Value = item.Asset.PurchaseDate;
						worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "mm/dd/yyyy";
						worksheet.Cells[recordIndex, 9].Value = item.Asset.InvoiceDate;
						worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "mm/dd/yyyy";
						//worksheet.Cells[recordIndex, 10].Value = item.CostCenterInitial != null ? item.CostCenterInitial.Code : "";
						//                  worksheet.Cells[recordIndex, 11].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null ? item.CostCenterInitial.Division.Name : "";
						//                  worksheet.Cells[recordIndex, 12].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null && item.CostCenterInitial.Division.Department != null ? item.CostCenterInitial.Division.Department.Name : "";
						//worksheet.Cells[recordIndex, 13].Value = item.Asset.Account != null ? item.Asset.Account.Name : "";
						//worksheet.Cells[recordIndex, 14].Value = item.Asset.ExpAccount != null ? item.Asset.ExpAccount.Name : "";
						//worksheet.Cells[recordIndex, 15].Value = item.Asset.AssetCategory != null ? item.Asset.AssetCategory.Name : "";
						//worksheet.Cells[recordIndex, 16].Value = item.Asset.Document !=null && item.Asset.Document.Partner != null ? item.Asset.Document.Partner.RegistryNumber : "";
						//worksheet.Cells[recordIndex, 17].Value = item.Asset.BudgetManager != null ? item.Asset.BudgetManager.Name : "";
						//worksheet.Cells[recordIndex, 18].Value = item.Asset.AssetNature != null ? item.Asset.AssetNature.Name : "";
						//worksheet.Cells[recordIndex, 19].Value = item.Asset.SubType != null && item.Asset.SubType.Type != null ? item.Asset.SubType.Type.Name : "";
						worksheet.Cells[recordIndex, 10].Value = item.EmployeeInitial != null ? item.EmployeeInitial.InternalCode : "";
						//worksheet.Cells[recordIndex, 21].Value = item.Asset.Material != null ? item.Asset.Material.Name : "";
						worksheet.Cells[recordIndex, 11].Value = item.SerialNumber;
						worksheet.Cells[recordIndex, 12].Value = item.Info;
						worksheet.Cells[recordIndex, 13].Value = item.TempReco;
						worksheet.Cells[recordIndex, 14].Value = item.TempName;
						worksheet.Cells[recordIndex, 15].Value = item.QInitial;
						worksheet.Cells[recordIndex, 16].Value = item.QFinal;
						worksheet.Cells[recordIndex, 17].Value = (item.QInitial > 0 && ((item.QInitial == item.QFinal) || (item.QFinal > item.QInitial))) ? 0 : (item.QInitial - item.QFinal) < 0 ? 0 : item.QInitial - item.QFinal;
						worksheet.Cells[recordIndex, 18].Value = (item.QInitial > 0 && item.QFinal > 0 && item.QFinal > item.QInitial) ? item.QFinal - item.QInitial : item.Asset.InvNo.Substring(0, 1) == "T" ? item.QFinal - item.QInitial : 0;
						worksheet.Cells[recordIndex, 19].Value = item.Asset.ERPCode;
						//worksheet.Cells[recordIndex, 31].Value = item.Asset.InsuranceCategory != null ? item.Asset.InsuranceCategory.Name : "";
						worksheet.Cells[recordIndex, 20].Value = item.Asset.AllowLabel == true ? "DA" : "NU";
						worksheet.Cells[recordIndex, 21].Value = item.StateInitial != null ? item.StateInitial.Name : "";

						worksheet.Cells[recordIndex, 22].Value = item.CostCenterInitial != null && item.CostCenterInitial.AdmCenter != null ? item.CostCenterInitial.AdmCenter.Code : "";
						worksheet.Cells[recordIndex, 23].Value = item.CostCenterInitial != null ? item.CostCenterInitial.Code : "";
						worksheet.Cells[recordIndex, 24].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null ? item.CostCenterInitial.Division.Name : "";
						worksheet.Cells[recordIndex, 25].Value = item.CostCenterInitial != null && item.CostCenterInitial.Division != null && item.CostCenterInitial.Division.Department != null ? item.CostCenterInitial.Division.Department.Name : "";

						worksheet.Cells[recordIndex, 26].Value = item.CostCenterFinal != null && item.CostCenterFinal.AdmCenter != null ? item.CostCenterFinal.AdmCenter.Code : "";
						worksheet.Cells[recordIndex, 27].Value = item.CostCenterFinal != null ? item.CostCenterFinal.Code : "";
						worksheet.Cells[recordIndex, 28].Value = item.CostCenterFinal != null && item.CostCenterFinal.Division != null ? item.CostCenterFinal.Division.Name : "";
						worksheet.Cells[recordIndex, 29].Value = item.CostCenterFinal != null && item.CostCenterFinal.Division != null && item.CostCenterFinal.Division.Department != null ? item.CostCenterFinal.Division.Department.Name : "";
						worksheet.Cells[recordIndex, 30].Value = item.StateFinal != null ? item.StateFinal.Name : "";

						//worksheet.Cells[recordIndex, 43].Value = item.Asset.AssetType != null ? item.Asset.AssetType.Name : "";
						//worksheet.Cells[recordIndex, 44].Value = item.Asset.Project != null ? item.Asset.Project.Name : "";
						worksheet.Cells[recordIndex, 31].Value = item.ModifiedByUser != null ? item.ModifiedByUser.Email : string.Empty;
						worksheet.Cells[recordIndex, 32].Value = item.ModifiedAt;
						worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "mm/dd/yyyy";
						worksheet.Cells[recordIndex, 33].Value = item.CurrBkValue;
						worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[recordIndex, 34].Value = item.AccumulDep;
						worksheet.Cells[recordIndex, 34].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[recordIndex, 35].Value = item.CurrentAPC;
						worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
						//worksheet.Cells[recordIndex, 36].Value = item.CostCenterInitial != null && item.CostCenterInitial.Administration != null ? (item.CostCenterInitial.Administration.Code == "HQ" ? "IT" : item.CostCenterInitial.Administration.Code == "TECH" ? "Facility" : item.CostCenterInitial.Administration.Name) : "";
						//worksheet.Cells[recordIndex, 37].Value = item.CostCenterFinal != null && item.CostCenterFinal.Administration != null ? (item.CostCenterFinal.Administration.Code == "HQ" ? "IT" : item.CostCenterFinal.Administration.Code == "TECH" ? "Facility" : item.CostCenterFinal.Administration.Name) : "";
						worksheet.Cells[recordIndex, 36].Value = item.CostCenterInitial != null && item.CostCenterInitial.Administration != null ? item.CostCenterInitial.Administration.Name : "";
						worksheet.Cells[recordIndex, 37].Value = item.CostCenterFinal != null && item.CostCenterFinal.Administration != null ? item.CostCenterFinal.Administration.Name : "";
						worksheet.Cells[recordIndex, 38].Value = item.Asset.AssetCategory != null && item.Asset.AssetCategory.Prefix != null && item.Asset.AssetCategory.Prefix != "" ? item.Asset.AssetCategory.Prefix : "";
						//worksheet.Cells[recordIndex, 39].Value = item.Asset.AssetCategory != null && item.Asset.AssetCategory.Prefix != null && item.Asset.AssetCategory.Prefix != "" ? item.Asset.AssetCategory.Prefix : "";
						//for (int i = 0; i < item.entityFiles.Count(); i++)
						//{
						//    worksheet.Cells[recordIndex, 43 + i].Formula = "HYPERLINK(\"" + FileRootPath + item.entityFiles[i] + "\",\"" + DisplayText + (i + 1) + "\")";
						//}


						if (diff == 0)
						{



							for (int i = 1; i < 39; i++)
							{
								worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
								worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
								worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

							}

							worksheet.Row(1).Height = 35.00;
							worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Row(2).Height = 35.00;
							worksheet.Row(2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Row(3).Height = 15.00;
							worksheet.Row(3).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.View.FreezePanes(4, 1);

							using (var cells = worksheet.Cells[1, 1, items.Count() + 3, 38])
							{
								cells.Style.Font.Bold = false;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
								cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
								cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
								cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
								cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
								cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

							}




							worksheet.Cells["A1:A2"].Merge = true;
							worksheet.Cells["A1:A2"].Value = "Nr. Crt";
							worksheet.Cells["B1:B2"].Merge = true;
							worksheet.Cells["B1:B2"].Value = "Asset";
							worksheet.Cells["C1:C2"].Merge = true;
							worksheet.Cells["C1:C2"].Value = "SubNo";
							worksheet.Cells["D1:D2"].Merge = true;
							worksheet.Cells["D1:D2"].Value = "PCS";
							worksheet.Cells["E1:E2"].Merge = true;
							worksheet.Cells["E1:E2"].Value = "Descriere";
							worksheet.Cells["F1:F2"].Merge = true;
							worksheet.Cells["F1:F2"].Value = "Inventory number";
							worksheet.Cells["G1:G2"].Merge = true;
							worksheet.Cells["G1:G2"].Value = "Serial number initial";
							worksheet.Cells["H1:H2"].Merge = true;
							worksheet.Cells["H1:H2"].Value = "Capitalized on";
							worksheet.Cells["I1:I2"].Merge = true;
							worksheet.Cells["I1:I2"].Value = "Ord.dep.start date";
							worksheet.Cells["J1:J2"].Merge = true;
							worksheet.Cells["J1:J2"].Value = "Marca P.";
							worksheet.Cells["K1:K2"].Merge = true;
							worksheet.Cells["K1:K2"].Value = "Serie inventar";
							worksheet.Cells["L1:L2"].Merge = true;
							worksheet.Cells["L1:L2"].Value = "Observatii inventar";
							worksheet.Cells["M1:M2"].Merge = true;
							worksheet.Cells["M1:M2"].Value = "Reconciliat";
							worksheet.Cells["N1:N2"].Merge = true;
							worksheet.Cells["N1:N2"].Value = "Reconciliat Denumire";
							worksheet.Cells["O1:O2"].Merge = true;
							worksheet.Cells["O1:O2"].Value = "Scriptic";
							worksheet.Cells["P1:P2"].Merge = true;
							worksheet.Cells["P1:P2"].Value = "Faptic";
							worksheet.Cells["Q1:Q2"].Merge = true;
							worksheet.Cells["Q1:Q2"].Value = "Minus";
							worksheet.Cells["R1:R2"].Merge = true;
							worksheet.Cells["R1:R2"].Value = "Plus";
							worksheet.Cells["S1:S2"].Merge = true;
							worksheet.Cells["S1:S2"].Value = "FAR";
							worksheet.Cells["T1:T2"].Merge = true;
							worksheet.Cells["T1:T2"].Value = "Etichetabil?";
							worksheet.Cells["U1:U2"].Merge = true;
							worksheet.Cells["U1:U2"].Value = "Stare initial";
							worksheet.Cells["V1:V2"].Merge = true;
							worksheet.Cells["V1:V2"].Value = "PC Initial";
							worksheet.Cells["W1:W2"].Merge = true;
							worksheet.Cells["W1:W2"].Value = "CC initial";
							worksheet.Cells["X1:X2"].Merge = true;
							worksheet.Cells["X1:X2"].Value = "Departament initial";
							worksheet.Cells["Y1:Y2"].Merge = true;
							worksheet.Cells["Y1:Y2"].Value = "B.U. initial";
							worksheet.Cells["Z1:Z2"].Merge = true;
							worksheet.Cells["Z1:Z2"].Value = "PC inventar";
							worksheet.Cells["AA1:AA2"].Merge = true;
							worksheet.Cells["AA1:AA2"].Value = "CC inventar";

							worksheet.Cells["AB1:AB2"].Merge = true;
							worksheet.Cells["AB1:AB2"].Value = "Departament inventar";

							worksheet.Cells["AC1:AC2"].Merge = true;
							worksheet.Cells["AC1:AC2"].Value = "B.U. inventar";

							worksheet.Cells["AD1:AD2"].Merge = true;
							worksheet.Cells["AD1:AD2"].Value = "Stare inventar";

							worksheet.Cells["AE1:AE2"].Merge = true;
							worksheet.Cells["AE1:AE2"].Value = "Utilizator";
							worksheet.Cells["AE1:AE2"].Style.WrapText = true;

							worksheet.Cells["AF1:AF2"].Merge = true;
							worksheet.Cells["AF1:AF2"].Value = "Data scanare";

							worksheet.Cells["AG1:AG2"].Merge = true;
							worksheet.Cells["AG1:AG2"].Value = "CurrBkValue";

							worksheet.Cells["AH1:AH2"].Merge = true;
							worksheet.Cells["AH1:AH2"].Value = "AccumulDep";

							worksheet.Cells["AI1:AI2"].Merge = true;
							worksheet.Cells["AI1:AI2"].Value = "CurrentAPC ";


							worksheet.Cells["AJ1:AJ2"].Merge = true;
							worksheet.Cells["AJ1:AJ2"].Value = "Locatie initial";

							worksheet.Cells["AK1:AK2"].Merge = true;
							worksheet.Cells["AK1:AK2"].Value = "Locatie inventar";

							worksheet.Cells["AL1:AL2"].Merge = true;
							worksheet.Cells["AL1:AL2"].Value = "IT/Facility";

							//worksheet.Cells["AM1:AM2"].Merge = true;
							//worksheet.Cells["AM1:AM2"].Value = "Categorie inventar";

							//worksheet.Cells["AN1:AN2"].Merge = true;
							//worksheet.Cells["AN1:AN2"].Value = "";

							//worksheet.Cells["AO1:AO2"].Merge = true;
							//worksheet.Cells["AO1:AO2"].Value = "";

							//worksheet.Cells["AP1:AP2"].Merge = true;
							//worksheet.Cells["AP1:AP2"].Value = "";

							//worksheet.Cells["AQ1:AQ2"].Merge = true;
							//worksheet.Cells["AQ1:AQ2"].Value = "Type";

							//worksheet.Cells["AR1:AR2"].Merge = true;
							//worksheet.Cells["AR1:AR2"].Value = "WBS";

							//worksheet.Cells["AS1:AS2"].Merge = true;
							//worksheet.Cells["AS1:AS2"].Value = "";

							//worksheet.Cells["AT1:AT2"].Merge = true;
							//worksheet.Cells["AT1:AT2"].Value = "";

							//worksheet.Cells["AU1:AU2"].Merge = true;
							//worksheet.Cells["AU1:AU2"].Value = "Poze 3";


							using (var cells = worksheet.Cells[3, 1, 3, 38])
							{
								cells.Style.Font.Bold = true;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
								cells.Style.Font.Color.SetColor(Color.White);
							}

							using (var cells = worksheet.Cells[1, 1, 2, 38])
							{
								cells.Style.Font.Bold = true;
							}

							using (var cells = worksheet.Cells[4, 1, items.Count() + 5, 39])
							{
								cells.Style.Font.Bold = false;
								cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
								cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
							}

							using (var cells = worksheet.Cells[3, 1, items.Count() + 5, 38])
							{
								for (int i = 4; i < items.Count() + 4; i++)
								{
									worksheet.Row(i).Height = 20.00;
									worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
									worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

									worksheet.Cells[$"A{i}"].Style.WrapText = true;
									worksheet.Cells[$"B{i}"].Style.WrapText = true;
									worksheet.Cells[$"C{i}"].Style.WrapText = true;
									worksheet.Cells[$"D{i}"].Style.WrapText = true;
									worksheet.Cells[$"E{i}"].Style.WrapText = true;
									worksheet.Cells[$"F{i}"].Style.WrapText = true;
									worksheet.Cells[$"G{i}"].Style.WrapText = true;
									worksheet.Cells[$"H{i}"].Style.WrapText = true;
									worksheet.Cells[$"I{i}"].Style.WrapText = true;
									worksheet.Cells[$"J{i}"].Style.WrapText = true;
									worksheet.Cells[$"K{i}"].Style.WrapText = true;
									worksheet.Cells[$"L{i}"].Style.WrapText = true;
									worksheet.Cells[$"M{i}"].Style.WrapText = true;
									worksheet.Cells[$"N{i}"].Style.WrapText = true;
									worksheet.Cells[$"O{i}"].Style.WrapText = true;
									worksheet.Cells[$"P{i}"].Style.WrapText = true;
									worksheet.Cells[$"Q{i}"].Style.WrapText = true;
									worksheet.Cells[$"R{i}"].Style.WrapText = true;
									worksheet.Cells[$"S{i}"].Style.WrapText = true;
									worksheet.Cells[$"T{i}"].Style.WrapText = true;
									worksheet.Cells[$"U{i}"].Style.WrapText = true;
									worksheet.Cells[$"V{i}"].Style.WrapText = true;
									worksheet.Cells[$"W{i}"].Style.WrapText = true;
									worksheet.Cells[$"W{i}"].Style.WrapText = true;
									worksheet.Cells[$"X{i}"].Style.WrapText = true;
									worksheet.Cells[$"Y{i}"].Style.WrapText = true;
									worksheet.Cells[$"Z{i}"].Style.WrapText = true;
									worksheet.Cells[$"AA{i}"].Style.WrapText = true;
									worksheet.Cells[$"AB{i}"].Style.WrapText = true;
									worksheet.Cells[$"AC{i}"].Style.WrapText = true;
									worksheet.Cells[$"AD{i}"].Style.WrapText = true;
									worksheet.Cells[$"AE{i}"].Style.WrapText = true;
									worksheet.Cells[$"AF{i}"].Style.WrapText = true;
									worksheet.Cells[$"AG{i}"].Style.WrapText = true;
									worksheet.Cells[$"AH{i}"].Style.WrapText = true;
									worksheet.Cells[$"AI{i}"].Style.WrapText = true;
									worksheet.Cells[$"AJ{i}"].Style.WrapText = true;
									worksheet.Cells[$"AK{i}"].Style.WrapText = true;
									worksheet.Cells[$"AL{i}"].Style.WrapText = true;
									//worksheet.Cells[$"AM{i}"].Style.WrapText = true;
								}



							}


							worksheet.View.ShowGridLines = false;
							worksheet.View.ZoomScale = 100;

							for (int i = 1; i < 39; i++)
							{
								worksheet.Column(i).AutoFit();
							}

							worksheet.Column(1).Width = 15.00;
							worksheet.Column(2).Width = 15.00;
							worksheet.Column(3).Width = 10.00;
							worksheet.Column(4).Width = 10.00;
							worksheet.Column(5).Width = 60.00;
							worksheet.Column(6).Width = 15.00;
							worksheet.Column(7).Width = 20.00;
							worksheet.Column(8).Width = 15.00;
							worksheet.Column(9).Width = 15.00;
							worksheet.Column(10).Width = 20.00;
							worksheet.Column(11).Width = 15.00;
							worksheet.Column(12).Width = 20.00;
							worksheet.Column(13).Width = 20.00;
							worksheet.Column(14).Width = 20.00;
							worksheet.Column(15).Width = 15.00;
							worksheet.Column(16).Width = 15.00;
							worksheet.Column(17).Width = 15.00;
							worksheet.Column(18).Width = 40.00;
							worksheet.Column(19).Width = 40.00;
							worksheet.Column(20).Width = 10.00;
							worksheet.Column(21).Width = 60.00;
							worksheet.Column(22).Width = 20.00;
							worksheet.Column(23).Width = 20.00;
							worksheet.Column(24).Width = 20.00;
							worksheet.Column(25).Width = 20.00;
							worksheet.Column(26).Width = 10.00;
							worksheet.Column(27).Width = 10.00;
							worksheet.Column(28).Width = 10.00;
							worksheet.Column(29).Width = 10.00;
							worksheet.Column(30).Width = 20.00;
							worksheet.Column(31).Width = 20.00;
							worksheet.Column(32).Width = 20.00;
							worksheet.Column(33).Width = 20.00;
							worksheet.Column(34).Width = 20.00;
							worksheet.Column(35).Width = 20.00;
							worksheet.Column(36).Width = 20.00;
							worksheet.Column(37).Width = 20.00;
							worksheet.Column(38).Width = 20.00;
							//worksheet.Column(39).Width = 20.00;
							//                     worksheet.Column(40).Width = 20.00;
							//                     worksheet.Column(41).Width = 20.00;
							//                     worksheet.Column(42).Width = 20.00;
							//                     worksheet.Column(44).Width = 20.00;
							//                     worksheet.Column(45).Width = 20.00;
							//worksheet.Column(46).Width = 20.00;
						}


						//  worksheet.Column(3).Style.Numberformat.Format = "MMMM";
						//  worksheet.Column(13).Style.Numberformat.Format = "yyyy-mm-dd";
						//  worksheet.Column(14).Style.Numberformat.Format = "yyyy-MM-dd";


						//  worksheet.Cells.AutoFitColumns();


						recordIndex++;
					}




					string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					//HttpContext.Response.ContentType = entityFile.FileType;
					HttpContext.Response.ContentType = contentType;
					FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
					{
						FileDownloadName = "Rezultat inventar.xlsx"
					};

					return result;

				}
			}

			using (ExcelPackage package = new ExcelPackage())
			{
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nu exista scanari");

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Rezultat inventar.xlsx"
				};

				return result;

			}
		}


		[HttpGet("exportSocGenEmail")]
		public IActionResult ExportSocGenEmail(int? page, int? pageSize, string sortColumn, string sortDirection,
			int inventoryId, int appStateId, string includes, string filter,
			string invStateIdsAll, string employeeIdsAll, string locationIdsAll, string regionIdsAll, string roomIdsAll, string companyIds, bool? custody)
		{
			string userName = string.Empty;
			string employee = string.Empty;
			string company = string.Empty;

			includes = includes ?? "Asset.AppState,Asset.Model,Asset.Brand,Asset.Employee,Asset.SubType.Type.MasterType,Asset.SubType.Type,";

			//List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).GetInventoryEmail(
			//   inventoryId, appStateId, includes, filter, custody, invStateIdsAll.TryToIntArray(),
			//	employeeIdsAll.TryToIntArray(), locationIdsAll.TryToIntArray(), regionIdsAll.TryToIntArray(), roomIdsAll.TryToIntArray(), companyIds.TryToIntArray(),
			//	sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

			if (employeeIdsAll != null)
			{
				employee = employeeIdsAll.Replace("[", "").Replace("]", "");
			}

			if (companyIds != null)
			{
				company = companyIds.Replace("[", "").Replace("]", "");
			}

			List<Model.Reporting.InventoryListEmail> items = null;


			items = _context.Set<Model.Reporting.InventoryListEmail>().FromSql("EmailReport {0}, {1}, {2}, {3}", inventoryId, appStateId, employee, company).ToList();

			//var itemsResource = _mapper.Map<List<Model.InventoryAsset>, List<Dto.InventoryAssetResource>>(items);


			using (ExcelPackage package = new ExcelPackage())
			{


				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Email");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Nr. crt";
				worksheet.Cells[1, 2].Value = "Status Email";
				worksheet.Cells[1, 3].Value = "Inventory number";
				worksheet.Cells[1, 4].Value = "Description";
				worksheet.Cells[1, 5].Value = "PIF Date";
				worksheet.Cells[1, 6].Value = "Value Dep";
				worksheet.Cells[1, 7].Value = "MasterType";
				worksheet.Cells[1, 8].Value = "Type";
				worksheet.Cells[1, 9].Value = "Brand";
				worksheet.Cells[1, 10].Value = "Model";
				worksheet.Cells[1, 11].Value = "Serial Number";
				worksheet.Cells[1, 12].Value = "IGG";
				worksheet.Cells[1, 13].Value = "FullName";
				worksheet.Cells[1, 14].Value = "Reason";
				



				int recordIndex = 2;

				for (int i = 0; i < items.Count; i++)
				{
					worksheet.Cells[recordIndex, 1].Value = GetDimensionRows(worksheet) + 1;
					worksheet.Cells[recordIndex, 2].Value = items[i].AppState;
					worksheet.Cells[recordIndex, 3].Value = items[i].InvNo;
					worksheet.Cells[recordIndex, 4].Value = items[i].Description;
					worksheet.Cells[recordIndex, 5].Value = items[i].PurchaseDate;
					worksheet.Cells[recordIndex, 5].Style.Numberformat.Format = "yyyy-mm-dd";
					worksheet.Cells[recordIndex, 6].Value = items[i].ValueDep;
					worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 7].Value = items[i].MasterType;
					worksheet.Cells[recordIndex, 8].Value = items[i].Type;
					worksheet.Cells[recordIndex, 9].Value = items[i].Brand;
					worksheet.Cells[recordIndex, 10].Value = items[i].Model;
					worksheet.Cells[recordIndex, 11].Value = items[i].SerialNumber;
					worksheet.Cells[recordIndex, 12].Value = items[i].InternalCode;
					worksheet.Cells[recordIndex, 13].Value = items[i].FullName;
					worksheet.Cells[recordIndex, 14].Value = items[i].Reason;
					


					recordIndex++;
				}

				worksheet.View.FreezePanes(2, 1);
				worksheet.Row(1).Height = 45;
				worksheet.Row(1).Style.WrapText = true;
				worksheet.Column(1).AutoFit();
				worksheet.Column(2).AutoFit();
				worksheet.Column(3).AutoFit();
				worksheet.Column(4).AutoFit();
				worksheet.Column(5).AutoFit();
				worksheet.Column(6).AutoFit();
				worksheet.Column(7).AutoFit();
				worksheet.Column(8).AutoFit();
				worksheet.Column(9).AutoFit();
				worksheet.Column(10).AutoFit();
				worksheet.Column(11).AutoFit();
				worksheet.Column(12).AutoFit();
				worksheet.Column(1).Width = 10;
				worksheet.Column(2).Width = 20;
				worksheet.Column(3).Width = 20;
				worksheet.Column(4).Width = 50;
				worksheet.Column(5).Width = 20;
				worksheet.Column(6).Width = 20;
				worksheet.Column(7).Width = 30;
				worksheet.Column(8).Width = 50;
				worksheet.Column(9).Width = 20;
				worksheet.Column(10).Width = 50;
				worksheet.Column(11).Width = 40;
				worksheet.Column(12).Width = 20;
				worksheet.Column(13).Width = 40;
				worksheet.Column(14).Width = 300;
				worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Row(1).Style.WrapText = true;

				worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(1).Style.WrapText = true;
				worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(3).Style.WrapText = true;
				worksheet.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(3).Style.WrapText = true;
				worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(4).Style.WrapText = true;
				worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(5).Style.WrapText = true;
				worksheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(6).Style.WrapText = true;
				worksheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(7).Style.WrapText = true;
				worksheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(8).Style.WrapText = true;
				worksheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(9).Style.WrapText = true;
				worksheet.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(10).Style.WrapText = true;
				worksheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(11).Style.WrapText = true;
				worksheet.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(12).Style.WrapText = true;
				worksheet.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(13).Style.WrapText = true;
				worksheet.Column(14).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Column(14).Style.WrapText = true;

				worksheet.Column(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(1).Style.WrapText = true;
				worksheet.Column(2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(2).Style.WrapText = true;
				worksheet.Column(3).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(3).Style.WrapText = true;
				worksheet.Column(4).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(4).Style.WrapText = true;
				worksheet.Column(5).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(5).Style.WrapText = true;
				worksheet.Column(6).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(6).Style.WrapText = true;
				worksheet.Column(7).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(7).Style.WrapText = true;
				worksheet.Column(8).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(8).Style.WrapText = true;
				worksheet.Column(9).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(9).Style.WrapText = true;
				worksheet.Column(10).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(10).Style.WrapText = true;
				worksheet.Column(11).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(11).Style.WrapText = true;
				worksheet.Column(12).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(12).Style.WrapText = true;
				worksheet.Column(13).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(14).Style.WrapText = true;
				worksheet.Column(14).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Column(14).Style.WrapText = true;

				using (var range = worksheet.Cells[1, 2, items.Count() + 1, 14])
				{
					range.Style.Font.Bold = true;
					range.Style.Fill.PatternType = ExcelFillStyle.Solid;
					range.Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
					range.Style.Font.Color.SetColor(Color.Black);
					range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
				}

				using (var range = worksheet.Cells[1, 1, items.Count() + 1, 1])
				{
					range.Style.Font.Bold = true;
					range.Style.Fill.PatternType = ExcelFillStyle.Solid;
					range.Style.Fill.BackgroundColor.SetColor(Color.Wheat);
					range.Style.Font.Color.SetColor(Color.Black);
					range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
				}

				using (var range = worksheet.Cells[1, 1, items.Count() + 1, 14])
				{
					range.Style.Font.Bold = true;
					range.Style.Fill.PatternType = ExcelFillStyle.Solid;
					range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(41, 116, 186));
					range.Style.Font.Color.SetColor(Color.Wheat);
					range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
				}

				using (var cells = worksheet.Cells[1, 1, 1, 14])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(89, 89, 89));

				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Email.xlsx"
				};

				return result;

			}
		}

		public int GetDimensionRows(ExcelWorksheet sheet)
        {
            var startRow = sheet.Dimension.Start.Row;
            var endRow = sheet.Dimension.End.Row;
            return endRow - startRow;
        }


        [HttpPost]
        [Route("printLabel")]
        public virtual IActionResult Label([FromBody] List<LabelDto> labels)
        {
            foreach (var label in labels)
            {

                PrintLabel(label.InvNo, label.Description, String.Format("{0:MM/dd/yyyy}", label.PurchaseDate), label.SerialNumber, "/", "DA");
            }

            //PrintBarcode(label.InvNo, label.Description, "1");

            return Ok(labels[0]);
        }

        private void PrintLabel(string invNo, string description, string purchaseDate, string serialNumber, string Cold, string NewMember = "")
        {

            UpcLabel lbl = new UpcLabel(invNo, description, purchaseDate, serialNumber, Cold, NewMember);

            PrinterSettings printerName = new PrinterSettings();

            string defaultPrinter;







            defaultPrinter = printerName.PrinterName;
            //Printer name
            lbl.Print(defaultPrinter);

        }
        private void PrintBarcode(string pProductName, string pLocation, string pNoOfCopies)
        {
            UpcLabel upcLabel = new UpcLabel();

            PrinterSettings printerName = new PrinterSettings();

            string defaultPrinter;

            defaultPrinter = printerName.PrinterName;
            //Printer name
            upcLabel.PrintBarcode(defaultPrinter, pProductName, pLocation, pNoOfCopies);

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


            //includes = includes + ",";


            List<Model.Asset> items = (_itemsRepository as IAssetsRepository)
                .GetFilteredDetailUI(includes, assetId, employeeId, sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

            var itemsResource = _mapper.Map<List<Model.Asset>, List<Dto.AssetUI>>(items);
            var result = new PagedResult<Dto.AssetUI>(itemsResource, new PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });



            return Ok(result);
        }

        [HttpPost]
        [Route("addTransfer")]
        public virtual IActionResult AddTransfer([FromBody] Dto.AssetTransferAdd assetTransferAdd)
        {
            Model.Asset assetOld = null;
            Model.Asset assetNew = null;
            Model.AssetInv assetInvOld = null;
            Model.AssetInv assetInvNew = null;
            Model.AssetOp assetOpOld = null;
            Model.AssetOp assetOpNew = null;
            Model.Document documentAdd = null;
            Model.Document documentRemove = null;
            Model.Inventory inventory = null;
            Model.AccMonth accMonth = null;
            Model.AssetAdmMD assetAdmMDOld = null;
            Model.AssetAdmMD assetAdmMDNew = null;
            Model.InventoryAsset inventoryAssetOld = null;
            Model.InventoryAsset inventoryAssetNew = null;
            string documentTypeAddCode = "ADD";
            string documentTypeRemoveCode = "REMOVE";

            var documentTypeAdd = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeAddCode).Single();
            var documentTypeRemove = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeRemoveCode).Single();
            inventory = _context.Set<Model.Inventory>().Where(i => i.Active == true).SingleOrDefault();
            accMonth = _context.Set<Model.AccMonth>().Where(i => i.IsActive == true).SingleOrDefault();
            

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

            for (int i = 0; i < assetTransferAdd.AssetOldIds.Length; i++)
            {
                assetOld = _context.Set<Model.Asset>().SingleOrDefault(a => a.Id == assetTransferAdd.AssetOldIds[i]);
                assetInvOld = _context.Set<Model.AssetInv>().Where(a => a.AssetId == assetTransferAdd.AssetOldIds[i]).SingleOrDefault();
                assetAdmMDOld = _context.Set<Model.AssetAdmMD>().SingleOrDefault(a => a.AssetId == assetTransferAdd.AssetOldIds[i] && a.AccMonthId == accMonth.Id);
                inventoryAssetOld = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == assetTransferAdd.AssetOldIds[i] && a.InventoryId == inventory.Id);

                if (assetOld != null)
                {
                    

                    assetOld.EmployeeId = null;
                    assetAdmMDOld.EmployeeId = null;

                    if (inventoryAssetOld.EmployeeIdFinal != null) {

                        inventoryAssetOld.EmployeeIdFinal = null;
                    } else
                    {
                        inventoryAssetOld.EmployeeIdInitial = null;
                    };

                    assetOpOld = new Model.AssetOp()
                    {
                        AccSystemId =1,
                        AdministrationIdFinal = assetOld.AdministrationId,
                        AdministrationIdInitial = assetOld.AdministrationId,
                        AssetCategoryIdFinal = assetOld.AssetCategoryId,
                        AssetCategoryIdInitial = assetOld.AssetCategoryId,
                        Asset = assetOld,
                        AssetStateIdFinal = assetOld.AssetStateId,
                        AssetStateIdInitial = assetOld.AssetStateId,
                        CostCenterIdFinal = assetOld.CostCenterId,
                        CostCenterIdInitial = assetOld.CostCenterId,
                        CreatedAt = DateTime.Now,
                        DepartmentIdFinal = assetOld.DepartmentId,
                        DepartmentIdInitial = assetOld.DepartmentId,
                        Document = documentRemove,
                        EmployeeIdInitial = assetOld.EmployeeId,
                        EmployeeIdFinal = null,
                        InvStateIdFinal = assetTransferAdd.InvStateId > 0 ? assetTransferAdd.InvStateId : assetOld.InvStateId,
                        InvStateIdInitial = assetOld.InvStateId,
                        IsDeleted = false,
                        RoomIdFinal = assetOld.RoomId,
                        RoomIdInitial = assetOld.RoomId,
                        AssetOpStateId = 3,
                        SrcConfAt= DateTime.Now,
                        AllowLabel = assetInvOld.AllowLabel.Value,
                        AssetTypeIdFinal = assetOld.AssetTypeId,
                        AssetTypeIdInitial = assetOld.AssetTypeId,
                        InvName = assetOld.Name,
                        Model = assetInvOld.Model,
                        Producer = assetInvOld.Producer,
                        Quantity = assetOld.Quantity,
                        SerialNumber = assetOld.SerialNumber,
                        Info = assetOld.Info,
                        Info2019 = assetOld.Info,
                        AssetNatureIdFinal = assetOld.AssetNatureId,
                        AssetNatureIdInitial = assetOld.AssetNatureId,
                        BudgetManagerIdFinal = assetOld.BudgetManagerId,
                        BudgetManagerIdInitial = assetOld.BudgetManagerId,
                        DimensionIdFinal = assetOld.DimensionId,
                        DimensionIdInitial = assetOld.DimensionId,
                        ProjectIdFinal = assetOld.ProjectId,
                        ProjectIdInitial = assetOld.ProjectId,
                        ModifiedAt = DateTime.Now
                    };

                    _context.Add(assetOpOld);
                    _context.Update(assetOld);
                    _context.Update(assetAdmMDOld);
                    _context.Update(inventoryAssetOld);

                    _context.SaveChanges();
                }
                else
                {
                    return Ok(StatusCode(404));
                }
            }

            for (int i = 0; i < assetTransferAdd.AssetNewIds.Length; i++)
            {
                assetNew = _context.Set<Model.Asset>().SingleOrDefault(a => a.Id == assetTransferAdd.AssetNewIds[i]);
                assetInvNew = _context.Set<Model.AssetInv>().Where(a => a.AssetId == assetTransferAdd.AssetNewIds[i]).SingleOrDefault();
                assetAdmMDNew = _context.Set<Model.AssetAdmMD>().SingleOrDefault(a => a.AssetId == assetTransferAdd.AssetNewIds[i] && a.AccMonthId == accMonth.Id);
                inventoryAssetNew = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == assetTransferAdd.AssetNewIds[i] && a.InventoryId == inventory.Id);

                if (assetNew != null)
                {


                    assetNew.EmployeeId = assetTransferAdd.EmployeeId;
                    assetAdmMDNew.EmployeeId = assetTransferAdd.EmployeeId;

                    if (inventoryAssetNew.EmployeeIdFinal != null)
                    {

                        inventoryAssetNew.EmployeeIdFinal = assetTransferAdd.EmployeeId;
                    }
                    else
                    {
                        inventoryAssetNew.EmployeeIdInitial = assetTransferAdd.EmployeeId;
                    };
                    _context.Update(assetNew);

                    assetOpNew = new Model.AssetOp()
                    {
                        AccSystemId = 1,
                        AdministrationIdFinal = assetNew.AdministrationId,
                        AdministrationIdInitial = assetNew.AdministrationId,
                        AssetCategoryIdFinal = assetNew.AssetCategoryId,
                        AssetCategoryIdInitial = assetNew.AssetCategoryId,
                        Asset = assetNew,
                        AssetStateIdFinal = assetNew.AssetStateId,
                        AssetStateIdInitial = assetNew.AssetStateId,
                        CostCenterIdFinal = assetNew.CostCenterId,
                        CostCenterIdInitial = assetNew.CostCenterId,
                        CreatedAt = DateTime.Now,
                        DepartmentIdFinal = assetNew.DepartmentId,
                        DepartmentIdInitial = assetNew.DepartmentId,
                        Document = documentAdd,
                        EmployeeIdInitial = assetTransferAdd.EmployeeId,
                        EmployeeIdFinal = assetTransferAdd.EmployeeId,
                        InvStateIdFinal = assetNew.InvStateId,
                        InvStateIdInitial = assetNew.InvStateId,
                        IsDeleted = false,
                        RoomIdFinal = assetNew.RoomId,
                        RoomIdInitial = assetNew.RoomId,
                        AssetOpStateId = 3,
                        SrcConfAt = DateTime.Now,
                        AllowLabel = assetInvNew.AllowLabel.Value,
                        AssetTypeIdFinal = assetNew.AssetTypeId,
                        AssetTypeIdInitial = assetNew.AssetTypeId,
                        InvName = assetNew.Name,
                        Model = assetInvNew.Model,
                        Producer = assetInvNew.Producer,
                        Quantity = assetNew.Quantity,
                        SerialNumber = assetNew.SerialNumber,
                        Info = assetNew.Info,
                        Info2019 = assetNew.Info,
                        AssetNatureIdFinal = assetNew.AssetNatureId,
                        AssetNatureIdInitial = assetNew.AssetNatureId,
                        BudgetManagerIdFinal = assetNew.BudgetManagerId,
                        BudgetManagerIdInitial = assetNew.BudgetManagerId,
                        DimensionIdFinal = assetNew.DimensionId,
                        DimensionIdInitial = assetNew.DimensionId,
                        ProjectIdFinal = assetNew.ProjectId,
                        ProjectIdInitial = assetNew.ProjectId,
                        ModifiedAt = DateTime.Now
                    };

                    _context.Add(assetOpNew);
                    _context.Update(assetNew);
                    _context.Update(assetAdmMDNew);
                    _context.Update(inventoryAssetNew);

                    _context.SaveChanges();
                }
                else
                {
                    return Ok(StatusCode(404));
                }
            }

            return Ok(StatusCode(200));
        }

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("employee/{inventoryId}")]
		public virtual IActionResult GetInventoryChartADetails(int inventoryId)
		{
			List<Model.InventoryEmployeeProcentage> items = _context.Set<Model.InventoryEmployeeProcentage>().FromSql("InventoryReportByEmployee {0}", inventoryId).ToList();

			if (items.Count == 0)
			{
				var item = new Model.InventoryEmployeeProcentage();
				item.DivisionName = "Nu exista active";
				item.DivisionCode = "Nu exista active";
				item.Procentage = 0;
				item.Total = 0;
				item.Scanned = 0;
				item.IsFinished = 0;

				items.Add(item);
			}

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("region/{inventoryId}")]
		public virtual IActionResult GetRegionDetails(int inventoryId)
		{
			List<Model.InventoryRegionProcentage> items = _context.Set<Model.InventoryRegionProcentage>().FromSql("InventoryReportByRegion {0}", inventoryId).ToList();

			if (items.Count == 0)
			{
				var item = new Model.InventoryRegionProcentage();
				item.RegionName = "Nu exista active";
				item.RegionCode = "Nu exista active";
				item.Procentage = 0;
				item.Total = 0;
				item.Scanned = 0;
				item.IsFinished = 0;

				items.Add(item);
			}

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("room/{locationId}/{inventoryId}")]
		public virtual IActionResult GetInventoryChartPerRoomDetails(int locationId, int inventoryId)
		{
			List<Model.InventoryRoomProcentage> items = _context.Set<Model.InventoryRoomProcentage>().FromSql("InventoryReportByRoom {0}, {1}", inventoryId, locationId).ToList();

			if (items.Count == 0)
			{
				var item = new Model.InventoryRoomProcentage();
				item.AdministrationName = "Nu exista active";
				item.AdministrationCode = "Nu exista active";
				item.Procentage = 0;
				item.Total = 0;
				item.Scanned = 0;
				item.IsFinished = 0;

				items.Add(item);
			}

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("location/{regionId}/{inventoryId}")]
		public virtual IActionResult GetInventoryChartPerLocationDetails(int regionId, int inventoryId)
		{
			List<Model.InventoryLocationProcentage> items = _context.Set<Model.InventoryLocationProcentage>().FromSql("InventoryReportByLocation {0}, {1}", inventoryId, regionId).ToList();

			if (items.Count == 0)
			{
				var item = new Model.InventoryLocationProcentage();
				item.LocationName = "Nu exista active";
				item.LocationCode = "Nu exista active";
				item.Procentage = 0;
				item.Total = 0;
				item.Scanned = 0;
				item.IsFinished = 0;

				items.Add(item);
			}

			return Ok(items);
		}


		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("InventoryPieChartByDay/{inventoryId}")]
		public virtual IActionResult GetInventoryPieChartByDayDetails(int inventoryId)
		{
			List<Model.InventoryPieChartByDay> items = _context.Set<Model.InventoryPieChartByDay>().FromSql("InventoryPieChartByDay {0}", inventoryId).ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("InventoryPieChartLocationFinishedByDay/{inventoryId}")]
		public virtual IActionResult GetInventoryPieChartByLocationFinishedDayDetails(int inventoryId)
		{
			List<Model.InventoryPieChartByDay> items = _context.Set<Model.InventoryPieChartByDay>().FromSql("InventoryPieChartLocationFinishedByDay {0}", inventoryId).ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditAdministrationChart/{administrationId}/{inventoryId}")]
		public virtual IActionResult GetAuditAdministration(int administrationId, int inventoryId)
		{
			List<Model.Audit> items = _context.Set<Model.Audit>().FromSql("AuditAdministrationWithValue {0}, {1}", inventoryId, administrationId).ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditDivisionChart/{divisionId}/{inventoryId}")]
		public virtual IActionResult GetAuditDivision(int divisionId, int inventoryId)
		{
			List<Model.Audit> items = _context.Set<Model.Audit>().FromSql("AuditDivisionWithValue {0}, {1}", inventoryId, divisionId > 0 ? divisionId : 76).ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditLocationChart/{locationId}/{inventoryId}")]
		public virtual IActionResult GetAuditLocation(int locationId, int inventoryId)
		{
			List<Model.Audit> items = _context.Set<Model.Audit>().FromSql("AuditLocationWithValue {0}, {1}", inventoryId, locationId).ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditRegionChart/{regionId}/{inventoryId}")]
		public virtual IActionResult GetAuditRegion(int regionId, int inventoryId)
		{
			List<Model.Audit> items = _context.Set<Model.Audit>().FromSql("AuditRegionWithValue {0}, {1}", inventoryId, regionId > 0 ? regionId : 37).ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditSubTypeChart")]
		public virtual IActionResult GetAuditSubtype()
		{
			List<Model.SubTypeReport> items = _context.Set<Model.SubTypeReport>().FromSql("InventoryReportBySubType").ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditTypeChart")]
		public virtual IActionResult GetAuditType()
		{
			List<Model.SubTypeReport> items = _context.Set<Model.SubTypeReport>().FromSql("InventoryReportByType").ToList();

			return Ok(items);
		}


		//[Authorize]
		[HttpGet]
		[AllowAnonymous]
		[Route("auditMasterTypeChart")]
		public virtual IActionResult GetAuditMasterType()
		{
			List<Model.SubTypeReport> items = _context.Set<Model.SubTypeReport>().FromSql("InventoryReportByMasterType").ToList();

			return Ok(items);
		}

		//[Authorize]
		[HttpPost]
		[Route("employeeValidate")]
		public async Task<IActionResult> EmployeeValidate([FromBody] Dto.EmployeeValidate[] employee)
		{
			Model.InventoryAsset inventoryAsset = null;
			Model.Asset asset = null;
			Model.AssetOp assetOp = null;
			Model.Document document = null;
			
			string documentTypeCode = "IS_MINUS";



			var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();
			int employeeId = _context.Set<Model.Employee>().Where(d => d.Guid == employee[0].Guid).AsNoTracking().Select(e => e.Id).SingleOrDefault();
			string userId = _context.Set<Model.ApplicationUser>().Where(d => d.EmployeeId == employeeId).AsNoTracking().Select(e => e.Id).SingleOrDefault();

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

			for (int i = 0; i < employee.Length; i++)
			{
				asset = _context.Set<Model.Asset>().Where(a => a.Id == employee[i].AssetId).SingleOrDefault();
				inventoryAsset = _context.Set<Model.InventoryAsset>().Include(a => a.Inventory).Where(inv => inv.Inventory.Active == true).SingleOrDefault(a => a.AssetId == employee[i].AssetId);

				asset.InfoPlus = employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 && employee[i].Accepted == false ? employee[i].Reason : "";
				asset.InfoMinus = employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 && employee[i].Accepted == false ? employee[i].Reason : "";
				asset.IsMinus = employee[i].Accepted;
				asset.IsPlus = asset.InfoPlus != null && asset.InfoPlus.Trim().Length > 1 ? !asset.IsMinus : false;
				asset.IsAccepted = employee[i].Accepted;
				asset.AppStateId = employee[i].Accepted ? 7 : employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 ? 8 : (int?)null;
				asset.EmployeeId = employeeId;

				inventoryAsset.InfoPlus = employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 && employee[i].Accepted == false ? employee[i].Reason : "";
				inventoryAsset.InfoMinus = employee[i].Reason != null && employee[i].Reason.Trim().Length > 1 && employee[i].Accepted == false ? employee[i].Reason : "";
				inventoryAsset.IsMinus = employee[i].Accepted;
				inventoryAsset.IsPlus = inventoryAsset.InfoPlus != null && inventoryAsset.InfoPlus.Trim().Length > 1 ? !inventoryAsset.IsMinus : false;
				

				if (inventoryAsset.RoomIdFinal == null && employee[i].Accepted)
				{
					inventoryAsset.RoomIdFinal = inventoryAsset.RoomIdInitial;
					inventoryAsset.CostCenterIdFinal = inventoryAsset.CostCenterIdInitial;
					inventoryAsset.EmployeeIdFinal = employeeId;
					inventoryAsset.Model = string.Empty;
					inventoryAsset.Producer = string.Empty;
					inventoryAsset.QFinal = inventoryAsset.QInitial;
					inventoryAsset.SerialNumber = inventoryAsset.SNInitial;
					inventoryAsset.StateIdFinal = inventoryAsset.StateIdInitial;
					inventoryAsset.Info = string.Empty;
					inventoryAsset.ModifiedAt = DateTime.Now;
					inventoryAsset.ModifiedBy = userId != null ? userId : null;
					inventoryAsset.AdministrationIdFinal = inventoryAsset.AdministrationIdInitial;
					inventoryAsset.Info2019 = string.Empty;
					inventoryAsset.DimensionIdFinal = asset.DimensionId;
					inventoryAsset.UomIdFinal = asset.UomId;

				}
				else if (inventoryAsset.RoomIdFinal != null && employee[i].Accepted)
				{
					inventoryAsset.EmployeeIdFinal = employeeId;
				}
				



				assetOp = new Model.AssetOp()
				{
					Asset = asset,
					DocumentId = document.Id,
					RoomIdInitial = inventoryAsset.RoomIdInitial,
					RoomIdFinal = inventoryAsset.RoomIdFinal,
					EmployeeIdInitial = inventoryAsset.EmployeeIdInitial,
					EmployeeIdFinal = inventoryAsset.EmployeeIdFinal,
					CostCenterIdInitial = inventoryAsset.CostCenterIdInitial,
					CostCenterIdFinal = inventoryAsset.CostCenterIdFinal,
					AssetCategoryIdInitial = asset.AssetCategoryId,
					AssetCategoryIdFinal = asset.AssetCategoryId,
					AssetTypeIdInitial = asset.AssetTypeId,
					AssetTypeIdFinal = asset.AssetTypeId,
					InvStateIdInitial = inventoryAsset.StateIdInitial,
					InvStateIdFinal = inventoryAsset.StateIdFinal,
					AdministrationIdInitial = asset.AdministrationId,
					AdministrationIdFinal = asset.AdministrationId,
					SerialNumber = inventoryAsset.SerialNumber,
					Model = inventoryAsset.Model,
					Producer = inventoryAsset.Producer,
					Quantity = inventoryAsset.QFinal,
					Info = inventoryAsset.Info,
					Info2019 = inventoryAsset.Info2019,
					ModifiedAt = inventoryAsset.ModifiedAt,
					ModifiedBy = inventoryAsset.ModifiedBy,
					AccSystemId = 1,
					AssetOpStateId = inventoryAsset.AssetRecoStateId,
					IsMinus = employee[i].Accepted,
					IsPlus = inventoryAsset.InfoPlus != null && inventoryAsset.InfoPlus.Trim() != "" ? !inventoryAsset.IsMinus : false,
					InfoMinus = employee[i].Reason != null && employee[i].Reason.Trim() != "" && employee[i].Accepted == false ? employee[i].Reason : "",
					InfoPlus = employee[i].Reason != null && employee[i].Reason.Trim() != "" && employee[i].Accepted == false ? employee[i].Reason : ""
				};

				_context.Add(assetOp);

				_context.SaveChanges();
			}

			return Ok(StatusCode(200));
		}

		[HttpGet]
		[Route("employeevalidate")]
		public virtual IActionResult EmployeeValidate(int? page, int? pageSize, string sortColumn, string sortDirection, string reportType,//, string assetState, 
		 int inventoryId, int documentTypeId, string includes, string filter, string userId,
		 string assetClassificationIds, string companyIds,
		 string admCenterIdsFin, string employeeIdsFin, string countyIdsFin, string cityIdsFin, string locationIdsFin, string invStateIdsFin,
		 bool? custody,
		 bool? allowLabel)
		{
			int totalItems = 0;
			string userName = string.Empty;
			// string userId = null;
			string employeeId = string.Empty;
			string locationIds = string.Empty;

			includes = "Asset,RoomInitial.Location,EmployeeInitial,EmployeeFinal,RoomFinal.Location,ModifiedByUser,Asset.Model,Asset.Brand";


			List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).EmployeeValidate(
				inventoryId, documentTypeId, includes, filter, userId, reportType, //assetState, 
				custody, allowLabel, false,
				sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

			var itemsResource = _mapper.Map<List<Model.InventoryAsset>, List<Dto.InventoryAssetResource>>(items);

			var result = new PagedResult<Dto.InventoryAssetResource>(itemsResource, new PagingInfo()
			{
				TotalItems = totalItems,
				CurrentPage = page.Value,
				PageSize = pageSize.Value
			});

			return Ok(result);
		}

        //[HttpGet("exportEmag")]
        //public IActionResult ExporteMAG(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        //{

        //    AssetDepTotal depTotal = null;
        //    AssetFilter assetFilter = null;
        //    string employeeId = string.Empty;
        //    string admCenterId = string.Empty;
        //    int rowNumber = 0;
        //    Paging paging = null;
        //    Sorting sorting = null;
        //    string userName = string.Empty;
        //    string role = string.Empty;

        //    assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


        //    includes = @"Asset.InvState,Adm.Account,Adm.ExpAccount,Adm.AssetCategory,Asset.Document.Partner,Asset.Document,Adm.Article,Adm.CostCenter.AdmCenter,Asset.Material,Dep.AccSystem,Adm.Department,Adm.Division,Adm.Room.Location.City.County.Country,Adm.Administration,Adm.BudgetManager,Adm.AssetNature,Adm.SubType.Type,Adm.Employee,Adm.InterCompany,Adm.SubType,Asset,Adm.AssetClass,Adm.CostCenter.Region,Adm.InsuranceCategory,Adm.AssetType,Adm.Project,";


        //    if (HttpContext.User.Identity.Name != null)
        //    {
        //        userName = HttpContext.User.Identity.Name;
        //        role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
        //        employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

        //        assetFilter.UserName = userName;
        //        assetFilter.Role = role;

        //        if (employeeId != null)
        //        {
        //            assetFilter.EmployeeId = int.Parse(employeeId);
        //        }
        //        else
        //        {
        //            assetFilter.EmployeeIds = null;
        //            assetFilter.EmployeeIds = new List<int?>();
        //            assetFilter.EmployeeIds.Add(int.Parse("-1"));
        //        }
        //    }
        //    else
        //    {
        //        assetFilter.EmployeeIds = null;
        //        assetFilter.EmployeeIds = new List<int?>();
        //        assetFilter.EmployeeIds.Add(int.Parse("-1"));
        //    }


        //    var items = (_itemsRepository as IAssetsRepository)
        //        .GetMonthInUse(assetFilter, new List<int?>(), includes, sorting, paging, out depTotal).ToList();

        //    using (ExcelPackage package = new ExcelPackage())
        //    {

        //        assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

        //        // add a new worksheet to the empty workbook
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Emag");
        //        //First add the headers
        //        worksheet.Cells[1, 1].Value = "Asset";
        //        worksheet.Cells[1, 2].Value = "PCS";
        //        worksheet.Cells[1, 3].Value = "Subnumber";
        //        worksheet.Cells[1, 4].Value = "Bal.sh.acct APC";
        //        worksheet.Cells[1, 5].Value = "Asset Class";
        //        worksheet.Cells[1, 6].Value = "Asset clasif.";
        //        worksheet.Cells[1, 7].Value = "Asset description";
        //        worksheet.Cells[1, 8].Value = "Inventorynumber";
        //        worksheet.Cells[1, 9].Value = "License plate number";
        //        worksheet.Cells[1, 10].Value = "Serial number";
        //        worksheet.Cells[1, 11].Value = "Vendor";
        //        worksheet.Cells[1, 12].Value = "Supplier name";
        //        worksheet.Cells[1, 13].Value = "Agreement number";

        //        worksheet.Cells[1, 14].Value = "Manufacturer of asset";
        //        worksheet.Cells[1, 15].Value = "Location";
        //        worksheet.Cells[1, 16].Value = "Cost Center";
        //        worksheet.Cells[1, 17].Value = "Capitalized on";
        //        worksheet.Cells[1, 18].Value = "Ord.dep.start date";
        //        worksheet.Cells[1, 19].Value = "Deactivation on";
        //        worksheet.Cells[1, 20].Value = "Useful life";
        //        worksheet.Cells[1, 21].Value = "Tot. life in periods";
        //        worksheet.Cells[1, 22].Value = "Exp.life in periods";
        //        worksheet.Cells[1, 23].Value = "Rem.life in periods";
        //        worksheet.Cells[1, 24].Value = "APC FY start";
        //        worksheet.Cells[1, 25].Value = "Dep. FY start";
        //        worksheet.Cells[1, 26].Value = "Bk.val.FY strt";

        //        worksheet.Cells[1, 27].Value = "Acquisition";
        //        worksheet.Cells[1, 28].Value = "Dep. for year";
        //        worksheet.Cells[1, 29].Value = "Retirement";
        //        worksheet.Cells[1, 30].Value = "Dep.retir.";
        //        worksheet.Cells[1, 31].Value = "Curr.bk.val.";
        //        worksheet.Cells[1, 32].Value = "Transfer";
        //        worksheet.Cells[1, 33].Value = "Dep.transfer";
        //        worksheet.Cells[1, 34].Value = "Post-capital.";
        //        worksheet.Cells[1, 35].Value = "Dep.post-cap.";
        //        worksheet.Cells[1, 36].Value = "Invest.support";
        //        worksheet.Cells[1, 37].Value = "Write-ups";
        //        worksheet.Cells[1, 38].Value = "Current APC";
        //        worksheet.Cells[1, 39].Value = "Accumul. dep.";

        //        worksheet.Cells[1, 40].Value = "Name CC";
        //        worksheet.Cells[1, 41].Value = "Description CC";
        //        worksheet.Cells[1, 42].Value = "Departament";
        //        worksheet.Cells[1, 43].Value = "Divizie";
        //        worksheet.Cells[1, 44].Value = "Adresa";
        //        worksheet.Cells[1, 45].Value = "Locatie";
        //        worksheet.Cells[1, 46].Value = "Oras";
        //        worksheet.Cells[1, 47].Value = "Judet";
        //        worksheet.Cells[1, 48].Value = "Tara";
        //        worksheet.Cells[1, 49].Value = "FY' Start";
        //        worksheet.Cells[1, 50].Value = "Text lung cont CM";
        //        worksheet.Cells[1, 51].Value = "Supracategorie";
        //        worksheet.Cells[1, 52].Value = "Marca P.";
        //        worksheet.Cells[1, 53].Value = "Material";

        //        worksheet.Cells[1, 54].Value = "Material Description";
        //        worksheet.Cells[1, 55].Value = "Supracategorie TRN";
        //        worksheet.Cells[1, 56].Value = "Username";
        //        worksheet.Cells[1, 57].Value = "Company";
        //        worksheet.Cells[1, 58].Value = "CostCenter HR";
        //        worksheet.Cells[1, 59].Value = "CC Name HR";
        //        worksheet.Cells[1, 60].Value = "Position HR";
        //        worksheet.Cells[1, 61].Value = "Business Unit HR";
        //        worksheet.Cells[1, 62].Value = "Departament HR";
        //        worksheet.Cells[1, 63].Value = "HireDate HR";
        //        worksheet.Cells[1, 64].Value = "Tip Assets";
        //        worksheet.Cells[1, 65].Value = "Mapare Anliza Assets Angajati/WBS";
        //        worksheet.Cells[1, 66].Value = "Nr. Inventar FAR";

        //        worksheet.Cells[1, 67].Value = "Clasificare asset";
        //        worksheet.Cells[1, 68].Value = "Profit Ctr";
        //        worksheet.Cells[1, 69].Value = "PC Detaliu";
        //        worksheet.Cells[1, 70].Value = "BS";
        //        worksheet.Cells[1, 71].Value = "Type";
        //        worksheet.Cells[1, 72].Value = "WBS element";





        //        int recordIndex = 2;
        //        int count = items.Count();

        //        foreach (var item in items)
        //        {
        //            rowNumber++;

        //            int diff = recordIndex - count;

        //            if (diff > 0)
        //            {
        //                diff = 0;
        //            }


        //            worksheet.Cells[recordIndex, 1].Value = item.Asset.InvNo;
        //            worksheet.Cells[recordIndex, 2].Value = item.Asset.Quantity;
        //            worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
        //            worksheet.Cells[recordIndex, 4].Value = item.Asset.Account != null ? item.Asset.Account.Name : ""; ;
        //            worksheet.Cells[recordIndex, 5].Value = item.Asset.ExpAccount != null ? item.Asset.ExpAccount.Name : "";
        //            worksheet.Cells[recordIndex, 6].Value = item.Asset.AssetCategory != null ? item.Asset.AssetCategory.Name : "";
        //            worksheet.Cells[recordIndex, 7].Value = item.Asset.Name;
        //            worksheet.Cells[recordIndex, 8].Value = item.Asset.SAPCode;
        //            worksheet.Cells[recordIndex, 9].Value = "";
        //            worksheet.Cells[recordIndex, 10].Value = item.Asset.SerialNumber;
        //            worksheet.Cells[recordIndex, 11].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.RegistryNumber : "";
        //            worksheet.Cells[recordIndex, 12].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.Name : "";
        //            worksheet.Cells[recordIndex, 13].Value = item.Asset.AgreementNo;
        //            worksheet.Cells[recordIndex, 14].Value = item.Asset.Manufacturer;
        //            worksheet.Cells[recordIndex, 15].Value = item.Asset.Article != null ? item.Asset.Article.Name : "";
        //            worksheet.Cells[recordIndex, 16].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
        //            worksheet.Cells[recordIndex, 17].Value = item.Asset.PurchaseDate;
        //            worksheet.Cells[recordIndex, 17].Style.Numberformat.Format = "yyyy-mm-dd";
        //            worksheet.Cells[recordIndex, 18].Value = item.Asset.InvoiceDate;
        //            worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "yyyy-mm-dd";
        //            worksheet.Cells[recordIndex, 19].Value = item.Asset.RemovalDate;
        //            worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "yyyy-mm-dd";
        //            worksheet.Cells[recordIndex, 20].Value = item.Dep.ExpLifeInPeriods;
        //            //worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 21].Value = item.Dep.UsefulLife;
        //            //worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 22].Value = item.Dep.TotLifeInpPeriods;
        //            //worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 23].Value = item.Dep.RemLifeInPeriods;
        //            //worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 24].Value = item.Dep.APCFYStart;
        //            worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 25].Value = item.Dep.DepFYStart;
        //            worksheet.Cells[recordIndex, 25].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 26].Value = item.Dep.BkValFYStart;
        //            worksheet.Cells[recordIndex, 26].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 27].Value = item.Dep.Acquisition;
        //            worksheet.Cells[recordIndex, 27].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 28].Value = item.Dep.DepForYear;
        //            worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 29].Value = item.Dep.DepRetirement;
        //            worksheet.Cells[recordIndex, 29].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 30].Value = item.Dep.Retirement;
        //            worksheet.Cells[recordIndex, 30].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 31].Value = item.Dep.CurrBkValue;
        //            worksheet.Cells[recordIndex, 31].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 32].Value = item.Dep.DepTransfer;
        //            worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 33].Value = item.Dep.Transfer;
        //            worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 34].Value = item.Dep.PosCap;
        //            worksheet.Cells[recordIndex, 34].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 35].Value = item.Dep.DepPostCap;
        //            worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 36].Value = item.Dep.InvestSupport;
        //            worksheet.Cells[recordIndex, 36].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 37].Value = item.Dep.WriteUps;
        //            worksheet.Cells[recordIndex, 37].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 38].Value = item.Dep.CurrentAPC;
        //            worksheet.Cells[recordIndex, 38].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 39].Value = item.Dep.AccumulDep;
        //            worksheet.Cells[recordIndex, 39].Style.Numberformat.Format = "#,##0.00";
        //            worksheet.Cells[recordIndex, 40].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Name : "";
        //            worksheet.Cells[recordIndex, 41].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.ERPCode : "";
        //            worksheet.Cells[recordIndex, 42].Value = item.Asset.Department != null ? item.Asset.Department.Name : "";
        //            worksheet.Cells[recordIndex, 43].Value = item.Asset.Division != null ? item.Asset.Division.Name : "";
        //            worksheet.Cells[recordIndex, 44].Value = item.Asset.Room != null ? item.Asset.Room.Name : "";
        //            worksheet.Cells[recordIndex, 45].Value = item.Asset.Administration != null ? item.Asset.Administration.Name : "";
        //            worksheet.Cells[recordIndex, 46].Value = item.Asset.Room != null && item.Asset.Room.Location != null && item.Asset.Room.Location.City != null ? item.Asset.Room.Location.City.Name : "";
        //            worksheet.Cells[recordIndex, 47].Value = item.Asset.Room != null && item.Asset.Room.Location != null && item.Asset.Room.Location.City != null && item.Asset.Room.Location.City.County != null ? item.Asset.Room.Location.City.County.Name : "";
        //            worksheet.Cells[recordIndex, 48].Value = item.Asset.Room != null && item.Asset.Room.Location != null && item.Asset.Room.Location.City != null && item.Asset.Room.Location.City.County != null && item.Asset.Room.Location.City.County.Country != null ? item.Asset.Room.Location.City.County.Country.Name : "";
        //            worksheet.Cells[recordIndex, 49].Value = item.Asset.BudgetManager != null ? item.Asset.BudgetManager.Name : "";
        //            worksheet.Cells[recordIndex, 50].Value = item.Asset.AssetNature != null ? item.Asset.AssetNature.Name : "";
        //            worksheet.Cells[recordIndex, 51].Value = item.Asset.SubType != null && item.Asset.SubType.Type != null ? item.Asset.SubType.Type.Name : "";
        //            worksheet.Cells[recordIndex, 52].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
        //            worksheet.Cells[recordIndex, 53].Value = item.Asset.Material != null ? item.Asset.Material.Code : "";
        //            worksheet.Cells[recordIndex, 54].Value = item.Asset.Material != null ? item.Asset.Material.Name : "";
        //            worksheet.Cells[recordIndex, 55].Value = item.Asset.InterCompany != null ? item.Asset.InterCompany.Name : "";
        //            worksheet.Cells[recordIndex, 56].Value = item.Asset.Employee != null ? item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName : "";
        //            worksheet.Cells[recordIndex, 57].Value = "";
        //            worksheet.Cells[recordIndex, 58].Value = "";
        //            worksheet.Cells[recordIndex, 59].Value = "";
        //            worksheet.Cells[recordIndex, 60].Value = "";
        //            worksheet.Cells[recordIndex, 61].Value = "";
        //            worksheet.Cells[recordIndex, 62].Value = "";
        //            worksheet.Cells[recordIndex, 63].Value = "";
        //            worksheet.Cells[recordIndex, 64].Value = item.Asset.SubType != null ? item.Asset.SubType.Name : "";
        //            worksheet.Cells[recordIndex, 65].Value = "";
        //            worksheet.Cells[recordIndex, 66].Value = item.Asset.ERPCode;
        //            worksheet.Cells[recordIndex, 67].Value = item.Adm.AssetClass != null ? item.Adm.AssetClass.Name : "";
        //            worksheet.Cells[recordIndex, 68].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Name : "";
        //            worksheet.Cells[recordIndex, 69].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.Region != null ? item.Asset.CostCenter.Region.Name : "";
        //            worksheet.Cells[recordIndex, 70].Value = item.Asset.InsuranceCategory != null ? item.Asset.InsuranceCategory.Name : "";
        //            worksheet.Cells[recordIndex, 71].Value = item.Asset.AssetType != null ? item.Asset.AssetType.Name : "";
        //            worksheet.Cells[recordIndex, 72].Value = item.Asset.Project != null ? item.Asset.Project.Name : "";

        //            if (diff == 0)
        //            {



        //                for (int i = 1; i < 73; i++)
        //                {
        //                    worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                    worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
        //                    worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

        //                }

        //                worksheet.Row(1).Height = 35.00;
        //                worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                worksheet.View.FreezePanes(2, 1);

        //                using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 72])
        //                {
        //                    cells.Style.Font.Bold = false;
        //                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
        //                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                    cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

        //                }







        //                using (var cells = worksheet.Cells[1, 1, 1, 72])
        //                {
        //                    cells.Style.Font.Bold = true;
        //                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
        //                    cells.Style.Font.Color.SetColor(Color.Black);
        //                }

        //                using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 72])
        //                {
        //                    cells.Style.Font.Bold = false;
        //                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                    cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
        //                }

        //                using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 72])
        //                {
        //                    for (int i = 2; i < items.Count() + 2; i++)
        //                    {
        //                        worksheet.Row(i).Height = 15.00;
        //                        worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //                        worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //                        worksheet.Cells[$"A{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"B{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"C{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"D{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"E{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"F{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"G{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"H{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"I{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"J{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"K{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"L{i}"].Style.WrapText = true;
        //                        worksheet.Cells[$"M{i}"].Style.WrapText = true;


        //                    }



        //                }


        //                worksheet.View.ShowGridLines = false;
        //                worksheet.View.ZoomScale = 100;

        //                for (int i = 1; i < 73; i++)
        //                {
        //                    worksheet.Column(i).AutoFit();
        //                }

        //                worksheet.Column(1).Width = 12.00;
        //                worksheet.Column(2).Width = 10.00;
        //                worksheet.Column(3).Width = 14.00;
        //                worksheet.Column(4).Width = 20.00;
        //                worksheet.Column(5).Width = 15.00;
        //                worksheet.Column(6).Width = 15.00;
        //                worksheet.Column(7).Width = 60.00;
        //                worksheet.Column(8).Width = 30.00;
        //                worksheet.Column(9).Width = 25.00;
        //                worksheet.Column(10).Width = 25.00;
        //                worksheet.Column(11).Width = 12.00;
        //                worksheet.Column(12).Width = 40.00;
        //                worksheet.Column(13).Width = 20.00;
        //                worksheet.Column(14).Width = 30.00;
        //                worksheet.Column(15).Width = 15.00;
        //                worksheet.Column(16).Width = 30.00;
        //                worksheet.Column(17).Width = 12.00;
        //                worksheet.Column(18).Width = 15.00;
        //                worksheet.Column(19).Width = 15.00;
        //                worksheet.Column(20).Width = 15.00;

        //                worksheet.Column(21).Width = 15.00;
        //                worksheet.Column(22).Width = 15.00;
        //                worksheet.Column(23).Width = 15.00;
        //                worksheet.Column(24).Width = 15.00;
        //                worksheet.Column(25).Width = 15.00;
        //                worksheet.Column(26).Width = 15.00;
        //                worksheet.Column(27).Width = 15.00;
        //                worksheet.Column(28).Width = 15.00;
        //                worksheet.Column(29).Width = 15.00;
        //                worksheet.Column(30).Width = 15.00;


        //                worksheet.Column(31).Width = 15.00;
        //                worksheet.Column(32).Width = 15.00;
        //                worksheet.Column(33).Width = 15.00;
        //                worksheet.Column(34).Width = 15.00;
        //                worksheet.Column(35).Width = 15.00;
        //                worksheet.Column(36).Width = 15.00;
        //                worksheet.Column(37).Width = 15.00;
        //                worksheet.Column(38).Width = 15.00;
        //                worksheet.Column(39).Width = 15.00;
        //                worksheet.Column(40).Width = 30.00;

        //                worksheet.Column(41).Width = 30.00;
        //                worksheet.Column(42).Width = 30.00;
        //                worksheet.Column(43).Width = 15.00;
        //                worksheet.Column(44).Width = 60.00;
        //                worksheet.Column(45).Width = 15.00;
        //                worksheet.Column(46).Width = 15.00;
        //                worksheet.Column(47).Width = 15.00;
        //                worksheet.Column(48).Width = 15.00;
        //                worksheet.Column(49).Width = 10.00;
        //                worksheet.Column(50).Width = 45.00;

        //                worksheet.Column(51).Width = 40.00;
        //                worksheet.Column(52).Width = 10.00;
        //                worksheet.Column(53).Width = 15.00;
        //                worksheet.Column(54).Width = 180.00;
        //                worksheet.Column(55).Width = 25.00;
        //                worksheet.Column(56).Width = 15.00;
        //                worksheet.Column(57).Width = 15.00;
        //                worksheet.Column(58).Width = 15.00;
        //                worksheet.Column(59).Width = 15.00;
        //                worksheet.Column(60).Width = 15.00;

        //                worksheet.Column(61).Width = 15.00;
        //                worksheet.Column(62).Width = 15.00;
        //                worksheet.Column(63).Width = 15.00;
        //                worksheet.Column(64).Width = 25.00;
        //                worksheet.Column(65).Width = 31.00;
        //                worksheet.Column(66).Width = 30.00;
        //                worksheet.Column(67).Width = 15.00;
        //                worksheet.Column(68).Width = 15.00;
        //                worksheet.Column(69).Width = 30.00;
        //                worksheet.Column(70).Width = 10.00;
        //                worksheet.Column(71).Width = 25.00;
        //                worksheet.Column(72).Width = 30.00;


        //                worksheet.Cells["A1:BT1"].AutoFilter = true;

        //            }

        //            recordIndex++;
        //        }


        //        using (var cells = worksheet.Cells[1, 1, 1, 72])
        //        {
        //            cells.Style.Font.Bold = true;
        //            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

        //        }

        //        string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        //HttpContext.Response.ContentType = entityFile.FileType;
        //        HttpContext.Response.ContentType = contentType;
        //        FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
        //        {
        //            FileDownloadName = "Export.xlsx"
        //        };

        //        return result;

        //    }
        //}

        [HttpGet("templateImportEmag")]
        public async Task<IActionResult> TemplateeMAG()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook

                int rowIndex = 0;

                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Template_Import_Far_Dante");
                //First add the headers
                worksheet.Cells[1, ++rowIndex].Value = "Asset";
                worksheet.Cells[1, ++rowIndex].Value = "Sub-number";
                worksheet.Cells[1, ++rowIndex].Value = "AssetS";
                worksheet.Cells[1, ++rowIndex].Value = "Bal.sh.acct APC";
                worksheet.Cells[1, ++rowIndex].Value = "Asset Class";
                worksheet.Cells[1, ++rowIndex].Value = "Asset Clasif.";
                worksheet.Cells[1, ++rowIndex].Value = "Asset Description";
                worksheet.Cells[1, ++rowIndex].Value = "Inventory Number";
                worksheet.Cells[1, ++rowIndex].Value = "Serial Number";
                worksheet.Cells[1, ++rowIndex].Value = "Vendor";
                worksheet.Cells[1, ++rowIndex].Value = "Supplier Name";
                worksheet.Cells[1, ++rowIndex].Value = "Manufacturer of Asset";
                worksheet.Cells[1, ++rowIndex].Value = "Location";
                worksheet.Cells[1, ++rowIndex].Value = "Cost Center";
                worksheet.Cells[1, ++rowIndex].Value = "Name CC";
                worksheet.Cells[1, ++rowIndex].Value = "Capitalized on";
                worksheet.Cells[1, ++rowIndex].Value = "Ord.Dep.Start Date";
                worksheet.Cells[1, ++rowIndex].Value = "Deactivation On";
                worksheet.Cells[1, ++rowIndex].Value = "Useful Life";
                worksheet.Cells[1, ++rowIndex].Value = "Tot. Life in Periods";
                worksheet.Cells[1, ++rowIndex].Value = "Exp.Life in Periods";
                worksheet.Cells[1, ++rowIndex].Value = "Rem.Life in Periods";
                worksheet.Cells[1, ++rowIndex].Value = "APC FY Start";
                worksheet.Cells[1, ++rowIndex].Value = "Dep. FY Start";
                worksheet.Cells[1, ++rowIndex].Value = "Bk.Val.FY Strt";
                worksheet.Cells[1, ++rowIndex].Value = "Acquisition";
                worksheet.Cells[1, ++rowIndex].Value = "Dep. For Year";
                worksheet.Cells[1, ++rowIndex].Value = "Retirement";
                worksheet.Cells[1, ++rowIndex].Value = "Dep.Retir.";
                worksheet.Cells[1, ++rowIndex].Value = "Curr.Bk.Val.";
                worksheet.Cells[1, ++rowIndex].Value = "Transfer";
                worksheet.Cells[1, ++rowIndex].Value = "Dep.Transfer";
                worksheet.Cells[1, ++rowIndex].Value = "Post-Capital.";
                worksheet.Cells[1, ++rowIndex].Value = "Dep.Post-Cap.";
                worksheet.Cells[1, ++rowIndex].Value = "Invest.Support";
                worksheet.Cells[1, ++rowIndex].Value = "Write-ups";
                worksheet.Cells[1, ++rowIndex].Value = "Current APC";
                worksheet.Cells[1, ++rowIndex].Value = "Accumul. dep.";
                worksheet.Cells[1, ++rowIndex].Value = "Identity Number";
                worksheet.Cells[1, ++rowIndex].Value = "Personnel Number";
                worksheet.Cells[1, ++rowIndex].Value = "Profit Center";
                worksheet.Cells[1, ++rowIndex].Value = "Usef.life in periods";
                worksheet.Cells[1, ++rowIndex].Value = "Locatie Initiala";

                worksheet.View.ZoomScale = 100;

                for(int i= 1; i <= rowIndex; i++)
                {
                    worksheet.Column(i).Width = 20.00;
                }

                worksheet.Row(1).Height = 30.00;

                worksheet.Cells["A1:AQ1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:AQ1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A1:AQ1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A1:AQ1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 192, 192));

                worksheet.Cells["C1"].Style.Fill.PatternType = ExcelFillStyle.Solid;  worksheet.Cells["C1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 176, 240));
                worksheet.Cells["AD1"].Style.Fill.PatternType = ExcelFillStyle.Solid;  worksheet.Cells["AD1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                worksheet.Cells["AK1"].Style.Fill.PatternType = ExcelFillStyle.Solid;  worksheet.Cells["AK1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
                worksheet.Cells["AQ1"].Style.Fill.PatternType = ExcelFillStyle.Solid;  worksheet.Cells["AQ1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Template_Import_Far_Dante.xlsx"
                };

                return result;
            }
        }

        [HttpGet("exportEmag")]
        public async Task<IActionResult> ExporteMAG(string jsonFilter, string propertyFilters, int? bfId)
        {
            List<Model.AssetMonthDetailExport> items = null;
            AssetFilter assetFilter = null;
            List<PropertyFilter> propFilters = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            string userName = string.Empty;
            string role = string.Empty;
            int? costCenterId = null;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
            propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);

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

            if(assetFilter.Export == "inuse")
			{
                items = (_itemsRepository as IAssetsRepository)
                .GetMonthExportInUse(assetFilter, propFilters).ToList();
            }
            else if (assetFilter.Export == "stockhistory")
            {
                items = (_itemsRepository as IAssetsRepository)
                .GetMonthExportStockHistory(assetFilter, bfId, propFilters).ToList();
            }
			else if (assetFilter.Export == "receptionhistory")
			{
				items = (_itemsRepository as IAssetsRepository)
				.GetMonthExportReceptionHistoryInUse(assetFilter, propFilters).ToList();
			}
            else if (assetFilter.Export == "scrap")
            {
                items = (_itemsRepository as IAssetsRepository)
                .GetMonthExportScrap(assetFilter, propFilters).ToList();
            }

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                int columnIndex = 1;

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Emag");
                //First add the headers
                worksheet.Cells[1, columnIndex++].Value = "Asset";
                worksheet.Cells[1, columnIndex++].Value = "Subnumber";
                worksheet.Cells[1, columnIndex++].Value = "Bal.sh.acct APC";
                worksheet.Cells[1, columnIndex++].Value = "Asset Class";
                worksheet.Cells[1, columnIndex++].Value = "Asset clasif.";
                worksheet.Cells[1, columnIndex++].Value = "Asset description";
                worksheet.Cells[1, columnIndex++].Value = "Inventorynumber";
                worksheet.Cells[1, columnIndex++].Value = "Serial number";
                worksheet.Cells[1, columnIndex++].Value = "Vendor";
                worksheet.Cells[1, columnIndex++].Value = "Supplier name";
                worksheet.Cells[1, columnIndex++].Value = "Manufacturer of asset";
                worksheet.Cells[1, columnIndex++].Value = "Location";
                worksheet.Cells[1, columnIndex++].Value = "Cost Center";
                worksheet.Cells[1, columnIndex++].Value = "Name CC";
                worksheet.Cells[1, columnIndex++].Value = "Capitalized on";
                worksheet.Cells[1, columnIndex++].Value = "Ord.dep.start date";
                worksheet.Cells[1, columnIndex++].Value = "Deactivation on";
                worksheet.Cells[1, columnIndex++].Value = "Useful life";
                worksheet.Cells[1, columnIndex++].Value = "Tot. life in periods";
                worksheet.Cells[1, columnIndex++].Value = "Exp.life in periods";
                worksheet.Cells[1, columnIndex++].Value = "Rem.life in periods";
                worksheet.Cells[1, columnIndex++].Value = "APC FY start";
                worksheet.Cells[1, columnIndex++].Value = "Dep. FY start";
                worksheet.Cells[1, columnIndex++].Value = "Bk.val.FY strt";
                worksheet.Cells[1, columnIndex++].Value = "Acquisition";
                worksheet.Cells[1, columnIndex++].Value = "Dep. for year";
                worksheet.Cells[1, columnIndex++].Value = "Retirement";
                worksheet.Cells[1, columnIndex++].Value = "Dep.retir.";
                worksheet.Cells[1, columnIndex++].Value = "Curr.bk.val.";
                worksheet.Cells[1, columnIndex++].Value = "Transfer";
                worksheet.Cells[1, columnIndex++].Value = "Dep.transfer";
                worksheet.Cells[1, columnIndex++].Value = "Post-capital.";
                worksheet.Cells[1, columnIndex++].Value = "Dep.post-cap.";
                worksheet.Cells[1, columnIndex++].Value = "Invest.support";
                worksheet.Cells[1, columnIndex++].Value = "Write-ups";
                worksheet.Cells[1, columnIndex++].Value = "Current APC";
                worksheet.Cells[1, columnIndex++].Value = "Accumul. dep.";
				worksheet.Cells[1, columnIndex++].Value = "SAP/Optima";
				worksheet.Cells[1, columnIndex++].Value = "Cod P.R.";
				worksheet.Cells[1, columnIndex++].Value = "Cod Oferta";
				worksheet.Cells[1, columnIndex++].Value = "Cod P.O.";
				worksheet.Cells[1, columnIndex++].Value = "Owner receptie";

                if (assetFilter.Export == "stockhistory")
                {
					worksheet.Cells[1, columnIndex++].Value = "Material";
					worksheet.Cells[1, columnIndex++].Value = "Referinta BGT";
					worksheet.Cells[1, columnIndex++].Value = "WBS";
				}

                worksheet.Cells[1, columnIndex++].Value = "Business Unit";
                worksheet.Cells[1, columnIndex++].Value = "Departament";
                worksheet.Cells[1, columnIndex++].Value = "Profit Center";
                worksheet.Cells[1, columnIndex++].Value = "Marca Employee";
                worksheet.Cells[1, columnIndex++].Value = "First Name Employee";
                worksheet.Cells[1, columnIndex++].Value = "Last Name Employee";
                worksheet.Cells[1, columnIndex++].Value = "Employee CC";

                int recordIndex = 2;
                int count = items.Count();
                int rowCell = items.Count() + 2;
                int rowTotal = items.Count() + 1;
                int columnCounter = columnIndex - 1;

                if (items.Count==0 && assetFilter.Export == "scrap")
                {
                    return new StatusCodeResult(418);
                }
                else
                {
                    for (int a = 0; a < items.Count; a++)
                    {
                        rowNumber++;
                        int diff = a - count;
                        columnIndex = 1;

                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].InvNo;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].SubNo;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Account;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].ExpAccount;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].AssetCategory;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Description;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].ErpCode;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].SerialNumber;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Partner;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].PartnerName;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Manufacturier;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Location;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].CostCenterCode;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].CostCenterName;
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].PurchaseDate; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "mm/dd/yyyy";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].InvoiceDate; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "mm/dd/yyyy";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].RemovalDate; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "mm/dd/yyyy";
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].UsefulLife;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].TotLifeInpPeriods;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].ExpLifeInPeriods;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].RemLifeInPeriods;
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].APCFYStart; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].DepFYStart; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].BkValFYStart; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].Acquisition; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].DepForYear; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].Retirement; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].DepRetirement; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].CurrBkValue; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].Transfer; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].DepTransfer; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].PosCap; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].DepPostCap; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].InvestSupport; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].WriteUps; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].CurrentAPC; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex].Value = items[a].AccumulDep; worksheet.Cells[recordIndex, columnIndex++].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Source; 
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].RequestCode;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].OfferCode;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].OrderCode;
                        if (items[a].ReceptionEmail is not null && items[a].ReceptionEmail != "")
                        {
                            var user = await userManager.FindByIdAsync(items[a].ReceptionEmail);

                            if (user != null)
                            {
                                worksheet.Cells[recordIndex, columnIndex++].Value = user.Email;
                            }
                        }
                        else
                            worksheet.Cells[recordIndex, columnIndex++].Value = "";


                        if (assetFilter.Export == "stockhistory")
                        {
                            worksheet.Cells[recordIndex, columnIndex++].Value = items[a].Material;
                            worksheet.Cells[recordIndex, columnIndex++].Value = items[a].BudgetBaseCode;
                            worksheet.Cells[recordIndex, columnIndex++].Value = items[a].ProjectCode;
                        }

                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].DepartmentName;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].DivisionName;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].ProfitCenterCode;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].EmployeeInternalCode;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].EmployeeFirstName;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].EmployeeLastName;
                        worksheet.Cells[recordIndex, columnIndex++].Value = items[a].EmployeeCostCenterCode;

                        if (diff == -1)
                        {
                            for (int i = 1; i <= columnCounter; i++)
                            {
                                worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                                worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 9));

                            }

                            worksheet.Row(1).Height = 35.00;
                            worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.View.FreezePanes(2, 1);

                            using (var cells = worksheet.Cells[1, 1, 1, columnCounter])
                            {
                                cells.Style.Font.Bold = true;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                                cells.Style.Font.Color.SetColor(Color.Black);
                            }

                            using (var cells = worksheet.Cells[2, 1, items.Count() + 1, columnCounter])
                            {
                                cells.Style.Font.Bold = false;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cells.Style.Font.SetFromFont(new Font("Times New Roman", 9));
                            }

                            worksheet.View.ShowGridLines = false;
                            worksheet.View.ZoomScale = 100;

                            for(int i = 1; i<= columnCounter; i++)
                            {
                                worksheet.Column(i).AutoFit();
                            }

                            worksheet.Cells["A1:AK1"].AutoFilter = true;

                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Merge = true;
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Value = "TOTAL";
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Formula = "SUM(V2:V" + rowTotal + ")";
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["V" + rowCell + ":V" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Formula = "SUM(W2:W" + rowTotal + ")";
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["W" + rowCell + ":W" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Formula = "SUM(X2:X" + rowTotal + ")";
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["X" + rowCell + ":X" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Formula = "SUM(Y2:Y" + rowTotal + ")";
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["Y" + rowCell + ":Y" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Formula = "SUM(Z2:Z" + rowTotal + ")";
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["Z" + rowCell + ":Z" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Formula = "SUM(AA2:AA" + rowTotal + ")";
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AA" + rowCell + ":AA" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Formula = "SUM(AB2:AB" + rowTotal + ")";
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AB" + rowCell + ":AB" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Formula = "SUM(AC2:AC" + rowTotal + ")";
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AC" + rowCell + ":AC" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Formula = "SUM(AD2:AD" + rowTotal + ")";
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AD" + rowCell + ":AD" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Formula = "SUM(AE2:AE" + rowTotal + ")";
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AE" + rowCell + ":AE" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Formula = "SUM(AF2:AF" + rowTotal + ")";
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AF" + rowCell + ":AF" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Formula = "SUM(AG2:AG" + rowTotal + ")";
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AG" + rowCell + ":AG" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Formula = "SUM(AH2:AH" + rowTotal + ")";
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AH" + rowCell + ":AH" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Formula = "SUM(AI2:AI" + rowTotal + ")";
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AI" + rowCell + ":AI" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Formula = "SUM(AJ2:AJ" + rowTotal + ")";
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AJ" + rowCell + ":AJ" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Formula = "SUM(AK2:AK" + rowTotal + ")";
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Font.Bold = true;
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Numberformat.Format = "#,##0.00";
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
                            worksheet.Cells["AK" + rowCell + ":AK" + rowCell].Style.Font.Color.SetColor(Color.Black);

                            package.Workbook.Calculate();

                        }

                        recordIndex++;
                    }
                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

		[HttpGet("exportReceptionHistory")]
		public async Task<IActionResult> ExporteReceptionHistory(string jsonFilter, string propertyFilters, int? bfId)
		{
			List<Model.AssetMonthDetailExport> items = null;
			AssetFilter assetFilter = null;
			List<PropertyFilter> propFilters = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			int rowNumber = 0;
			string userName = string.Empty;
			string role = string.Empty;
			int? costCenterId = null;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
			propFilters = propertyFilters != null ? JsonConvert.DeserializeObject<List<PropertyFilter>>(propertyFilters) : new List<PropertyFilter>();


			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);

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

			items = (_itemsRepository as IAssetsRepository)
				.GetMonthExportReceptionHistoryInUse(assetFilter, propFilters).ToList();

			using (ExcelPackage package = new ExcelPackage())
			{

				assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Istoric receptii");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Asset";
				worksheet.Cells[1, 2].Value = "Subnumber";
				worksheet.Cells[1, 3].Value = "Asset Class";
				worksheet.Cells[1, 4].Value = "Asset description";
				worksheet.Cells[1, 5].Value = "Cost Center";
				worksheet.Cells[1, 6].Value = "Name CC";
				worksheet.Cells[1, 7].Value = "Capitalized on";
				worksheet.Cells[1, 8].Value = "Ord.dep.start date";
				worksheet.Cells[1, 9].Value = "Deactivation on";
				worksheet.Cells[1, 10].Value = "Useful life";
				worksheet.Cells[1, 11].Value = "Curr.bk.val.";
				worksheet.Cells[1, 12].Value = "Current APC";
				worksheet.Cells[1, 13].Value = "Accumul. dep.";

				worksheet.Cells[1, 14].Value = "Cod WBS";
				worksheet.Cells[1, 15].Value = "Nr. Contract";
				worksheet.Cells[1, 16].Value = "Furnizor";

				worksheet.Cells[1, 17].Value = "Cod P.R.";
				worksheet.Cells[1, 18].Value = "Data creare";


				worksheet.Cells[1, 19].Value = "Cod Oferta";
				worksheet.Cells[1, 20].Value = "Data creare";

				worksheet.Cells[1, 21].Value = "Cod P.O.";
				worksheet.Cells[1, 22].Value = "Data creare";
				worksheet.Cells[1, 23].Value = "Data aprobare";

				worksheet.Cells[1, 24].Value = "Data receptie";
				worksheet.Cells[1, 25].Value = "Owner receptie";
				

			
				int recordIndex = 2;
				int count = items.Count();
				int rowCell = items.Count() + 2;
				int rowTotal = items.Count() + 1;

				for (int a = 0; a < items.Count; a++)
				{
					rowNumber++;
					int diff = a - count;

					worksheet.Cells[recordIndex, 1].Value = items[a].InvNo;
					worksheet.Cells[recordIndex, 2].Value = items[a].SubNo;
					//worksheet.Cells[recordIndex, 3].Value = items[a].Account;
					worksheet.Cells[recordIndex, 3].Value = items[a].ExpAccount;
					//worksheet.Cells[recordIndex, 5].Value = items[a].AssetCategory;
					worksheet.Cells[recordIndex, 4].Value = items[a].Description;
					//worksheet.Cells[recordIndex, 7].Value = items[a].ErpCode;
					//worksheet.Cells[recordIndex, 8].Value = items[a].SerialNumber;
					//worksheet.Cells[recordIndex, 9].Value = items[a].Partner;
					//worksheet.Cells[recordIndex, 10].Value = items[a].PartnerName;
					//worksheet.Cells[recordIndex, 11].Value = items[a].Manufacturier;
					//worksheet.Cells[recordIndex, 12].Value = items[a].Location;
					worksheet.Cells[recordIndex, 5].Value = items[a].CostCenterCode;
					worksheet.Cells[recordIndex, 6].Value = items[a].CostCenterName;
					worksheet.Cells[recordIndex, 7].Value = items[a].PurchaseDate;
					worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 8].Value = items[a].InvoiceDate;
					worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 9].Value = items[a].RemovalDate;
					worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 10].Value = items[a].UsefulLife;
					//worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 19].Value = items[a].TotLifeInpPeriods;
					////worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 20].Value = items[a].ExpLifeInPeriods;
					////worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 21].Value = items[a].RemLifeInPeriods;
					////worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 22].Value = items[a].APCFYStart;
					//worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 23].Value = items[a].DepFYStart;
					//worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 24].Value = items[a].BkValFYStart;
					//worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 25].Value = items[a].Acquisition;
					//worksheet.Cells[recordIndex, 25].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 26].Value = items[a].DepForYear;
					//worksheet.Cells[recordIndex, 26].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 27].Value = items[a].Retirement;
					//worksheet.Cells[recordIndex, 27].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 28].Value = items[a].DepRetirement;
					//worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 11].Value = items[a].CurrBkValue;
					worksheet.Cells[recordIndex, 11].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 30].Value = items[a].Transfer;
					//worksheet.Cells[recordIndex, 30].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 31].Value = items[a].DepTransfer;
					//worksheet.Cells[recordIndex, 31].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 32].Value = items[a].PosCap;
					//worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 33].Value = items[a].DepPostCap;
					//worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 34].Value = items[a].InvestSupport;
					//worksheet.Cells[recordIndex, 34].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 35].Value = items[a].WriteUps;
					//worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 12].Value = items[a].CurrentAPC;
					worksheet.Cells[recordIndex, 12].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 13].Value = items[a].AccumulDep;
					worksheet.Cells[recordIndex, 13].Style.Numberformat.Format = "#,##0.00";
					//worksheet.Cells[recordIndex, 38].Value = items[a].Source;

					worksheet.Cells[recordIndex, 14].Value = items[a].ProjectCode;
					worksheet.Cells[recordIndex, 15].Value = items[a].ContractID;
					worksheet.Cells[recordIndex, 16].Value = items[a].PartnerName;
					
					worksheet.Cells[recordIndex, 17].Value = items[a].RequestCode;
					worksheet.Cells[recordIndex, 18].Value = items[a].RequestDate;
					worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "mm/dd/yyyy";

					worksheet.Cells[recordIndex, 19].Value = items[a].OfferCode;
					worksheet.Cells[recordIndex, 20].Value = items[a].OfferDate;
					worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "mm/dd/yyyy";

					worksheet.Cells[recordIndex, 21].Value = items[a].OrderCode;
					worksheet.Cells[recordIndex, 22].Value = items[a].OrderDate;
					worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "mm/dd/yyyy";
					worksheet.Cells[recordIndex, 23].Value = items[a].OrderEndDate;
					worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "mm/dd/yyyy";

					worksheet.Cells[recordIndex, 24].Value = items[a].ReceptionDate;
					worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "mm/dd/yyyy";

					if (items[a].ReceptionEmail != "")
					{
						var user = await userManager.FindByIdAsync(items[a].ReceptionEmail);

						if (user != null)
						{
							worksheet.Cells[recordIndex, 25].Value = user.Email;
						}
					}

					

					if (diff == -1)
					{
						for (int i = 1; i < 26; i++)
						{
							worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 9));

						}

						worksheet.Row(1).Height = 35.00;
						worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.View.FreezePanes(2, 1);

						using (var cells = worksheet.Cells[1, 1, 1,25])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 1, 25])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Font.SetFromFont(new Font("Times New Roman", 9));
						}

						worksheet.View.ShowGridLines = false;
						worksheet.View.ZoomScale = 100;

						worksheet.Column(1).Width = 12.00;
						worksheet.Column(2).Width = 10.00;
						worksheet.Column(3).Width = 14.00;
						worksheet.Column(4).Width = 20.00;
						worksheet.Column(5).Width = 15.00;
						worksheet.Column(6).Width = 55.00;
						worksheet.Column(7).Width = 60.00;
						worksheet.Column(8).Width = 30.00;
						worksheet.Column(9).Width = 25.00;
						worksheet.Column(10).Width = 25.00;
						worksheet.Column(11).Width = 12.00;
						worksheet.Column(12).Width = 40.00;
						worksheet.Column(13).Width = 20.00;
						worksheet.Column(14).Width = 30.00;
						worksheet.Column(15).Width = 15.00;
						worksheet.Column(16).Width = 30.00;
						worksheet.Column(17).Width = 12.00;
						worksheet.Column(18).Width = 15.00;
						worksheet.Column(19).Width = 15.00;
						worksheet.Column(20).Width = 15.00;
						worksheet.Column(21).Width = 15.00;
						worksheet.Column(22).Width = 15.00;
						worksheet.Column(23).Width = 15.00;
						worksheet.Column(24).Width = 15.00;
						worksheet.Column(25).Width = 15.00;


						worksheet.Cells["A1:Y1"].AutoFilter = true;

						worksheet.Cells["A" + rowCell + ":U" + rowCell].Merge = true;
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Value = "TOTAL";
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
						worksheet.Cells["A" + rowCell + ":U" + rowCell].Style.Font.Color.SetColor(Color.Black);

						worksheet.Cells["K" + rowCell + ":K" + rowCell].Formula = "SUM(K2:K" + rowTotal + ")";
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
						worksheet.Cells["K" + rowCell + ":K" + rowCell].Style.Font.Color.SetColor(Color.Black);

						worksheet.Cells["L" + rowCell + ":L" + rowCell].Formula = "SUM(L2:L" + rowTotal + ")";
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
						worksheet.Cells["L" + rowCell + ":L" + rowCell].Style.Font.Color.SetColor(Color.Black);

						worksheet.Cells["M" + rowCell + ":M" + rowCell].Formula = "SUM(M2:M" + rowTotal + ")";
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.Font.Bold = true;
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
						worksheet.Cells["M" + rowCell + ":M" + rowCell].Style.Font.Color.SetColor(Color.Black);

						package.Workbook.Calculate();

					}

					recordIndex++;
				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Export.xlsx"
				};

				return result;

			}
		}

		[HttpGet("exportAccounting")]
        public IActionResult ExportAccounting(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter, string columnFilter)
        {

            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            ColumnAssetFilter colAssetFilters = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();
			colAssetFilters = columnFilter != null ? JsonConvert.DeserializeObject<ColumnAssetFilter>(columnFilter) : new ColumnAssetFilter();

			includes = @"Adm.ExpAccount,Asset.Tax";


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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


            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthInUse(assetFilter, colAssetFilters, includes, sorting, paging, out depTotal).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Registru");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Nr.Crt";
                worksheet.Cells[1, 2].Value = "Asset";
                worksheet.Cells[1, 3].Value = "Bal.sh.acct APC";
                worksheet.Cells[1, 4].Value = "Asset description";
                worksheet.Cells[1, 5].Value = "Data Achizitiei/Fabricarii/Modernizarii";
                worksheet.Cells[1, 6].Value = "Valoarea (Baza Impozabila)";
                worksheet.Cells[1, 7].Value = "Taxa Deductibila";
                worksheet.Cells[1, 8].Value = "Taxa Dedusa";
                worksheet.Cells[1, 9].Value = "Ajustari";
                worksheet.Cells[1, 10].Value = "Cota TVA";

                int recordIndex = 2;
                int count = items.Count();

                foreach (var item in items)
                {
                    rowNumber++;

                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }


                    worksheet.Cells[recordIndex, 1].Value = rowNumber;
                    worksheet.Cells[recordIndex, 2].Value = item.Asset.InvNo;
                    worksheet.Cells[recordIndex, 3].Value = item.Asset.ExpAccount != null ? item.Asset.ExpAccount.Code : "";
                    worksheet.Cells[recordIndex, 4].Value = item.Asset.Name;
                    worksheet.Cells[recordIndex, 5].Value = item.Asset.PurchaseDate;
                    worksheet.Cells[recordIndex, 5].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 6].Value = item.Asset.ValueInv;
                    worksheet.Cells[recordIndex, 6].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 7].Value = item.Asset.TaxAmount;
                    worksheet.Cells[recordIndex, 7].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 8].Value = item.Asset.TaxAmount;
                    worksheet.Cells[recordIndex, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 9].Value = 0.0;
                    worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 10].Value = item.Asset.Tax != null ? item.Asset.Tax.Value: "";
                    worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "#,##0.00";

                    if (diff == 0)
                    {



                        for (int i = 1; i < 11; i++)
                        {
                            worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(1).Height = 35.00;
                        worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(2, 1);

                        using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 10])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        using (var cells = worksheet.Cells[1, 1, 1, 10])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 10])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 10])
                        {
                            for (int i = 2; i < items.Count() + 2; i++)
                            {
                                worksheet.Row(i).Height = 15.00;
                                worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                worksheet.Cells[$"I{i}"].Style.WrapText = true;
                                worksheet.Cells[$"J{i}"].Style.WrapText = true;
                            }



                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

                        for (int i = 1; i < 11; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        worksheet.Column(1).Width = 12.00;
                        worksheet.Column(2).Width = 10.00;
                        worksheet.Column(3).Width = 14.00;
                        worksheet.Column(4).Width = 20.00;
                        worksheet.Column(5).Width = 15.00;
                        worksheet.Column(6).Width = 15.00;
                        worksheet.Column(7).Width = 60.00;
                        worksheet.Column(8).Width = 30.00;
                        worksheet.Column(9).Width = 25.00;
                        worksheet.Column(10).Width = 25.00;


                        worksheet.Cells["A1:J1"].AutoFilter = true;

                    }

                    recordIndex++;
                }


                using (var cells = worksheet.Cells[1, 1, 1, 72])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

        [HttpGet("exportStockIT")]
        public IActionResult ExportStockIT(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {

            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


            includes = @"Asset.InvState,Adm.Account,Adm.ExpAccount,Adm.AssetCategory,Asset.Document.Partner,Asset.Document,Adm.Article,Adm.CostCenter.AdmCenter,Asset.Material,Dep.AccSystem,Adm.Department,Adm.Division,Adm.Room.Location.City.County.Country,Adm.Administration,Adm.BudgetManager,Adm.AssetNature,Adm.SubType.Type,Adm.Employee,Adm.InterCompany,Adm.SubType,Asset,Adm.AssetClass,Adm.CostCenter.Region,Adm.InsuranceCategory,Adm.AssetType,Adm.Project,";


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }


            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockIT(assetFilter, includes, sorting, paging, out depTotal).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("STOCK-IT");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Asset";
                worksheet.Cells[1, 2].Value = "PCS";
                worksheet.Cells[1, 3].Value = "Subnumber";
                worksheet.Cells[1, 4].Value = "Bal.sh.acct APC";
                worksheet.Cells[1, 5].Value = "Asset Class";
                worksheet.Cells[1, 6].Value = "Asset clasif.";
                worksheet.Cells[1, 7].Value = "Asset description";
                worksheet.Cells[1, 8].Value = "Inventorynumber";
                worksheet.Cells[1, 9].Value = "License plate number";
                worksheet.Cells[1, 10].Value = "Serial number";
                worksheet.Cells[1, 11].Value = "Vendor";
                worksheet.Cells[1, 12].Value = "Supplier name";
                worksheet.Cells[1, 13].Value = "Agreement number";

                worksheet.Cells[1, 14].Value = "Manufacturer of asset";
                worksheet.Cells[1, 15].Value = "Location";
                worksheet.Cells[1, 16].Value = "Cost Center";
                worksheet.Cells[1, 17].Value = "Capitalized on";
                worksheet.Cells[1, 18].Value = "Ord.dep.start date";
                worksheet.Cells[1, 19].Value = "Deactivation on";
                worksheet.Cells[1, 20].Value = "Useful life";
                worksheet.Cells[1, 21].Value = "Tot. life in periods";
                worksheet.Cells[1, 22].Value = "Exp.life in periods";
                worksheet.Cells[1, 23].Value = "Rem.life in periods";
                worksheet.Cells[1, 24].Value = "APC FY start";
                worksheet.Cells[1, 25].Value = "Dep. FY start";
                worksheet.Cells[1, 26].Value = "Bk.val.FY strt";

                worksheet.Cells[1, 27].Value = "Acquisition";
                worksheet.Cells[1, 28].Value = "Dep. for year";
                worksheet.Cells[1, 29].Value = "Retirement";
                worksheet.Cells[1, 30].Value = "Dep.retir.";
                worksheet.Cells[1, 31].Value = "Curr.bk.val.";
                worksheet.Cells[1, 32].Value = "Transfer";
                worksheet.Cells[1, 33].Value = "Dep.transfer";
                worksheet.Cells[1, 34].Value = "Post-capital.";
                worksheet.Cells[1, 35].Value = "Dep.post-cap.";
                worksheet.Cells[1, 36].Value = "Invest.support";
                worksheet.Cells[1, 37].Value = "Write-ups";
                worksheet.Cells[1, 38].Value = "Current APC";
                worksheet.Cells[1, 39].Value = "Accumul. dep.";

                worksheet.Cells[1, 40].Value = "Name CC";
                worksheet.Cells[1, 41].Value = "Description CC";
                worksheet.Cells[1, 42].Value = "Departament";
                worksheet.Cells[1, 43].Value = "Divizie";
                worksheet.Cells[1, 44].Value = "Adresa";
                worksheet.Cells[1, 45].Value = "Locatie";
                worksheet.Cells[1, 46].Value = "Oras";
                worksheet.Cells[1, 47].Value = "Judet";
                worksheet.Cells[1, 48].Value = "Tara";
                worksheet.Cells[1, 49].Value = "FY' Start";
                worksheet.Cells[1, 50].Value = "Text lung cont CM";
                worksheet.Cells[1, 51].Value = "Supracategorie";
                worksheet.Cells[1, 52].Value = "Marca P.";
                worksheet.Cells[1, 53].Value = "Material";

                worksheet.Cells[1, 54].Value = "Material Description";
                worksheet.Cells[1, 55].Value = "Supracategorie TRN";
                worksheet.Cells[1, 56].Value = "Username";
                worksheet.Cells[1, 57].Value = "Company";
                worksheet.Cells[1, 58].Value = "CostCenter HR";
                worksheet.Cells[1, 59].Value = "CC Name HR";
                worksheet.Cells[1, 60].Value = "Position HR";
                worksheet.Cells[1, 61].Value = "Business Unit HR";
                worksheet.Cells[1, 62].Value = "Departament HR";
                worksheet.Cells[1, 63].Value = "HireDate HR";
                worksheet.Cells[1, 64].Value = "Tip Assets";
                worksheet.Cells[1, 65].Value = "Mapare Anliza Assets Angajati/WBS";
                worksheet.Cells[1, 66].Value = "Nr. Inventar FAR";

                worksheet.Cells[1, 67].Value = "Clasificare asset";
                worksheet.Cells[1, 68].Value = "Profit Ctr";
                worksheet.Cells[1, 69].Value = "PC Detaliu";
                worksheet.Cells[1, 70].Value = "BS";
                worksheet.Cells[1, 71].Value = "Type";
                worksheet.Cells[1, 72].Value = "WBS element";





                int recordIndex = 2;
                int count = items.Count();

                foreach (var item in items)
                {
                    rowNumber++;

                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }


                    worksheet.Cells[recordIndex, 1].Value = item.Asset.InvNo;
                    worksheet.Cells[recordIndex, 2].Value = item.Asset.Quantity;
                    worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
                    worksheet.Cells[recordIndex, 4].Value = item.Asset.Account != null ? item.Asset.Account.Name : ""; ;
                    worksheet.Cells[recordIndex, 5].Value = item.Asset.ExpAccount != null ? item.Asset.ExpAccount.Name : "";
                    worksheet.Cells[recordIndex, 6].Value = item.Asset.AssetCategory != null ? item.Asset.AssetCategory.Name : "";
                    worksheet.Cells[recordIndex, 7].Value = item.Asset.Name;
                    worksheet.Cells[recordIndex, 8].Value = item.Asset.SAPCode;
                    worksheet.Cells[recordIndex, 9].Value = "";
                    worksheet.Cells[recordIndex, 10].Value = item.Asset.SerialNumber;
                    worksheet.Cells[recordIndex, 11].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.RegistryNumber : "";
                    worksheet.Cells[recordIndex, 12].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.Name : "";
                    worksheet.Cells[recordIndex, 13].Value = item.Asset.AgreementNo;
                    worksheet.Cells[recordIndex, 14].Value = item.Asset.Manufacturer;
                    worksheet.Cells[recordIndex, 15].Value = item.Asset.Article != null ? item.Asset.Article.Name : "";
                    worksheet.Cells[recordIndex, 16].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
                    worksheet.Cells[recordIndex, 17].Value = item.Asset.PurchaseDate;
                    worksheet.Cells[recordIndex, 17].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 18].Value = item.Asset.InvoiceDate;
                    worksheet.Cells[recordIndex, 18].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 19].Value = item.Asset.RemovalDate;
                    worksheet.Cells[recordIndex, 19].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 20].Value = item.Dep.ExpLifeInPeriods;
                    //worksheet.Cells[recordIndex, 20].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 21].Value = item.Dep.UsefulLife;
                    //worksheet.Cells[recordIndex, 21].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 22].Value = item.Dep.TotLifeInpPeriods;
                    //worksheet.Cells[recordIndex, 22].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 23].Value = item.Dep.RemLifeInPeriods;
                    //worksheet.Cells[recordIndex, 23].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 24].Value = item.Dep.APCFYStart;
                    worksheet.Cells[recordIndex, 24].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 25].Value = item.Dep.DepFYStart;
                    worksheet.Cells[recordIndex, 25].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 26].Value = item.Dep.BkValFYStart;
                    worksheet.Cells[recordIndex, 26].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 27].Value = item.Dep.Acquisition;
                    worksheet.Cells[recordIndex, 27].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 28].Value = item.Dep.DepForYear;
                    worksheet.Cells[recordIndex, 28].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 29].Value = item.Dep.DepRetirement;
                    worksheet.Cells[recordIndex, 29].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 30].Value = item.Dep.Retirement;
                    worksheet.Cells[recordIndex, 30].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 31].Value = item.Dep.CurrBkValue;
                    worksheet.Cells[recordIndex, 31].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 32].Value = item.Dep.DepTransfer;
                    worksheet.Cells[recordIndex, 32].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 33].Value = item.Dep.Transfer;
                    worksheet.Cells[recordIndex, 33].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 34].Value = item.Dep.PosCap;
                    worksheet.Cells[recordIndex, 34].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 35].Value = item.Dep.DepPostCap;
                    worksheet.Cells[recordIndex, 35].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 36].Value = item.Dep.InvestSupport;
                    worksheet.Cells[recordIndex, 36].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 37].Value = item.Dep.WriteUps;
                    worksheet.Cells[recordIndex, 37].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 38].Value = item.Dep.CurrentAPC;
                    worksheet.Cells[recordIndex, 38].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 39].Value = item.Dep.AccumulDep;
                    worksheet.Cells[recordIndex, 39].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 40].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Name : "";
                    worksheet.Cells[recordIndex, 41].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.ERPCode : "";
                    worksheet.Cells[recordIndex, 42].Value = item.Asset.Department != null ? item.Asset.Department.Name : "";
                    worksheet.Cells[recordIndex, 43].Value = item.Asset.Division != null ? item.Asset.Division.Name : "";
                    worksheet.Cells[recordIndex, 44].Value = item.Asset.Room != null ? item.Asset.Room.Name : "";
                    worksheet.Cells[recordIndex, 45].Value = item.Asset.Administration != null ? item.Asset.Administration.Name : "";
                    worksheet.Cells[recordIndex, 46].Value = item.Asset.Room != null && item.Asset.Room.Location != null && item.Asset.Room.Location.City != null ? item.Asset.Room.Location.City.Name : "";
                    worksheet.Cells[recordIndex, 47].Value = item.Asset.Room != null && item.Asset.Room.Location != null && item.Asset.Room.Location.City != null && item.Asset.Room.Location.City.County != null ? item.Asset.Room.Location.City.County.Name : "";
                    worksheet.Cells[recordIndex, 48].Value = item.Asset.Room != null && item.Asset.Room.Location != null && item.Asset.Room.Location.City != null && item.Asset.Room.Location.City.County != null && item.Asset.Room.Location.City.County.Country != null ? item.Asset.Room.Location.City.County.Country.Name : "";
                    worksheet.Cells[recordIndex, 49].Value = item.Asset.BudgetManager != null ? item.Asset.BudgetManager.Name : "";
                    worksheet.Cells[recordIndex, 50].Value = item.Asset.AssetNature != null ? item.Asset.AssetNature.Name : "";
                    worksheet.Cells[recordIndex, 51].Value = item.Asset.SubType != null && item.Asset.SubType.Type != null ? item.Asset.SubType.Type.Name : "";
                    worksheet.Cells[recordIndex, 52].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
                    worksheet.Cells[recordIndex, 53].Value = item.Asset.Material != null ? item.Asset.Material.Code : "";
                    worksheet.Cells[recordIndex, 54].Value = item.Asset.Material != null ? item.Asset.Material.Name : "";
                    worksheet.Cells[recordIndex, 55].Value = item.Asset.InterCompany != null ? item.Asset.InterCompany.Name : "";
                    worksheet.Cells[recordIndex, 56].Value = item.Asset.Employee != null ? item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName : "";
                    worksheet.Cells[recordIndex, 57].Value = "";
                    worksheet.Cells[recordIndex, 58].Value = "";
                    worksheet.Cells[recordIndex, 59].Value = "";
                    worksheet.Cells[recordIndex, 60].Value = "";
                    worksheet.Cells[recordIndex, 61].Value = "";
                    worksheet.Cells[recordIndex, 62].Value = "";
                    worksheet.Cells[recordIndex, 63].Value = "";
                    worksheet.Cells[recordIndex, 64].Value = item.Asset.SubType != null ? item.Asset.SubType.Name : "";
                    worksheet.Cells[recordIndex, 65].Value = "";
                    worksheet.Cells[recordIndex, 66].Value = item.Asset.ERPCode;
                    worksheet.Cells[recordIndex, 67].Value = item.Adm.AssetClass != null ? item.Adm.AssetClass.Name : "";
                    worksheet.Cells[recordIndex, 68].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Name : "";
                    worksheet.Cells[recordIndex, 69].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.Region != null ? item.Asset.CostCenter.Region.Name : "";
                    worksheet.Cells[recordIndex, 70].Value = item.Asset.InsuranceCategory != null ? item.Asset.InsuranceCategory.Name : "";
                    worksheet.Cells[recordIndex, 71].Value = item.Asset.AssetType != null ? item.Asset.AssetType.Name : "";
                    worksheet.Cells[recordIndex, 72].Value = item.Asset.Project != null ? item.Asset.Project.Name : "";

                    if (diff == 0)
                    {



                        for (int i = 1; i < 73; i++)
                        {
                            worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(1).Height = 35.00;
                        worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(2, 1);

                        using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 72])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }







                        using (var cells = worksheet.Cells[1, 1, 1, 72])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 72])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 72])
                        {
                            for (int i = 2; i < items.Count() + 2; i++)
                            {
                                worksheet.Row(i).Height = 15.00;
                                worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                worksheet.Cells[$"I{i}"].Style.WrapText = true;
                                worksheet.Cells[$"J{i}"].Style.WrapText = true;
                                worksheet.Cells[$"K{i}"].Style.WrapText = true;
                                worksheet.Cells[$"L{i}"].Style.WrapText = true;
                                worksheet.Cells[$"M{i}"].Style.WrapText = true;


                            }



                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

                        for (int i = 1; i < 73; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        worksheet.Column(1).Width = 12.00;
                        worksheet.Column(2).Width = 10.00;
                        worksheet.Column(3).Width = 14.00;
                        worksheet.Column(4).Width = 20.00;
                        worksheet.Column(5).Width = 15.00;
                        worksheet.Column(6).Width = 15.00;
                        worksheet.Column(7).Width = 60.00;
                        worksheet.Column(8).Width = 30.00;
                        worksheet.Column(9).Width = 25.00;
                        worksheet.Column(10).Width = 25.00;
                        worksheet.Column(11).Width = 12.00;
                        worksheet.Column(12).Width = 40.00;
                        worksheet.Column(13).Width = 20.00;
                        worksheet.Column(14).Width = 30.00;
                        worksheet.Column(15).Width = 15.00;
                        worksheet.Column(16).Width = 30.00;
                        worksheet.Column(17).Width = 12.00;
                        worksheet.Column(18).Width = 15.00;
                        worksheet.Column(19).Width = 15.00;
                        worksheet.Column(20).Width = 15.00;

                        worksheet.Column(21).Width = 15.00;
                        worksheet.Column(22).Width = 15.00;
                        worksheet.Column(23).Width = 15.00;
                        worksheet.Column(24).Width = 15.00;
                        worksheet.Column(25).Width = 15.00;
                        worksheet.Column(26).Width = 15.00;
                        worksheet.Column(27).Width = 15.00;
                        worksheet.Column(28).Width = 15.00;
                        worksheet.Column(29).Width = 15.00;
                        worksheet.Column(30).Width = 15.00;


                        worksheet.Column(31).Width = 15.00;
                        worksheet.Column(32).Width = 15.00;
                        worksheet.Column(33).Width = 15.00;
                        worksheet.Column(34).Width = 15.00;
                        worksheet.Column(35).Width = 15.00;
                        worksheet.Column(36).Width = 15.00;
                        worksheet.Column(37).Width = 15.00;
                        worksheet.Column(38).Width = 15.00;
                        worksheet.Column(39).Width = 15.00;
                        worksheet.Column(40).Width = 30.00;

                        worksheet.Column(41).Width = 30.00;
                        worksheet.Column(42).Width = 30.00;
                        worksheet.Column(43).Width = 15.00;
                        worksheet.Column(44).Width = 60.00;
                        worksheet.Column(45).Width = 15.00;
                        worksheet.Column(46).Width = 15.00;
                        worksheet.Column(47).Width = 15.00;
                        worksheet.Column(48).Width = 15.00;
                        worksheet.Column(49).Width = 10.00;
                        worksheet.Column(50).Width = 45.00;

                        worksheet.Column(51).Width = 40.00;
                        worksheet.Column(52).Width = 10.00;
                        worksheet.Column(53).Width = 15.00;
                        worksheet.Column(54).Width = 180.00;
                        worksheet.Column(55).Width = 25.00;
                        worksheet.Column(56).Width = 15.00;
                        worksheet.Column(57).Width = 15.00;
                        worksheet.Column(58).Width = 15.00;
                        worksheet.Column(59).Width = 15.00;
                        worksheet.Column(60).Width = 15.00;

                        worksheet.Column(61).Width = 15.00;
                        worksheet.Column(62).Width = 15.00;
                        worksheet.Column(63).Width = 15.00;
                        worksheet.Column(64).Width = 25.00;
                        worksheet.Column(65).Width = 31.00;
                        worksheet.Column(66).Width = 30.00;
                        worksheet.Column(67).Width = 15.00;
                        worksheet.Column(68).Width = 15.00;
                        worksheet.Column(69).Width = 30.00;
                        worksheet.Column(70).Width = 10.00;
                        worksheet.Column(71).Width = 25.00;
                        worksheet.Column(72).Width = 30.00;


                        worksheet.Cells["A1:BT1"].AutoFilter = true;

                    }

                    recordIndex++;
                }


                using (var cells = worksheet.Cells[1, 1, 1, 72])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

        [HttpGet("exportStockITMFX")]
        public IActionResult ExportStockITMFX(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {

            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


            includes = @"Asset.InvState,Adm.Account,Adm.ExpAccount,Adm.AssetCategory,Asset.Document.Partner,Asset.Document,Adm.Article,Adm.CostCenter.AdmCenter,Asset.Material,Dep.AccSystem,Adm.Department,Adm.Division,Adm.Room.Location.City.County.Country,Adm.Administration,Adm.BudgetManager,Adm.AssetNature,Adm.SubType.Type,Adm.Employee,Adm.InterCompany,Adm.SubType,Asset,Adm.AssetClass,Adm.CostCenter.Region,Adm.InsuranceCategory,Adm.AssetType,Adm.Project,";


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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


            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockITMFX(assetFilter, includes, sorting, paging, out depTotal).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("STOCK-IT-MFX");
				
                //First add the headers
                worksheet.Cells[1, 1].Value = "Asset";
                worksheet.Cells[1, 2].Value = "PCS";
                worksheet.Cells[1, 3].Value = "Subnumber";
                worksheet.Cells[1, 4].Value = "Asset description";
                worksheet.Cells[1, 5].Value = "Serial number";
                worksheet.Cells[1, 6].Value = "Vendor";
                worksheet.Cells[1, 7].Value = "Supplier name";
                worksheet.Cells[1, 8].Value = "Cost Center";
                worksheet.Cells[1, 9].Value = "Capitalized on";
                worksheet.Cells[1, 10].Value = "Current APC";
    
                worksheet.Cells[1, 11].Value = "Departament";
                worksheet.Cells[1, 12].Value = "Divizie";
                worksheet.Cells[1, 13].Value = "FY' Start";
                worksheet.Cells[1, 14].Value = "Marca P.";
                worksheet.Cells[1, 15].Value = "Material";

                worksheet.Cells[1, 16].Value = "Material Description";
                worksheet.Cells[1, 17].Value = "Username";
                worksheet.Cells[1, 18].Value = "Profit Ctr";
                worksheet.Cells[1, 19].Value = "PC Detaliu";
                worksheet.Cells[1, 20].Value = "Type";
                worksheet.Cells[1, 21].Value = "WBS element";





                int recordIndex = 2;
                int count = items.Count();

                foreach (var item in items)
                {
                    rowNumber++;

                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }


                    worksheet.Cells[recordIndex, 1].Value = item.Asset.InvNo;
                    worksheet.Cells[recordIndex, 2].Value = item.Asset.Quantity;
                    worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
                    worksheet.Cells[recordIndex, 4].Value = item.Asset.Name;
                    worksheet.Cells[recordIndex, 5].Value = item.Asset.SerialNumber;
                    worksheet.Cells[recordIndex, 6].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.RegistryNumber : "";
                    worksheet.Cells[recordIndex, 7].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.Name : "";
                    worksheet.Cells[recordIndex, 8].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
                    worksheet.Cells[recordIndex, 9].Value = item.Asset.PurchaseDate;
                    worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "yyyy-mm-dd";
                    worksheet.Cells[recordIndex, 10].Value = item.Dep.CurrentAPC;
                    worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[recordIndex, 11].Value = item.Asset.Department != null ? item.Asset.Department.Name : "";
                    worksheet.Cells[recordIndex, 12].Value = item.Asset.Division != null ? item.Asset.Division.Name : "";
                    worksheet.Cells[recordIndex, 13].Value = item.Asset.BudgetManager != null ? item.Asset.BudgetManager.Name : "";
                    worksheet.Cells[recordIndex, 14].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
                    worksheet.Cells[recordIndex, 15].Value = item.Asset.Material != null ? item.Asset.Material.Code : "";
                    worksheet.Cells[recordIndex, 16].Value = item.Asset.Material != null ? item.Asset.Material.Name : "";
                    worksheet.Cells[recordIndex, 17].Value = item.Asset.Employee != null ? item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName : "";
                    worksheet.Cells[recordIndex, 18].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Name : "";
                    worksheet.Cells[recordIndex, 19].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.Region != null ? item.Asset.CostCenter.Region.Name : "";
                    worksheet.Cells[recordIndex, 20].Value = item.Asset.AssetType != null ? item.Asset.AssetType.Name : "";
                    worksheet.Cells[recordIndex, 21].Value = item.Asset.Project != null ? item.Asset.Project.Name : "";

                    if (diff == 0)
                    {



                        for (int i = 1; i < 22; i++)
                        {
                            worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(1).Height = 35.00;
                        worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(2, 1);

                        using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 21])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        using (var cells = worksheet.Cells[1, 1, 1, 21])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 21])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 21])
                        {
                            for (int i = 2; i < items.Count() + 2; i++)
                            {
                                worksheet.Row(i).Height = 15.00;
                                worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                worksheet.Cells[$"I{i}"].Style.WrapText = true;
                                worksheet.Cells[$"J{i}"].Style.WrapText = true;
                                worksheet.Cells[$"K{i}"].Style.WrapText = true;
                                worksheet.Cells[$"L{i}"].Style.WrapText = true;
                                worksheet.Cells[$"M{i}"].Style.WrapText = true;
                            }
                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

						for (int i = 1; i < 29; i++)
						{
							worksheet.Column(i).AutoFit();
						}

						worksheet.Column(1).Width = 21.00;
						worksheet.Column(2).Width = 10.00;
						worksheet.Column(3).Width = 14.00;
						worksheet.Column(4).Width = 50.00;
						worksheet.Column(5).Width = 20.00;
						worksheet.Column(6).Width = 15.00;
						worksheet.Column(7).Width = 30.00;
						worksheet.Column(8).Width = 15.00;
						worksheet.Column(9).Width = 15.00;
						worksheet.Column(10).Width = 15.00;
						worksheet.Column(11).Width = 20.00;
						worksheet.Column(12).Width = 20.00;
						worksheet.Column(13).Width = 12.00;
						worksheet.Column(14).Width = 15.00;
						worksheet.Column(15).Width = 15.00;
						worksheet.Column(16).Width = 120.00;
						worksheet.Column(17).Width = 20.00;
						worksheet.Column(18).Width = 20.00;
						worksheet.Column(19).Width = 20.00;
						worksheet.Column(20).Width = 20.00;

						//worksheet.Column(21).Width = 15.00;
						//worksheet.Column(22).Width = 15.00;
						//worksheet.Column(23).Width = 15.00;
						//worksheet.Column(24).Width = 15.00;
						//worksheet.Column(25).Width = 15.00;
						//worksheet.Column(26).Width = 15.00;
						//worksheet.Column(27).Width = 15.00;
						//worksheet.Column(28).Width = 15.00;


						worksheet.Cells["A1:U1"].AutoFilter = true;
                        //worksheet.Column(1).BestFit = true;
                        //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                        //worksheet.Cells["B1:U" + items.Count()].AutoFitColumns();
                        //const double minWidth = 0.00;
                        //const double maxWidth = 100.00;
                        //worksheet.Cells.AutoFitColumns(minWidth, maxWidth);
                    }

                    recordIndex++;
                }


                using (var cells = worksheet.Cells[1, 1, 1, 21])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

		[HttpGet("exportStockITToEmployee")]
		public IActionResult ExportStockITToEmployee(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
		{

			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			int rowNumber = 0;
			Paging paging = null;
			Sorting sorting = null;
			string userName = string.Empty;
			string role = string.Empty;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


			includes = @"Asset.InvState,Adm.Account,Adm.ExpAccount,Adm.AssetCategory,Asset.Document.Partner,Asset.Document,Adm.Article,Adm.CostCenter.AdmCenter,Asset.Material,Dep.AccSystem,Adm.Department,Adm.Division,Adm.Room.Location.City.County.Country,Adm.Administration,Adm.BudgetManager,Adm.AssetNature,Adm.SubType.Type,Adm.Employee,Adm.InterCompany,Adm.SubType,Asset,Adm.AssetClass,Adm.CostCenter.Region,Adm.InsuranceCategory,Adm.AssetType,Adm.Project,";


			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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


			var items = (_itemsRepository as IAssetsRepository)
				.GetMonthStockITToValidateEmployee(assetFilter, includes, sorting, paging, out depTotal).ToList();

			using (ExcelPackage package = new ExcelPackage())
			{

				assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("STOCK-IT-MFX");

				//First add the headers
				worksheet.Cells[1, 1].Value = "Asset";
				worksheet.Cells[1, 2].Value = "PCS";
				worksheet.Cells[1, 3].Value = "Subnumber";
				worksheet.Cells[1, 4].Value = "Asset description";
				worksheet.Cells[1, 5].Value = "Serial number";
				worksheet.Cells[1, 6].Value = "Vendor";
				worksheet.Cells[1, 7].Value = "Supplier name";
				worksheet.Cells[1, 8].Value = "Cost Center";
				worksheet.Cells[1, 9].Value = "Capitalized on";
				worksheet.Cells[1, 10].Value = "Current APC";

				worksheet.Cells[1, 11].Value = "Departament";
				worksheet.Cells[1, 12].Value = "Divizie";
				worksheet.Cells[1, 13].Value = "FY' Start";
				worksheet.Cells[1, 14].Value = "Marca P.";
				worksheet.Cells[1, 15].Value = "Material";

				worksheet.Cells[1, 16].Value = "Material Description";
				worksheet.Cells[1, 17].Value = "Username";
				worksheet.Cells[1, 18].Value = "Profit Ctr";
				worksheet.Cells[1, 19].Value = "PC Detaliu";
				worksheet.Cells[1, 20].Value = "Type";
				worksheet.Cells[1, 21].Value = "WBS element";





				int recordIndex = 2;
				int count = items.Count();

				foreach (var item in items)
				{
					rowNumber++;

					int diff = recordIndex - count;

					if (diff > 0)
					{
						diff = 0;
					}


					worksheet.Cells[recordIndex, 1].Value = item.Asset.InvNo;
					worksheet.Cells[recordIndex, 2].Value = item.Asset.Quantity;
					worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
					worksheet.Cells[recordIndex, 4].Value = item.Asset.Name;
					worksheet.Cells[recordIndex, 5].Value = item.Asset.SerialNumber;
					worksheet.Cells[recordIndex, 6].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.RegistryNumber : "";
					worksheet.Cells[recordIndex, 7].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.Name : "";
					worksheet.Cells[recordIndex, 8].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
					worksheet.Cells[recordIndex, 9].Value = item.Asset.PurchaseDate;
					worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "yyyy-mm-dd";
					worksheet.Cells[recordIndex, 10].Value = item.Dep.CurrentAPC;
					worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 11].Value = item.Asset.Department != null ? item.Asset.Department.Name : "";
					worksheet.Cells[recordIndex, 12].Value = item.Asset.Division != null ? item.Asset.Division.Name : "";
					worksheet.Cells[recordIndex, 13].Value = item.Asset.BudgetManager != null ? item.Asset.BudgetManager.Name : "";
					worksheet.Cells[recordIndex, 14].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
					worksheet.Cells[recordIndex, 15].Value = item.Asset.Material != null ? item.Asset.Material.Code : "";
					worksheet.Cells[recordIndex, 16].Value = item.Asset.Material != null ? item.Asset.Material.Name : "";
					worksheet.Cells[recordIndex, 17].Value = item.Asset.Employee != null ? item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName : "";
					worksheet.Cells[recordIndex, 18].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Name : "";
					worksheet.Cells[recordIndex, 19].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.Region != null ? item.Asset.CostCenter.Region.Name : "";
					worksheet.Cells[recordIndex, 20].Value = item.Asset.AssetType != null ? item.Asset.AssetType.Name : "";
					worksheet.Cells[recordIndex, 21].Value = item.Asset.Project != null ? item.Asset.Project.Name : "";

					if (diff == 0)
					{



						for (int i = 1; i < 22; i++)
						{
							worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						worksheet.Row(1).Height = 35.00;
						worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.View.FreezePanes(2, 1);

						using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 21])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						using (var cells = worksheet.Cells[1, 1, 1, 21])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 21])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 21])
						{
							for (int i = 2; i < items.Count() + 2; i++)
							{
								worksheet.Row(i).Height = 15.00;
								worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

								worksheet.Cells[$"A{i}"].Style.WrapText = true;
								worksheet.Cells[$"B{i}"].Style.WrapText = true;
								worksheet.Cells[$"C{i}"].Style.WrapText = true;
								worksheet.Cells[$"D{i}"].Style.WrapText = true;
								worksheet.Cells[$"E{i}"].Style.WrapText = true;
								worksheet.Cells[$"F{i}"].Style.WrapText = true;
								worksheet.Cells[$"G{i}"].Style.WrapText = true;
								worksheet.Cells[$"H{i}"].Style.WrapText = true;
								worksheet.Cells[$"I{i}"].Style.WrapText = true;
								worksheet.Cells[$"J{i}"].Style.WrapText = true;
								worksheet.Cells[$"K{i}"].Style.WrapText = true;
								worksheet.Cells[$"L{i}"].Style.WrapText = true;
								worksheet.Cells[$"M{i}"].Style.WrapText = true;
							}
						}


						worksheet.View.ShowGridLines = false;
						worksheet.View.ZoomScale = 100;

						for (int i = 1; i < 29; i++)
						{
							worksheet.Column(i).AutoFit();
						}

						worksheet.Column(1).Width = 21.00;
						worksheet.Column(2).Width = 10.00;
						worksheet.Column(3).Width = 14.00;
						worksheet.Column(4).Width = 50.00;
						worksheet.Column(5).Width = 20.00;
						worksheet.Column(6).Width = 15.00;
						worksheet.Column(7).Width = 30.00;
						worksheet.Column(8).Width = 15.00;
						worksheet.Column(9).Width = 15.00;
						worksheet.Column(10).Width = 15.00;
						worksheet.Column(11).Width = 20.00;
						worksheet.Column(12).Width = 20.00;
						worksheet.Column(13).Width = 12.00;
						worksheet.Column(14).Width = 15.00;
						worksheet.Column(15).Width = 15.00;
						worksheet.Column(16).Width = 120.00;
						worksheet.Column(17).Width = 20.00;
						worksheet.Column(18).Width = 20.00;
						worksheet.Column(19).Width = 20.00;
						worksheet.Column(20).Width = 20.00;

						//worksheet.Column(21).Width = 15.00;
						//worksheet.Column(22).Width = 15.00;
						//worksheet.Column(23).Width = 15.00;
						//worksheet.Column(24).Width = 15.00;
						//worksheet.Column(25).Width = 15.00;
						//worksheet.Column(26).Width = 15.00;
						//worksheet.Column(27).Width = 15.00;
						//worksheet.Column(28).Width = 15.00;


						worksheet.Cells["A1:U1"].AutoFilter = true;
						//worksheet.Column(1).BestFit = true;
						//worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
						//worksheet.Cells["B1:U" + items.Count()].AutoFitColumns();
						//const double minWidth = 0.00;
						//const double maxWidth = 100.00;
						//worksheet.Cells.AutoFitColumns(minWidth, maxWidth);
					}

					recordIndex++;
				}


				using (var cells = worksheet.Cells[1, 1, 1, 21])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Export.xlsx"
				};

				return result;

			}
		}

		[HttpGet("exportStockITFromEmployee")]
		public IActionResult ExportStockITFromEmployee(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
		{

			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			int rowNumber = 0;
			Paging paging = null;
			Sorting sorting = null;
			string userName = string.Empty;
			string role = string.Empty;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


			includes = @"Asset.InvState,Adm.Account,Adm.ExpAccount,Adm.AssetCategory,Asset.Document.Partner,Asset.Document,Adm.Article,Adm.CostCenter.AdmCenter,Asset.Material,Dep.AccSystem,Adm.Department,Adm.Division,Adm.Room.Location.City.County.Country,Adm.Administration,Adm.BudgetManager,Adm.AssetNature,Adm.SubType.Type,Adm.Employee,Adm.InterCompany,Adm.SubType,Asset,Adm.AssetClass,Adm.CostCenter.Region,Adm.InsuranceCategory,Adm.AssetType,Adm.Project,";


			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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


			var items = (_itemsRepository as IAssetsRepository)
				.GetMonthStockITToValidate(assetFilter, includes, sorting, paging, out depTotal).ToList();

			using (ExcelPackage package = new ExcelPackage())
			{

				assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("STOCK-IT-MFX");

				//First add the headers
				worksheet.Cells[1, 1].Value = "Asset";
				worksheet.Cells[1, 2].Value = "PCS";
				worksheet.Cells[1, 3].Value = "Subnumber";
				worksheet.Cells[1, 4].Value = "Asset description";
				worksheet.Cells[1, 5].Value = "Serial number";
				worksheet.Cells[1, 6].Value = "Vendor";
				worksheet.Cells[1, 7].Value = "Supplier name";
				worksheet.Cells[1, 8].Value = "Cost Center";
				worksheet.Cells[1, 9].Value = "Capitalized on";
				worksheet.Cells[1, 10].Value = "Current APC";

				worksheet.Cells[1, 11].Value = "Departament";
				worksheet.Cells[1, 12].Value = "Divizie";
				worksheet.Cells[1, 13].Value = "FY' Start";
				worksheet.Cells[1, 14].Value = "Marca P.";
				worksheet.Cells[1, 15].Value = "Material";

				worksheet.Cells[1, 16].Value = "Material Description";
				worksheet.Cells[1, 17].Value = "Username";
				worksheet.Cells[1, 18].Value = "Profit Ctr";
				worksheet.Cells[1, 19].Value = "PC Detaliu";
				worksheet.Cells[1, 20].Value = "Type";
				worksheet.Cells[1, 21].Value = "WBS element";





				int recordIndex = 2;
				int count = items.Count();

				foreach (var item in items)
				{
					rowNumber++;

					int diff = recordIndex - count;

					if (diff > 0)
					{
						diff = 0;
					}


					worksheet.Cells[recordIndex, 1].Value = item.Asset.InvNo;
					worksheet.Cells[recordIndex, 2].Value = item.Asset.Quantity;
					worksheet.Cells[recordIndex, 3].Value = item.Asset.SubNo;
					worksheet.Cells[recordIndex, 4].Value = item.Asset.Name;
					worksheet.Cells[recordIndex, 5].Value = item.Asset.SerialNumber;
					worksheet.Cells[recordIndex, 6].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.RegistryNumber : "";
					worksheet.Cells[recordIndex, 7].Value = item.Asset.Document.Partner != null ? item.Asset.Document.Partner.Name : "";
					worksheet.Cells[recordIndex, 8].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
					worksheet.Cells[recordIndex, 9].Value = item.Asset.PurchaseDate;
					worksheet.Cells[recordIndex, 9].Style.Numberformat.Format = "yyyy-mm-dd";
					worksheet.Cells[recordIndex, 10].Value = item.Dep.CurrentAPC;
					worksheet.Cells[recordIndex, 10].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[recordIndex, 11].Value = item.Asset.Department != null ? item.Asset.Department.Name : "";
					worksheet.Cells[recordIndex, 12].Value = item.Asset.Division != null ? item.Asset.Division.Name : "";
					worksheet.Cells[recordIndex, 13].Value = item.Asset.BudgetManager != null ? item.Asset.BudgetManager.Name : "";
					worksheet.Cells[recordIndex, 14].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
					worksheet.Cells[recordIndex, 15].Value = item.Asset.Material != null ? item.Asset.Material.Code : "";
					worksheet.Cells[recordIndex, 16].Value = item.Asset.Material != null ? item.Asset.Material.Name : "";
					worksheet.Cells[recordIndex, 17].Value = item.Asset.Employee != null ? item.Asset.Employee.FirstName + " " + item.Asset.Employee.LastName : "";
					worksheet.Cells[recordIndex, 18].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Name : "";
					worksheet.Cells[recordIndex, 19].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.Region != null ? item.Asset.CostCenter.Region.Name : "";
					worksheet.Cells[recordIndex, 20].Value = item.Asset.AssetType != null ? item.Asset.AssetType.Name : "";
					worksheet.Cells[recordIndex, 21].Value = item.Asset.Project != null ? item.Asset.Project.Name : "";

					if (diff == 0)
					{



						for (int i = 1; i < 22; i++)
						{
							worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						worksheet.Row(1).Height = 35.00;
						worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.View.FreezePanes(2, 1);

						using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 21])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						using (var cells = worksheet.Cells[1, 1, 1, 21])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 21])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 21])
						{
							for (int i = 2; i < items.Count() + 2; i++)
							{
								worksheet.Row(i).Height = 15.00;
								worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

								worksheet.Cells[$"A{i}"].Style.WrapText = true;
								worksheet.Cells[$"B{i}"].Style.WrapText = true;
								worksheet.Cells[$"C{i}"].Style.WrapText = true;
								worksheet.Cells[$"D{i}"].Style.WrapText = true;
								worksheet.Cells[$"E{i}"].Style.WrapText = true;
								worksheet.Cells[$"F{i}"].Style.WrapText = true;
								worksheet.Cells[$"G{i}"].Style.WrapText = true;
								worksheet.Cells[$"H{i}"].Style.WrapText = true;
								worksheet.Cells[$"I{i}"].Style.WrapText = true;
								worksheet.Cells[$"J{i}"].Style.WrapText = true;
								worksheet.Cells[$"K{i}"].Style.WrapText = true;
								worksheet.Cells[$"L{i}"].Style.WrapText = true;
								worksheet.Cells[$"M{i}"].Style.WrapText = true;
							}
						}


						worksheet.View.ShowGridLines = false;
						worksheet.View.ZoomScale = 100;

						for (int i = 1; i < 29; i++)
						{
							worksheet.Column(i).AutoFit();
						}

						worksheet.Column(1).Width = 21.00;
						worksheet.Column(2).Width = 10.00;
						worksheet.Column(3).Width = 14.00;
						worksheet.Column(4).Width = 50.00;
						worksheet.Column(5).Width = 20.00;
						worksheet.Column(6).Width = 15.00;
						worksheet.Column(7).Width = 30.00;
						worksheet.Column(8).Width = 15.00;
						worksheet.Column(9).Width = 15.00;
						worksheet.Column(10).Width = 15.00;
						worksheet.Column(11).Width = 20.00;
						worksheet.Column(12).Width = 20.00;
						worksheet.Column(13).Width = 12.00;
						worksheet.Column(14).Width = 15.00;
						worksheet.Column(15).Width = 15.00;
						worksheet.Column(16).Width = 120.00;
						worksheet.Column(17).Width = 20.00;
						worksheet.Column(18).Width = 20.00;
						worksheet.Column(19).Width = 20.00;
						worksheet.Column(20).Width = 20.00;

						//worksheet.Column(21).Width = 15.00;
						//worksheet.Column(22).Width = 15.00;
						//worksheet.Column(23).Width = 15.00;
						//worksheet.Column(24).Width = 15.00;
						//worksheet.Column(25).Width = 15.00;
						//worksheet.Column(26).Width = 15.00;
						//worksheet.Column(27).Width = 15.00;
						//worksheet.Column(28).Width = 15.00;


						worksheet.Cells["A1:U1"].AutoFilter = true;
						//worksheet.Column(1).BestFit = true;
						//worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
						//worksheet.Cells["B1:U" + items.Count()].AutoFitColumns();
						//const double minWidth = 0.00;
						//const double maxWidth = 100.00;
						//worksheet.Cells.AutoFitColumns(minWidth, maxWidth);
					}

					recordIndex++;
				}


				using (var cells = worksheet.Cells[1, 1, 1, 21])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Export.xlsx"
				};

				return result;

			}
		}

		[HttpGet("exportImportStockITMFX")]
        public async Task<IActionResult> ExportImportStockITMFX(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {
            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            Model.Asset asset = null;
			Dictionary<int, string> duplicate = new Dictionary<int, string>();
			string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


            includes = @"Adm.CostCenter.AdmCenter,Adm.Employee,Adm.AssetType,Adm.Project,Asset.BudgetBase.Project,Asset.Material";


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
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


            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthStockIT(assetFilter, includes, sorting, paging, out depTotal).ToList().OrderBy(a => a.Asset.MaterialId);

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("STOCK-IT-MFX-IMPORT");

                //First add the headers
                worksheet.Cells[1, 1].Value = "Cod OPTIMA";
                worksheet.Cells[1, 2].Value = "Descriere";
                worksheet.Cells[1, 3].Value = "Cod Centru de cost";
				worksheet.Cells[1, 4].Value = "Centru de cost";
				worksheet.Cells[1, 5].Value = "Profit Center";
				worksheet.Cells[1, 6].Value = "Cod Produs";
				worksheet.Cells[1, 7].Value = "Cod Produs unic";
				worksheet.Cells[1, 8].Value = "SN/IMEI";
                worksheet.Cells[1, 9].Value = "Marca user";
                worksheet.Cells[1, 10].Value = "User";
                worksheet.Cells[1, 11].Value = "Cod WBS";
                worksheet.Cells[1, 12].Value = "Cod Buget OPTIMA";

                int recordIndex = 2;
                int duplicateIndex = 0;
				int count = items.Count();

				var duplicates = items.GroupBy(t => t.Asset.MaterialId)
					 .Where(g => g.Count() > 1)
					 .SelectMany(t => t);

				var unique = items.GroupBy(t => t.Asset.MaterialId)
					 .Where(g => g.Count() < 2)
					 .SelectMany(t => t);

				foreach (var item in duplicates)
				{
					duplicateIndex++;
					asset = await _context.Set<Model.Asset>().Include(m => m.Material).Where(a => a.Id == item.Asset.Id).SingleAsync();

                    if(asset.Material != null && duplicate.Count == 0)
                    {
						duplicate.Add(asset.MaterialId.Value, asset.Material.Code);
					}
                    else
                    {
						bool keyExists = duplicate.ContainsKey(asset.MaterialId.Value);

						if (!keyExists)
						{
							duplicate.Add(asset.MaterialId.Value, asset.Material.Code);
							duplicateIndex = 1;
						}
					}
					
					asset.MaterialUnique = asset.Material != null ? asset.Material.Code + "\\" + duplicateIndex : "";
                    _context.Update(asset);

				}

				foreach (var item in unique)
				{
					asset = await _context.Set<Model.Asset>().Include(m => m.Material).Where(a => a.Id == item.Asset.Id).SingleAsync();
					asset.MaterialUnique = asset.Material != null ? asset.Material.Code : "";
					_context.Update(asset);
				}

				_context.SaveChanges();

				foreach (var item in items)
                {
                    rowNumber++;

                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }


                    worksheet.Cells[recordIndex, 1].Value = item.Asset.ERPCode;
                    worksheet.Cells[recordIndex, 2].Value = item.Asset.Name;
                    worksheet.Cells[recordIndex, 3].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
					worksheet.Cells[recordIndex, 4].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Name : "";
					worksheet.Cells[recordIndex, 5].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Code : "";
					worksheet.Cells[recordIndex, 6].Value = item.Asset.Material != null ? item.Asset.Material.Code : "";
					worksheet.Cells[recordIndex, 7].Value = item.Asset.MaterialUnique;
					worksheet.Cells[recordIndex, 8].Value = item.Asset.SerialNumber != null ? item.Asset.SerialNumber : "";
                    worksheet.Cells[recordIndex, 9].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
                    worksheet.Cells[recordIndex, 10].Value = item.Asset.Employee != null && item.Asset.Employee.Email != null ? item.Asset.Employee.Email.Substring(0, item.Asset.Employee.Email.IndexOf('@')) : "";
                    worksheet.Cells[recordIndex, 11].Value = item.Asset.BudgetBase != null && item.Asset.BudgetBase.Project != null ? item.Asset.BudgetBase.Project.Code : "";
                    worksheet.Cells[recordIndex, 12].Value = item.Asset.BudgetBase != null ? item.Asset.BudgetBase.Code : "";

                    if (diff == 0)
                    {
                        for (int i = 1; i < 13; i++)
                        {
                            worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(1).Height = 35.00;
                        worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(2, 1);

                        using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 12])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        using (var cells = worksheet.Cells[1, 1, 1, 12])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 12])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 12])
                        {
                            for (int i = 2; i < items.Count() + 2; i++)
                            {
                                worksheet.Row(i).Height = 15.00;
                                worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                worksheet.Cells[$"I{i}"].Style.WrapText = true;
								worksheet.Cells[$"J{i}"].Style.WrapText = true;
								worksheet.Cells[$"K{i}"].Style.WrapText = true;
								worksheet.Cells[$"L{i}"].Style.WrapText = true;
							}
                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

                        for (int i = 1; i < 13; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        worksheet.Column(1).Width = 21.00;
                        worksheet.Column(2).Width = 80.00;
                        worksheet.Column(3).Width = 20.00;
                        worksheet.Column(4).Width = 20.00;
                        worksheet.Column(5).Width = 20.00;
                        worksheet.Column(6).Width = 20.00;
                        worksheet.Column(7).Width = 20.00;
                        worksheet.Column(8).Width = 20.00;
                        worksheet.Column(9).Width = 20.00;
						worksheet.Column(10).Width = 20.00;
						worksheet.Column(11).Width = 20.00;
						worksheet.Column(12).Width = 20.00;

						worksheet.Cells["A1:L1"].AutoFilter = true;

                        // Lock the worksheet. You can do this here, or in the end. Doesn't really matter.
                        worksheet.Protection.IsProtected = true;
                        worksheet.Protection.SetPassword("Opt1ma2022");
                        worksheet.Protection.AllowSelectUnlockedCells = true;
                        worksheet.Protection.AllowSelectLockedCells = false;
                        worksheet.Protection.AllowAutoFilter = true;
                        worksheet.Protection.AllowDeleteRows = true;
                        //worksheet.Protection.AllowEditObject = true;
                        // Assuming `HEADER_ROW_OFFSET` is the first row that's not a header,
                        // we first define a "data" range as starting from the first column of that row, 
                        // to the very last used row & column.
                        var dataCells = worksheet.Cells[2, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];

                        // Now go through each cell in that range,
                        foreach (var cel in dataCells)
                        {
                            // and unlock when it has content.
                            if (cel.Value != null) cel.Style.Locked = false;
                        }
                    }

                    recordIndex++;
                }


                using (var cells = worksheet.Cells[1, 1, 1, 12])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                worksheet.Cells[items.Count() + 2, 1].Value = "END";
                worksheet.Row(items.Count() + 2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(items.Count() + 2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;

            }
        }

		[HttpGet("exportPrintTemplate")]
		public IActionResult ExportPrintTemplate()
		{
			int i = 2;
			using (ExcelPackage package = new ExcelPackage())
			{
				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("model-import-etichete");
				//First add the headers
				worksheet.Cells[1, 1].Value = "ASSET";
				worksheet.Cells[2, 1].Value = "2112000000100";

				worksheet.Column(1).AutoFit();

				worksheet.Column(1).Width = 20;

				worksheet.Cells["A1:A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Cells["A1:A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

				worksheet.Cells["A1:A2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A1:A2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A1:A2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A1:A2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				//worksheet.Protection.IsProtected = true;
				//worksheet.Cells["A1:A3"].Style.Locked = false;

				using (var cells = worksheet.Cells[1, 1, 1, 1])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.MediumAquamarine);
				}

				//for (int a = 5000 - i; i < 5000; i++)
				//{
				//	//Unlock non-Id fields
				//	worksheet.Cells["A" + i.ToString()].Style.Locked = false;
				//}

				//Set worksheet protection attributes
				//worksheet.Protection.AllowInsertRows = true;
				//worksheet.Protection.AllowSort = true;
				//worksheet.Protection.AllowSelectUnlockedCells = true;
				//worksheet.Protection.AllowAutoFilter = true;
				//worksheet.Protection.AllowInsertRows = true;
				//worksheet.Protection.IsProtected = true;



				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "model-import-printae etichete.xlsx"
				};

				return result;

			}
		}

		[HttpGet("exportImportPrintLabel")]
		public IActionResult ExportImportPrintLabel(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
		{
			AssetDepTotal depTotal = null;
			AssetFilter assetFilter = null;
			string employeeId = string.Empty;
			string admCenterId = string.Empty;
			int rowNumber = 0;
			Paging paging = null;
			Sorting sorting = null;
			string userName = string.Empty;
			string role = string.Empty;

			assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


			includes = @"Adm.CostCenter.AdmCenter,Adm.Employee,Adm.AssetType,Adm.Project,Asset.BudgetBase.Project";


			if (HttpContext.User.Identity.Name != null)
			{
				userName = HttpContext.User.Identity.Name;
				role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
				employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

				assetFilter.UserName = userName;
				assetFilter.Role = role;

				if (employeeId != null)
				{
					assetFilter.EmployeeId = int.Parse(employeeId);
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


			var items = (_itemsRepository as IAssetsRepository)
				.GetMonthStockIT(assetFilter, includes, sorting, paging, out depTotal).ToList();

			using (ExcelPackage package = new ExcelPackage())
			{

				assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("STOCK-IT-MFX-IMPORT");

				//First add the headers
				worksheet.Cells[1, 1].Value = "Cod OPTIMA";
				worksheet.Cells[1, 2].Value = "Descriere";
				worksheet.Cells[1, 3].Value = "Centru de cost";
				worksheet.Cells[1, 4].Value = "Profit Center";
				worksheet.Cells[1, 5].Value = "SN/IMEI";
				worksheet.Cells[1, 6].Value = "Marca user";
				worksheet.Cells[1, 7].Value = "User";
				worksheet.Cells[1, 8].Value = "Cod WBS";
				worksheet.Cells[1, 9].Value = "Cod Buget OPTIMA";

				int recordIndex = 2;
				int count = items.Count();

				foreach (var item in items)
				{
					rowNumber++;

					int diff = recordIndex - count;

					if (diff > 0)
					{
						diff = 0;
					}


					worksheet.Cells[recordIndex, 1].Value = item.Asset.ERPCode;
					worksheet.Cells[recordIndex, 2].Value = item.Asset.Name;
					worksheet.Cells[recordIndex, 3].Value = item.Asset.CostCenter != null ? item.Asset.CostCenter.Code : "";
					worksheet.Cells[recordIndex, 4].Value = item.Asset.CostCenter != null && item.Asset.CostCenter.AdmCenter != null ? item.Asset.CostCenter.AdmCenter.Code : "";
					worksheet.Cells[recordIndex, 5].Value = item.Asset.SerialNumber != null ? item.Asset.SerialNumber : "";
					worksheet.Cells[recordIndex, 6].Value = item.Asset.Employee != null ? item.Asset.Employee.InternalCode : "";
					worksheet.Cells[recordIndex, 7].Value = item.Asset.Employee != null && item.Asset.Employee.Email != null ? item.Asset.Employee.Email.Substring(0, item.Asset.Employee.Email.IndexOf('@')) : "";
					worksheet.Cells[recordIndex, 8].Value = item.Asset.BudgetBase != null && item.Asset.BudgetBase.Project != null ? item.Asset.BudgetBase.Project.Code : "";
					worksheet.Cells[recordIndex, 9].Value = item.Asset.BudgetBase != null ? item.Asset.BudgetBase.Code : "";

					if (diff == 0)
					{
						for (int i = 1; i < 10; i++)
						{
							worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						worksheet.Row(1).Height = 35.00;
						worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.View.FreezePanes(2, 1);

						using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 9])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						using (var cells = worksheet.Cells[1, 1, 1, 9])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 9])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
						}

						using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 9])
						{
							for (int i = 2; i < items.Count() + 2; i++)
							{
								worksheet.Row(i).Height = 15.00;
								worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

								worksheet.Cells[$"A{i}"].Style.WrapText = true;
								worksheet.Cells[$"B{i}"].Style.WrapText = true;
								worksheet.Cells[$"C{i}"].Style.WrapText = true;
								worksheet.Cells[$"D{i}"].Style.WrapText = true;
								worksheet.Cells[$"E{i}"].Style.WrapText = true;
								worksheet.Cells[$"F{i}"].Style.WrapText = true;
								worksheet.Cells[$"G{i}"].Style.WrapText = true;
								worksheet.Cells[$"H{i}"].Style.WrapText = true;
								worksheet.Cells[$"I{i}"].Style.WrapText = true;
							}
						}


						worksheet.View.ShowGridLines = false;
						worksheet.View.ZoomScale = 100;

						for (int i = 1; i < 10; i++)
						{
							worksheet.Column(i).AutoFit();
						}

						worksheet.Column(1).Width = 21.00;
						worksheet.Column(2).Width = 80.00;
						worksheet.Column(3).Width = 20.00;
						worksheet.Column(4).Width = 20.00;
						worksheet.Column(5).Width = 20.00;
						worksheet.Column(6).Width = 20.00;
						worksheet.Column(7).Width = 20.00;
						worksheet.Column(8).Width = 20.00;
						worksheet.Column(9).Width = 20.00;

						worksheet.Cells["A1:I1"].AutoFilter = true;

						// Lock the worksheet. You can do this here, or in the end. Doesn't really matter.
						worksheet.Protection.IsProtected = true;
						worksheet.Protection.SetPassword("Opt1ma2022");
						worksheet.Protection.AllowSelectUnlockedCells = true;
						worksheet.Protection.AllowSelectLockedCells = false;
						worksheet.Protection.AllowAutoFilter = true;
						worksheet.Protection.AllowDeleteRows = true;
						//worksheet.Protection.AllowEditObject = true;
						// Assuming `HEADER_ROW_OFFSET` is the first row that's not a header,
						// we first define a "data" range as starting from the first column of that row, 
						// to the very last used row & column.
						var dataCells = worksheet.Cells[2, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];

						// Now go through each cell in that range,
						foreach (var cel in dataCells)
						{
							// and unlock when it has content.
							if (cel.Value != null) cel.Style.Locked = false;
						}
					}

					recordIndex++;
				}


				using (var cells = worksheet.Cells[1, 1, 1, 9])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

				}

				worksheet.Cells[items.Count() + 2, 1].Value = "END";
				worksheet.Row(items.Count() + 2).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Row(items.Count() + 2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Export.xlsx"
				};

				return result;

			}
		}

		[HttpGet("exportWFH")]
        public IActionResult ExportWFH(int page, int pageSize, string sortColumn, string sortDirection, string includes, string jsonFilter)
        {

            AssetDepTotal depTotal = null;
            AssetFilter assetFilter = null;
            string employeeId = string.Empty;
            string admCenterId = string.Empty;
            int rowNumber = 0;
            Paging paging = null;
            Sorting sorting = null;
            string userName = string.Empty;
            string role = string.Empty;

            assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();


            includes = @"Asset.DictionaryItem,Asset.Brand,Asset.Model,Adm.Employee.CostCenter.Division.Department,Asset.WFHState";


            if (HttpContext.User.Identity.Name != null)
            {
                userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                assetFilter.UserName = userName;
                assetFilter.Role = role;

                if (employeeId != null)
                {
                    assetFilter.EmployeeId = int.Parse(employeeId);
                }
                else
                {
                    assetFilter.EmployeeIds = null;
                    assetFilter.EmployeeIds = new List<int?>();
                    assetFilter.EmployeeIds.Add(int.Parse("100000000"));
                }
            }
            else
            {
                assetFilter.EmployeeIds = null;
                assetFilter.EmployeeIds = new List<int?>();
                assetFilter.EmployeeIds.Add(int.Parse("100000000"));
            }


            var items = (_itemsRepository as IAssetsRepository)
                .GetMonthWFH(assetFilter, includes, sorting, paging, out depTotal).ToList();

            using (ExcelPackage package = new ExcelPackage())
            {

                assetFilter = jsonFilter != null ? JsonConvert.DeserializeObject<AssetFilter>(jsonFilter) : new AssetFilter();

                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("WFH-2024");
                //First add the headers
                worksheet.Cells[1, 1].Value = "Confirmat";
                worksheet.Cells[1, 2].Value = "Marca";
                worksheet.Cells[1, 3].Value = "Nume";
                worksheet.Cells[1, 4].Value = "Prenume";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "Echipament";
                worksheet.Cells[1, 7].Value = "Marca/Brand";
                worksheet.Cells[1, 8].Value = "Model";
                worksheet.Cells[1, 9].Value = "Serie";
				worksheet.Cells[1, 10].Value = "IMEI";
				worksheet.Cells[1, 11].Value = "Numar telefon";
				worksheet.Cells[1, 12].Value = "Nr. inventar";
                worksheet.Cells[1, 13].Value = "Observatii";
				worksheet.Cells[1, 14].Value = "Centru de cost";
				worksheet.Cells[1, 15].Value = "Departament";
				worksheet.Cells[1, 16].Value = "B.U.";
				worksheet.Cells[1, 17].Value = "Cod OPT";
                worksheet.Cells[1, 18].Value = "Status";
                worksheet.Cells[1, 19].Value = "Data adaugare";
                worksheet.Cells[1, 20].Value = "Data validare";

                int recordIndex = 2;
                int count = items.Count();

                foreach (var item in items)
                {
                    rowNumber++;

                    int diff = recordIndex - count;

                    if (diff > 0)
                    {
                        diff = 0;
                    }

                    if (item.Asset.WFHState.Id == 88)
                        worksheet.Cells[recordIndex, 1].Value = "NU";
                    else if (item.Asset.WFHState.Id == 89)
                        worksheet.Cells[recordIndex, 1].Value = "DA";
                    worksheet.Cells[recordIndex, 2].Value = item.Adm.Employee.InternalCode;
                    worksheet.Cells[recordIndex, 3].Value = item.Adm.Employee.FirstName;
                    worksheet.Cells[recordIndex, 4].Value = item.Adm.Employee.LastName;
                    worksheet.Cells[recordIndex, 5].Value = item.Adm.Employee.Email; ;
                    worksheet.Cells[recordIndex, 6].Value = item.Asset.DictionaryItem != null ? item.Asset.DictionaryItem.Name : "";
                    worksheet.Cells[recordIndex, 7].Value = item.Asset.Brand != null ? item.Asset.Brand.Name : "";
                    worksheet.Cells[recordIndex, 8].Value = item.Asset.Model != null ? item.Asset.Model.Name : "";
                    worksheet.Cells[recordIndex, 9].Value = item.Asset.SerialNumber;
					worksheet.Cells[recordIndex, 10].Value = item.Asset.Imei;
					worksheet.Cells[recordIndex, 11].Value = item.Asset.PhoneNumber;
					worksheet.Cells[recordIndex, 12].Value = item.Asset.SAPCode;
                    worksheet.Cells[recordIndex, 13].Value = item.Asset.Info;
					worksheet.Cells[recordIndex, 14].Value = item.Asset.Employee != null && item.Asset.Employee.CostCenter != null ? item.Asset.Employee.CostCenter.Code : "" ;
					worksheet.Cells[recordIndex, 15].Value = item.Asset.Employee != null && item.Asset.Employee.CostCenter != null && item.Asset.Employee.CostCenter.Division != null ? item.Asset.Employee.CostCenter.Division.Name : "";
					worksheet.Cells[recordIndex, 16].Value = item.Asset.Employee != null && item.Asset.Employee.CostCenter != null && item.Asset.Employee.CostCenter.Division != null && item.Asset.Employee.CostCenter.Division.Department != null ? item.Asset.Employee.CostCenter.Division.Department.Name : "";
					worksheet.Cells[recordIndex, 17].Value = item.Asset.InvNo;
                    worksheet.Cells[recordIndex, 18].Value = item.Asset.WFHState != null ? item.Asset.WFHState.Name : "";
                    worksheet.Cells[recordIndex, 19].Value = item.Asset.CreatedAt.ToString();
                    worksheet.Cells[recordIndex, 20].Value = item.Asset.ModifiedAt.ToString();

                    if (diff == 0)
                    {

                        for (int i = 1; i < 21; i++)
                        {
                            worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        worksheet.Row(1).Height = 35.00;
                        worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.View.FreezePanes(2, 1);

                        using (var cells = worksheet.Cells[1, 1, items.Count() + 1, 20])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
                            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

                        }

                        using (var cells = worksheet.Cells[1, 1, 1, 20])
                        {
                            cells.Style.Font.Bold = true;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
                            cells.Style.Font.Color.SetColor(Color.Black);
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 20])
                        {
                            cells.Style.Font.Bold = false;
                            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
                        }

                        using (var cells = worksheet.Cells[2, 1, items.Count() + 3, 20])
                        {
                            for (int i = 2; i < items.Count() + 2; i++)
                            {
                                worksheet.Row(i).Height = 15.00;
                                worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[$"A{i}"].Style.WrapText = true;
                                worksheet.Cells[$"B{i}"].Style.WrapText = true;
                                worksheet.Cells[$"C{i}"].Style.WrapText = true;
                                worksheet.Cells[$"D{i}"].Style.WrapText = true;
                                worksheet.Cells[$"E{i}"].Style.WrapText = true;
                                worksheet.Cells[$"F{i}"].Style.WrapText = true;
                                worksheet.Cells[$"G{i}"].Style.WrapText = true;
                                worksheet.Cells[$"H{i}"].Style.WrapText = true;
                                worksheet.Cells[$"I{i}"].Style.WrapText = true;
                                worksheet.Cells[$"J{i}"].Style.WrapText = true;
								worksheet.Cells[$"K{i}"].Style.WrapText = true;
								worksheet.Cells[$"L{i}"].Style.WrapText = true;
								worksheet.Cells[$"M{i}"].Style.WrapText = true;
								worksheet.Cells[$"N{i}"].Style.WrapText = true;
								worksheet.Cells[$"O{i}"].Style.WrapText = true;
								worksheet.Cells[$"P{i}"].Style.WrapText = true;
							}



                        }


                        worksheet.View.ShowGridLines = false;
                        worksheet.View.ZoomScale = 100;

                        for (int i = 1; i < 21; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }
                        worksheet.Column(1).Width = 10.00;
                        worksheet.Column(2).Width = 10.00;
                        worksheet.Column(3).Width = 20.00;
                        worksheet.Column(4).Width = 22.00;
                        worksheet.Column(5).Width = 31.00;
                        worksheet.Column(6).Width = 15.00;
                        worksheet.Column(7).Width = 25.00;
                        worksheet.Column(8).Width = 44.00;
                        worksheet.Column(9).Width = 47.00;
                        worksheet.Column(10).Width = 43.00;
						worksheet.Column(11).Width = 43.00;
						worksheet.Column(12).Width = 43.00;
						worksheet.Column(13).Width = 143.00;
						worksheet.Column(14).Width = 31.00;
						worksheet.Column(15).Width = 31.00;
						worksheet.Column(16).Width = 31.00;
						worksheet.Column(17).Width = 31.00;
                        worksheet.Column(18).Width = 31.00;
                        worksheet.Column(19).Width = 31.00;
                        worksheet.Column(20).Width = 31.00;

                        worksheet.Cells["A1:T1"].AutoFilter = true;

                    }

                    recordIndex++;
                }


                using (var cells = worksheet.Cells[1, 1, 1, 20])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

                }

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //HttpContext.Response.ContentType = entityFile.FileType;
                HttpContext.Response.ContentType = contentType;
                FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
                {
                    FileDownloadName = "Export.xlsx"
                };

                return result;
            }
        }

        [Authorize]
        [HttpPost("uploadPrintDate")]
        public async Task<IActionResult> UpdatePrintDate([FromBody] int[] assetIds)
        {
            var updated = false;

            if (assetIds.Count() > 0)
            {
                foreach (var item in assetIds)
                {
                    Model.Asset asset = await _context.Set<Model.Asset>().Where(a => a.Id == item).SingleAsync();

                    asset.IsPrinted = true;
                    asset.PrintDate = DateTime.Now;

                    _context.Update(asset);

                    _context.SaveChanges();

                    updated = true;


                }
            }

            if (updated)
            {
                return Ok(StatusCode(200));
            }
            else
            {
                return Ok(StatusCode(404));
            }

        }

        [AllowAnonymous]
        [HttpPut("assetIsPrinted/{assetId}")]
        public int UpdateAssetIsPrinted(int assetId)
        {
            Model.Asset asset = null;

            asset = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

            if (asset != null)
            {
                asset.IsPrinted = true;
                asset.PrintDate = DateTime.Now;
                asset.ModifiedAt = DateTime.Now;
                _context.Update(asset);
                _context.SaveChanges();
            }

            return asset.Id;
        }

        //[HttpPost("createAsset")]
        //public async Task<GetStockResult> GetContractsAsync()
        //{
        //    HttpClient clientContract = null;

        //    var bearerToken = "";
        //    var baseUrl = _BASEURL;
        //    GetStockResult result = null;
        //    //using (HttpClient client = new HttpClient())
        //    //{
        //    //    client.BaseAddress = new Uri("https://api-eu.ariba.com/v2/oauth/token");

        //    //    HttpRequestMessage request = new HttpRequestMessage();
        //    //    request.Method = HttpMethod.Post;

        //    //    var keysValues = new List<KeyValuePair<string, string>>();
        //    //    // keysValues.Add(new KeyValuePair<string, string>("client_id", "1bfe66b6-7814-4d29-8a7a-60c9f4fa55ed"));
        //    //    // keysValues.Add(new KeyValuePair<string, string>("resource", "https://api-eu.ariba.com/v2/oauth/token"));
        //    //    //keysValues.Add(new KeyValuePair<string, string>("client_secret", "W~g9Q6-1V8-hnU-FIGZAXePO4ez~-347AV"));
        //    //    keysValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
        //    //    // keysValues.Add(new KeyValuePair<string, string>("Authorization", "Basic NGQwYTQwYWMtOGRkNy00YTFhLTg2MzktNzY5ZGExYjdmZmNiOnBUTFZiWGh6alV0WWgycHBTcFl6bjVyZUVScXB4U2E5"));

        //    //    request.Content = new FormUrlEncodedContent(keysValues);
        //    //    request.Headers.Add("Authorization", "Basic NGQwYTQwYWMtOGRkNy00YTFhLTg2MzktNzY5ZGExYjdmZmNiOnBUTFZiWGh6alV0WWgycHBTcFl6bjVyZUVScXB4U2E5");

        //    //    var bearerResult = await client.SendAsync(request);
        //    //    var bearerData = await bearerResult.Content.ReadAsStringAsync();
        //    //    bearerToken = JObject.Parse(bearerData)["access_token"].ToString();
        //    //}

        //    using (clientContract = new HttpClient())
        //    {
        //        //HttpRequestMessage request = new HttpRequestMessage();
        //        //request.Method = HttpMethod.Post;

        //        //clientContract.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);
        //        //clientContract.DefaultRequestHeaders.Add("apikey", "ymgtoKPybgdursz3U0n7NXB4uzIjdIyV");

        //        // Dto.SAP.Data[] data = new Dto.SAP.Data[1];

        //        IList<Dto.Data> oIList1 = new List<Dto.Data> 
        //        { 
        //            new Dto.Data() 
        //                { 
        //                    I_INPUT = new Dto.Input()
        //                        { 
        //                            Plant = "RO02", Category= "91", Storage_Location = "MFX" 
        //                        } 
        //                 } 
        //        };

        //        var postUser = new Dto.GetStock
        //        {
        //            Sap_function = "ZMMF_GET_STOCK",
        //            Options = new Dto.Options()
        //            {
        //                Api_call_timeout = 180
        //            },
        //            Remote_host_name = "test",
        //            Data = oIList1
        //        };

        //        JsonContent contentJson = JsonContent.Create(postUser);
        //        clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

        //        try
        //        {
        //            var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

        //            //if (!httpResponse.IsSuccessStatusCode)
        //            //{
        //            //    throw new Exception("Cannot retrieve tasks");
        //            //}

        //            // result = await httpResponse.Content.ReadAsStringAsync();
        //            var content = await httpResponse.Content.ReadAsStringAsync();
        //            //close out the client
        //            clientContract.Dispose();

        //            // return result;


        //            result = JsonConvert.DeserializeObject<GetStockResult>(content);

                    
        //        }
        //        catch (Exception e)
        //        {
        //            Console.Write("Error", ConsoleColor.Red);
        //            Console.Write(e.Message, ConsoleColor.DarkRed);

        //            // for other error types just write the info without the FailedRecipient
        //            using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
        //            {
        //                errorfile.WriteLine(e.StackTrace);
        //                errorfile.WriteLine(e.ToString());

        //            };
        //        }
        //        finally
        //        {
                   
        //        }

        //        return result;
        //    }

        //}

        [HttpPost("createAsset")]
        public async Task<string> CreateAssetAsync(List<Dto.CreateAssetSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            // CreateAssetResult result = null;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.CreateAssetData> oIList1 = new List<Dto.CreateAssetData>
                {
                    new Dto.CreateAssetData()
                        {
                            I_INPUT = new Dto.CreateAssetInput()
                                {
                                    XSUBNO = "",
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    ASSET = asset[0].ASSET,
                                    SUBNUMBER = asset[0].SUBNUMBER,
                                    ASSETCLASS = asset[0].ASSETCLASS,
                                    POSTCAP = asset[0].POSTCAP,
                                    DESCRIPT = asset[0].DESCRIPT,
                                    DESCRIPT2 = asset[0].DESCRIPT2,
                                    INVENT_NO = asset[0].INVENT_NO,
                                    SERIAL_NO = asset[0].SERIAL_NO,
                                    QUANTITY = asset[0].QUANTITY,
                                    BASE_UOM = asset[0].BASE_UOM,
                                    LAST_INVENTORY_DATE = asset[0].LAST_INVENTORY_DATE,
                                    LAST_INVENTORY_DOCNO = asset[0].LAST_INVENTORY_DOCNO,
                                    CAP_DATE = asset[0].CAP_DATE,
                                    COSTCENTER = asset[0].COSTCENTER,
                                    RESP_CCTR = asset[0].RESP_CCTR,
                                    INTERN_ORD = asset[0].INTERN_ORD,
                                    PLANT = asset[0].PLANT,
                                    LOCATION = asset[0].LOCATION,
                                    ROOM = asset[0].ROOM,
                                    PERSON_NO = asset[0].PERSON_NO,
                                    PLATE_NO = asset[0].PLATE_NO,
                                    ZZCLAS = asset[0].ZZCLAS,
                                    IN_CONSERVATION = asset[0].IN_CONSERVATION,
                                    PROP_IND = asset[0].PROP_IND,
                                    OPTIMA_ASSET_NO = asset[0].OPTIMA_ASSET_NO,
                                    OPTIMA_ASSET_PARENT_NO = asset[0].OPTIMA_ASSET_PARENT_NO,
                                    TESTRUN = asset[0].TESTRUN,
                                    VENDOR_NO = asset[0].VENDOR_NO,
                                }
                         }
                };

                var postUser = new Dto.CreateAsset
                {
                    Sap_function = "ZFIF_FIXED_ASSET_CREATE",
                    Options = new Dto.CreateAssetDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    //result = JsonConvert.DeserializeObject<CreateAssetResult>(content);


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    //using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
                    //{
                    //    errorfile.WriteLine(e.StackTrace);
                    //    errorfile.WriteLine(e.ToString());

                    //};
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("changeAsset")]
        public async Task<string> AssetChangeAsync(List<Dto.AssetChangeSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            // CreateAssetResult result = null;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.AssetChangeData> oIList1 = new List<Dto.AssetChangeData>
                {
                    new Dto.AssetChangeData()
                        {
                            I_INPUT = new Dto.AssetChangeInput()
                                {
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    ASSET = asset[0].ASSET,
                                    SUBNUMBER = asset[0].SUBNUMBER,
                                    ASSETCLASS = asset[0].ASSETCLASS,
                                    POSTCAP = asset[0].POSTCAP,
                                    DESCRIPT = asset[0].DESCRIPT,
                                    DESCRIPT2 = asset[0].DESCRIPT2,
                                    INVENT_NO = asset[0].INVENT_NO,
                                    SERIAL_NO = asset[0].SERIAL_NO,
                                    QUANTITY = asset[0].QUANTITY,
                                    BASE_UOM = asset[0].BASE_UOM,
                                    LAST_INVENTORY_DATE = asset[0].LAST_INVENTORY_DATE,
                                    LAST_INVENTORY_DOCNO = asset[0].LAST_INVENTORY_DOCNO,
                                    CAP_DATE = asset[0].CAP_DATE,
                                    COSTCENTER = asset[0].COSTCENTER,
                                    INTERN_ORD = asset[0].INTERN_ORD,
                                    PLANT = asset[0].PLANT,
                                    LOCATION = asset[0].LOCATION,
                                    ROOM = asset[0].ROOM,
                                    PERSON_NO = asset[0].PERSON_NO,
                                    PLATE_NO = asset[0].PLATE_NO,
                                    ZZCLAS = asset[0].ZZCLAS,
                                    IN_CONSERVATION = asset[0].IN_CONSERVATION,
                                    PROP_IND = asset[0].PROP_IND,
                                    OPTIMA_ASSET_NO = asset[0].OPTIMA_ASSET_NO,
                                    OPTIMA_ASSET_PARENT_NO = asset[0].OPTIMA_ASSET_PARENT_NO,
                                    VENDOR_NO = asset[0].VENDOR_NO,
                                }
                         }
                };

                var postUser = new Dto.AssetChange
                {
                    Sap_function = "ZFIF_FIXED_ASSET_CHANGE",
                    Options = new Dto.AssetChangeDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {

					using (var errorfile = System.IO.File.CreateText("contentJsonTransfer" + DateTime.Now.Ticks + ".txt"))
					{
						errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

					};


					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    //result = JsonConvert.DeserializeObject<CreateAssetResult>(content);


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }


        [HttpPost("acquisitionAsset")]
        public async Task<string> CreateAcquisitionAssetAsync(List<Dto.AcquisitionAssetSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            // CreateAssetResult result = null;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.AcquisitionAssetData> oIList1 = new List<Dto.AcquisitionAssetData>
                {
                    new Dto.AcquisitionAssetData()
                        {
                            I_INPUT = new Dto.AcquisitionAssetInput()
                                {
                                    STORNO = "",
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    DOC_DATE = asset[0].DOC_DATE,
                                    PSTNG_DATE = asset[0].PSTNG_DATE,
                                    REF_DOC_NO = asset[0].REF_DOC_NO,
                                    HEADER_TXT = asset[0].HEADER_TXT,
                                    VENDOR_NO = asset[0].VENDOR_NO,
                                    CURRENCY = asset[0].CURRENCY,
                                    EXCH_RATE = asset[0].EXCH_RATE,
                                    TOTAL_AMOUNT = asset[0].TOTAL_AMOUNT,
                                    ASSETS = asset[0].ASSETS
                                    //ASSETS = new List<Dto.AcquisitionAssets>
                                    //    {
                                    //       new Dto.AcquisitionAssets()
                                    //       {
                                    //            ASSET = asset[0].ASSETS[0].ASSET,
                                    //            SUBNUMBER = asset[0].ASSETS[0].SUBNUMBER,
                                    //            ITEM_TEXT = asset[0].ASSETS[0].ITEM_TEXT,
                                    //            TAX_CODE = asset[0].ASSETS[0].TAX_CODE,
                                    //            NET_AMOUNT = asset[0].ASSETS[0].NET_AMOUNT,
                                    //            TAX_AMOUNT = asset[0].ASSETS[0].TAX_AMOUNT,
                                    //            GL_ACCOUNT = asset[0].ASSETS[0].GL_ACCOUNT,
                                    //       }
                                    //    }
                                }
                         }
                };

                var postUser = new Dto.AcquisitionAsset
                {
                    Sap_function = "ZFIF_FIXED_ASSET_ACQUISITION",
                    Options = new Dto.AcquisitionAssetDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    //result = JsonConvert.DeserializeObject<CreateAssetResult>(content);


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("transferAsset")]
        public async Task<string> CreateTransferAssetAsync(List<Dto.TransferAssetSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.TransferAssetData> oIList1 = new List<Dto.TransferAssetData>
                {
                    new Dto.TransferAssetData()
                        {
                            I_INPUT = new Dto.TransferAssetInput()
                                {
                                    FROM_ASSET = asset[0].FROM_ASSET,
                                    FROM_SUBNUMBER = asset[0].FROM_SUBNUMBER,
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    DOC_DATE = asset[0].DOC_DATE,
                                    PSTNG_DATE = asset[0].PSTNG_DATE,
                                    ASVAL_DATE = asset[0].ASVAL_DATE,
                                    ITEM_TEXT = asset[0].ITEM_TEXT,
                                    TO_ASSET = asset[0].TO_ASSET,
                                    TO_SUBNUMBER = asset[0].TO_SUBNUMBER,
                                    FIS_PERIOD = asset[0].FIS_PERIOD,
                                    DOC_TYPE = asset[0].DOC_TYPE,
                                    REF_DOC_NO = asset[0].REF_DOC_NO,
                                    COMPL_TRANSFER = asset[0].COMPL_TRANSFER,
                                    AMOUNT = asset[0].AMOUNT,
                                    CURRENCY = asset[0].CURRENCY,
                                    PERCENT = asset[0].PERCENT,
                                    QUANTITY = asset[0].QUANTITY,
                                    BASE_UOM = asset[0].BASE_UOM,
                                    PRIOR_YEAR_ACQUISITIONS = asset[0].PRIOR_YEAR_ACQUISITIONS,
                                    CURRENT_YEAR_ACQUISITIONS = asset[0].CURRENT_YEAR_ACQUISITIONS,
                                }
                         }
                };

                var postUser = new Dto.TransferAsset
                {
                    Sap_function = "ZFIF_FIXED_ASSET_TRANSFER",
                    Options = new Dto.TransferAssetDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("retireAsset")]
        public async Task<string> CreateRetireAssetAsync(List<Dto.RetireAssetSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.RetireAssetData> oIList1 = new List<Dto.RetireAssetData>
                {
                    new Dto.RetireAssetData()
                        {
                            I_INPUT = new Dto.RetireAssetInput()
                                {
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    ASSET = asset[0].ASSET,
                                    SUBNUMBER = asset[0].SUBNUMBER,
                                    DOC_DATE = asset[0].DOC_DATE,
                                    PSTNG_DATE = asset[0].PSTNG_DATE,
                                    VALUEDATE = asset[0].VALUEDATE,
                                    ITEM_TEXT = asset[0].ITEM_TEXT,
                                    FIS_PERIOD = asset[0].FIS_PERIOD,
                                    DOC_TYPE = asset[0].DOC_TYPE,
                                    REF_DOC_NO = asset[0].REF_DOC_NO,
                                    COMPL_RET = asset[0].COMPL_RET,
                                    AMOUNT = asset[0].AMOUNT,
                                    CURRENCY = asset[0].CURRENCY,
                                    PERCENT = asset[0].PERCENT,
                                    QUANTITY = asset[0].QUANTITY,
                                    BASE_UOM = asset[0].BASE_UOM,
                                    PRIOR_YEAR_ACQUISITIONS = asset[0].PRIOR_YEAR_ACQUISITIONS,
                                    CURRENT_YEAR_ACQUISITIONS = asset[0].CURRENT_YEAR_ACQUISITIONS,
                                }
                         }
                };

                var postUser = new Dto.RetireAsset
                {
                    Sap_function = "ZFIF_FIXED_ASSET_RETIRE",
                    Options = new Dto.RetireAssetDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("assetInvPlus")]
        public async Task<string> CreateAssetInvPlusAsync(List<Dto.AssetInvPlusSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.AssetInvPlusData> oIList1 = new List<Dto.AssetInvPlusData>
                {
                    new Dto.AssetInvPlusData()
                        {
                            I_INPUT = new Dto.AssetInvPlusInput()
                                {
                                    ASSET = asset[0].ASSET,
                                    SUBNUMBER = asset[0].SUBNUMBER,
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    DOC_DATE = asset[0].DOC_DATE,
                                    PSTNG_DATE = asset[0].PSTNG_DATE,
                                    ASVAL_DATE = asset[0].ASVAL_DATE,
                                    ITEM_TEXT = asset[0].ITEM_TEXT,
                                    DOC_TYPE = asset[0].DOC_TYPE,
                                    REF_DOC_NO = asset[0].REF_DOC_NO,
                                    AMOUNT = asset[0].AMOUNT,
                                    TRANSTYPE = asset[0].TRANSTYPE
                                }
                         }
                };

                var postUser = new Dto.AssetInvPlus
                {
                    Sap_function = "ZFIF_FIXED_ASSET_INV_PLUS",
                    Options = new Dto.AssetInvPlusDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("assetInvMinus")]
        public async Task<string> CreateAssetInvMinusAsync(List<Dto.AssetInvMinusSAP> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.AssetInvMinusData> oIList1 = new List<Dto.AssetInvMinusData>
                {
                    new Dto.AssetInvMinusData()
                        {
                            I_INPUT = new Dto.AssetInvMinusInput()
                                {
                                    ASSET = asset[0].ASSET,
                                    SUBNUMBER = asset[0].SUBNUMBER,
                                    COMPANYCODE = asset[0].COMPANYCODE,
                                    DOC_DATE = asset[0].DOC_DATE,
                                    PSTNG_DATE = asset[0].PSTNG_DATE,
                                    ASVAL_DATE = asset[0].ASVAL_DATE,
                                    ITEM_TEXT = asset[0].ITEM_TEXT,
                                    DOC_TYPE = asset[0].DOC_TYPE,
                                    REF_DOC_NO = asset[0].REF_DOC_NO,
                                    TRANSTYPE = asset[0].TRANSTYPE,
                                    INVENTORY_DIFF = asset[0].INVENTORY_DIFF,
                                    //AMOUNT = asset[0].AMOUNT,
                                    
                                }
                         }
                };

                var postUser = new Dto.AssetInvMinus
                {
                    Sap_function = "ZFIF_FIXED_ASSET_INV_MINUS",
                    Options = new Dto.AssetInvMinusDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpGet("getStockByCategoryID/{categoryID}")]
        public async Task<IActionResult> ProcessStock(string categoryID)
        {

            var result = await GetStock(categoryID);

            GetStockResult saveStockResult = null;

            if (result != "")
            {

				try
                {
					//using (var errorfile = System.IO.File.CreateText("stock-result-API-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));

					//};

					saveStockResult = JsonConvert.DeserializeObject<GetStockResult>(result);


					//using (var errorfile = System.IO.File.CreateText("stock-result-API-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(saveStockResult, Formatting.Indented));

					//};

					//using (var errorfile = System.IO.File.CreateText("stock-result-API-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(saveStockResult.Meta.ToString());
					//	errorfile.WriteLine(saveStockResult.Data.ToString());

					//};

					if (saveStockResult.Data != null && saveStockResult.Data.Return_Code == "1")
                    {
                        Model.Category category = null;
                        Model.Company company = null;
                        Model.Uom uom = null;
                        Model.Material material = null;
                        Model.Brand brand = null;
                        Model.Partner partner = null;
                        Model.PartnerLocation partnerLocation = null;
                        Model.Stock stock = null;
                        Model.Storage storage = null;
                        Model.Storage storageInitial = null;
                        Model.Plant plantInitial = null;
                        Model.Plant plantActual = null;

                        for (int c = 0; c < saveStockResult.Data.E_OutPut.Count; c++)
                        {
                            if (saveStockResult.Data.E_OutPut[c].Plant_Initial != "")
                            {
                                plantInitial = _context.Set<Model.Plant>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Plant_Initial.Trim()).FirstOrDefault();

                                if (plantInitial == null)
                                {
                                    plantInitial = new Model.Plant()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Plant_Initial.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Plant_Initial.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(plantInitial);
                                }
                            }

                            if (saveStockResult.Data.E_OutPut[c].Plant != "")
                            {
                                plantActual = _context.Set<Model.Plant>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Plant.Trim()).FirstOrDefault();

                                if (plantActual == null)
                                {
                                    plantActual = new Model.Plant()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Plant.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Plant.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(plantActual);
                                }
                            }


                            if (saveStockResult.Data.E_OutPut[c].Storage_Location_Initial != "")
                            {
                                storageInitial = _context.Set<Model.Storage>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Storage_Location_Initial.Trim()).FirstOrDefault();

                                if (storageInitial == null)
                                {
                                    storageInitial = new Model.Storage()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Storage_Location_Initial.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Storage_Location_Initial.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(storageInitial);
                                }
                            }

                            if (saveStockResult.Data.E_OutPut[c].Storage_Location != "")
                            {
                                storage = _context.Set<Model.Storage>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Storage_Location.Trim()).FirstOrDefault();

                                if (storage == null)
                                {
                                    storage = new Model.Storage()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Storage_Location.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Storage_Location.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(storage);
                                }
                            }



                            if (saveStockResult.Data.E_OutPut[c].Category != "")
                            {
                                category = _context.Set<Model.Category>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Category.Trim()).FirstOrDefault();

                                if (category == null)
                                {
                                    category = new Model.Category()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Category.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Category_Descr.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(category);
                                }
                            }
							

                            if (saveStockResult.Data.E_OutPut[c].CompanyCode != "")
                            {
                                company = _context.Set<Model.Company>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].CompanyCode.Trim()).FirstOrDefault();

                                if (company == null)
                                {
                                    company = new Model.Company()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].CompanyCode.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].CompanyCode.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(company);
                                }
                            }

                            if (saveStockResult.Data.E_OutPut[c].Currency != "")
                            {
                                uom = _context.Set<Model.Uom>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Currency.Trim()).FirstOrDefault();

                                if (uom == null)
                                {
                                    uom = new Model.Uom()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Currency.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Currency.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(uom);
                                }
                            }

                            if (saveStockResult.Data.E_OutPut[c].Material != "")
                            {
                                material = _context.Set<Model.Material>().Where(com => com.Code.Trim() == saveStockResult.Data.E_OutPut[c].Material.Trim()).FirstOrDefault();

                                if (material == null)
                                {
                                    material = new Model.Material()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Material.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Long_Descr.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(material);
                                }
                               
                            }

                            if (saveStockResult.Data.E_OutPut[c].Producer != "")
                            {
                                brand = _context.Set<Model.Brand>().Where(com => com.Name.Trim() == saveStockResult.Data.E_OutPut[c].Producer.Trim()).FirstOrDefault();

                                if (brand == null)
                                {
                                    brand = new Model.Brand()
                                    {
                                        Code = saveStockResult.Data.E_OutPut[c].Producer.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].Producer.Trim(),
                                        IsDeleted = false
                                    };

                                    _context.Add(brand);
                                }
                            }

                            if (saveStockResult.Data.E_OutPut[c].SupplierId != "")
                            {

                                partnerLocation = _context.Set<Model.PartnerLocation>().Where(com => com.Cui.Trim() == saveStockResult.Data.E_OutPut[c].SupplierId.Trim()).FirstOrDefault();

                                if (partnerLocation == null)
                                {
                                    partnerLocation = new Model.PartnerLocation()
                                    {
                                        Cui = saveStockResult.Data.E_OutPut[c].SupplierId.Trim(),
                                        Denumire = saveStockResult.Data.E_OutPut[c].SupplierName.Trim(),
                                        //RegistryNumber = saveStockResult.Data.E_OutPut[c].SupplierId.Trim(),
                                        //FiscalCode = saveStockResult.Data.E_OutPut[c].SupplierId.Trim(),
                                    };

                                    _context.Add(partnerLocation);
                                }
                               

                                partner = _context.Set<Model.Partner>().Where(com => com.RegistryNumber.Trim() == saveStockResult.Data.E_OutPut[c].SupplierId.Trim()).FirstOrDefault();

                                if (partner == null)
                                {
                                    partner = new Model.Partner()
                                    {
                                        ErpCode = saveStockResult.Data.E_OutPut[c].SupplierId.Trim(),
                                        Name = saveStockResult.Data.E_OutPut[c].SupplierName.Trim(),
                                        RegistryNumber = saveStockResult.Data.E_OutPut[c].SupplierId.Trim(),
                                        FiscalCode = saveStockResult.Data.E_OutPut[c].SupplierId.Trim(),
                                        PartnerLocation = partnerLocation
                                    };

                                    _context.Add(partner);
                                }

                            }


                            List<Model.Stock> stocks = _context.Set<Model.Stock>().Include(m => m.Material).Where(a => a.Code.Trim() == saveStockResult.Data.E_OutPut[c].Batch.Trim() && a.Material.Code.Trim() == saveStockResult.Data.E_OutPut[c].Material.Trim() && a.IsDeleted == false).OrderBy(a => a.Code).ToList();

                            if(stocks.Count > 0)
							{

                                for (int a = 0; a < stocks.Count; a++)
								{
                                    Model.Stock stockToUpdate = _context.Set<Model.Stock>().Where(s => s.Id == stocks[a].Id).SingleOrDefault();

                                    //stockToUpdate.Name = saveStockResult.Data.E_OutPut[c].Short_Descr.Trim();
                                    //stockToUpdate.LongName = saveStockResult.Data.E_OutPut[c].Long_Descr.Trim();
                                    //stockToUpdate.Plant = saveStockResult.Data.E_OutPut[c].Plant.Trim();
                                    //stockToUpdate.Quantity = saveStockResult.Data.E_OutPut[c].Quantity;
                                    //stockToUpdate.Storage_Location = saveStockResult.Data.E_OutPut[c].Storage_Location.Trim();
                                    //stockToUpdate.Value = saveStockResult.Data.E_OutPut[c].UnitCost;
                                    //stockToUpdate.UM = saveStockResult.Data.E_OutPut[c].Uom.Trim();
                                    //stockToUpdate.Category = category;
                                    //stockToUpdate.Company = company;
                                    //stockToUpdate.Uom = uom;
                                    //stockToUpdate.Brand = brand;
                                    //stockToUpdate.Partner = partner;

                                    //if (saveStockResult.Data.E_OutPut[c].Last_Incoming_Date != "")
                                    //{
                                    //    int year = int.Parse(saveStockResult.Data.E_OutPut[c].Last_Incoming_Date.Trim().Substring(0, 4));
                                    //    int month = int.Parse(saveStockResult.Data.E_OutPut[c].Last_Incoming_Date.Trim().Substring(4, 2));
                                    //    int day = int.Parse(saveStockResult.Data.E_OutPut[c].Last_Incoming_Date.Trim().Substring(6, 2));

                                    //    var date = new DateTime(year, month, day);

                                    //    stockToUpdate.Last_Incoming_Date = date;
                                    //}

                                    stockToUpdate.IsDeleted = true;

                                    _context.Update(stockToUpdate);
                                    _context.SaveChanges();
                                }
							}


                            stock = new Model.Stock()
                            {
                                Code = saveStockResult.Data.E_OutPut[c].Batch.Trim(),
                                Name = saveStockResult.Data.E_OutPut[c].Short_Descr.Trim(),
                                LongName = saveStockResult.Data.E_OutPut[c].Long_Descr.Trim(),
                                Plant = saveStockResult.Data.E_OutPut[c].Plant.Trim(),
                                Quantity = saveStockResult.Data.E_OutPut[c].Quantity,
                                Storage_Location = saveStockResult.Data.E_OutPut[c].Storage_Location.Trim(),
                                Value = saveStockResult.Data.E_OutPut[c].UnitCost,
                                UM = saveStockResult.Data.E_OutPut[c].Uom.Trim(),
                                Category = category,
                                Company = company,
                                Uom = uom,
                                Material = material,
                                Brand = brand,
                                Partner = partner,
                                EAN = saveStockResult.Data.E_OutPut[c].EAN,
                                Invoice = saveStockResult.Data.E_OutPut[c].Invoice,
                                PlantInitial = plantInitial,
                                PlantActual = plantActual,
                                Storage = storage,
                                StorageInitial = storageInitial
                            };


                            if (saveStockResult.Data.E_OutPut[c].Last_Incoming_Date != "")
                            {
                                int year = int.Parse(saveStockResult.Data.E_OutPut[c].Last_Incoming_Date.Trim().Substring(0, 4));
                                int month = int.Parse(saveStockResult.Data.E_OutPut[c].Last_Incoming_Date.Trim().Substring(4, 2));
                                int day = int.Parse(saveStockResult.Data.E_OutPut[c].Last_Incoming_Date.Trim().Substring(6, 2));

                                var date = new DateTime(year, month, day);

                                stock.Last_Incoming_Date = date;
                            }

                            _context.Add(stock);
                            _context.SaveChanges();


                        }
					}
					else
					{
                        if(saveStockResult.Meta.Code == 400)
						{
                            Model.Category category = _context.Set<Model.Category>().Where(a => a.IsDeleted == false && a.Code == categoryID).FirstOrDefault();

                            if(category != null)
							{
                                List<Model.Stock> stocks = _context.Set<Model.Stock>().Where(a => a.CategoryId == category.Id  && a.IsDeleted == false).ToList();

                                if (stocks.Count > 0)
                                {

                                    for (int a = 0; a < stocks.Count; a++)
                                    {
                                        Model.Stock stockToUpdate = _context.Set<Model.Stock>().Where(s => s.Id == stocks[a].Id).SingleOrDefault();
                                        stockToUpdate.IsDeleted = true;

                                        _context.Update(stockToUpdate);
                                        _context.SaveChanges();
                                    }
                                }
                            }
						}
					}
                }
                catch (Exception ex)
                {

                    using (var errorfile = System.IO.File.CreateText("errorStock-" + DateTime.Now.Ticks + ".txt"))
                    {
                        errorfile.WriteLine(ex.StackTrace);
                        errorfile.WriteLine(ex.ToString());

                    };
                }



                return Ok(saveStockResult);
            }
            else
            {
                //using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(result);
                //    errorfile.WriteLine(result);

                //};

                return Ok(result);
            }
        }

        public async Task<string> GetStock(string categoryID)
		{
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.StockData> oIList1 = new List<Dto.StockData>
                {
                    new Dto.StockData()
                        {
                            I_INPUT = new Dto.StockInput()
                                {
                                    Plant = "RO02",
                                    Storage_Location = "MFX",
                                    Category = categoryID != "NOCODE" && categoryID != null ? categoryID :"",
                                    Material = "",
                                    Batch = ""
                                }
                         }
                };

                var postUser = new Dto.GetStock
                {
                    Sap_function = "ZMMF_GET_STOCK",
                    Options = new Dto.StockOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

#if DEBUG
                string jsonString = await contentJson.ReadAsStringAsync();

                string resultFilePath = System.IO.Path.Combine("SAP-RESULTS", "GET_STOCK_" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(resultFilePath))
                {
                    errorfile.WriteLine(jsonString);

                };
#endif

                try
                {

					//using (var errorfile = System.IO.File.CreateText("contentJson" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

					//};

					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }
        }

        public async Task<string> GetNewAssetStock(string categoryID, string material, string batch)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.StockData> oIList1 = new List<Dto.StockData>
                {
                    new Dto.StockData()
                        {
                            I_INPUT = new Dto.StockInput()
                                {
                                    Plant = "RO02",
                                    Storage_Location = "MFX",
                                    Category = categoryID != "NOCODE" && categoryID != null ? categoryID :"",
                                    Material = material,
                                    Batch = batch
                                }
                         }
                };

                var postUser = new Dto.GetStock
                {
                    Sap_function = "ZMMF_GET_STOCK",
                    Options = new Dto.StockOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {

                    //using (var errorfile = System.IO.File.CreateText("contentJson" + DateTime.Now.Ticks + ".txt"))
                    //{
                    //	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

                    //};

                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }
        }

        [HttpPost("transferInStock")]
        public async Task<string> TransferInStockAsync(List<TransferInStockInput> asset)
        {
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            // CreateAssetResult result = null;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.TransferInStockData> oIList1 = new List<Dto.TransferInStockData>
                {
                    new Dto.TransferInStockData()
                        {
                            I_INPUT = new Dto.TransferInStockInput()
                                {
                                    Doc_Date = asset[0].Doc_Date,
                                    Pstng_Date = asset[0].Pstng_Date,
                                    Material = asset[0].Material,
                                    Plant = asset[0].Plant,
                                    Storage_Location = asset[0].Storage_Location,
                                    Quantity = asset[0].Quantity,
                                    Uom = asset[0].Uom,
                                    Batch = asset[0].Batch,
                                    // Gl_Account = asset[0].Gl_Account.Length == 6 ? "00" + asset[0].Gl_Account : asset[0].Gl_Account,
                                    Gl_Account = "",
                                    Item_Text = asset[0].Item_Text,
                                    Asset = asset[0].Asset,
                                    SubNumber = "0000"
                                }
                         }
                };

                var postUser = new Dto.TransferInStock
                {
                    Sap_function = "ZMMF_TRANSFER_STOCK_TO_ASSET",
                    Options = new Dto.TransferInStockOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {
                    using (var errorfile = System.IO.File.CreateText("before-transfer-in-stock-create2-" + DateTime.Now.Ticks + ".txt"))
                    {
                        errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

                    };

                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    //result = JsonConvert.DeserializeObject<CreateAssetResult>(content);


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("blockAsset")]
        public async Task<bool> BlockAsset([FromBody] int assetId)
        {

            BlockAssetResult blockAssetResult = null;
            bool success = false;
            if (assetId > 0)
            {

                var result = await this.BlockAssetAsync(assetId);

                if (result != "")
                {

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(result);

                    };

                    try
                    {
                        blockAssetResult = JsonConvert.DeserializeObject<BlockAssetResult>(result);

                        if (blockAssetResult.Data != null && blockAssetResult.Data.Return_Code == "1")
                        {
                            Model.Asset assetUpdate = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

                            // assetUpdate.IsDeleted = true;
                            assetUpdate.AssetStateId = 7;
                            assetUpdate.ModifiedAt = DateTime.Now;
                            _context.Update(assetUpdate);
                            _context.SaveChanges();

                            success = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-block-asset-" + DateTime.Now.Ticks + ".txt")))
                        {
                            errorfile.WriteLine(ex.StackTrace);
                            errorfile.WriteLine(ex.ToString());

                        };
                    }



                    return success;
                }
                else
                {

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(result);
                        errorfile.WriteLine(result);

                    };

                    return success;
                }


            }


            return success;
        }

        [HttpPost("blockAsset/{assetId}")]
        public async Task<string> BlockAssetAsync(int assetId)
        {

            Model.Asset asset = await _context.Set<Model.Asset>().Include(c => c.Company).Where(a => a.Id == assetId).SingleAsync();
            BlockAssetResult blockAssetResult = null;
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            // CreateAssetResult result = null;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.BlockAssetData> oIList1 = new List<Dto.BlockAssetData>
                {
                    new Dto.BlockAssetData()
                        {
                            I_INPUT = new Dto.BlockAssetInput()
                                {
                                    COMPANYCODE = asset.Company.Code,
                                    ASSET = asset.InvNo,
                                    SUBNUMBER = asset.SubNo.ToString(),
                                    BLOCK = "X",
                                    OPTIMA_ASSET_NO = asset.InvNo,
                                    OPTIMA_ASSET_PARENT_NO = asset.InvNo
                                }
                         }
                };

                var postUser = new Dto.BlockAsset
                {
                    Sap_function = "ZFIF_FIXED_ASSET_BLOCK",
                    Options = new Dto.BlockAssetDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {

                    using (var errorfile = System.IO.File.CreateText("contentJsonTransfer" + DateTime.Now.Ticks + ".txt"))
                    {
                        errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

                    };


                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    //result = JsonConvert.DeserializeObject<CreateAssetResult>(content);


                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };
                }
                finally
                {

                }

                return result;
            }

        }

        [HttpPost("deleteAsset")]
        public async Task<bool> DeleteAsset([FromBody] int assetId)
        {

            DeleteAssetResult deleteAssetResult = null;
            bool success = false;
            if (assetId > 0)
            {

                var result = await this.DeleteAssetAsync(assetId);

                if (result != "")
                {

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-block-asset-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(result);

                    };

                    try
                    {
                        deleteAssetResult = JsonConvert.DeserializeObject<DeleteAssetResult>(result);

                        if (deleteAssetResult.Data != null && deleteAssetResult.Data.Return_Code == "1")
                        {
                            Model.Asset assetUpdate = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

                            // assetUpdate.IsDeleted = true;
                            assetUpdate.ModifiedAt = DateTime.Now;
                            assetUpdate.AssetStateId = 8;
                            _context.Update(assetUpdate);
                            _context.SaveChanges();

                            success = true;
                        }
                    }
                    catch (Exception ex)
                    {

                        using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-block-asset-" + DateTime.Now.Ticks + ".txt")))
                        {
                            errorfile.WriteLine(ex.StackTrace);
                            errorfile.WriteLine(ex.ToString());

                        };
                    }



                    return success;
                }
                else
                {

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(result);
                        errorfile.WriteLine(result);

                    };

                    return success;
                }


            }


            return success;
        }

        [HttpPost("deleteAsset/{assetId}")]
        public async Task<string> DeleteAssetAsync(int assetId)
        {

            Model.Asset asset = await _context.Set<Model.Asset>().Include(c => c.Company).Where(a => a.Id == assetId).SingleAsync();
            DeleteAssetResult blockAssetResult = null;
            HttpClient clientContract = null;

            var baseUrl = _BASEURL;
            // CreateAssetResult result = null;
            string result = "";
            using (clientContract = new HttpClient())
            {
                IList<Dto.DeleteAssetData> oIList1 = new List<Dto.DeleteAssetData>
                {
                    new Dto.DeleteAssetData()
                        {
                            I_INPUT = new Dto.DeleteAssetInput()
                                {
                                    COMPANYCODE = asset.Company.Code,
                                    ASSET = asset.InvNo,
                                    SUBNUMBER = asset.SubNo.ToString(),
                                    DELETE_IND = "X",
                                    OPTIMA_ASSET_NO = asset.InvNo,
                                    OPTIMA_ASSET_PARENT_NO = asset.InvNo
                                }
                         }
                };

                var postUser = new Dto.DeleteAsset
                {
                    Sap_function = "ZFIF_FIXED_ASSET_DELETE",
                    Options = new Dto.DeleteAssetDataOptions()
                    {
                        Api_call_timeout = 180
                    },
                    Remote_host_name = "test",
                    Data = oIList1
                };

                JsonContent contentJson = JsonContent.Create(postUser);
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                try
                {

                    using (var errorfile = System.IO.File.CreateText("contentJsonTransfer" + DateTime.Now.Ticks + ".txt"))
                    {
                        errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

                    };


                    var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

                    result = await httpResponse.Content.ReadAsStringAsync();

                    clientContract.Dispose();

                    //result = JsonConvert.DeserializeObject<CreateAssetResult>(content);

                    return result;
                }
                catch (Exception e)
                {
                    Console.Write("Error", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                    };

                    return result;
                }
            }

        }

        // [AllowAnonymous]
        [HttpPost]
        [Route("employeedetail")]
        public async Task<WFHResult> PostEmployeeDetail([FromBody] AssetEmployeeSave asset)
        {
            string employeeId = string.Empty;
            string userName = string.Empty;
            // string role = string.Empty;

            if (HttpContext.User.Identity.Name != null)
            {
				userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				_context.UserId = user.Id.ToString();
                employeeId = user.EmployeeId != null ? user.EmployeeId.ToString() : "";

				//userName = HttpContext.User.Identity.Name;
    //            // role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
    //            employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (employeeId == null || employeeId == "")
                {
					return new WFHResult { Success = false, Message = "Userul nu are asignat niciun utilizator!" };
				}
				else
				{
                    var result = await (_itemsRepository as IAssetsRepository).CreateOrUpdateAssetEmployee(asset, int.Parse(employeeId));

                    if (result.Success)
                    {
						await this._notifyService.NotifyDataWFHValidateAsync(result);
					}

					
					return new WFHResult { Success = result.Success, Message = result.Message };
				}
 
            }
            else
            {
                return new WFHResult { Success = false, Message = "Va rugam sa va autentificati!"};
            }
        }

   //     [AllowAnonymous]
   //     [HttpPost]
   //     [Route("employeelistdetail")]
   //     public async Task<IActionResult> PostEmployeeListDetail([FromBody] List<AssetEmployeeSave> assets)
   //     {
   //         int assetId = 0;
   //         string userName = string.Empty;

			//for (int i = 0; i < assets.Count; i++)
			//{
   //             assetId = (_itemsRepository as IAssetsRepository).CreateOrUpdateAssetEmployee(assets[i], assets[i].EmployeeId);
   //         }

            

   //         return Ok(assetId);


   //     }

        //[AllowAnonymous]
        //[HttpPut]
        //[Route("employeedetail/{guid}")]
        //public virtual IActionResult PutEmployeeDetail([FromBody] AssetEmployeeSave asset, Guid guid)
        //{
        //    int assetId = 0;

        //    if (guid == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }
        //    else
        //    {
        //        assetId = (_itemsRepository as IAssetsRepository).CreateOrUpdateAssetEmployee(asset, guid);

        //        return Ok(assetId);
        //    }
        //}

        [HttpPost("deleteValidationAsset/{assetId}")]
        public async Task<WFHResult> DeleteAssetValidation(int assetId)
        {
            string employeeId = string.Empty;
            string userName = string.Empty;
            // string role = string.Empty;

            if (HttpContext.User.Identity.Name != null)
            {
				userName = HttpContext.User.Identity.Name;
				var user = await userManager.FindByEmailAsync(userName);

				if (user == null)
				{
					user = await userManager.FindByNameAsync(userName);
				}

				_context.UserId = user.Id.ToString();
				employeeId = user.EmployeeId != null ? user.EmployeeId.ToString() : "";

				if (employeeId == null)
                {
                    return new WFHResult { Success = false, Message = "Userul nu a fost gasit!"};
                }
                else
                {
                    var result = await (_itemsRepository as IAssetsRepository).DeleteAssetValidation(assetId, int.Parse(employeeId));

                    return new WFHResult { Success = result.Success, Message = result.Message};
                }

            }
            else
            {
				return new WFHResult { Success = false, Message = "Va rugam sa va autentificati!" };
			}


           
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("employeevalidatenew")]
        public virtual IActionResult EmployeeValidateNew(int? page, int? pageSize, string sortColumn, string sortDirection, string includes)

        {
            int totalItems = 0;

            string employeeId = string.Empty;
            // string userName = string.Empty;
            string role = string.Empty;
            string userId = string.Empty;

            if (HttpContext.User.Identity.Name != null)
            {
                // userName = HttpContext.User.Identity.Name;
                role = HttpContext.User.Claims.First(c => c.Type.EndsWith("role")).Value;
                employeeId = HttpContext.User.Claims.First(c => c.Type.EndsWith("employeeId")).Value;

                if (employeeId == null)
                {
                    return BadRequest();
                }
                else
                {
                    List<Model.InventoryAsset> items = (_itemsRepository as IAssetsRepository).EmployeeValidateNew(includes, role, int.Parse(employeeId), sortColumn, sortDirection, page, pageSize, out totalItems).ToList();

                    var itemsResource = _mapper.Map<List<Model.InventoryAsset>, List<Dto.InventoryAssetResource>>(items);

                    var result = new PagedResult<Dto.InventoryAssetResource>(itemsResource, new PagingInfo()
                    {
                        TotalItems = totalItems,
                        CurrentPage = page.Value,
                        PageSize = pageSize.Value
                    });

                    return Ok(result);
                }

            }
            else
            {
                return BadRequest();
            }


           
        }

        [HttpPost]
        [Route("reco")]
        public virtual IActionResult SaveReco([FromBody] Dto.AssetRecoSave reco)
        {
            Model.Asset assetTemp = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.InventoryAsset inventoryAssetTemp = null;
            Model.Asset asset = null;
            Model.AssetOp assetOp = null;
            Model.AssetOp assetOpTemp = null;
            Model.Document document = null;

            string documentTypeCode = "RECO_PROPOSAL";


            Model.Inventory inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Id == reco.InventoryId).SingleOrDefault();


            assetTemp = _context.Set<Model.Asset>().SingleOrDefault(a => a.Id == reco.AssetTempId);
            inventoryAssetTemp = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == reco.AssetTempId && a.InventoryId == inventory.Id);
            inventoryAsset = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == reco.AssetId && a.InventoryId == inventory.Id);
            asset = _context.Set<Model.Asset>().Where(a => a.Id == reco.AssetId).SingleOrDefault();

            var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

            document = new Model.Document
            {
                Approved = true,
                DocumentType = documentType,
                DocNo1 = string.Empty,
                DocNo2 = string.Empty,
                DocumentDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Partner = null,
                ParentDocumentId = inventory.DocumentId
            };

            _context.Add(document);

            assetOp = new Model.AssetOp()
            {
                Asset = asset,
                DocumentId = document.Id,
                RoomIdInitial = inventoryAsset.RoomIdInitial,
                RoomIdFinal = inventoryAssetTemp.RoomIdFinal,
                EmployeeIdInitial = inventoryAsset.EmployeeIdInitial,
                EmployeeIdFinal = inventoryAssetTemp.EmployeeIdFinal,
                CostCenterIdInitial = inventoryAsset.CostCenterIdInitial,
                CostCenterIdFinal = inventoryAssetTemp.CostCenterIdFinal,
                AssetCategoryIdInitial = asset.AssetCategoryId,
                AssetCategoryIdFinal = asset.AssetCategoryId,
                AssetTypeIdInitial = asset.AssetTypeId,
                AssetTypeIdFinal = asset.AssetTypeId,
                InvStateIdInitial = inventoryAsset.StateIdInitial,
                InvStateIdFinal = inventoryAssetTemp.StateIdFinal,
                AdministrationIdInitial = asset.AdministrationId,
                AdministrationIdFinal = asset.AdministrationId,
                SerialNumber = inventoryAssetTemp.SerialNumber,
                Model = inventoryAssetTemp.Model,
                Producer = inventoryAssetTemp.Producer,
                Quantity = inventoryAssetTemp.QFinal,
                Info = inventoryAssetTemp.Info,
                Info2019 = inventoryAssetTemp.Info2019,
                ModifiedAt = inventoryAssetTemp.ModifiedAt,
                ModifiedBy = inventoryAssetTemp.ModifiedBy,
                AccSystemId = 3,
                AssetOpStateId = 79
            };

            _context.Add(assetOp);

            //assetOpTemp = new Model.AssetOp()
            //{
            //    Asset = assetTemp,
            //    DocumentId = document.Id,
            //    RoomIdInitial = inventoryAssetTemp.RoomIdInitial,
            //    RoomIdFinal = inventoryAssetTemp.RoomIdFinal,
            //    EmployeeIdInitial = inventoryAssetTemp.EmployeeIdInitial,
            //    EmployeeIdFinal = inventoryAssetTemp.EmployeeIdFinal,
            //    CostCenterIdInitial = inventoryAssetTemp.CostCenterIdInitial,
            //    CostCenterIdFinal = inventoryAssetTemp.CostCenterIdFinal,
            //    AssetCategoryIdInitial = assetTemp.AssetCategoryId,
            //    AssetCategoryIdFinal = assetTemp.AssetCategoryId,
            //    AssetTypeIdInitial = assetTemp.AssetTypeId,
            //    AssetTypeIdFinal = assetTemp.AssetTypeId,
            //    InvStateIdInitial = inventoryAssetTemp.StateIdInitial,
            //    InvStateIdFinal = inventoryAssetTemp.StateIdFinal,
            //    AdministrationIdInitial = assetTemp.AdministrationId,
            //    AdministrationIdFinal = assetTemp.AdministrationId,
            //    AccSystemId = 1,
            //    AssetOpStateId = 6
            //};

            //_context.Add(assetOpTemp);

            asset.TempReco = assetTemp.InvNo;
            asset.TempName = assetTemp.Name;
            asset.TempSerialNumber = assetTemp.SerialNumber;
            asset.AssetRecoStateId = 79;

            inventoryAsset.TempReco = assetTemp.InvNo;
            inventoryAsset.TempName = assetTemp.Name;
            inventoryAsset.TempRecoSerialNumber = assetTemp.SerialNumber;
            inventoryAsset.AssetRecoStateId = 79;

            assetTemp.TempReco = asset.InvNo;
            assetTemp.TempName = asset.Name;
            assetTemp.TempSerialNumber = asset.SerialNumber;
            assetTemp.AssetRecoStateId = 79;

            inventoryAssetTemp.TempReco = asset.InvNo;
            inventoryAssetTemp.TempName = asset.Name;
            inventoryAssetTemp.TempRecoSerialNumber = asset.SerialNumber;
            inventoryAssetTemp.AssetRecoStateId = 79;

            _context.SaveChanges();

            return Ok(StatusCode(200));
        }

        //[AllowAnonymous]
        [HttpPost("recolist")]
        public virtual IActionResult SaveRecoList([FromBody] List<Dto.AssetRecoSave> reco)
        {
            Model.Asset assetTemp = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.InventoryAsset inventoryAssetTemp = null;
            Model.Asset asset = null;
            Model.AssetOp assetOp = null;
            Model.AssetOp assetOpTemp = null;
            Model.Document document = null;

            string documentTypeCode = "RECO_PROPOSAL";


            Model.Inventory inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).SingleOrDefault();


            for (int i = 0; i < reco.Count; i++)
            {
                assetTemp = _context.Set<Model.Asset>().SingleOrDefault(a => a.Id == reco[i].AssetTempId);
                inventoryAssetTemp = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == reco[i].AssetTempId && a.InventoryId == inventory.Id);
                inventoryAsset = _context.Set<Model.InventoryAsset>().SingleOrDefault(a => a.AssetId == reco[i].AssetId && a.InventoryId == inventory.Id);
                asset = _context.Set<Model.Asset>().Where(a => a.Id == reco[i].AssetId).SingleOrDefault();

                var documentType = _context.Set<Model.DocumentType>().Where(d => d.Code == documentTypeCode).Single();

                document = new Model.Document
                {
                    Approved = true,
                    DocumentType = documentType,
                    DocNo1 = string.Empty,
                    DocNo2 = string.Empty,
                    DocumentDate = DateTime.Now,
                    RegisterDate = DateTime.Now,
                    Partner = null,
                    ParentDocumentId = inventory.DocumentId
                };

                _context.Add(document);

                assetOp = new Model.AssetOp()
                {
                    Asset = asset,
                    DocumentId = document.Id,
                    RoomIdInitial = inventoryAsset.RoomIdInitial,
                    RoomIdFinal = inventoryAssetTemp.RoomIdFinal,
                    EmployeeIdInitial = inventoryAsset.EmployeeIdInitial,
                    EmployeeIdFinal = inventoryAssetTemp.EmployeeIdFinal,
                    CostCenterIdInitial = inventoryAsset.CostCenterIdInitial,
                    CostCenterIdFinal = inventoryAssetTemp.CostCenterIdFinal,
                    AssetCategoryIdInitial = asset.AssetCategoryId,
                    AssetCategoryIdFinal = asset.AssetCategoryId,
                    AssetTypeIdInitial = asset.AssetTypeId,
                    AssetTypeIdFinal = asset.AssetTypeId,
                    InvStateIdInitial = inventoryAsset.StateIdInitial,
                    InvStateIdFinal = inventoryAssetTemp.StateIdFinal,
                    AdministrationIdInitial = asset.AdministrationId,
                    AdministrationIdFinal = asset.AdministrationId,
                    SerialNumber = inventoryAssetTemp.SerialNumber,
                    Model = inventoryAssetTemp.Model,
                    Producer = inventoryAssetTemp.Producer,
                    Quantity = inventoryAssetTemp.QFinal,
                    Info = inventoryAssetTemp.Info,
                    Info2019 = inventoryAssetTemp.Info2019,
                    ModifiedAt = inventoryAssetTemp.ModifiedAt,
                    ModifiedBy = inventoryAssetTemp.ModifiedBy,
                    AccSystemId = 3,
                    AssetOpStateId = 79
                };

                _context.Add(assetOp);

                //assetOpTemp = new Model.AssetOp()
                //{
                //    Asset = assetTemp,
                //    DocumentId = document.Id,
                //    RoomIdInitial = inventoryAssetTemp.RoomIdInitial,
                //    RoomIdFinal = inventoryAssetTemp.RoomIdFinal,
                //    EmployeeIdInitial = inventoryAssetTemp.EmployeeIdInitial,
                //    EmployeeIdFinal = inventoryAssetTemp.EmployeeIdFinal,
                //    CostCenterIdInitial = inventoryAssetTemp.CostCenterIdInitial,
                //    CostCenterIdFinal = inventoryAssetTemp.CostCenterIdFinal,
                //    AssetCategoryIdInitial = assetTemp.AssetCategoryId,
                //    AssetCategoryIdFinal = assetTemp.AssetCategoryId,
                //    AssetTypeIdInitial = assetTemp.AssetTypeId,
                //    AssetTypeIdFinal = assetTemp.AssetTypeId,
                //    InvStateIdInitial = inventoryAssetTemp.StateIdInitial,
                //    InvStateIdFinal = inventoryAssetTemp.StateIdFinal,
                //    AdministrationIdInitial = assetTemp.AdministrationId,
                //    AdministrationIdFinal = assetTemp.AdministrationId,
                //    AccSystemId = 1,
                //    AssetOpStateId = 6
                //};

                //_context.Add(assetOpTemp);

                asset.TempReco = assetTemp.InvNo;
                asset.TempName = assetTemp.Name;
                asset.TempSerialNumber = assetTemp.SerialNumber;
                asset.AssetRecoStateId = 79;

                inventoryAsset.TempReco = assetTemp.InvNo;
                inventoryAsset.TempName = assetTemp.Name;
                inventoryAsset.TempRecoSerialNumber = assetTemp.SerialNumber;
                inventoryAsset.AssetRecoStateId = 79;

                assetTemp.TempReco = asset.InvNo;
                assetTemp.TempName = asset.Name;
                assetTemp.TempSerialNumber = asset.SerialNumber;
                assetTemp.AssetRecoStateId = 79;

                inventoryAssetTemp.TempReco = asset.InvNo;
                inventoryAssetTemp.TempName = asset.Name;
                inventoryAssetTemp.TempRecoSerialNumber = asset.SerialNumber;
                inventoryAssetTemp.AssetRecoStateId = 79;

                _context.SaveChanges();
            }

            return Ok(StatusCode(200));
        }

		[HttpPost]
		[Route("mapassettemp")]
		public async Task<IActionResult> MapAssetTemp([FromBody] Dto.MapAssetTemp item)
		{
			Model.Asset asset = null;
			Model.InventoryAsset inventoryAsset = null;

			int inventoryId = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).SingleOrDefault().Id;

			var temps = item.assetTempIds.ToList();

			if (item != null && item.userId != null && item.userId != "")
			{
				var user = await userManager.FindByIdAsync(item.userId);

				for (int i = 0; i < temps.Count; i++)
				{
					asset = await _context.Set<Model.Asset>().Where(a => a.Id == temps[i]).SingleOrDefaultAsync();

					if (asset != null)
					{
						asset.TempUserId = item.userId;
                        asset.TempUserName = user.UserName;
                        asset.ModifiedAt = DateTime.Now;

						inventoryAsset = await _context.Set<Model.InventoryAsset>().Where(a => a.AssetId == temps[i] && a.InventoryId == inventoryId).SingleOrDefaultAsync();

						inventoryAsset.TempUserId = item.userId;

						_context.SaveChanges();
					}
				}
			}


			return Ok(StatusCode(200));
		}

		[HttpGet("generateMF/{item}")]
		public async Task<IActionResult> PreviewA(int item)
		{
			var ms = await this._assetService.PreviewAppendixAAsync(item);

			var fileType = "application/octet-stream";

			HttpContext.Response.ContentType = fileType;
			FileContentResult result = new FileContentResult(ms.ToArray(), fileType)
			{
				FileDownloadName = "Fisa mijloc fix.pdf"
			};

			return result;
		}

		[HttpPost("updatestorno")]
		public async virtual Task<RequestResult> StornoUpdate([FromBody] List<RequestBudgetForecastMaterialStorno> materialToUpdates)
		{
			Model.Asset asset = null;
            Model.AppState appState = null;
            Model.EntityType entityType = null;
            List<Model.EntityFile> entityFiles = null;

			appState = await _context.Set<Model.AppState>().Where(a => a.Code == "STORNO_ASSET" && a.IsDeleted == false).SingleOrDefaultAsync();
			entityType = await _context.Set<Model.EntityType>().Where(a => a.Code == "STORNO" && a.IsDeleted == false).SingleOrDefaultAsync();
			int assetStateId = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "TOVALIDATE").Select(a => a.Id).SingleOrDefault();

			for (int i = 0; i < materialToUpdates.Count; i++)
			{
				if (materialToUpdates[i].StornoValue > 0)
				{
					asset = await _context.Set<Model.Asset>().Where(a => a.Id == materialToUpdates[i].Id && a.IsDeleted == false).SingleOrDefaultAsync();

					if (asset != null)
					{
						if (asset.Storno)
						{
							return new Model.RequestResult { Success = false, Message = $"Numarul de inventar {asset.InvNo} a fost deja propus pentru stornare!", RequestId = 0 };
                        }
                        else
                        {
							entityFiles = await _context.Set<Model.EntityFile>().Where(a => a.EntityTypeId == entityType.Id && a.EntityId == asset.Id && a.IsDeleted == false).ToListAsync();

                            if(entityFiles.Count > 0)
                            {
								asset.Storno = materialToUpdates[i].Storno;
								asset.StornoValue = materialToUpdates[i].StornoValue;
								asset.StornoValueRon = materialToUpdates[i].StornoValue;
								asset.AssetStateId = assetStateId;
								asset.AppStateId = appState.Id;
								asset.ModifiedAt = DateTime.Now;
								_context.Update(asset);
								_context.SaveChanges();
							}

							
						}
					}
                    else
                    {
						return new Model.RequestResult { Success = false, Message = $"Numarul de inventar nu a fost gasit!", RequestId = 0 };
					}
				}

			}
			return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
		}

		[HttpPost("stornoMF")]
		public async virtual Task<RequestResult> StornoMF([FromBody] List<int> assetIds)
		{
			Model.Asset asset = null;
            Model.AppState appState = null;

			appState = await _context.Set<Model.AppState>().Where(a => a.Code == "STORNO_ASSET" && a.IsDeleted == false).SingleOrDefaultAsync();
			int assetStateId = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "RECEPTION").Select(a => a.Id).SingleOrDefault();

			for (int i = 0; i < assetIds.Count; i++)
			{
				asset = await _context.Set<Model.Asset>().Include(d => d.Document).Where(a => a.Id == assetIds[i] && a.IsDeleted == false).SingleOrDefaultAsync();

				if (asset != null)
				{
                    if(asset.Document.DocNo1 == null || asset.Document.DocNo1.Trim().Length < 3)
                    {
						return new Model.RequestResult { Success = false, Message = $"Va rugam completati o factura pentru numarul de inventar {asset.InvNo}!", RequestId = 0 };
					}
                    else
                    {
						asset.AssetStateId = assetStateId;
						asset.AppStateId = appState.Id;
						asset.Storno = true;
						asset.StornoValue = 0;
						asset.StornoValueRon = 0;

						_context.Update(asset);
						_context.SaveChanges();
					}
                    
				}
				else
				{
					return new Model.RequestResult { Success = false, Message = $"Numarul de inventar nu a fost gasit!", RequestId = 0 };
				}

			}
			return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
		}

		private static decimal NormalizeAndParse(string strDouble)
        {
            string strDoubleNormalized;

            if (strDouble.Contains(","))
            {
                var strReplaced = strDouble.Replace(",", ".");
                var decimalSeparatorPos = strReplaced.LastIndexOf('.');
                var strInteger = strReplaced.Substring(0, decimalSeparatorPos);
                var strFractional = strReplaced.Substring(decimalSeparatorPos);

                strInteger = strInteger.Replace(".", string.Empty);
                strDoubleNormalized = strInteger + strFractional;
            }
            else
            {
                strDoubleNormalized = strDouble;
            }

            return decimal.Parse(strDoubleNormalized, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        [HttpPost("updateAssetParent")]
        public async virtual Task<RequestResult> MaterialUpdateAssetParent([FromBody] List<RequestBudgetForecastMaterialUpdateAssetParent> materialToUpdates)
        {
            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
            Model.Asset asset = null;

            for (int i = 0; i < materialToUpdates.Count; i++)
            {
                requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                    .Include(o => o.OfferMaterial).ThenInclude(o => o.Offer).ThenInclude(o => o.OfferType)
                    .Where(a => a.Id == materialToUpdates[i].Id).SingleAsync();

                asset = await _context.Set<Model.Asset>()
                   .Where(a => a.Id == materialToUpdates[i].AssetId).SingleOrDefaultAsync();

                if (requestBudgetForecastMaterial != null)
                {
                    requestBudgetForecastMaterial.HasParentAsset = asset != null ? true : false;
                    requestBudgetForecastMaterial.ParentAsset = asset != null ? asset.InvNo : String.Empty;

                    _context.Update(requestBudgetForecastMaterial);
                    _context.SaveChanges();
                }
            }

            return new Model.RequestResult { Success = true, Message = "Datele au fost actualizate cu sucess!", RequestId = 0 };
        }
    }
}
