using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Optima.Fais.Api.Services
{
	public class CreateAssetSAPService : ICreateAssetSAPService
	{
        private const string BaseUrl = @"http://www.bnr.ro/nbrfxrates.xml";
		private readonly IConfiguration _configuration;

		private readonly string _BASEURL;
		private readonly string _TOKEN;

		public IServiceProvider Services { get; }
		public INotifyService _notifyService { get; }

		public CreateAssetSAPService(IServiceProvider services, INotifyService notifyService, IConfiguration configuration)
        {
            Services = services;
			_notifyService = notifyService;
			_configuration = configuration;

			this._BASEURL = configuration.GetSection("SAP").GetValue<string>("URL");
			this._TOKEN = configuration.GetSection("SAP").GetValue<string>("SAP-PROXY-AUTH-TOKEN");
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewAssetSAPAsync()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext =
                   scope.ServiceProvider
                       .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "CREATEASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "CREATEASSET",
						Name = "Sincronizare aseturi noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
                        /*List<Dto.CreateAssetSAP> assets = dbContext.Set<Model.CreateAssetSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.SyncErrorCount < 3 && com.IsTesting == false).Select(a => new Dto.CreateAssetSAP() 
						{
							XSUBNO = a.XSUBNO,
							COMPANYCODE = a.COMPANYCODE,
							ASSET = a.ASSET,
							SUBNUMBER = a.SUBNUMBER,
							ASSETCLASS = a.ASSETCLASS,
							POSTCAP = a.POSTCAP,
							DESCRIPT = a.DESCRIPT,
							DESCRIPT2 = a.DESCRIPT2,
							INVENT_NO = a.INVENT_NO,
							SERIAL_NO = a.SERIAL_NO,
							QUANTITY = a.QUANTITY,
							BASE_UOM = a.BASE_UOM,
							LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
							LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
							CAP_DATE = a.CAP_DATE,
							COSTCENTER = a.COSTCENTER,
							RESP_CCTR = a.RESP_CCTR,
							INTERN_ORD = a.INTERN_ORD,
							PLANT = a.PLANT,
							LOCATION = a.LOCATION,
							ROOM = a.ROOM,
							PERSON_NO = a.PERSON_NO,
							PLATE_NO = a.PLATE_NO,
							ZZCLAS = a.ZZCLAS,
							IN_CONSERVATION = a.IN_CONSERVATION,
							PROP_IND = a.PROP_IND,
							OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
							OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
							TESTRUN = a.TESTRUN,
							VENDOR_NO = a.VENDOR_NO,
							INVOICE = a.INVOICE
							
							}).ToList();*/

                        List<Dto.CreateAssetSAP> assets = dbContext.Set<Model.CreateAssetSAP>().FromSql("CreateAssetSAPService_TakeCreateAsset")
                            .Select(a => new Dto.CreateAssetSAP()
                            {
                                XSUBNO = a.XSUBNO,
                                COMPANYCODE = a.COMPANYCODE,
                                ASSET = a.ASSET,
                                SUBNUMBER = a.SUBNUMBER,
                                ASSETCLASS = a.ASSETCLASS,
                                POSTCAP = a.POSTCAP,
                                DESCRIPT = a.DESCRIPT,
                                DESCRIPT2 = a.DESCRIPT2,
                                INVENT_NO = a.INVENT_NO,
                                SERIAL_NO = a.SERIAL_NO,
                                QUANTITY = a.QUANTITY,
                                BASE_UOM = a.BASE_UOM,
                                LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
                                LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
                                CAP_DATE = a.CAP_DATE,
                                COSTCENTER = a.COSTCENTER,
                                RESP_CCTR = a.RESP_CCTR,
                                INTERN_ORD = a.INTERN_ORD,
                                PLANT = a.PLANT,
                                LOCATION = a.LOCATION,
                                ROOM = a.ROOM,
                                PERSON_NO = a.PERSON_NO,
                                PLATE_NO = a.PLATE_NO,
                                ZZCLAS = a.ZZCLAS,
                                IN_CONSERVATION = a.IN_CONSERVATION,
                                PROP_IND = a.PROP_IND,
                                OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
                                OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
                                TESTRUN = a.TESTRUN,
                                VENDOR_NO = a.VENDOR_NO,
                                INVOICE = a.INVOICE,
								AssetId = a.AssetId
                            }).ToList();

                        List<int> assetSyncErrorIds = new List<int>();
                        for (int i = 0; i < assets.Count; i++)
						{
							Model.AssetSyncError assetSyncError = new();
							assetSyncError.AssetId = assets[i].AssetId;
							assetSyncError.SyncCode = "CREATEASSET";
							assetSyncError.SyncStatus = "New Asset synchronization start";
							assetSyncError.SyncDate = DateTime.Now.ToString();
							dbContext.Add(assetSyncError);
							dbContext.SaveChanges();
                            assetSyncErrorIds.Add(assetSyncError.Id);
                        }

                        syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							result = await CreateAssetSAPAsync(assets[i], assetSyncErrorIds[i]);

							await this._notifyService.NotifyDataCreateAssetSAPAsync(result);
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();
					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewTransferAssetSAPAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "TRANSFERASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "TRANSFERASSET",
						Name = "Sincronizare transferuri noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.TransferAssetSAP> assets = dbContext.Set<Model.TransferAssetSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.SyncErrorCount < 3 && com.IsTesting == false).Select(a => new Dto.TransferAssetSAP()
							{
								FROM_ASSET = a.FROM_ASSET,
								FROM_SUBNUMBER = a.FROM_SUBNUMBER,
								COMPANYCODE = a.COMPANYCODE,
								DOC_DATE = a.DOC_DATE,
								PSTNG_DATE = a.PSTNG_DATE,
								ASVAL_DATE = a.ASVAL_DATE,
								ITEM_TEXT = a.ITEM_TEXT,
								TO_ASSET = a.TO_ASSET,
								TO_SUBNUMBER = a.TO_SUBNUMBER,
								FIS_PERIOD = a.FIS_PERIOD,
								DOC_TYPE = a.DOC_TYPE,
								REF_DOC_NO = a.REF_DOC_NO,
								COMPL_TRANSFER = a.COMPL_TRANSFER,
								AMOUNT = a.AMOUNT,
								CURRENCY = a.CURRENCY,
								PERCENT = a.PERCENT,
								QUANTITY = a.QUANTITY,
								BASE_UOM = a.BASE_UOM,
								PRIOR_YEAR_ACQUISITIONS = a.PRIOR_YEAR_ACQUISITIONS,
								CURRENT_YEAR_ACQUISITIONS = a.CURRENT_YEAR_ACQUISITIONS
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							result = await CreateTransferAssetAsync(assets[i]);

							await this._notifyService.NotifyDataCreateAssetAsync(result);

							//return result;
						}

						//var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewRetireAssetSAPAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "RETIREASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "RETIREASSET",
						Name = "Sincronizare casari noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.RetireAssetSAP> assets = dbContext.Set<Model.RetireAssetSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.SyncErrorCount < 5 && com.IsTesting == false).Select(a => new Dto.RetireAssetSAP()
							{
								COMPANYCODE = a.COMPANYCODE,
								ASSET = a.ASSET,
								SUBNUMBER = a.SUBNUMBER,
								DOC_DATE = a.DOC_DATE,
								PSTNG_DATE = a.PSTNG_DATE,
								VALUEDATE = a.VALUEDATE,
								ITEM_TEXT = a.ITEM_TEXT,
								FIS_PERIOD = a.FIS_PERIOD,
								DOC_TYPE = a.DOC_TYPE,
								REF_DOC_NO = a.REF_DOC_NO,
								COMPL_RET = a.COMPL_RET,
								AMOUNT = a.AMOUNT,
								CURRENCY = a.CURRENCY,
								PERCENT = a.PERCENT,
								QUANTITY = a.QUANTITY,
								BASE_UOM = a.BASE_UOM,
								PRIOR_YEAR_ACQUISITIONS = a.PRIOR_YEAR_ACQUISITIONS,
								CURRENT_YEAR_ACQUISITIONS = a.CURRENT_YEAR_ACQUISITIONS,
								STORNO = a.STORNO,
								STORNO_DOC = a.STORNO_DOC,
								STORNO_DATE = a.STORNO_DATE,
								STORNO_REASON = a.STORNO_REASON,
								AssetId = a.AssetId
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							result = await CreateRetireAssetAsync(assets[i]);

							await this._notifyService.NotifyDataCreateAssetAsync(result);

							return result;
						}

						// var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewAcquisitionAssetSAPAsync()
		{

			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "ACQUISITIONASSET").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "ACQUISITIONASSET",
						Name = "Sincronizare achizitii noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.AcquisitionAssets> acquisitionAssets = new List<Dto.AcquisitionAssets>();


						//List<Dto.AcquisitionAssetSAP> assets = dbContext.Set<Model.AcquisitionAssetSAP>()
						//		 .Where(com => com.IsDeleted == false && com.NotSync == true && com.IsTesting == false && com.SyncErrorCount < 3).AsEnumerable()
						//	.GroupBy(item => item.Guid,
						//		(key, group) => new Dto.AcquisitionAssetSAP()
						//		{
						//			STORNO = group.FirstOrDefault().STORNO,
						//			COMPANYCODE = group.FirstOrDefault().COMPANYCODE,
						//			DOC_DATE = group.FirstOrDefault().DOC_DATE,
						//			PSTNG_DATE = group.FirstOrDefault().PSTNG_DATE,
						//			REF_DOC_NO = group.FirstOrDefault().REF_DOC_NO,
						//			HEADER_TXT = group.FirstOrDefault().HEADER_TXT,
						//			VENDOR_NO = group.FirstOrDefault().VENDOR_NO,
						//			CURRENCY = group.FirstOrDefault().CURRENCY,
						//			EXCH_RATE = group.FirstOrDefault().EXCH_RATE,
						//			TOTAL_AMOUNT = group.FirstOrDefault().TOTAL_AMOUNT,
						//			AssetId = group.FirstOrDefault().AssetId,
						//			ASSETS = group.ToList().Select(a => new Dto.AcquisitionAssets
						//			{
						//				ASSET = a.ASSET,
						//				SUBNUMBER = a.SUBNUMBER,
						//				ITEM_TEXT = a.ITEM_TEXT,
						//				TAX_CODE = a.TAX_CODE,
						//				NET_AMOUNT = a.NET_AMOUNT,
						//				TAX_AMOUNT = a.TAX_AMOUNT,
						//				GL_ACCOUNT = a.GL_ACCOUNT,
						//				ASVAL_DATE = a.ASVAL_DATE,
						//				WBS_ELEMENT = a.WBS_ELEMENT
						//			}).ToList()
						//		})
						//	.ToList();
						List<Dto.AcquisitionAssetSAP> assets = dbContext.Set<Model.AcquisitionAssetSAP>().FromSql("CreateAssetSAPService_TakeAcquisitionAsset")
					    .AsEnumerable()
						.GroupBy(item => item.Guid,
							(key, group) => new Dto.AcquisitionAssetSAP()
							{
								STORNO = group.FirstOrDefault().STORNO,
								COMPANYCODE = group.FirstOrDefault().COMPANYCODE,
								DOC_DATE = group.FirstOrDefault().DOC_DATE,
								PSTNG_DATE = group.FirstOrDefault().PSTNG_DATE,
								REF_DOC_NO = group.FirstOrDefault().REF_DOC_NO,
								HEADER_TXT = group.FirstOrDefault().HEADER_TXT,
								VENDOR_NO = group.FirstOrDefault().VENDOR_NO,
								CURRENCY = group.FirstOrDefault().CURRENCY,
								EXCH_RATE = group.FirstOrDefault().EXCH_RATE,
								TOTAL_AMOUNT = group.FirstOrDefault().TOTAL_AMOUNT,
								AssetId = group.FirstOrDefault().AssetId,
								ASSETS = group.ToList().Select(a => new Dto.AcquisitionAssets
								{
									ASSET = a.ASSET,
									SUBNUMBER = a.SUBNUMBER,
									ITEM_TEXT = a.ITEM_TEXT,
									TAX_CODE = a.TAX_CODE,
									NET_AMOUNT = a.NET_AMOUNT,
									TAX_AMOUNT = a.TAX_AMOUNT,
									GL_ACCOUNT = a.GL_ACCOUNT,
									ASVAL_DATE = a.ASVAL_DATE,
									WBS_ELEMENT = a.WBS_ELEMENT
								}).ToList()
							})
						.ToList();

						List<int> assetSyncErrorIds = new List<int>();
                        for (int i = 0; i < assets.Count; i++)
                        {
                            Model.AssetSyncError assetSyncError = new();
                            assetSyncError.AssetId = assets[i].AssetId;
                            assetSyncError.SyncCode = "ACQUISITIONASSET";
                            assetSyncError.SyncStatus = "Acquisition Asset synchronization start";
                            assetSyncError.SyncDate = DateTime.Now.ToString();
                            dbContext.Add(assetSyncError);
                            dbContext.SaveChanges();
                            assetSyncErrorIds.Add(assetSyncError.Id);
                        }
                        syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
						     result = await CreateAcquisitionAssetAsync(assets[i], assetSyncErrorIds[i]);

							await this._notifyService.NotifyDataAcquisitionAssetSAPAsync(result);

							//return result;
						}

						// var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewAssetInvPlusSAPAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "ASSETINVPLUS").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "ASSETINVPLUS",
						Name = "Sincronizare plusuri noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.AssetInvPlusSAP> assets = dbContext.Set<Model.AssetInvPlusSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.SyncErrorCount < 5 && com.IsTesting == false).Select(a => new Dto.AssetInvPlusSAP()
							{
								ASSET = a.ASSET,
								SUBNUMBER = a.SUBNUMBER,
								COMPANYCODE = a.COMPANYCODE,
								DOC_DATE = a.DOC_DATE,
								PSTNG_DATE = a.PSTNG_DATE,
								ASVAL_DATE = a.ASVAL_DATE,
								ITEM_TEXT = a.ITEM_TEXT,
								DOC_TYPE = a.DOC_TYPE,
								REF_DOC_NO = a.REF_DOC_NO,
								AMOUNT = a.AMOUNT,
								TRANSTYPE = a.TRANSTYPE
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							result = await CreateAssetInvPlusAsync(assets[i]);
							await this._notifyService.NotifyDataCreateAssetAsync(result);

							return result;
						}

						var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewAssetInvMinusSAPAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "ASSETINVMINUS").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "ASSETINVMINUS",
						Name = "Sincronizare minusuri noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.AssetInvMinusSAP> assets = dbContext.Set<Model.AssetInvMinusSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.SyncErrorCount < 5 && com.IsTesting == false).Select(a => new Dto.AssetInvMinusSAP()
							{
								ASSET = a.ASSET,
								SUBNUMBER = a.SUBNUMBER,
								COMPANYCODE = a.COMPANYCODE,
								DOC_DATE = a.DOC_DATE,
								PSTNG_DATE = a.PSTNG_DATE,
								ASVAL_DATE = a.ASVAL_DATE,
								ITEM_TEXT = a.ITEM_TEXT,
								DOC_TYPE = a.DOC_TYPE,
								REF_DOC_NO = a.REF_DOC_NO,
								TRANSTYPE = a.TRANSTYPE,
								INVENTORY_DIFF = a.INVENTORY_DIFF,
								//AMOUNT = a.AMOUNT,
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							result = await CreateAssetInvMinusAsync(assets[i]);
							await this._notifyService.NotifyDataCreateAssetAsync(result);

							return result;
						}

						//var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewTransferInStockSAPAsync()
		{
			Model.Asset asset = null;
			Model.TransferInStockSAP transferInStockSAP = null;
			Dto.CreateAssetSAP createAssetSAP = null;
			GetStockResult checkStockResult = null;
			Model.AssetState assetState = null;

			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "TRANSFERINSTOCK").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"" };
				assetState = dbContext.Set<Model.AssetState>().Where(a => a.Code == "STOCK_IT").Single();
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "TRANSFERINSTOCK",
						Name = "Sincronizare transferuri din stoc noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.TransferInStockInput> assets = dbContext.Set<Model.TransferInStockSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.Storno != "X" && com.SyncErrorCount < 3 && com.IsTesting == false).Select(a => new Dto.TransferInStockInput()
							{
								Doc_Date = a.Doc_Date,
								Pstng_Date = a.Pstng_Date,
								Material = a.Material,
								Plant = a.Plant,
								Storage_Location = a.Storage_Location,
								Quantity = a.Quantity,
								Uom = a.Uom,
								Batch = a.Batch,
								Gl_Account = a.Gl_Account,
								Item_Text = a.Item_Text,
								Asset = a.Asset,
								SubNumber = a.SubNumber,
								Ref_Doc_No = a.Ref_Doc_No,
								Header_Txt = a.Header_Txt,
								Storno = a.Storno,
								Storno_Doc = a.Storno_Doc,
								Storno_Date = a.Storno_Date,
								Storno_Year = a.Storno_Year,
								Storno_User = ""
								//Id = a.Id
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;
						var res = string.Empty;
						//for (int i = 0; i < assets.Count; i++)
						//{
						//	asset = dbContext.Set<Model.Asset>()
						//		.Include(s => s.Stock).ThenInclude(s => s.Material)
						//		.Include(s => s.Stock).ThenInclude(s => s.Category)
						//		.AsNoTracking()
						//		.Where(a => a.InvNo == assets[i].Asset && a.SubNo == assets[i].SubNumber && a.AssetStateId == assetState.Id)
						//		.SingleOrDefault();

						//	if(asset != null)
						//	{
						//		res = await CheckAssetStock(asset.Stock.Category.Code, assets[i].Material, asset.Stock.Code);

						//		if (res != "")
						//		{
						//			try
						//			{
						//				checkStockResult = JsonConvert.DeserializeObject<GetStockResult>(res);

						//				if (checkStockResult.Data != null && checkStockResult.Data.Return_Code == "1")
						//				{
						//					transferInStockSAP = dbContext.Set<Model.TransferInStockSAP>().Include(a => a.CreateAssetSAP).Where(a => a.Asset == assets[i].Asset).SingleOrDefault();

						//					createAssetSAP = new Dto.CreateAssetSAP()
						//					{
						//						XSUBNO = transferInStockSAP.CreateAssetSAP.XSUBNO,
						//						COMPANYCODE = transferInStockSAP.CreateAssetSAP.COMPANYCODE,
						//						ASSET = transferInStockSAP.CreateAssetSAP.ASSET,
						//						SUBNUMBER = transferInStockSAP.CreateAssetSAP.SUBNUMBER,
						//						ASSETCLASS = transferInStockSAP.CreateAssetSAP.ASSETCLASS,
						//						POSTCAP = transferInStockSAP.CreateAssetSAP.POSTCAP,
						//						DESCRIPT = transferInStockSAP.CreateAssetSAP.DESCRIPT,
						//						DESCRIPT2 = transferInStockSAP.CreateAssetSAP.DESCRIPT2,
						//						INVENT_NO = transferInStockSAP.CreateAssetSAP.INVENT_NO,
						//						SERIAL_NO = transferInStockSAP.CreateAssetSAP.SERIAL_NO,
						//						QUANTITY = transferInStockSAP.CreateAssetSAP.QUANTITY,
						//						BASE_UOM = transferInStockSAP.CreateAssetSAP.BASE_UOM,
						//						LAST_INVENTORY_DATE = transferInStockSAP.CreateAssetSAP.LAST_INVENTORY_DATE,
						//						LAST_INVENTORY_DOCNO = transferInStockSAP.CreateAssetSAP.LAST_INVENTORY_DOCNO,
						//						CAP_DATE = transferInStockSAP.CreateAssetSAP.CAP_DATE,
						//						COSTCENTER = transferInStockSAP.CreateAssetSAP.COSTCENTER,
						//						RESP_CCTR = transferInStockSAP.CreateAssetSAP.RESP_CCTR,
						//						INTERN_ORD = transferInStockSAP.CreateAssetSAP.INTERN_ORD,
						//						PLANT = transferInStockSAP.CreateAssetSAP.PLANT,
						//						LOCATION = transferInStockSAP.CreateAssetSAP.LOCATION,
						//						ROOM = transferInStockSAP.CreateAssetSAP.ROOM,
						//						PERSON_NO = transferInStockSAP.CreateAssetSAP.PERSON_NO,
						//						PLATE_NO = transferInStockSAP.CreateAssetSAP.PLATE_NO,
						//						ZZCLAS = transferInStockSAP.CreateAssetSAP.ZZCLAS,
						//						IN_CONSERVATION = transferInStockSAP.CreateAssetSAP.IN_CONSERVATION,
						//						PROP_IND = transferInStockSAP.CreateAssetSAP.PROP_IND,
						//						OPTIMA_ASSET_NO = transferInStockSAP.CreateAssetSAP.OPTIMA_ASSET_NO,
						//						OPTIMA_ASSET_PARENT_NO = transferInStockSAP.CreateAssetSAP.OPTIMA_ASSET_PARENT_NO,
						//						TESTRUN = transferInStockSAP.CreateAssetSAP.TESTRUN,
						//						VENDOR_NO = transferInStockSAP.CreateAssetSAP.VENDOR_NO,
						//						INVOICE = transferInStockSAP.CreateAssetSAP.INVOICE,
						//					};

						//					if (asset.NotSync)
						//					{
						//						result = await CreateAssetSAPAsync(createAssetSAP);
						//					}

						//					if (result.Success)
						//					{
						//						var rst = await TransferInStockAsync(assets[i]);
						//						await this._notifyService.NotifyDataCreateAssetAsync(result);
						//						//return result;
						//					}
						//				}
						//			}
						//			catch (Exception ex)
						//			{
						//			}
						//		}
								
						//	}
							
						//}

						// var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewStornoTransferInStockSAPAsync()
		{
			Model.Asset asset = null;
			Model.TransferInStockSAP transferInStockSAP = null;

			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "STORNOINSTOCK").SingleOrDefault();

				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "STORNOINSTOCK",
						Name = "Sincronizare transferuri din stoc noi - storno",
						SyncEnabled = true,
						SyncInterval = 10
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Dto.TransferInStockInput> assets = dbContext.Set<Model.TransferInStockSAP>()
							.Where(com => com.IsDeleted == false && com.NotSync == true && com.Storno == "X" && com.SyncErrorCount < 3 && com.IsTesting == false).Select(a => new Dto.TransferInStockInput()
							{
								Doc_Date = a.Doc_Date,
								Pstng_Date = a.Pstng_Date,
								Material = a.Material,
								Plant = a.Plant,
								Storage_Location = a.Storage_Location,
								Quantity = a.Quantity,
								Uom = a.Uom,
								Batch = a.Batch,
								Gl_Account = a.Gl_Account,
								Item_Text = a.Item_Text,
								Asset = a.Asset,
								SubNumber = a.SubNumber,
								Ref_Doc_No = a.Ref_Doc_No,
								Header_Txt = a.Header_Txt,
								Storno = a.Storno,
								Storno_Doc = a.Storno_Doc,
								Storno_Date = a.Storno_Date,
								Storno_Year = a.Storno_Year,
								Storno_User = ""
								//Id = a.Id
							}).ToList();

						syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							var rst = await TransferStornoInStockAsync(assets[i]);
							await this._notifyService.NotifyDataCreateAssetAsync(rst);

							return rst;

						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> SearchNewAssetChangeSAPAsync()
		{
			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "ASSETCHANGE").SingleOrDefault();
				Model.CreateAssetSAPResult result = new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"" };
				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "ASSETCHANGE",
						Name = "Sincronizare modificari noi",
						SyncEnabled = true,
						SyncInterval = 1
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{

                        List<Dto.AssetChangeSAP> assets = dbContext.Set<Model.AssetChangeSAP>().FromSql("CreateAssetSAPService_TakeChangeAsset")
                        .Select(a => new Dto.AssetChangeSAP()
						{
							COMPANYCODE = a.COMPANYCODE,
							ASSET = a.ASSET,
							SUBNUMBER = a.SUBNUMBER,
							ASSETCLASS = a.ASSETCLASS,
							POSTCAP = a.POSTCAP,
							DESCRIPT = a.DESCRIPT,
							DESCRIPT2 = a.DESCRIPT2,
							INVENT_NO = a.INVENT_NO,
							SERIAL_NO = a.SERIAL_NO,
							QUANTITY = a.QUANTITY,
							BASE_UOM = a.BASE_UOM,
							LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
							LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
							CAP_DATE = a.CAP_DATE,
							COSTCENTER = a.COSTCENTER,
							INTERN_ORD = a.INTERN_ORD,
							PLANT = a.PLANT,
							LOCATION = a.LOCATION,
							ROOM = a.ROOM,
							PERSON_NO = a.PERSON_NO,
							PLATE_NO = a.PLATE_NO,
							ZZCLAS = a.ZZCLAS,
							IN_CONSERVATION = a.IN_CONSERVATION,
							PROP_IND = a.PROP_IND,
							OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
							OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
							VENDOR_NO = a.VENDOR_NO,
							RESP_CCTR = a.RESP_CCTR,
							FROM_DATE = a.FROM_DATE,
							INVOICE = a.INVOICE,
							AssetId = a.AssetId

						}).ToList();
						//List<Dto.AssetChangeSAP> assets = dbContext.Set<Model.AssetChangeSAP>()
						//	.Where(com => com.IsDeleted == false && com.NotSync == true && com.SyncErrorCount < 3 && com.IsTesting == false).Select(a => new Dto.AssetChangeSAP()
						//	{
						//		COMPANYCODE = a.COMPANYCODE,
						//		ASSET = a.ASSET,
						//		SUBNUMBER = a.SUBNUMBER,
						//		ASSETCLASS = a.ASSETCLASS,
						//		POSTCAP = a.POSTCAP,
						//		DESCRIPT = a.DESCRIPT,
						//		DESCRIPT2 = a.DESCRIPT2,
						//		INVENT_NO = a.INVENT_NO,
						//		SERIAL_NO = a.SERIAL_NO,
						//		QUANTITY = a.QUANTITY,
						//		BASE_UOM = a.BASE_UOM,
						//		LAST_INVENTORY_DATE = a.LAST_INVENTORY_DATE,
						//		LAST_INVENTORY_DOCNO = a.LAST_INVENTORY_DOCNO,
						//		CAP_DATE = a.CAP_DATE,
						//		COSTCENTER = a.COSTCENTER,
						//		INTERN_ORD = a.INTERN_ORD,
						//		PLANT = a.PLANT,
						//		LOCATION = a.LOCATION,
						//		ROOM = a.ROOM,
						//		PERSON_NO = a.PERSON_NO,
						//		PLATE_NO = a.PLATE_NO,
						//		ZZCLAS = a.ZZCLAS,
						//		IN_CONSERVATION = a.IN_CONSERVATION,
						//		PROP_IND = a.PROP_IND,
						//		OPTIMA_ASSET_NO = a.OPTIMA_ASSET_NO,
						//		OPTIMA_ASSET_PARENT_NO = a.OPTIMA_ASSET_PARENT_NO,
						//		VENDOR_NO = a.VENDOR_NO,
						//		RESP_CCTR = a.RESP_CCTR,
						//		FROM_DATE = a.FROM_DATE,
						//		INVOICE = a.INVOICE,
						//		AssetId = a.AssetId
						//		//DOC_YEAR = a.DOC_YEAR,
						//		//MAT_DOC = a.MAT_DOC,
						//		//WBS_ELEMENT = a.WBS_ELEMENT

						//	}).ToList();


						List<int> assetSyncErrorIds = new List<int>();
                        for (int i = 0; i < assets.Count; i++)
                        {
                            Model.AssetSyncError assetSyncError = new();
                            assetSyncError.AssetId = assets[i].AssetId;
                            assetSyncError.SyncCode = "ASSETCHANGE";
                            assetSyncError.SyncStatus = "Change Asset synchronization start";
                            assetSyncError.SyncDate = DateTime.Now.ToString();
                            dbContext.Add(assetSyncError);
                            dbContext.SaveChanges();
                            assetSyncErrorIds.Add(assetSyncError.Id);
                        }
                        syncStatus.SyncStart = DateTime.Now;

						for (int i = 0; i < assets.Count; i++)
						{
							result = await AssetChangeAsync(assets[i], assetSyncErrorIds[i]);

							await this._notifyService.NotifyDataChangeAssetAsync(result);

							//using (var errorfile = System.IO.File.CreateText(assets[i].OPTIMA_ASSET_NO + "___" + DateTime.Now.Ticks + ".txt"))
							//{
							//	errorfile.WriteLine();

							//};

							//return result;
						}

						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}


			return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = "" };
		}

		public async Task<Model.CreateAssetSAPResult> CreateAssetSAPAsync(CreateAssetSAP asset, int? assetSyncErrorId = null)
		{
			Model.Asset assetToUpdate = null;
			Model.Error error = null;
			Model.ErrorType errorType = null;
			Model.AssetState assetState = null;
			Model.AssetState assetStateStockIT = null;
			HttpClient clientContract = null;

			var baseUrl = _BASEURL;
			CreateAssetResult createAssetResult = null;
			CreateAssetResult createAssetResultNew = null;
			string result = "";
			using (clientContract = new HttpClient())
			{
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				//Model.Asset assetToUpdate = _context.Set<Model.Asset>().Include(c => c.CreateAssetSAP).Where(a => a.InvNo == asset.OPTIMA_ASSET_NO && a.SubNo == asset.SUBNUMBER).Single();
				assetToUpdate = await _context.Set<Model.Asset>().Include(c => c.CreateAssetSAP).Where(a => a.InvNo == asset.OPTIMA_ASSET_NO).LastOrDefaultAsync();
				error = new();
				errorType = await _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "CREATEASSET").SingleAsync();
				assetState = await _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "TOVALIDATE").SingleAsync();
				assetStateStockIT = await _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "STOCK_IT").SingleAsync();

				IList<Dto.CreateAssetData> oIList1 = new List<Dto.CreateAssetData>
				{
					new Dto.CreateAssetData()
						{
							I_INPUT = new Dto.CreateAssetInput()
								{
									XSUBNO = asset.XSUBNO,
									COMPANYCODE = asset.COMPANYCODE,
									ASSET = asset.ASSET,
									SUBNUMBER = asset.SUBNUMBER,
									ASSETCLASS = asset.ASSETCLASS,
									POSTCAP = asset.POSTCAP,
									DESCRIPT = asset.DESCRIPT,
									DESCRIPT2 = asset.DESCRIPT2,
									INVENT_NO = asset.INVENT_NO,
									SERIAL_NO = asset.SERIAL_NO,
									QUANTITY = asset.QUANTITY,
									BASE_UOM = asset.BASE_UOM,
									LAST_INVENTORY_DATE = asset.LAST_INVENTORY_DATE,
									LAST_INVENTORY_DOCNO = asset.LAST_INVENTORY_DOCNO,
									CAP_DATE = asset.CAP_DATE,
									COSTCENTER = asset.COSTCENTER,
									RESP_CCTR = asset.RESP_CCTR,
									INTERN_ORD = asset.INTERN_ORD,
									PLANT = asset.PLANT,
									LOCATION = asset.LOCATION,
									ROOM = asset.ROOM,
									PERSON_NO = asset.PERSON_NO,
									PLATE_NO = asset.PLATE_NO,
									ZZCLAS = asset.ZZCLAS != "-" ? asset.ZZCLAS : "",
									IN_CONSERVATION = asset.IN_CONSERVATION,
									PROP_IND = asset.PROP_IND,
									OPTIMA_ASSET_NO = asset.OPTIMA_ASSET_NO,
									OPTIMA_ASSET_PARENT_NO = asset.OPTIMA_ASSET_PARENT_NO,
									TESTRUN = asset.TESTRUN,
									VENDOR_NO = asset.VENDOR_NO,
									INVOICE = asset.INVOICE
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

#if DEBUG
                string jsonString = await contentJson.ReadAsStringAsync();
                
                string resultFilePath = System.IO.Path.Combine("SAP-RESULTS", "CREATEASSETSAP_" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(resultFilePath))
                {
                    errorfile.WriteLine(jsonString);

                };
#endif
                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);
				try
				{
					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					clientContract.Dispose();

					if (result != "")
					{
						Model.Error errorResponse = new();

						errorResponse.AssetId = assetToUpdate.Id;
						errorResponse.ErrorTypeId = errorType.Id;
						errorResponse.CreatedAt = DateTime.Now;
						errorResponse.CreatedBy = _context.UserId;
						errorResponse.ModifiedAt = DateTime.Now;
						errorResponse.ModifiedBy = _context.UserId;
						errorResponse.Code = "RESULT-ASSET-CREATE";
						errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
						errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse.UserId = _context.UserId;
						errorResponse.IsDeleted = false;

						_context.Add(errorResponse);
						_context.SaveChanges();
                        createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);

                        if (assetSyncErrorId != null) {
							var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == assetSyncErrorId);

							if (assetSyncErrorToUpdate != null)
							{
								if (createAssetResultNew.Meta.Code == 200)
								{
									assetSyncErrorToUpdate.SyncStatus = "Synchronization Success";
									assetSyncErrorToUpdate.Error = errorResponse.Id;

									_context.SaveChanges();
								}
								if (createAssetResultNew.Meta.Code == 400)
								{
									assetSyncErrorToUpdate.SyncStatus = "Synchronization Fail";
									assetSyncErrorToUpdate.Error = errorResponse.Id;

									_context.SaveChanges();
								}
								else
								{ 
								
								}
                            }
						}

						try
						{
							createAssetResult = JsonConvert.DeserializeObject<CreateAssetResult>(result);

							if (createAssetResult.Data != null && createAssetResult.Data.Return_Code == "1")
							{
								Model.CreateAssetSAP createAssetSAP = _context.Set<Model.CreateAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 3 && a.IsDeleted == false && a.IsTesting == false).SingleOrDefault();

								if(createAssetSAP != null)
								{
									createAssetSAP.NotSync = false;
									createAssetSAP.SyncErrorCount = 0;
									createAssetSAP.ASSET = createAssetResult.Data.Asset;
									createAssetSAP.SUBNUMBER = createAssetResult.Data.SubNumber;
									createAssetSAP.Error = errorResponse;

									if (createAssetSAP.FromStock)
									{
										Model.TransferInStockSAP transferInStockSAP = _context.Set<Model.TransferInStockSAP>().Where(a => a.Guid == createAssetSAP.Guid).FirstOrDefault();
										transferInStockSAP.Asset = createAssetResult.Data.Asset;
										transferInStockSAP.SubNumber = createAssetResult.Data.SubNumber;
										_context.Update(transferInStockSAP);
									}

									if (createAssetSAP.InvPlus)
									{
										Model.AssetInvPlusSAP assetInvPlusSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.CreateAssetSAPId == null && a.IsDeleted == false).FirstOrDefault();
										assetInvPlusSAP.ASSET = createAssetResult.Data.Asset;
										assetInvPlusSAP.CreateAssetSAPId = createAssetSAP.Id;
										assetInvPlusSAP.SUBNUMBER = createAssetResult.Data.SubNumber;
										_context.Update(assetInvPlusSAP);
									}

									if (createAssetSAP.FromClone)
									{
										Model.TransferAssetSAP transferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.Guid == createAssetSAP.Guid).FirstOrDefault();
										transferAssetSAP.TO_ASSET = createAssetResult.Data.Asset;
										transferAssetSAP.TO_SUBNUMBER = createAssetResult.Data.SubNumber;
										_context.Update(transferAssetSAP);
									}


									_context.Update(createAssetSAP);
								}


								assetToUpdate.InvNo = createAssetResult.Data.Asset;
								assetToUpdate.SubNo = createAssetResult.Data.SubNumber;
								assetToUpdate.ModifiedAt = DateTime.Now;
								assetToUpdate.AssetStateId = assetState.Id;
								assetToUpdate.IsLocked = false;


								if (createAssetSAP.FromStock)
								{
									assetToUpdate.AssetStateId = assetStateStockIT.Id;
								}

								if (createAssetSAP.FromClone)
								{
									assetToUpdate.IsDeleted = false;
								}

								assetToUpdate.NotSync = false;

								_context.Update(assetToUpdate);
								_context.SaveChanges();

								//Model.CreateAssetSAP createAssetSAPEntity = _context.Set<Model.CreateAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

								//if (createAssetSAPEntity != null)
								//{
								//	List<Model.EntityFile> entityFiles = _context.Set<Model.EntityFile>().Where(e => e.Guid == createAssetSAPEntity.Guid).ToList();

								//	if (entityFiles.Count > 0)
								//	{
								//		for (int i = 0; i < entityFiles.Count; i++)
								//		{
								//			entityFiles[i].EntityId = assetToUpdate.Id;
								//			_context.Update(entityFiles[i]);
								//			_context.SaveChanges();
								//		}
								//	}
								//}



								//var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToList();
								//var countA = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
								//var countB = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();
								//var countC = _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToList();
								//var countD = _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToList();
								//var countE = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
								//var countF = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP: Numarul de inventar ${createAssetResult.Data.Asset + "|" + createAssetResult.Data.SubNumber} a fost creat cu succes!" };
							}
							else
							{
								if (createAssetResult.Meta.Code == 400)
								{
									Model.CreateAssetSAP createAssetSAP = _context.Set<Model.CreateAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 1 && a.IsDeleted == false).SingleOrDefault();

									if (createAssetSAP != null)
									{
										createAssetSAP.SyncErrorCount++;
										createAssetSAP.Error = errorResponse;

										if (createAssetSAP.InvPlus)
										{
											Model.AssetInvPlusSAP assetInvPlusSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.ASSET == asset.ASSET).FirstOrDefault();
											assetInvPlusSAP.ASSET = createAssetResult.Data.Asset;
											assetInvPlusSAP.CreateAssetSAPId = createAssetSAP.Id;
											_context.Update(assetInvPlusSAP);
										}

										assetToUpdate.InvNo = createAssetResult.Data.Asset;
										assetToUpdate.SubNo = createAssetResult.Data.SubNumber;
										assetToUpdate.ModifiedAt = DateTime.Now;
										assetToUpdate.AssetStateId = assetState.Id;
										if (createAssetSAP.FromStock)
										{
											assetToUpdate.AssetStateId = assetStateStockIT.Id;
										}
										assetToUpdate.NotSync = false;

										_context.Update(assetToUpdate);

										_context.Update(createAssetSAP);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Errors[0].Detail + " " + createAssetResult.Errors[0].Meta.Exception_class}!" };

								//var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToList();

								//var countA1 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
								//var countB1 = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();
								//var countC1 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToList();
								//var countD1 = _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToList();
								//var countE1 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
								//var countF1 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();

							}
						}
						catch (Exception ex)
						{
							// var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetToUpdate.Id).Single();

							//assetToDelete.IsDeleted = true;
							//_context.Update(assetToDelete);

							Model.CreateAssetSAP createAssetSAP = _context.Set<Model.CreateAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 1 && a.IsDeleted == false).SingleOrDefault();

							if (createAssetSAP != null)
							{
								createAssetSAP.SyncErrorCount++;
								createAssetSAP.Error = errorResponse;

								_context.Update(createAssetSAP);
								_context.SaveChanges();
							}

							//Model.Error errorResponse2 = new();

							//errorResponse2.AssetId = assetToUpdate.Id;
							//errorResponse2.ErrorTypeId = errorType.Id;
							//errorResponse2.CreatedAt = DateTime.Now;
							//errorResponse2.CreatedBy = _context.UserId;
							//errorResponse2.ModifiedAt = DateTime.Now;
							//errorResponse2.ModifiedBy = _context.UserId;
							//errorResponse2.Code = "RESULT-SAP-ASSET-CREATE-ERROR";
							//errorResponse2.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
							//errorResponse2.UserId = _context.UserId;
							//errorResponse2.IsDeleted = false;

							//_context.Add(errorResponse2);
							//_context.SaveChanges();

							//using (var errorfile = System.IO.File.CreateText("error-create-asset-" + DateTime.Now.Ticks + ".txt"))
							//{
							//	errorfile.WriteLine(ex.StackTrace);
							//	errorfile.WriteLine(ex.ToString());

							//};

							//var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToList();

							//var countA2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
							//var countB2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();
							//var countC2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToList();
							//var countD2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToList();
							//var countE2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
							//var countF2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Errors[0].Detail + " " + createAssetResult.Errors[0].Meta.Exception_class}!" };
						}
					}
					else
					{
						//var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetToUpdate.Id).Single();

						// assetToUpdate.IsDeleted = true;
						Model.CreateAssetSAP createAssetSAP = _context.Set<Model.CreateAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (createAssetSAP != null)
						{
							createAssetSAP.SyncErrorCount++;
							// createAssetSAP.Error = errorResponse;

							_context.Update(createAssetSAP);
							_context.SaveChanges();
						}

						//Model.Error errorResponse2 = new();

						//errorResponse2.AssetId = assetToUpdate.Id;
						//errorResponse2.ErrorTypeId = errorType.Id;
						//errorResponse2.CreatedAt = DateTime.Now;
						//errorResponse2.CreatedBy = _context.UserId;
						//errorResponse2.ModifiedAt = DateTime.Now;
						//errorResponse2.ModifiedBy = _context.UserId;
						//errorResponse2.Code = "RESULT-SAP-ASSET-CREATE-ERROR";
						//errorResponse2.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						//errorResponse2.UserId = _context.UserId;
						//errorResponse2.IsDeleted = false;

						//_context.Add(errorResponse2);
						//_context.SaveChanges();

						//using (var errorfile = System.IO.File.CreateText("error-create-asset-" + DateTime.Now.Ticks + ".txt"))
						//{
						//	errorfile.WriteLine(result);
						//	errorfile.WriteLine(result);

						//};

						//var count = _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToList();

						//var countA3 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
						//var countB3 = _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgets").ToList();
						//var countC3 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToList();
						//var countD3 = _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToList();
						//var countE3 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials").ToList();
						//var countF3 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Errors[0].Detail + " " + createAssetResult.Errors[0].Meta.Exception_class}!" };
					}
				}
				catch (Exception e)
				{
					//Console.Write("Error", ConsoleColor.Red);
					//Console.Write(e.Message, ConsoleColor.DarkRed);

					//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
					//{
					//    errorfile.WriteLine(e.StackTrace);
					//    errorfile.WriteLine(e.ToString());

					//};

					Model.CreateAssetSAP createAssetSAP = _context.Set<Model.CreateAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (createAssetSAP != null)
					{
						createAssetSAP.SyncErrorCount++;

						_context.Update(createAssetSAP);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}
		}

		public async Task<Model.CreateAssetSAPResult> CreateTransferAssetAsync(TransferAssetSAP asset)
		{
			HttpClient clientContract = null;

			var baseUrl = _BASEURL;
			string result = "";
			using (clientContract = new HttpClient())
			{
				TransferAssetResult transferAssetResult = null;
				Model.Inventory inventory = null;
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();


				inventory = _context.Set<Model.Inventory>().Include(a => a.AccMonth).AsNoTracking().Where(i => i.Active == true).Single();

				Model.Asset fromAsset = null;
				Model.Asset toAsset = null;
				Model.AssetDep fromAssetDep = null;
				Model.AssetDep toAssetDep = null;
				Model.AssetDepMD fromAssetDepMD = null;
				Model.AssetDepMD toAssetDepMD = null;

				fromAsset = _context.Set<Model.Asset>().Include(a => a.Company).Where(c => c.InvNo == asset.FROM_ASSET && c.SubNo == asset.FROM_SUBNUMBER).Single();
				toAsset = _context.Set<Model.Asset>().Where(c => c.InvNo == asset.TO_ASSET && c.SubNo == asset.TO_SUBNUMBER).Single();

				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "TRANSFERASSET").Single();

				IList<Dto.TransferAssetData> oIList1 = new List<Dto.TransferAssetData>
				{
					new Dto.TransferAssetData()
						{
							I_INPUT = new Dto.TransferAssetInput()
								{
									FROM_ASSET = asset.FROM_ASSET,
									FROM_SUBNUMBER = asset.FROM_SUBNUMBER,
									COMPANYCODE = asset.COMPANYCODE,
									DOC_DATE = asset.DOC_DATE,
									PSTNG_DATE = asset.PSTNG_DATE,
									ASVAL_DATE = asset.ASVAL_DATE,
									ITEM_TEXT = asset.ITEM_TEXT,
									TO_ASSET = asset.TO_ASSET,
									TO_SUBNUMBER = asset.TO_SUBNUMBER,
									FIS_PERIOD = asset.FIS_PERIOD,
									DOC_TYPE = asset.DOC_TYPE,
									REF_DOC_NO = asset.REF_DOC_NO,
									COMPL_TRANSFER = asset.COMPL_TRANSFER,
									AMOUNT = asset.AMOUNT,
									CURRENCY = asset.CURRENCY,
									PERCENT = asset.PERCENT,
									QUANTITY = asset.QUANTITY,
									BASE_UOM = asset.BASE_UOM,
									PRIOR_YEAR_ACQUISITIONS = asset.PRIOR_YEAR_ACQUISITIONS,
									CURRENT_YEAR_ACQUISITIONS = asset.CURRENT_YEAR_ACQUISITIONS,
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


					if (result != "")
					{

						Model.Error errorResponse = new();

						errorResponse.AssetId = fromAsset.Id;
						errorResponse.ErrorTypeId = errorType.Id;
						errorResponse.CreatedAt = DateTime.Now;
						errorResponse.CreatedBy = _context.UserId;
						errorResponse.ModifiedAt = DateTime.Now;
						errorResponse.ModifiedBy = _context.UserId;
						errorResponse.Code = "RESULT-ASSET-TRANSFER";
						errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse.UserId = _context.UserId;
						errorResponse.IsDeleted = false;

						_context.Add(errorResponse);
						_context.SaveChanges();

						try
						{
							transferAssetResult = JsonConvert.DeserializeObject<TransferAssetResult>(result);

							if (transferAssetResult.Data != null && transferAssetResult.Data.Return_Code == "1")
							{
								fromAsset.ModifiedAt = DateTime.Now;
								toAsset.ModifiedAt = DateTime.Now;
								fromAsset.NotSync = false;
								toAsset.NotSync = false;



								if (asset.COMPL_TRANSFER == "X")
								{
									Model.TransferAssetSAP transferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 3 && a.IsDeleted == false).SingleOrDefault();

									if (transferAssetSAP != null && transferAssetSAP.FromClone)
									{

										// await BlockAsset(fromAsset.Id);
										fromAsset.AssetStateId = 35;
										toAsset.IsDeleted = false;
										toAsset.AssetStateId = 1;
										_context.SaveChanges();
									}
									else
									{
										if (transferAssetSAP != null)
										{
											transferAssetSAP.NotSync = false;
											transferAssetSAP.SyncErrorCount = 0;
											transferAssetSAP.Error = errorResponse;

											_context.Update(transferAssetSAP);
										}

										var quantity = fromAsset.Quantity;
										var valueInv = fromAsset.ValueInv;
										var valueRem = fromAsset.ValueRem;

										fromAsset.Quantity = 0;
										//toAsset.Quantity += quantity;


										fromAsset.ValueInv = 0;
										fromAsset.ValueRem = 0;

										toAsset.ValueInv += valueInv;
										toAsset.ValueRem += valueRem;

										fromAssetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == fromAsset.Id).FirstOrDefault();
										toAssetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == toAsset.Id).FirstOrDefault();

										fromAssetDep.ValueInv = 0;
										fromAssetDep.ValueInvIn = 0;
										fromAssetDep.ValueRem = 0;
										fromAssetDep.ValueRemIn = 0;

										_context.Set<Model.AssetDep>().Update(fromAssetDep);

										toAssetDep.ValueInv += valueInv;
										toAssetDep.ValueInvIn += valueInv;
										toAssetDep.ValueRem += valueRem;
										toAssetDep.ValueRemIn += valueRem;

										_context.Set<Model.AssetDep>().Update(toAssetDep);

										fromAssetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AssetId == fromAsset.Id && a.AccMonthId == inventory.AccMonthId).FirstOrDefault();
										toAssetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AssetId == fromAsset.Id && a.AccMonthId == inventory.AccMonthId).FirstOrDefault();

										fromAssetDepMD.CurrentAPC = 0;
										fromAssetDepMD.PosCap = 0;
										fromAssetDepMD.APCFYStart = 0;
										fromAssetDepMD.DepPostCap = 0;

										_context.Set<Model.AssetDepMD>().Update(fromAssetDepMD);

										toAssetDepMD.CurrentAPC += valueInv;
										toAssetDepMD.PosCap += valueRem;
										toAssetDepMD.APCFYStart += valueInv;
										toAssetDepMD.DepPostCap += valueRem;

										_context.Set<Model.AssetDepMD>().Update(toAssetDepMD);

										_context.Set<Model.Asset>().Update(fromAsset);
										_context.Set<Model.Asset>().Update(toAsset);

										_context.SaveChanges();
									}
									
								}
								else if (asset.QUANTITY > 0)
								{
									Model.TransferAssetSAP transferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (transferAssetSAP != null && transferAssetSAP.FromClone)
									{
										// await BlockAsset(fromAsset.Id);
										fromAsset.AssetStateId = 35;
										toAsset.IsDeleted = false;
										toAsset.AssetStateId = 1;
										_context.SaveChanges();
									}
									else
									{
										if (transferAssetSAP != null)
										{
											transferAssetSAP.NotSync = false;
											transferAssetSAP.SyncErrorCount = 0;

											_context.Update(transferAssetSAP);
										}

										fromAsset.Quantity -= (float)asset.QUANTITY;
										//toAsset.Quantity += (float)newAssetTransferSAP.Quantity;
									}

								}
								else if (asset.AMOUNT > 0)
								{
									Model.TransferAssetSAP transferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if(transferAssetSAP != null && transferAssetSAP.FromClone)
									{
										// await BlockAsset(fromAsset.Id);

										fromAsset.AssetStateId = 35;
										toAsset.IsDeleted = false;
										toAsset.AssetStateId = 1;
										_context.SaveChanges();
									}
									else
									{
										if (transferAssetSAP != null)
										{
											transferAssetSAP.NotSync = false;
											transferAssetSAP.SyncErrorCount = 0;

											_context.Update(transferAssetSAP);

										}
										fromAsset.ValueInv -= asset.AMOUNT;
										fromAsset.ValueRem -= asset.AMOUNT;

										toAsset.ValueInv += asset.AMOUNT;
										toAsset.ValueRem += asset.AMOUNT;

										fromAssetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == fromAsset.Id).FirstOrDefault();
										toAssetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == toAsset.Id).FirstOrDefault();

										fromAssetDep.ValueInv -= asset.AMOUNT;
										fromAssetDep.ValueInvIn -= asset.AMOUNT;
										fromAssetDep.ValueRem -= asset.AMOUNT;
										fromAssetDep.ValueRemIn -= asset.AMOUNT;

										_context.Set<Model.AssetDep>().Update(fromAssetDep);

										toAssetDep.ValueInv += asset.AMOUNT;
										toAssetDep.ValueInvIn += asset.AMOUNT;
										toAssetDep.ValueRem += asset.AMOUNT;
										toAssetDep.ValueRemIn += asset.AMOUNT;

										_context.Set<Model.AssetDep>().Update(toAssetDep);

										fromAssetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AssetId == fromAsset.Id && a.AccMonthId == inventory.AccMonthId).FirstOrDefault();
										toAssetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AssetId == toAsset.Id && a.AccMonthId == inventory.AccMonthId).FirstOrDefault();

										fromAssetDepMD.CurrentAPC -= asset.AMOUNT;
										fromAssetDepMD.PosCap -= asset.AMOUNT;
										fromAssetDepMD.APCFYStart -= asset.AMOUNT;
										fromAssetDepMD.DepPostCap -= asset.AMOUNT;

										_context.Set<Model.AssetDepMD>().Update(fromAssetDepMD);

										toAssetDepMD.CurrentAPC += asset.AMOUNT;
										toAssetDepMD.PosCap += asset.AMOUNT;
										toAssetDepMD.APCFYStart += asset.AMOUNT;
										toAssetDepMD.DepPostCap += asset.AMOUNT;

										_context.Set<Model.AssetDepMD>().Update(toAssetDepMD);

										_context.Set<Model.Asset>().Update(fromAsset);
										_context.Set<Model.Asset>().Update(toAsset);

										_context.SaveChanges();
									}
									
								}

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP: Numarul de inventar ${transferAssetResult.Data.Asset + "|" + transferAssetResult.Data.SubNumber} a fost transferat cu succes!" };
							}
							else
							{
								Model.TransferAssetSAP tranferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

								if (tranferAssetSAP != null)
								{
									tranferAssetSAP.SyncErrorCount++;
									tranferAssetSAP.Error = errorResponse;
									_context.Update(tranferAssetSAP);
									_context.SaveChanges();
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferAssetResult.Meta.Code}!" };
							}
						}
						catch (Exception ex)
						{
							Model.TransferAssetSAP tranferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

							if (tranferAssetSAP != null)
							{
								tranferAssetSAP.SyncErrorCount++;
								tranferAssetSAP.Error = errorResponse;
								_context.Update(tranferAssetSAP);
								_context.SaveChanges();
							}

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferAssetResult.Meta.Code}!" };
						}
					}
					else
					{

						Model.TransferAssetSAP tranferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (tranferAssetSAP != null)
						{
							tranferAssetSAP.SyncErrorCount++;

							_context.Update(tranferAssetSAP);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferAssetResult.Meta.Code}!" };
					}

				}
				catch (Exception e)
				{
					Model.TransferAssetSAP tranferAssetSAP = _context.Set<Model.TransferAssetSAP>().Where(a => a.AssetId == fromAsset.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (tranferAssetSAP != null)
					{
						tranferAssetSAP.SyncErrorCount++;

						_context.Update(tranferAssetSAP);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}

			}

		}

		public async Task<Model.CreateAssetSAPResult> CreateRetireAssetAsync(RetireAssetSAP asset)
		{
			HttpClient clientContract = null;

			var baseUrl = _BASEURL;
			string result = "";
			using (clientContract = new HttpClient())
			{
				RetireAssetResult retireAssetResult = null;
				Model.Asset assetToUpdate = null;
				Model.AssetDep assetDep = null;
				Model.AssetDepMD assetDepMD = null;
				Model.Inventory inventory = null;
				Model.Error errorResponse = null;
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();


				inventory = _context.Set<Model.Inventory>().Include(a => a.AccMonth).AsNoTracking().Where(i => i.Active == true).Single();
				assetToUpdate = _context.Set<Model.Asset>().Include(a => a.Company).Where(c => c.Id == asset.AssetId).FirstOrDefault();

				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "RETIREASSET").Single();

				if(asset.STORNO == "X")
				{
					errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "STORNOASSET").Single();
				}

				IList<Dto.RetireAssetData> oIList1 = new List<Dto.RetireAssetData>
				{
					new Dto.RetireAssetData()
						{
							I_INPUT = new Dto.RetireAssetInput()
								{
									COMPANYCODE = asset.COMPANYCODE,
									ASSET = asset.ASSET,
									SUBNUMBER = asset.SUBNUMBER,
									DOC_DATE = asset.DOC_DATE,
									PSTNG_DATE = asset.PSTNG_DATE,
									VALUEDATE = asset.VALUEDATE,
									ITEM_TEXT = asset.ITEM_TEXT,
									FIS_PERIOD = asset.FIS_PERIOD,
									DOC_TYPE = asset.DOC_TYPE,
									REF_DOC_NO = asset.REF_DOC_NO,
									COMPL_RET = asset.COMPL_RET,
									AMOUNT = asset.AMOUNT,
									CURRENCY = asset.CURRENCY,
									PERCENT = asset.PERCENT,
									QUANTITY = asset.QUANTITY,
									BASE_UOM = asset.BASE_UOM,
									PRIOR_YEAR_ACQUISITIONS = asset.PRIOR_YEAR_ACQUISITIONS,
									CURRENT_YEAR_ACQUISITIONS = asset.CURRENT_YEAR_ACQUISITIONS,
									STORNO = asset.STORNO,
									STORNO_DOC = asset.STORNO_DOC,
									STORNO_DATE = asset.STORNO_DATE,
									STORNO_REASON = asset.STORNO_REASON
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

#if DEBUG
                string jsonString = await contentJson.ReadAsStringAsync();

                string resultFilePath = System.IO.Path.Combine("SAP-RESULTS", "RETIREASSET_SAP" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(resultFilePath))
                {
                    errorfile.WriteLine(jsonString);

                };
#endif

                try
                {

					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					errorResponse = new();

					errorResponse.AssetId = assetToUpdate.Id;
					errorResponse.ErrorTypeId = errorType.Id;
					errorResponse.CreatedAt = DateTime.Now;
					errorResponse.CreatedBy = _context.UserId;
					errorResponse.ModifiedAt = DateTime.Now;
					errorResponse.ModifiedBy = _context.UserId;
					errorResponse.Code = asset.STORNO == "X" ? "RESULT-ASSET-STORNO" : "RESULT-ASSET-RETIRE";
					errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
					errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
					errorResponse.UserId = _context.UserId;
					errorResponse.IsDeleted = false;

					_context.Add(errorResponse);
					// _context.SaveChanges();

					clientContract.Dispose();

					if (result != "")
					{

						try
						{
							retireAssetResult = JsonConvert.DeserializeObject<RetireAssetResult>(result);

							if (retireAssetResult.Data != null && retireAssetResult.Data.Return_Code == "1")
							{
								assetToUpdate.ModifiedAt = DateTime.Now;
								assetToUpdate.NotSync = false;

								errorResponse.BUKRS = retireAssetResult.Data.E_OutPut.BUKRS;
								errorResponse.BELNR = retireAssetResult.Data.E_OutPut.BELNR;
								errorResponse.GJAHR = retireAssetResult.Data.E_OutPut.GJAHR;

								if (asset.COMPL_RET == "X")
								{

									Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (retireAssetSAP != null)
									{
										retireAssetSAP.NotSync = false;
										retireAssetSAP.SyncErrorCount = 0;
										retireAssetSAP.Error = errorResponse;
										_context.Update(retireAssetSAP);
									}


									assetToUpdate.Quantity = 0;
									assetToUpdate.ValueInv = 0;
									assetToUpdate.ValueRem = 0;

									assetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == assetToUpdate.Id).FirstOrDefault();

									assetDep.ValueInv = 0;
									assetDep.ValueInvIn = 0;
									assetDep.ValueRem = 0;
									assetDep.ValueRemIn = 0;

									_context.Set<Model.AssetDep>().Update(assetDep);

									assetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AssetId == assetToUpdate.Id && a.AccMonthId == inventory.AccMonthId).FirstOrDefault();

									assetDepMD.CurrentAPC = 0;
									assetDepMD.PosCap = 0;
									assetDepMD.APCFYStart = 0;
									assetDepMD.DepPostCap = 0;

									_context.Set<Model.AssetDepMD>().Update(assetDepMD);

									_context.Set<Model.Asset>().Update(assetToUpdate);

									_context.SaveChanges();
								}
								else if (asset.QUANTITY > 0)
								{
									assetToUpdate.Quantity -= (float)asset.QUANTITY;

									Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (retireAssetSAP != null)
									{
										retireAssetSAP.NotSync = false;
										retireAssetSAP.SyncErrorCount = 0;
										retireAssetSAP.Error = errorResponse;

										_context.Update(retireAssetSAP);
									}
								}
								else if (asset.AMOUNT > 0)
								{

									Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (retireAssetSAP != null)
									{
										retireAssetSAP.NotSync = false;
										retireAssetSAP.SyncErrorCount = 0;
										retireAssetSAP.Error = errorResponse;

										_context.Update(retireAssetSAP);


									}
									assetToUpdate.ValueInv -= asset.AMOUNT;
									assetToUpdate.ValueRem -= asset.AMOUNT;

									assetDep = _context.Set<Model.AssetDep>().Where(a => a.AssetId == assetToUpdate.Id).FirstOrDefault();

									assetDep.ValueInv -= asset.AMOUNT;
									assetDep.ValueInvIn -= asset.AMOUNT;
									assetDep.ValueRem -= asset.AMOUNT;
									assetDep.ValueRemIn -= asset.AMOUNT;

									_context.Set<Model.AssetDep>().Update(assetDep);

									assetDepMD = _context.Set<Model.AssetDepMD>().Where(a => a.AssetId == assetToUpdate.Id && a.AccMonthId == inventory.AccMonthId).FirstOrDefault();

									assetDepMD.CurrentAPC -= asset.AMOUNT;
									assetDepMD.PosCap -= asset.AMOUNT;
									assetDepMD.APCFYStart -= asset.AMOUNT;
									assetDepMD.DepPostCap -= asset.AMOUNT;

									_context.Set<Model.AssetDepMD>().Update(assetDepMD);

									_context.Set<Model.Asset>().Update(assetToUpdate);

									_context.SaveChanges();
								}



								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP: Numarul de inventar ${retireAssetResult.Data.Return_Message} a fost casat cu succes!" };
							}
							else
							{
								if (retireAssetResult.Meta.Code == 400)
								{
									Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (retireAssetSAP != null)
									{
										retireAssetSAP.SyncErrorCount++;
										retireAssetSAP.Error = errorResponse;
										_context.Update(retireAssetSAP);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {retireAssetResult.Meta.Code}!" };

							}
						}
						catch (Exception ex)
						{
							Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

							if (retireAssetSAP != null)
							{
								retireAssetSAP.SyncErrorCount++;
								retireAssetSAP.Error = errorResponse;
								_context.Update(retireAssetSAP);
								_context.SaveChanges();
							}

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {retireAssetResult.Meta.Code}!" };
						}

					}
					else
					{

						Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (retireAssetSAP != null)
						{
							retireAssetSAP.SyncErrorCount++;
							retireAssetSAP.Error = errorResponse;
							_context.Update(retireAssetSAP);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {retireAssetResult.Meta.Code}!" };
					}

				}
				catch (Exception e)
				{

					errorResponse = new();

					errorResponse.AssetId = assetToUpdate.Id;
					errorResponse.ErrorTypeId = errorType.Id;
					errorResponse.CreatedAt = DateTime.Now;
					errorResponse.CreatedBy = _context.UserId;
					errorResponse.ModifiedAt = DateTime.Now;
					errorResponse.ModifiedBy = _context.UserId;
					errorResponse.Code = asset.STORNO == "X" ? "RESULT-ASSET-STORNO" : "RESULT-ASSET-RETIRE";
					errorResponse.Name = "EROARE CONECTARE";
					errorResponse.UserId = _context.UserId;
					errorResponse.IsDeleted = false;

					_context.Add(errorResponse);


					Model.RetireAssetSAP retireAssetSAP = _context.Set<Model.RetireAssetSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (retireAssetSAP != null)
					{
						retireAssetSAP.SyncErrorCount++;
						retireAssetSAP.Error = errorResponse;
						_context.Update(retireAssetSAP);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}

		}

		public async Task<Model.CreateAssetSAPResult> CreateAcquisitionAssetAsync(AcquisitionAssetSAP asset, int? assetSyncErrorId = null)
		{
			HttpClient clientContract = null;
			Model.ErrorType errorType = null;
			AcquisitionAssetResult createAssetResult = null;
			Model.Asset assetToUpdate = null;
			Model.AssetState assetState = null;
			Model.AcquisitionAssetSAP acquisitionAssetSAP = null;

			var baseUrl = _BASEURL;
			// CreateAssetResult result = null;
			string result = "";
			using (clientContract = new HttpClient())
			{
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				errorType = await _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "ACQUISITIONASSET" && e.IsDeleted == false).SingleOrDefaultAsync();
				assetState = await _context.Set<Model.AssetState>().AsNoTracking().Where(e => e.Code == "TOVALIDATE" && e.IsDeleted == false).SingleOrDefaultAsync();

				IList<Dto.AcquisitionAssetData> oIList1 = new List<Dto.AcquisitionAssetData>
				{
					new Dto.AcquisitionAssetData()
						{
							I_INPUT = new Dto.AcquisitionAssetInput()
								{
									STORNO = asset.STORNO,
									COMPANYCODE = asset.COMPANYCODE,
									DOC_DATE = asset.DOC_DATE,
									PSTNG_DATE = asset.PSTNG_DATE,
									REF_DOC_NO = asset.REF_DOC_NO,
									HEADER_TXT = asset.HEADER_TXT,
									VENDOR_NO = asset.VENDOR_NO,
									CURRENCY = asset.CURRENCY,
									EXCH_RATE = asset.EXCH_RATE,
									TOTAL_AMOUNT = asset.TOTAL_AMOUNT,
									ASSETS = asset.ASSETS
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

#if DEBUG
                string jsonString = await contentJson.ReadAsStringAsync();

                string resultFilePath = System.IO.Path.Combine("SAP-RESULTS", "ACQUISIONASSETSAP_" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(resultFilePath))
                {
                    errorfile.WriteLine(jsonString);

                };
#endif

                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

                string errorFolderPath = "errors";

                if (!System.IO.Directory.Exists(errorFolderPath))
                {
                    System.IO.Directory.CreateDirectory(errorFolderPath);
                }

                string errorFilePath = System.IO.Path.Combine(errorFolderPath, "before-acquisition-create-" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(errorFilePath))
                {
                    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));
                }

                try
				{

					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					clientContract.Dispose();

					//using (var errorfile = System.IO.File.CreateText("after-acquisition-create-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(result);

					//};

					if (result != "")
					{
						try
						{
							createAssetResult = JsonConvert.DeserializeObject<AcquisitionAssetResult>(result);

							if ((createAssetResult.Data != null && (createAssetResult.Data.Return_Code == "1")) || (createAssetResult.Meta.Code == 200))
							{
								for (int i = 0; i < asset.ASSETS.Count; i++)
								{

									assetToUpdate = await _context.Set<Model.Asset>()
										.Include(a => a.AcquisitionAssetSAP)
										.Where(a => a.InvNo == asset.ASSETS[i].ASSET && a.SubNo == asset.ASSETS[i].SUBNUMBER && a.IsDeleted == false && a.AssetStateId == assetState.Id)
										.FirstOrDefaultAsync();

									if (assetToUpdate != null)
									{
										Model.Error errorResponse = new();

										errorResponse.AssetId = assetToUpdate.Id;
										errorResponse.ErrorTypeId = errorType.Id;
										errorResponse.CreatedAt = DateTime.Now;
										errorResponse.CreatedBy = _context.UserId;
										errorResponse.ModifiedAt = DateTime.Now;
										errorResponse.ModifiedBy = _context.UserId;
										errorResponse.Code = "RESULT-ACQUISITION-CREATE";
										errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
										errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
										errorResponse.UserId = _context.UserId;
										errorResponse.IsDeleted = false;
										errorResponse.BUKRS = createAssetResult.Data.E_OutPut.BUKRS;
										errorResponse.BELNR = createAssetResult.Data.E_OutPut.BELNR;
										errorResponse.GJAHR = createAssetResult.Data.E_OutPut.GJAHR;

										_context.Add(errorResponse);
                                        //_context.SaveChanges();
                                        var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);

                                        if (assetSyncErrorId != null)
                                        {
                                            var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == assetSyncErrorId);

                                            if (assetSyncErrorToUpdate != null)
                                            {
                                                if (createAssetResultNew.Meta.Code == 200)
                                                {
                                                    assetSyncErrorToUpdate.SyncStatus = "Synchronization Success";
                                                    assetSyncErrorToUpdate.Error = errorResponse.Id;

                                                    _context.SaveChanges();
                                                }
                                                if (createAssetResultNew.Meta.Code == 400)
                                                {
                                                    assetSyncErrorToUpdate.SyncStatus = "Synchronization Fail";
                                                    assetSyncErrorToUpdate.Error = errorResponse.Id;

                                                    _context.SaveChanges();
                                                }
                                                else
                                                {

                                                }
                                            }
                                        }
                                        acquisitionAssetSAP = await _context.Set<Model.AcquisitionAssetSAP>()
											.Where(a => a.AssetId == assetToUpdate.Id && a.IsDeleted == false && a.SyncErrorCount < 5 && a.NotSync == true)
											.FirstOrDefaultAsync();

										if (acquisitionAssetSAP != null)
										{
											acquisitionAssetSAP.SyncErrorCount = 0;
											acquisitionAssetSAP.NotSync = false;
											acquisitionAssetSAP.ModifiedAt = DateTime.Now;
											acquisitionAssetSAP.Error = errorResponse;

											assetToUpdate.AssetStateId = asset.STORNO == "X" ? 9 : 1;
											assetToUpdate.ModifiedAt = DateTime.Now;
											_context.Update(assetToUpdate);
											_context.Update(acquisitionAssetSAP);
											_context.SaveChanges();
										}
									}
								}

								// assetUpdate.InvNo = createAssetResult.Data.Asset;
								//assetUpdate.ModifiedAt = DateTime.Now;
								//assetUpdate.AssetStateId = 1;
								//_context.Update(assetUpdate);
								//_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP ACQUISITION: ${createAssetResult.Data.Return_Message} a fost actualizat cu succes!" };
							}
							else if (createAssetResult.Meta.Code == 400)
							{
                                using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "after-acquisition-create-error-400" + DateTime.Now.Ticks + ".txt")))
                                {
                                    errorfile.WriteLine(result);
                                }

                                for (int i = 0; i < asset.ASSETS.Count; i++)
								{
									assetToUpdate = await _context.Set<Model.Asset>()
										.Include(a => a.AcquisitionAssetSAP)
										.Where(a => a.InvNo == asset.ASSETS[i].ASSET && a.SubNo == asset.ASSETS[i].SUBNUMBER && a.IsDeleted == false && a.AssetStateId == assetState.Id)
										.FirstOrDefaultAsync();

									if (assetToUpdate != null)
									{
										Model.Error errorResponse = new();

										errorResponse.AssetId = assetToUpdate.Id;
										errorResponse.ErrorTypeId = errorType.Id;
										errorResponse.CreatedAt = DateTime.Now;
										errorResponse.CreatedBy = _context.UserId;
										errorResponse.ModifiedAt = DateTime.Now;
										errorResponse.ModifiedBy = _context.UserId;
										errorResponse.Code = "RESULT-ACQUISITION-CREATE";
										errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
										errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
										errorResponse.UserId = _context.UserId;
										errorResponse.IsDeleted = false;
										//errorResponse.BUKRS = createAssetResult.Data != null && createAssetResult.Data.E_OutPut != null ? createAssetResult.Data.E_OutPut.BUKRS : "";
										//errorResponse.BELNR = createAssetResult.Data != null && createAssetResult.Data.E_OutPut != null ? createAssetResult.Data.E_OutPut.BELNR : "";
										//errorResponse.GJAHR = createAssetResult.Data != null && createAssetResult.Data.E_OutPut != null ? createAssetResult.Data.E_OutPut.GJAHR : "";

										_context.Add(errorResponse);
                                        //_context.SaveChanges();
                                        var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);

                                        if (assetSyncErrorId != null)
                                        {
                                            var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == assetSyncErrorId);

                                            if (assetSyncErrorToUpdate != null)
                                            {
                                                if (createAssetResultNew.Meta.Code == 200)
                                                {
                                                    assetSyncErrorToUpdate.SyncStatus = "Synchronization Success";
                                                    assetSyncErrorToUpdate.Error = errorResponse.Id;

                                                    _context.SaveChanges();
                                                }
                                                if (createAssetResultNew.Meta.Code == 400)
                                                {
                                                    assetSyncErrorToUpdate.SyncStatus = "Synchronization Fail";
                                                    assetSyncErrorToUpdate.Error = errorResponse.Id;

                                                    _context.SaveChanges();
                                                }
                                                else
                                                {

                                                }
                                            }
                                        }
                                        acquisitionAssetSAP = await _context.Set<Model.AcquisitionAssetSAP>()
											.Where(a => a.AssetId == assetToUpdate.Id && a.IsDeleted == false && a.SyncErrorCount < 1 && a.NotSync == true)
											.FirstOrDefaultAsync();

										if (acquisitionAssetSAP != null)
										{
											acquisitionAssetSAP.SyncErrorCount++;
											acquisitionAssetSAP.ModifiedAt = DateTime.Now;
											acquisitionAssetSAP.Error = errorResponse;

											_context.Update(acquisitionAssetSAP);
											_context.SaveChanges();
										}
									}


								}

								// assetUpdate.InvNo = createAssetResult.Data.Asset;
								//assetUpdate.ModifiedAt = DateTime.Now;
								//assetUpdate.AssetStateId = 1;
								//_context.Update(assetUpdate);
								//_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP ACQUISITION: {createAssetResult.Data.Return_Code + " " + createAssetResult.Data.Return_Message}!" };
							}
						}
						catch (Exception ex)
						{
							for (int i = 0; i < asset.ASSETS.Count; i++)
							{
								assetToUpdate = await _context.Set<Model.Asset>()
										.Include(a => a.AcquisitionAssetSAP)
										.Where(a => a.InvNo == asset.ASSETS[i].ASSET && a.SubNo == asset.ASSETS[i].SUBNUMBER && a.IsDeleted == false && a.AssetStateId == assetState.Id)
										.FirstOrDefaultAsync();

								if (assetToUpdate != null)
								{
									Model.Error errorResponse = new();

									errorResponse.AssetId = assetToUpdate.Id;
									errorResponse.ErrorTypeId = errorType.Id;
									errorResponse.CreatedAt = DateTime.Now;
									errorResponse.CreatedBy = _context.UserId;
									errorResponse.ModifiedAt = DateTime.Now;
									errorResponse.ModifiedBy = _context.UserId;
									errorResponse.Code = "RESULT-ACQUISITION-CREATE";
									errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
									errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
									errorResponse.UserId = _context.UserId;
									errorResponse.IsDeleted = false;

									_context.Add(errorResponse);
                                    var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);

                                    if (assetSyncErrorId != null)
                                    {
                                        var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == assetSyncErrorId);

                                        if (assetSyncErrorToUpdate != null)
                                        {
                                            if (createAssetResultNew.Meta.Code == 200)
                                            {
                                                assetSyncErrorToUpdate.SyncStatus = "Synchronization Success";
                                                assetSyncErrorToUpdate.Error = errorResponse.Id;

                                                _context.SaveChanges();
                                            }
                                            if (createAssetResultNew.Meta.Code == 400)
                                            {
                                                assetSyncErrorToUpdate.SyncStatus = "Synchronization Fail";
                                                assetSyncErrorToUpdate.Error = errorResponse.Id;

                                                _context.SaveChanges();
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    acquisitionAssetSAP = await _context.Set<Model.AcquisitionAssetSAP>()
											.Where(a => a.AssetId == assetToUpdate.Id && a.IsDeleted == false && a.SyncErrorCount < 5 && a.NotSync == true)
											.FirstOrDefaultAsync();

									if (acquisitionAssetSAP != null)
									{
										acquisitionAssetSAP.SyncErrorCount++;
										acquisitionAssetSAP.ModifiedAt = DateTime.Now;
										acquisitionAssetSAP.Error = errorResponse;
										_context.Update(acquisitionAssetSAP);
										_context.SaveChanges();
									}
								}
							}

							//assetToDelete.IsDeleted = true;
							//_context.Update(assetToDelete);
							//_context.SaveChanges();

							//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
							//{
							//	errorfile.WriteLine(ex.StackTrace);
							//	errorfile.WriteLine(ex.ToString());

							//};
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP ACQUISITION: {createAssetResult.Meta.Code}!" };
					}
					else
					{

						for (int i = 0; i < asset.ASSETS.Count; i++)
						{
							assetToUpdate = await _context.Set<Model.Asset>()
										.Include(a => a.AcquisitionAssetSAP)
										.Where(a => a.InvNo == asset.ASSETS[i].ASSET && a.SubNo == asset.ASSETS[i].SUBNUMBER && a.IsDeleted == false && a.AssetStateId == assetState.Id)
										.FirstOrDefaultAsync();

							if (assetToUpdate != null)
							{
								Model.Error errorResponse = new();

								errorResponse.AssetId = assetToUpdate.Id;
								errorResponse.ErrorTypeId = errorType.Id;
								errorResponse.CreatedAt = DateTime.Now;
								errorResponse.CreatedBy = _context.UserId;
								errorResponse.ModifiedAt = DateTime.Now;
								errorResponse.ModifiedBy = _context.UserId;
								errorResponse.Code = "RESULT-ACQUISITION-CREATE";
								errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
								errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
								errorResponse.UserId = _context.UserId;
								errorResponse.IsDeleted = false;

								_context.Add(errorResponse);
                                var createAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);

                                if (assetSyncErrorId != null)
                                {
                                    var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == assetSyncErrorId);

                                    if (assetSyncErrorToUpdate != null)
                                    {
                                        if (createAssetResultNew.Meta.Code == 200)
                                        {
                                            assetSyncErrorToUpdate.SyncStatus = "Synchronization Success";
                                            assetSyncErrorToUpdate.Error = errorResponse.Id;

                                            _context.SaveChanges();
                                        }
                                        if (createAssetResultNew.Meta.Code == 400)
                                        {
                                            assetSyncErrorToUpdate.SyncStatus = "Synchronization Fail";
                                            assetSyncErrorToUpdate.Error = errorResponse.Id;

                                            _context.SaveChanges();
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                                acquisitionAssetSAP = await _context.Set<Model.AcquisitionAssetSAP>()
											.Where(a => a.AssetId == assetToUpdate.Id && a.IsDeleted == false && a.SyncErrorCount < 5 && a.NotSync == true)
											.FirstOrDefaultAsync();

								if (acquisitionAssetSAP != null)
								{
									acquisitionAssetSAP.SyncErrorCount++;
									acquisitionAssetSAP.ModifiedAt = DateTime.Now;
									acquisitionAssetSAP.Error = errorResponse;
									_context.Update(acquisitionAssetSAP);
									_context.SaveChanges();
								}
							}
						}

						//assetToDelete.IsDeleted = true;
						//_context.Update(assetToDelete);
						//_context.SaveChanges();

						//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
						//{
						//	errorfile.WriteLine(result);
						//	errorfile.WriteLine(result);

						//};

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP ACQUISITION: {createAssetResult.Meta.Code}!" };
					}

				}
				catch (Exception ex)
				{
					//Console.Write("Error", ConsoleColor.Red);
					//Console.Write(e.Message, ConsoleColor.DarkRed);

					//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(e.StackTrace);
					//	errorfile.WriteLine(e.ToString());

					//};

					for (int i = 0; i < asset.ASSETS.Count; i++)
					{
						assetToUpdate = await _context.Set<Model.Asset>()
										.Include(a => a.AcquisitionAssetSAP)
										.Where(a => a.InvNo == asset.ASSETS[i].ASSET && a.SubNo == asset.ASSETS[i].SUBNUMBER && a.IsDeleted == false && a.AssetStateId == assetState.Id)
										.FirstOrDefaultAsync();


						if (assetToUpdate != null)
						{
							acquisitionAssetSAP = await _context.Set<Model.AcquisitionAssetSAP>()
											.Where(a => a.AssetId == assetToUpdate.Id && a.IsDeleted == false && a.SyncErrorCount < 5 && a.NotSync == true)
											.FirstOrDefaultAsync();

							if (acquisitionAssetSAP != null)
							{
								acquisitionAssetSAP.SyncErrorCount++;
								acquisitionAssetSAP.ModifiedAt = DateTime.Now;
								_context.Update(acquisitionAssetSAP);
								_context.SaveChanges();
							}							
						}
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP ACQUISITION: Lipsa conexiune!" };
				}
			}

		}

		public async Task<Model.CreateAssetSAPResult> CreateAssetInvPlusAsync(AssetInvPlusSAP asset)
		{
			HttpClient clientContract = null;
			AssetInvPlusResult createAssetResult = null;
			var baseUrl = _BASEURL;
			string result = "";
			using (clientContract = new HttpClient())
			{
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.Asset assetToUpdate = _context.Set<Model.Asset>().Include(c => c.CreateAssetSAP).Where(a => a.InvNo == asset.ASSET).Single();
				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "ASSETINVPLUS").Single();

				IList<Dto.AssetInvPlusData> oIList1 = new List<Dto.AssetInvPlusData>
				{
					new Dto.AssetInvPlusData()
						{
							I_INPUT = new Dto.AssetInvPlusInput()
								{
									ASSET = asset.ASSET,
									SUBNUMBER = asset.SUBNUMBER,
									COMPANYCODE = asset.COMPANYCODE,
									DOC_DATE = asset.DOC_DATE,
									PSTNG_DATE = asset.PSTNG_DATE,
									ASVAL_DATE = asset.ASVAL_DATE,
									ITEM_TEXT = asset.ITEM_TEXT,
									DOC_TYPE = asset.DOC_TYPE,
									REF_DOC_NO = asset.REF_DOC_NO,
									AMOUNT = asset.AMOUNT,
									TRANSTYPE = asset.TRANSTYPE
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

					if (result != "")
					{
						Model.Error errorResponse = new();

						errorResponse.AssetId = assetToUpdate.Id;
						errorResponse.ErrorTypeId = errorType.Id;
						errorResponse.CreatedAt = DateTime.Now;
						errorResponse.CreatedBy = _context.UserId;
						errorResponse.ModifiedAt = DateTime.Now;
						errorResponse.ModifiedBy = _context.UserId;
						errorResponse.Code = "RESULT-SAP-INV-PLUS";
						errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse.UserId = _context.UserId;
						errorResponse.IsDeleted = false;

						_context.Add(errorResponse);
						_context.SaveChanges();

						try
						{
							createAssetResult = JsonConvert.DeserializeObject<AssetInvPlusResult>(result);

							if (createAssetResult.Data != null && createAssetResult.Data.Return_Code == "1")
							{
								Model.AssetInvPlusSAP createAssetSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

								if (createAssetSAP != null)
								{
									createAssetSAP.NotSync = false;
									createAssetSAP.SyncErrorCount = 0;
									_context.Update(createAssetSAP);
								}


								assetToUpdate.ModifiedAt = DateTime.Now;
								// assetToUpdate.AssetStateId = 9;
								assetToUpdate.NotSync = false;

								_context.Update(assetToUpdate);
								_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP: ${createAssetResult.Data.Return_Message}!" };
							}
							else
							{
								if (createAssetResult.Meta.Code == 400)
								{
									Model.AssetInvPlusSAP createAssetSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (createAssetSAP != null)
									{
										createAssetSAP.SyncErrorCount++;
										_context.Update(createAssetSAP);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Meta.Code}!" };

							}
						}
						catch (Exception ex)
						{

							Model.AssetInvPlusSAP createAssetSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

							if (createAssetSAP != null)
							{
								createAssetSAP.SyncErrorCount++;
								_context.Update(createAssetSAP);
								_context.SaveChanges();
							}

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Meta.Code}!" };
						}
					}
					else
					{
						Model.AssetInvPlusSAP createAssetSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (createAssetSAP != null)
						{
							createAssetSAP.SyncErrorCount++;

							_context.Update(createAssetSAP);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Meta.Code}!" };
					}


				}
				catch (Exception e)
				{
					Model.AssetInvPlusSAP createAssetSAP = _context.Set<Model.AssetInvPlusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (createAssetSAP != null)
					{
						createAssetSAP.SyncErrorCount++;

						_context.Update(createAssetSAP);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}

		}

		public async Task<Model.CreateAssetSAPResult> CreateAssetInvMinusAsync(AssetInvMinusSAP asset)
		{
			HttpClient clientContract = null;
			AssetInvMinusResult createAssetResult = null;
			var baseUrl = _BASEURL;
			string result = "";
			using (clientContract = new HttpClient())
			{

				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.Asset assetToUpdate = _context.Set<Model.Asset>().Include(c => c.CreateAssetSAP).Where(a => a.InvNo == asset.ASSET).Single();
				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "ASSETINVMINUS").Single();


				IList<Dto.AssetInvMinusData> oIList1 = new List<Dto.AssetInvMinusData>
				{
					new Dto.AssetInvMinusData()
						{
							I_INPUT = new Dto.AssetInvMinusInput()
								{
									ASSET = asset.ASSET,
									SUBNUMBER = asset.SUBNUMBER,
									COMPANYCODE = asset.COMPANYCODE,
									DOC_DATE = asset.DOC_DATE,
									PSTNG_DATE = asset.PSTNG_DATE,
									ASVAL_DATE = asset.ASVAL_DATE,
									ITEM_TEXT = asset.ITEM_TEXT,
									DOC_TYPE = asset.DOC_TYPE,
									REF_DOC_NO = asset.REF_DOC_NO,
									TRANSTYPE = asset.TRANSTYPE,
									INVENTORY_DIFF = asset.INVENTORY_DIFF,
									//AMOUNT = asset.AMOUNT,

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

					if (result != "")
					{
						Model.Error errorResponse = new();

						errorResponse.AssetId = assetToUpdate.Id;
						errorResponse.ErrorTypeId = errorType.Id;
						errorResponse.CreatedAt = DateTime.Now;
						errorResponse.CreatedBy = _context.UserId;
						errorResponse.ModifiedAt = DateTime.Now;
						errorResponse.ModifiedBy = _context.UserId;
						errorResponse.Code = "RESULT-SAP-INV-MINUS";
						errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse.UserId = _context.UserId;
						errorResponse.IsDeleted = false;

						_context.Add(errorResponse);
						_context.SaveChanges();

						try
						{
							createAssetResult = JsonConvert.DeserializeObject<AssetInvMinusResult>(result);

							if (createAssetResult.Data != null && createAssetResult.Data.Return_Code == "1")
							{
								Model.AssetInvMinusSAP createAssetSAP = _context.Set<Model.AssetInvMinusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

								if (createAssetSAP != null)
								{
									createAssetSAP.NotSync = false;
									createAssetSAP.SyncErrorCount = 0;
									_context.Update(createAssetSAP);
								}


								assetToUpdate.ModifiedAt = DateTime.Now;
								// assetToUpdate.AssetStateId = 9;
								assetToUpdate.NotSync = false;

								_context.Update(assetToUpdate);
								_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP: Numarul de inventar ${createAssetResult.Data.Return_Code + "|" + createAssetResult.Data.Return_Message}!" };
							}
							else
							{
								if (createAssetResult.Meta.Code == 400)
								{
									Model.AssetInvMinusSAP createAssetSAP = _context.Set<Model.AssetInvMinusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (createAssetSAP != null)
									{
										createAssetSAP.SyncErrorCount++;
										_context.Update(createAssetSAP);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Meta.Code}!" };

							}
						}
						catch (Exception ex)
						{

							Model.AssetInvMinusSAP createAssetSAP = _context.Set<Model.AssetInvMinusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

							if (createAssetSAP != null)
							{
								createAssetSAP.SyncErrorCount++;
								_context.Update(createAssetSAP);
								_context.SaveChanges();
							}
							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Meta.Code}!" };
						}
					}
					else
					{
						Model.AssetInvMinusSAP createAssetSAP = _context.Set<Model.AssetInvMinusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (createAssetSAP != null)
						{
							createAssetSAP.SyncErrorCount++;

							_context.Update(createAssetSAP);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {createAssetResult.Meta.Code}!" };
					}


				}
				catch (Exception e)
				{
					Model.AssetInvMinusSAP createAssetSAP = _context.Set<Model.AssetInvMinusSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (createAssetSAP != null)
					{
						createAssetSAP.SyncErrorCount++;

						_context.Update(createAssetSAP);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}

		}

		public async Task<Model.CreateAssetSAPResult> TransferInStockAsync(TransferInStockInput asset)
		{
			HttpClient clientContract = null;

			var baseUrl = _BASEURL;
			TransferInStockResult transferInStockResult = null;
			string result = "";
			using (clientContract = new HttpClient())
			{
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();


				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "CREATEASSETSTOCK").Single();
				List<Dto.CreateAssetSAP> newAssetSAP = new();
				Model.Inventory inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Single();
				Model.Asset assetToUpdate = _context.Set<Model.Asset>().Include(d => d.Document).Where(a => a.InvNo == asset.Asset).Single();

				//Model.SubCategory subCategory = _context.Set<Model.SubCategory>()
				//	.Include(c => c.Category).ThenInclude(c => c.InterCompany).ThenInclude(a => a.AssetCategory)
				//	.Include(c => c.Category).ThenInclude(c => c.InterCompany).ThenInclude(a => a.ExpAccount)
				//	.Where(a => a.Id == asset.SubCategoryId).FirstOrDefault();


				IList<Dto.TransferInStockData> oIList1 = new List<Dto.TransferInStockData>
				{
					new Dto.TransferInStockData()
						{
							I_INPUT = new Dto.TransferInStockInput()
								{
									Doc_Date = asset.Doc_Date,
									Pstng_Date = asset.Pstng_Date,
									Material = asset.Material,
									Plant = asset.Plant,
									Storage_Location = asset.Storage_Location,
									Quantity = asset.Quantity,
									Uom = asset.Uom,
									Batch = asset.Batch,
                                    // Gl_Account = asset[0].Gl_Account.Length == 6 ? "00" + asset[0].Gl_Account : asset[0].Gl_Account,
                                    Gl_Account = "",
									Item_Text = asset.Item_Text,
									Asset = asset.Asset,
									SubNumber = "0000",
									Ref_Doc_No = asset.Ref_Doc_No,
									Header_Txt = asset.Header_Txt,
									Storno = asset.Storno,
									Storno_Date = asset.Storno_Date,
									Storno_Doc = asset.Storno_Doc,
									Storno_Year = asset.Storno_Year,
									Storno_User = asset.Storno_User
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
					Model.Error error3 = new();

					error3.AssetId = assetToUpdate.Id;
					error3.ErrorTypeId = errorType.Id;
					error3.CreatedAt = DateTime.Now;
					error3.CreatedBy = _context.UserId;
					error3.ModifiedAt = DateTime.Now;
					error3.ModifiedBy = _context.UserId;
					error3.Code = "BEFORE-ASSET-TRANSFER-IN-STOCK";
					error3.Name = JsonConvert.SerializeObject(asset, Formatting.Indented).ToString();
					error3.UserId = _context.UserId;
					error3.IsDeleted = false;
					error3.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();

					await _context.AddAsync(error3);
					await _context.SaveChangesAsync();

					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					clientContract.Dispose();

					if (result != "")
					{
						Model.Error errorResponse3 = new();

						errorResponse3.AssetId = assetToUpdate.Id;
						errorResponse3.ErrorTypeId = errorType.Id;
						errorResponse3.CreatedAt = DateTime.Now;
						errorResponse3.CreatedBy = _context.UserId;
						errorResponse3.ModifiedAt = DateTime.Now;
						errorResponse3.ModifiedBy = _context.UserId;
						errorResponse3.Code = "R-SAP-AFTER-ASSET-TRA-IN-STOCK";
						errorResponse3.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse3.UserId = _context.UserId;
						errorResponse3.IsDeleted = false;
						errorResponse3.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();

						await _context.AddAsync(errorResponse3);
						await _context.SaveChangesAsync();

						try
						{
							transferInStockResult = JsonConvert.DeserializeObject<TransferInStockResult>(result);

							if (transferInStockResult.Data != null && transferInStockResult.Data.Return_Code == "1")
							{
								Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.Storno != "X" && a.SyncErrorCount < 3 && a.IsDeleted == false).SingleOrDefault();

								if (transferInStock != null)
								{
									transferInStock.SyncErrorCount = 0;
									transferInStock.NotSync = false;
									_context.Update(transferInStock);

								}

								//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
								//{
								//	errorfile.WriteLine(transferInStockResult);
								//	errorfile.WriteLine(transferInStockResult);

								//};

								assetToUpdate.ModifiedAt = DateTime.Now;
								assetToUpdate.Document.DocNo2 = transferInStockResult.Data.E_OutPut.Mat_Doc.Trim();

								//Model.BudgetManager budgetManager = _context.Set<Model.BudgetManager>().Where(a => a.Code == transferInStockResult.Data.E_OutPut.Doc_Year.Trim()).FirstOrDefault();

								//if (budgetManager != null)
								//{
								//	assetToUpdate.BudgetManagerId = budgetManager.Id;
								//}
								_context.Update(assetToUpdate);
								_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP:  ${transferInStockResult.Data.Return_Code + "|" + transferInStockResult.Data.Return_Message}!" };
							}
							else
							{
								if (transferInStockResult.Meta.Code == 400)
								{
									Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (transferInStock != null)
									{
										transferInStock.SyncErrorCount++;

										_context.Update(transferInStock);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferInStockResult.Meta.Code}!" };
							}
						}
						catch (Exception ex)
						{
							Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

							if (transferInStock != null)
							{
								transferInStock.SyncErrorCount++;

								_context.Update(transferInStock);
								_context.SaveChanges();
							}

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferInStockResult.Meta.Code}!" };
						}
					}
					else
					{
						//Model.Error errorResponse5 = new();

						//errorResponse5.AssetId = assetId;
						//errorResponse5.ErrorTypeId = errorType.Id;
						//errorResponse5.CreatedAt = DateTime.Now;
						//errorResponse5.CreatedBy = _context.UserId;
						//errorResponse5.ModifiedAt = DateTime.Now;
						//errorResponse5.ModifiedBy = _context.UserId;
						//errorResponse5.Code = "ER-SAP-ASSET-CREATE-FROM-STOCK";
						//errorResponse5.Name = "EMPTY-RESPONSE";
						//errorResponse5.UserId = _context.UserId;
						//errorResponse5.IsDeleted = false;

						//await _context.AddAsync(errorResponse5);
						//await _context.SaveChangesAsync();

						Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (transferInStock != null)
						{
							transferInStock.SyncErrorCount++;

							_context.Update(transferInStock);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferInStockResult.Meta.Code}!" };
					}


				}
				catch (Exception e)
				{
					//Console.Write("Error", ConsoleColor.Red);
					//Console.Write(e.Message, ConsoleColor.DarkRed);

					//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(e.StackTrace);
					//	errorfile.WriteLine(e.ToString());

					//};

					//return false;

					Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (transferInStock != null)
					{
						transferInStock.SyncErrorCount++;

						_context.Update(transferInStock);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}

		}

		public async Task<Model.CreateAssetSAPResult> TransferStornoInStockAsync(TransferInStockInput asset)
		{
			HttpClient clientContract = null;

			var baseUrl = _BASEURL;
			TransferInStockResult transferInStockResult = null;
			string result = "";
			using (clientContract = new HttpClient())
			{
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();


				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "CREATEASSETSTOCK").Single();
				Model.Inventory inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(i => i.Active == true).Single();
				Model.Asset assetToUpdate = _context.Set<Model.Asset>().Include(d => d.Document).Where(a => a.InvNo == asset.Asset).Single();


				IList<Dto.TransferInStockData> oIList1 = new List<Dto.TransferInStockData>
				{
					new Dto.TransferInStockData()
						{
							I_INPUT = new Dto.TransferInStockInput()
								{
									Doc_Date = asset.Doc_Date,
									Pstng_Date = asset.Pstng_Date,
									Material = asset.Material,
									Plant = asset.Plant,
									Storage_Location = asset.Storage_Location,
									Quantity = asset.Quantity,
									Uom = asset.Uom,
									Batch = asset.Batch,
                                    // Gl_Account = asset[0].Gl_Account.Length == 6 ? "00" + asset[0].Gl_Account : asset[0].Gl_Account,
                                    Gl_Account = "",
									Item_Text = asset.Item_Text,
									Asset = asset.Asset,
									SubNumber = "0000",
									Ref_Doc_No = asset.Ref_Doc_No,
									Header_Txt = asset.Header_Txt,
									Storno = asset.Storno,
									Storno_Date = asset.Storno_Date,
									Storno_Doc = asset.Storno_Doc,
									Storno_Year = asset.Storno_Year,
									Storno_User = asset.Storno_User
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
					Model.Error error3 = new();

					error3.AssetId = assetToUpdate.Id;
					error3.ErrorTypeId = errorType.Id;
					error3.CreatedAt = DateTime.Now;
					error3.CreatedBy = _context.UserId;
					error3.ModifiedAt = DateTime.Now;
					error3.ModifiedBy = _context.UserId;
					error3.Code = "BEFORE-STOR-TRANSFER-IN-STOCK";
					error3.Name = JsonConvert.SerializeObject(asset, Formatting.Indented).ToString();
					error3.UserId = _context.UserId;
					error3.IsDeleted = false;
					error3.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();

					await _context.AddAsync(error3);
					await _context.SaveChangesAsync();

					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					clientContract.Dispose();

					if (result != "")
					{
						Model.Error errorResponse3 = new();

						errorResponse3.AssetId = assetToUpdate.Id;
						errorResponse3.ErrorTypeId = errorType.Id;
						errorResponse3.CreatedAt = DateTime.Now;
						errorResponse3.CreatedBy = _context.UserId;
						errorResponse3.ModifiedAt = DateTime.Now;
						errorResponse3.ModifiedBy = _context.UserId;
						errorResponse3.Code = "R-SAP-AFTER-STOR-TRA-IN-STOCK";
						errorResponse3.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse3.UserId = _context.UserId;
						errorResponse3.IsDeleted = false;
						errorResponse3.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();

						await _context.AddAsync(errorResponse3);
						await _context.SaveChangesAsync();

						try
						{
							transferInStockResult = JsonConvert.DeserializeObject<TransferInStockResult>(result);

							if (transferInStockResult.Data != null && transferInStockResult.Data.Return_Code == "1")
							{
								Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.Storno == "X" && a.SyncErrorCount < 3 && a.IsDeleted == false).SingleOrDefault();

								if (transferInStock != null)
								{
									transferInStock.SyncErrorCount = 0;
									transferInStock.NotSync = false;
									_context.Update(transferInStock);

								}

								assetToUpdate.ModifiedAt = DateTime.Now;
								assetToUpdate.IsDeleted = true;

								_context.Update(assetToUpdate);
								_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP:  ${transferInStockResult.Data.Return_Code + "|" + transferInStockResult.Data.Return_Message}!" };
							}
							else
							{
								if (transferInStockResult.Meta.Code == 400)
								{
									Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

									if (transferInStock != null)
									{
										transferInStock.SyncErrorCount++;

										_context.Update(transferInStock);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferInStockResult.Meta.Code}!" };
							}
						}
						catch (Exception ex)
						{
							Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

							if (transferInStock != null)
							{
								transferInStock.SyncErrorCount++;

								_context.Update(transferInStock);
								_context.SaveChanges();
							}

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferInStockResult.Meta.Code}!" };
						}
					}
					else
					{
						//Model.Error errorResponse5 = new();

						//errorResponse5.AssetId = assetId;
						//errorResponse5.ErrorTypeId = errorType.Id;
						//errorResponse5.CreatedAt = DateTime.Now;
						//errorResponse5.CreatedBy = _context.UserId;
						//errorResponse5.ModifiedAt = DateTime.Now;
						//errorResponse5.ModifiedBy = _context.UserId;
						//errorResponse5.Code = "ER-SAP-ASSET-CREATE-FROM-STOCK";
						//errorResponse5.Name = "EMPTY-RESPONSE";
						//errorResponse5.UserId = _context.UserId;
						//errorResponse5.IsDeleted = false;

						//await _context.AddAsync(errorResponse5);
						//await _context.SaveChangesAsync();

						Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

						if (transferInStock != null)
						{
							transferInStock.SyncErrorCount++;

							_context.Update(transferInStock);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: {transferInStockResult.Meta.Code}!" };
					}


				}
				catch (Exception e)
				{
					//Console.Write("Error", ConsoleColor.Red);
					//Console.Write(e.Message, ConsoleColor.DarkRed);

					//using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(e.StackTrace);
					//	errorfile.WriteLine(e.ToString());

					//};

					//return false;

					Model.TransferInStockSAP transferInStock = _context.Set<Model.TransferInStockSAP>().Where(a => a.AssetStockId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 5 && a.IsDeleted == false).SingleOrDefault();

					if (transferInStock != null)
					{
						transferInStock.SyncErrorCount++;

						_context.Update(transferInStock);
						_context.SaveChanges();
					}

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}

		}

		public async Task<Model.CreateAssetSAPResult> AssetChangeAsync(AssetChangeSAP asset, int? assetSyncErrorId = null)
		{
			HttpClient clientContract = null;

			var baseUrl = _BASEURL;
			AssetChangeResult changeAssetResult = null;
			string result = "";
			using (clientContract = new HttpClient())
			{
				var scope = Services.CreateScope();

				var _context =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.Asset assetToUpdate = _context.Set<Model.Asset>().Include(c => c.AssetChangeSAP).Where(a => a.InvNo == asset.ASSET && a.SubNo == asset.SUBNUMBER).LastOrDefault();
				Model.Error error = new();
				Model.ErrorType errorType = _context.Set<Model.ErrorType>().AsNoTracking().Where(e => e.Code == "CHANGEASSET").Single();

				IList<Dto.AssetChangeData> oIList1 = new List<Dto.AssetChangeData>
				{
					new Dto.AssetChangeData()
						{
							I_INPUT = new Dto.AssetChangeInput()
								{
									COMPANYCODE = asset.COMPANYCODE,
									ASSET = asset.ASSET,
									SUBNUMBER = asset.SUBNUMBER,
									ASSETCLASS = asset.ASSETCLASS,
									POSTCAP = asset.POSTCAP,
									DESCRIPT = asset.DESCRIPT,
									DESCRIPT2 = asset.DESCRIPT2,
									INVENT_NO = asset.INVENT_NO,
									SERIAL_NO = asset.SERIAL_NO,
									QUANTITY = asset.QUANTITY,
									BASE_UOM = asset.BASE_UOM,
									LAST_INVENTORY_DATE = asset.LAST_INVENTORY_DATE,
									LAST_INVENTORY_DOCNO = asset.LAST_INVENTORY_DOCNO,
									CAP_DATE = asset.CAP_DATE,
									COSTCENTER = asset.COSTCENTER,
									INTERN_ORD = asset.INTERN_ORD,
									PLANT = asset.PLANT,
									LOCATION = asset.LOCATION,
									ROOM = asset.ROOM,
									PERSON_NO = asset.PERSON_NO != "VIRTUAL" ? asset.PERSON_NO : "",
									PLATE_NO = asset.PLATE_NO,
									ZZCLAS = asset.ZZCLAS,
									IN_CONSERVATION = asset.IN_CONSERVATION,
									PROP_IND = asset.PROP_IND,
									OPTIMA_ASSET_NO = asset.OPTIMA_ASSET_NO,
									OPTIMA_ASSET_PARENT_NO = asset.OPTIMA_ASSET_PARENT_NO,
									VENDOR_NO = asset.VENDOR_NO,
									RESP_CCTR = asset.RESP_CCTR,
									FROM_DATE = asset.FROM_DATE,
									INVOICE  = asset.INVOICE,
									//DOC_YEAR = asset.DOC_YEAR,
									//MAT_DOC = asset.MAT_DOC,
									//WBS_ELEMENT = asset.WBS_ELEMENT
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

#if DEBUG
                string jsonString = await contentJson.ReadAsStringAsync();

                string resultFilePath = System.IO.Path.Combine("SAP-RESULTS", "ASSETCHANGESAP_" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(resultFilePath))
                {
                    errorfile.WriteLine(jsonString);

                };
#endif

                clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

				try
				{

					//using (var errorfile = System.IO.File.CreateText("contentJsonTransfer" + DateTime.Now.Ticks + ".txt"))
					//{
					//	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

					//};


					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					clientContract.Dispose();

					if (result != "")
					{
						Model.Error errorResponse = new();

						errorResponse.AssetId = assetToUpdate.Id;
						errorResponse.ErrorTypeId = errorType.Id;
						errorResponse.CreatedAt = DateTime.Now;
						errorResponse.CreatedBy = _context.UserId;
						errorResponse.ModifiedAt = DateTime.Now;
						errorResponse.ModifiedBy = _context.UserId;
						errorResponse.Code = "RESULT-ASSET-CHANGE";
						errorResponse.Request = JsonConvert.SerializeObject(oIList1, Formatting.Indented).ToString();
						errorResponse.Name = JsonConvert.SerializeObject(result, Formatting.Indented).ToString();
						errorResponse.UserId = _context.UserId;
						errorResponse.IsDeleted = false;

						_context.Add(errorResponse);
						_context.SaveChanges();

                       var changeAssetResultNew = JsonConvert.DeserializeObject<CreateAssetResult>(result);

						if (assetSyncErrorId != null)
						{
							var assetSyncErrorToUpdate = _context.AssetSyncErrors.FirstOrDefault(a => a.Id == assetSyncErrorId);

							if (assetSyncErrorToUpdate != null)
							{
								if (changeAssetResultNew.Meta.Code == 200)
								{
									assetSyncErrorToUpdate.SyncStatus = "Synchronization Success";
									assetSyncErrorToUpdate.Error = errorResponse.Id;

									_context.SaveChanges();
								}
								if (changeAssetResultNew.Meta.Code == 400)
								{
									assetSyncErrorToUpdate.SyncStatus = "Synchronization Fail";
									assetSyncErrorToUpdate.Error = errorResponse.Id;

									_context.SaveChanges();
								}
								else
								{

								}
							}
						}
                            try
						{
							changeAssetResult = JsonConvert.DeserializeObject<AssetChangeResult>(result);

							if (changeAssetResult.Data != null && changeAssetResult.Data.Return_Code == "1")
							{
								Model.AssetChangeSAP createAssetSAP = _context.Set<Model.AssetChangeSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 3 && a.IsDeleted == false).OrderBy(a => a.Id).FirstOrDefault();

								if (createAssetSAP != null)
								{
									createAssetSAP.NotSync = false;
									createAssetSAP.SyncErrorCount = 0;

									createAssetSAP.Error = errorResponse;

									_context.Update(createAssetSAP);
								}

								assetToUpdate.ModifiedAt = DateTime.Now;
								//assetToUpdate.AssetStateId = 1;
								assetToUpdate.IsInTransfer = false;
								assetToUpdate.NotSync = false;

								_context.Update(assetToUpdate);
								_context.SaveChanges();

								return new Model.CreateAssetSAPResult { Success = true, ErrorMessage = $"SAP: Numarul de inventar {assetToUpdate.InvNo}/{assetToUpdate.SubNo} a fost modificat cu succes!" };
							}
							else
							{
								if (changeAssetResult.Meta.Code == 400)
								{
									Model.AssetChangeSAP createAssetSAP = _context.Set<Model.AssetChangeSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 3 && a.IsDeleted == false).OrderBy(a => a.Id).FirstOrDefault();

									if (createAssetSAP != null)
									{
										createAssetSAP.SyncErrorCount++;
										createAssetSAP.Error = errorResponse;

										assetToUpdate.ModifiedAt = DateTime.Now;
										// assetToUpdate.AssetStateId = 1;
										assetToUpdate.IsInTransfer = false;
										assetToUpdate.NotSync = false;

										_context.Update(assetToUpdate);

										_context.Update(createAssetSAP);
										_context.SaveChanges();
									}
								}

								return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: !" };

							}
						}
						catch (Exception ex)
						{
							// var assetToDelete = _context.Set<Model.Asset>().Where(a => a.Id == assetToUpdate.Id).Single();

							//assetToDelete.IsDeleted = true;
							//_context.Update(assetToDelete);

							Model.AssetChangeSAP createAssetSAP = _context.Set<Model.AssetChangeSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 3 && a.IsDeleted == false).OrderBy(a => a.Id).FirstOrDefault();

							if (createAssetSAP != null)
							{
								createAssetSAP.SyncErrorCount++;
								createAssetSAP.Error = errorResponse;

								_context.Update(createAssetSAP);
								_context.SaveChanges();
							}

							return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: !" };
						}
					}
					else
					{

						Model.AssetChangeSAP createAssetSAP = _context.Set<Model.AssetChangeSAP>().Where(a => a.AssetId == assetToUpdate.Id && a.NotSync == true && a.SyncErrorCount < 3 && a.IsDeleted == false).OrderBy(a => a.Id).FirstOrDefault();

						if (createAssetSAP != null)
						{
							createAssetSAP.SyncErrorCount++;
							// createAssetSAP.Error = errorResponse;

							_context.Update(createAssetSAP);
							_context.SaveChanges();
						}

						return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: !" };
					}


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

					return new Model.CreateAssetSAPResult { Success = false, ErrorMessage = $"SAP: Lipsa conexiune!" };
				}
			}

		}

		public async Task<string> CheckAssetStock(string categoryID, string material, string batch)
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
					var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

					result = await httpResponse.Content.ReadAsStringAsync();

					clientContract.Dispose();

					return result;

				}
				catch (Exception e)
				{
					return result;
				}
			}
		}

		//public async Task<bool> BlockAsset(int assetId)
		//{

		//	BlockAssetResult blockAssetResult = null;
		//	bool success = false;

		//	if (assetId > 0)
		//	{
		//		var scope = Services.CreateScope();

		//		var _context =
		//		   scope.ServiceProvider
		//			   .GetRequiredService<ApplicationDbContext>();

		//		var result = await this.BlockAssetAsync(assetId);

		//		if (result != "")
		//		{

		//			using (var errorfile = System.IO.File.CreateText("error-block-ASSET" + DateTime.Now.Ticks + ".txt"))
		//			{
		//				errorfile.WriteLine(result);

		//			};

		//			try
		//			{
		//				blockAssetResult = JsonConvert.DeserializeObject<BlockAssetResult>(result);

		//				if (blockAssetResult.Data.Return_Code == "1)
		//				{
		//					Model.Asset assetUpdate = _context.Set<Model.Asset>().Where(a => a.Id == assetId).Single();

		//					assetUpdate.AssetStateId = 7;
		//					assetUpdate.ModifiedAt = DateTime.Now;
		//					_context.Update(assetUpdate);
		//					_context.SaveChanges();

		//					success = true;
		//				}
		//			}
		//			catch (Exception ex)
		//			{

		//				using (var errorfile = System.IO.File.CreateText("error-block-asset-" + DateTime.Now.Ticks + ".txt"))
		//				{
		//					errorfile.WriteLine(ex.StackTrace);
		//					errorfile.WriteLine(ex.ToString());

		//				};
		//			}



		//			return success;
		//		}
		//		else
		//		{

		//			using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
		//			{
		//				errorfile.WriteLine(result);
		//				errorfile.WriteLine(result);

		//			};

		//			return success;
		//		}


		//	}


		//	return success;
		//}

		//public async Task<string> BlockAssetAsync(int assetId)
		//{
		//	var scope = Services.CreateScope();

		//	var _context =
		//	   scope.ServiceProvider
		//		   .GetRequiredService<ApplicationDbContext>();

		//	Model.Asset asset = await _context.Set<Model.Asset>().Include(c => c.Company).Where(a => a.Id == assetId).SingleAsync();
		//	BlockAssetResult blockAssetResult = null;
		//	HttpClient clientContract = null;

		//	var baseUrl = _BASEURL;
		//	// CreateAssetResult result = null;
		//	string result = "";
		//	using (clientContract = new HttpClient())
		//	{
		//		IList<Dto.BlockAssetData> oIList1 = new List<Dto.BlockAssetData>
		//		{
		//			new Dto.BlockAssetData()
		//				{
		//					I_INPUT = new Dto.BlockAssetInput()
		//						{
		//							COMPANYCODE = asset.Company.Code,
		//							ASSET = asset.InvNo,
		//							SUBNUMBER = asset.SubNo.ToString(),
		//							BLOCK = "X",
		//							OPTIMA_ASSET_NO = asset.InvNo,
		//							OPTIMA_ASSET_PARENT_NO = asset.InvNo
		//						}
		//				 }
		//		};

		//		var postUser = new Dto.BlockAsset
		//		{
		//			Sap_function = "ZFIF_FIXED_ASSET_BLOCK",
		//			Options = new Dto.BlockAssetDataOptions()
		//			{
		//				Api_call_timeout = 180
		//			},
		//			Remote_host_name = "test",
		//			Data = oIList1
		//		};

		//		JsonContent contentJson = JsonContent.Create(postUser);
		//		clientContract.DefaultRequestHeaders.Add("SAP-PROXY-AUTH-TOKEN", _TOKEN);

		//		try
		//		{

		//			using (var errorfile = System.IO.File.CreateText("contentJsonTransfer" + DateTime.Now.Ticks + ".txt"))
		//			{
		//				errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(contentJson, Formatting.Indented));

		//			};


		//			var httpResponse = await clientContract.PostAsync(baseUrl, contentJson);

		//			result = await httpResponse.Content.ReadAsStringAsync();

		//			clientContract.Dispose();

		//			//result = JsonConvert.DeserializeObject<CreateAssetResult>(content);


		//		}
		//		catch (Exception e)
		//		{
		//			Console.Write("Error", ConsoleColor.Red);
		//			Console.Write(e.Message, ConsoleColor.DarkRed);

		//			using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
		//			{
		//				errorfile.WriteLine(e.StackTrace);
		//				errorfile.WriteLine(e.ToString());

		//			};
		//		}

		//		return result;
		//	}

		//}
	}
}


