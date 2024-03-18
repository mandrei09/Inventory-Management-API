using System;
using Optima.Fais.Data;
using Optima.Fais.Repository;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Optima.Fais.Model;
using Optima.Fais.Dto;

namespace Optima.Fais.EfRepository
{
    public class DocumentsRepository : Repository<Model.Document>, IDocumentsRepository
    {
        public DocumentsRepository(ApplicationDbContext context)
            : base(context, (filter) => { return (d) => (d.DocNo1.Contains(filter) || d.DocNo2.Contains(filter)); })
        { }

        public async Task<Model.TransferResult> PersonelValidate(Dto.PersonelValidate data)
        {
            Model.Asset asset = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.AssetOp assetOp = null;
            Model.Inventory inventory = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.AppState dstEmployeeAppState = null;
            //Model.AppState dstManagerAppState = null;
            Model.AppState newAppState = null;
            Model.AssetChangeSAP assetChangeSAP = null;
            Model.EmailStatus emailStatus = null;
            Model.Employee empFinal = null;

            if (data != null)
            {
                dstEmployeeAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
                if (dstEmployeeAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

                //dstManagerAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_MANAGER_VALIDATE").FirstOrDefaultAsync();
                //if (dstManagerAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare manager" };

                newAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_VALIDATE").FirstOrDefaultAsync();
                if (newAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare final" };

                inventory = await _context.Set<Model.Inventory>().Where(d => d.Active == true).FirstOrDefaultAsync();
                if (inventory == null) return new Model.TransferResult { Success = false, Message = "Nu exista inventar activ" };

                var date = DateTime.Now.ToString("yyyyMMdd");

                for (int i = 0; i < data.AssetIds.Count; i++)
                {
                    asset = await _context.Set<Model.Asset>()
                             .Include(a => a.AppState)
                             .Include(c => c.Company)
							 .Include(c => c.Stock)
							 .Include(c => c.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(b => b.Project)
							 .Include(a => a.AssetCategory)
                             .Include(a => a.ExpAccount)
                             .Include(a => a.Document).ThenInclude(p => p.Partner)
                         .Where(a => a.Id == data.AssetIds[i] && a.IsInTransfer == true).FirstOrDefaultAsync();
                    if (asset == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intr - un flux de tranfer" };

                    emailStatus = await _context.Set<Model.EmailStatus>()
                        .Include(a => a.AppState)
                        .Include(a => a.EmailType)
                        .Include(a => a.CostCenterInitial)
                        .Include(a => a.CostCenterFinal)
                        .Include(a => a.EmployeeInitial).ThenInclude(c => c.CostCenter)
                        .Include(a => a.EmployeeFinal).ThenInclude(c => c.CostCenter)
                        .Where(a => a.AssetId == data.AssetIds[i] && a.IsDeleted == false && a.AppStateId == dstEmployeeAppState.Id && a.EmailType.Code == "TRANSFER").FirstOrDefaultAsync();
                    if (emailStatus == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intr - un flux de tranfer" };

                    assetOp = await _context.Set<Model.AssetOp>().Where(a => a.Id == emailStatus.AssetOpId).FirstOrDefaultAsync();
                    if (assetOp == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu nu are operatie de transfer!" };

					empFinal = await _context.Set<Model.Employee>().Include(a => a.CostCenter).Where(a => a.Id == emailStatus.EmployeeIdFinal).FirstOrDefaultAsync();
					if (empFinal == null) return new Model.TransferResult { Success = false, Message = "Nu exista utilizator final" };

                    if(asset.Name.ToUpper().Contains("PHONE") && asset.FirstTransfer)
                    {
                        asset.PhoneNumber = data.PhoneNumber;

					}

					asset.EmployeeId = emailStatus.EmployeeIdFinal;
                    //asset.EmployeeTransferId = null;
                    asset.ModifiedAt = DateTime.Now;
                    asset.ModifiedBy = _context.UserId;
                    asset.CostCenterId = emailStatus.CostCenterIdFinal;

                    Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

                    asset.DepartmentId = costCenter.Division.DepartmentId;
                    asset.DivisionId = costCenter.DivisionId;

                    asset.IsLocked = false;
                    asset.AssetStateId = 1;


                    asset.AppStateId = null;
                    asset.IsInTransfer = false;
                    asset.FirstTransfer = false;
                    asset.EmployeeTransferId = null;

                    _context.Update(asset);

                    assetAdmMD = await _context.Set<Model.AssetAdmMD>().Where(a => a.AccMonthId == inventory.AccMonthId && a.AssetId == asset.Id).FirstOrDefaultAsync();

                    assetAdmMD.EmployeeId = emailStatus.EmployeeIdFinal;
                    assetAdmMD.CostCenterId = emailStatus.CostCenterIdFinal;

                    costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetAdmMD.CostCenterId).SingleOrDefault();

                    assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                    assetAdmMD.DivisionId = costCenter.DivisionId;

                    assetAdmMD.AssetStateId = 1;
                    _context.Update(assetAdmMD);



                    assetOp.AssetOpStateId = newAppState.Id;
                    assetOp.DstConfAt = DateTime.Now;
                    assetOp.DstConfBy = _context.UserId;
                    assetOp.ModifiedAt = DateTime.Now;
                    assetOp.ModifiedBy = _context.UserId;

                    _context.Update(assetOp);

                    emailStatus.AppStateId = newAppState.Id;
                    emailStatus.DstEmployeeValidateAt = DateTime.Now;
                    emailStatus.DstEmployeeValidateBy = _context.UserId;


                    emailStatus.DstManagerEmailSend = false;
                    emailStatus.NotDstManagerSync = false;

                    emailStatus.ModifiedAt = DateTime.Now;
                    emailStatus.ModifiedBy = _context.UserId;

                    emailStatus.NotSync = true;
                    emailStatus.SyncErrorCount = 0;
                    emailStatus.NotCompletedSync = true;

                    inventoryAsset = await _context.Set<Model.InventoryAsset>().Where(a => a.InventoryId == inventory.Id && a.AssetId == asset.Id).FirstOrDefaultAsync();

                    if(inventoryAsset != null)
                    {
                        

                        if(inventoryAsset.CostCenterIdFinal != null)
                        {
                            inventoryAsset.EmployeeIdFinal = emailStatus.EmployeeIdFinal;
                            inventoryAsset.CostCenterIdFinal = emailStatus.CostCenterIdFinal;
                        }
                        else
                        {
                            inventoryAsset.EmployeeIdInitial = emailStatus.EmployeeIdFinal;
                            inventoryAsset.CostCenterIdInitial = emailStatus.CostCenterIdFinal;
                        }

                        _context.Update(inventoryAsset);
                    }

                    var names = SplitToLines(asset.Name, 50);
                    var countNames = names.Count();

                    assetChangeSAP = new Model.AssetChangeSAP()
                    {
                        COMPANYCODE = asset.Company.Code,
                        ASSET = asset.InvNo,
                        SUBNUMBER = asset.SubNo == "0" ? "0000" : asset.SubNo,
                        ASSETCLASS = asset.ExpAccount.Name,
                        POSTCAP = "",
                        DESCRIPT = countNames > 0 ? names.ElementAt(0) : "",
                        DESCRIPT2 = countNames > 1 ? names.ElementAt(1) : "",
                        INVENT_NO = asset.ERPCode,
                        SERIAL_NO = asset.SerialNumber,
                        QUANTITY = (int)asset.Quantity,
                        BASE_UOM = "ST",
                        LAST_INVENTORY_DATE = "00000000",
                        LAST_INVENTORY_DOCNO = "",
                        CAP_DATE = "00000000",
                        COSTCENTER = emailStatus.CostCenterFinal.Code,
                        RESP_CCTR = empFinal != null && empFinal.CostCenter != null ? empFinal.CostCenter.Code : emailStatus.CostCenterFinal.Code,
                        INTERN_ORD = "",
                        PLANT = "RO02",
                        LOCATION = "",
                        ROOM = "",
                        PERSON_NO = emailStatus.EmployeeFinal.InternalCode,
                        PLATE_NO = asset.AgreementNo != null ? asset.AgreementNo : "",
                        ZZCLAS = asset.AssetCategory.Code,
                        IN_CONSERVATION = "",
                        PROP_IND = "1",
                        OPTIMA_ASSET_NO = "",
                        OPTIMA_ASSET_PARENT_NO = "",
                        VENDOR_NO = asset.Document.Partner.RegistryNumber,
                        FROM_DATE = date,
                        AccMonthId = inventory.AccMonthId.Value,
                        AssetId = asset.Id,
                        BudgetManagerId = inventory.BudgetManagerId.Value,
                        NotSync = true,
                        CreatedAt = DateTime.Now,
                        CreatedBy = _context.UserId,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
						INVOICE = asset.Stock != null && asset.Stock.Invoice != null ? asset.Stock.Invoice : (asset.Document.DocNo1 != null ? asset.Document.DocNo1 : ""),
						DOC_YEAR = "00000000",
                        MAT_DOC = "",
                        WBS_ELEMENT = asset.BudgetForecast != null && asset.BudgetForecast.BudgetBase != null && asset.BudgetForecast.BudgetBase.Project != null ? asset.BudgetForecast.BudgetBase.Project.Code : ""
                    };
                    _context.Add(assetChangeSAP);
                    _context.SaveChanges();
                }
                return new Model.TransferResult { Success = true, Message = "Transferul a fost salvat cu success!" };
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
            }
        }

        public async Task<Model.TransferResult> ManagerValidate(Dto.PersonelValidate data)
        {
            Model.Asset asset = null;
            Model.AssetOp assetOp = null;
            Model.Inventory inventory = null;
            Model.AppState dstEmployeeAppState = null;
            Model.AppState dstManagerAppState = null;
            Model.AppState newAppState = null;
            Model.EmailStatus emailStatus = null;

            if (data != null)
            {
                dstEmployeeAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
                if (dstEmployeeAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

                dstManagerAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_MANAGER_VALIDATE").FirstOrDefaultAsync();
                if (dstManagerAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare manager" };

                newAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_VALIDATE").FirstOrDefaultAsync();
                if (newAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare final" };

                inventory = await _context.Set<Model.Inventory>().Where(d => d.Active == true).FirstOrDefaultAsync();
                if (inventory == null) return new Model.TransferResult { Success = false, Message = "Nu exista inventar activ" };

                var date = DateTime.Now.ToString("yyyyMMdd");

                for (int i = 0; i < data.AssetIds.Count; i++)
                {
                    asset = await _context.Set<Model.Asset>()
                             .Include(a => a.AppState)
                             .Include(c => c.Company)
                             .Include(a => a.AssetCategory)
                             .Include(a => a.ExpAccount)
                             .Include(a => a.Document).ThenInclude(p => p.Partner)
                         .Where(a => a.Id == data.AssetIds[i]).FirstOrDefaultAsync();
                    if (asset == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intr - un flux de transfer" };

                    emailStatus = await _context.Set<Model.EmailStatus>()
                        .Include(a => a.AppState)
                        .Include(a => a.EmailType)
                        .Include(a => a.CostCenterInitial)
                        .Include(a => a.CostCenterFinal)
                        .Include(a => a.EmployeeInitial).ThenInclude(c => c.CostCenter)
                        .Include(a => a.EmployeeFinal).ThenInclude(c => c.CostCenter)
                        .Where(a => a.AssetId == data.AssetIds[i] && a.IsDeleted == false && a.AppStateId == dstManagerAppState.Id && a.EmailType.Code == "TRANSFER").FirstOrDefaultAsync();
                    if (emailStatus == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intr - un flux de tranfer" };

                    assetOp = await _context.Set<Model.AssetOp>().Where(a => a.Id == emailStatus.AssetOpId).FirstOrDefaultAsync();
                    if (assetOp == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu nu are operatie de transfer!" };

                    asset.ModifiedAt = DateTime.Now;
                    asset.ModifiedBy = _context.UserId;
                    asset.AppStateId = null;
                    asset.IsInTransfer = false;
					asset.FirstTransfer = false;
					asset.EmployeeTransferId = null;
                    //asset.IsLocked = true;

                    _context.Update(asset);

                    assetOp.AssetOpStateId = newAppState.Id;
                    assetOp.ModifiedAt = DateTime.Now;
                    assetOp.ModifiedBy = _context.UserId;

                    _context.Update(assetOp);

                    emailStatus.DstManagerValidateAt = DateTime.Now;
                    emailStatus.DstManagerValidateBy = _context.UserId;
                    emailStatus.NotSync = true;
                    emailStatus.SyncErrorCount = 0;
                    emailStatus.NotCompletedSync = true;

                    emailStatus.NotDstManagerSync = false;
                    emailStatus.AppStateId = newAppState.Id;


                    _context.Update(emailStatus);

                    _context.SaveChanges();
                }

                return new Model.TransferResult { Success = true, Message = "Transferul a fost salvat cu success!" };
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
            }
        }

        public async Task<Model.TransferResult> Validate(Dto.DocumentUpload documentUpload)
        {
            Model.Asset assetOld = null;
            Model.Asset asset = null;
            Model.AssetAdmMD assetAdmMD = null;
            Model.AssetOp assetOp = null;
            Model.InventoryAsset inventoryAsset = null;
            Model.Inventory inventory = null;
            Model.AppState dstEmployeeAppState = null;
            Model.AppState dstManagerAppState = null;
			Model.AppState stockAppState = null;
			Model.AppState newAppState = null;
            Model.AssetChangeSAP assetChangeSAP = null;
            bool managerValidate = false;

            if (documentUpload.Operations != null)
            {
                dstEmployeeAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
                if (dstEmployeeAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

				stockAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
				if (stockAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

				dstManagerAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_MANAGER_VALIDATE").FirstOrDefaultAsync();
                if (dstManagerAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare manager" };

                newAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_VALIDATE").FirstOrDefaultAsync();
                if (newAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare final" };

                inventory = await _context.Set<Model.Inventory>().Where(d => d.Active == true).FirstOrDefaultAsync();
                if (inventory == null) return new Model.TransferResult { Success = false, Message = "Nu exista inventar activ" };

                var date = DateTime.Now.ToString("yyyyMMdd");

                List<Model.EmailStatus> emailStatuses = new List<Model.EmailStatus>();

                foreach (Dto.OperationUpload op in documentUpload.Operations)
                {
                    assetOld = await _context.Set<Model.Asset>().Include(a => a.AppState).Where(a => a.Id == op.AssetId && a.IsInTransfer == true).SingleOrDefaultAsync();
                    if (assetOld == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intru - un flux de tranfer" };

                    if (assetOld.AppState != null && ((assetOld.AppState.Code == dstEmployeeAppState.Code) || (assetOld.AppState.Code == stockAppState.Code)))
                    {
                        emailStatuses = await _context.Set<Model.EmailStatus>()
                          .Include(e => e.EmailType)
                          .Include(c => c.CostCenterFinal)
                          .Include(c => c.EmployeeFinal).ThenInclude(c => c.CostCenter)
                          .Where(a => a.AssetId == op.AssetId && a.IsDeleted == false && (a.AppStateId == dstEmployeeAppState.Id || a.AppStateId == stockAppState.Id)).ToListAsync();
                    }
                    else
                    {
                        //if (assetOld.AppState != null && assetOld.AppState.Code == "STOCK_VALIDATE")
                        //{

                        //}
                        //else
                        //{
                        //    emailStatuses = await _context.Set<Model.EmailStatus>()
                        //     .Include(e => e.EmailType)
                        //     .Include(c => c.CostCenterFinal)
                        //     .Include(c => c.EmployeeFinal).ThenInclude(c => c.CostCenter)
                        //     .Where(a => a.Guid == assetOld.Guid && a.IsDeleted == false && a.AppStateId == dstManagerAppState.Id).ToListAsync();
                        //}

						emailStatuses = await _context.Set<Model.EmailStatus>()
							 .Include(e => e.EmailType)
							 .Include(c => c.CostCenterFinal)
							 .Include(c => c.EmployeeFinal).ThenInclude(c => c.CostCenter)
							 .Where(a => a.Guid == assetOld.Guid && a.IsDeleted == false && a.AppStateId == dstManagerAppState.Id).ToListAsync();

					}


                    for (int i = 0; i < emailStatuses.Count; i++)
                    {
                        if (emailStatuses[i] != null && emailStatuses[i].EmailType.Code == "TRANSFER")
                        {
                            asset = await _context.Set<Model.Asset>()
                                .Include(a => a.AppState)
                                .Include(c => c.Company)
                                .Include(a => a.AssetCategory)
                                .Include(a => a.ExpAccount)
								.Include(c => c.Stock)
							    .Include(c => c.BudgetForecast).ThenInclude(b => b.BudgetBase).ThenInclude(b => b.Project)
								.Include(a => a.Document).ThenInclude(p => p.Partner)
                                .Where(a => a.Id == emailStatuses[i].AssetId).SingleAsync();

                            assetOp = await _context.Set<Model.AssetOp>().Where(a => a.Id == emailStatuses[i].AssetOpId).SingleAsync();

							managerValidate = true;

							asset.EmployeeId = asset.EmployeeTransferId;

							asset.ModifiedAt = DateTime.Now;
							asset.ModifiedBy = _context.UserId;
							asset.AppStateId = dstManagerAppState.Id;
							asset.CostCenterId = emailStatuses[i].CostCenterIdFinal;

                            Model.CostCenter costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == asset.CostCenterId).SingleOrDefault();

                            asset.DepartmentId = costCenter.Division.DepartmentId;
                            asset.DivisionId = costCenter.DivisionId;

                            asset.EmployeeTransferId = null;
							asset.IsLocked = true;

							_context.Update(asset);

							assetAdmMD = _context.Set<Model.AssetAdmMD>().Where(a => (a.AccMonthId == inventory.AccMonthId) && a.AssetId == asset.Id).SingleOrDefault();

							assetAdmMD.EmployeeId = emailStatuses[i].EmployeeIdFinal;
							assetAdmMD.CostCenterId = emailStatuses[i].CostCenterIdFinal;

                            costCenter = _context.Set<Model.CostCenter>().Include(c => c.Division).Where(c => c.Id == assetAdmMD.CostCenterId).SingleOrDefault();

                            assetAdmMD.DepartmentId = costCenter.Division.DepartmentId;
                            assetAdmMD.DivisionId = costCenter.DivisionId;

                            _context.Update(assetAdmMD);



							assetOp.AssetOpStateId = dstManagerAppState.Id;
							assetOp.DstConfAt = DateTime.Now;
							assetOp.DstConfBy = _context.UserId;
							assetOp.ModifiedAt = DateTime.Now;
							assetOp.ModifiedBy = _context.UserId;

							_context.Update(assetOp);


							inventoryAsset = await _context.Set<Model.InventoryAsset>().Where(a => (a.AssetId == op.AssetId && a.InventoryId == inventory.Id)).SingleAsync();

							if (inventoryAsset.CostCenterIdFinal != null)
							{
								inventoryAsset.EmployeeIdFinal = emailStatuses[i].EmployeeIdFinal;
								inventoryAsset.CostCenterIdFinal = emailStatuses[i].CostCenterIdFinal;
								_context.Update(inventoryAsset);
							}
							else
							{
								inventoryAsset.EmployeeIdInitial = emailStatuses[i].EmployeeIdFinal;
								inventoryAsset.CostCenterIdInitial = emailStatuses[i].CostCenterIdFinal;
								_context.Update(inventoryAsset);
							}

							emailStatuses[i].AppStateId = dstManagerAppState.Id;
							emailStatuses[i].DstEmployeeValidateAt = DateTime.Now;
							emailStatuses[i].DstEmployeeValidateBy = _context.UserId;


							emailStatuses[i].DstManagerEmailSend = false;
							emailStatuses[i].NotDstManagerSync = true;

							emailStatuses[i].ModifiedAt = DateTime.Now;
							emailStatuses[i].ModifiedBy = _context.UserId;

							_context.Update(emailStatuses[i]);

							//if (asset.AppState.Code == dstManagerAppState.Code)
							//                     {
							//                         managerValidate = true;
							//                     }
							//                     else
							//                     {

							//                     }


							var names = SplitToLines(asset.Name, 50);
                            var countNames = names.Count();



                            if (managerValidate)
                            {
                                emailStatuses[i].DstManagerValidateAt = DateTime.Now;
                                emailStatuses[i].DstManagerValidateBy = _context.UserId;
                                emailStatuses[i].NotSync = true;
                                emailStatuses[i].SyncErrorCount = 0;
                                emailStatuses[i].NotCompletedSync = true;

                                emailStatuses[i].NotDstManagerSync = false;
                                emailStatuses[i].AppStateId = newAppState.Id;

                                asset.AppStateId = null;
                                asset.IsInTransfer = false;
                                assetOp.AssetOpStateId = newAppState.Id;
                            }

                            if (!asset.IsWFH)
                            {
                                assetChangeSAP = new Model.AssetChangeSAP()
                                {
                                    COMPANYCODE = asset.Company.Code,
                                    ASSET = asset.InvNo,
                                    SUBNUMBER = asset.SubNo == "0" ? "0000" : asset.SubNo,
                                    ASSETCLASS = asset.ExpAccount.Name,
                                    POSTCAP = "",
                                    DESCRIPT = countNames > 0 ? names.ElementAt(0) : "",
                                    DESCRIPT2 = countNames > 1 ? names.ElementAt(1) : "",
                                    INVENT_NO = asset.ERPCode,
                                    SERIAL_NO = asset.SerialNumber,
                                    QUANTITY = (int)asset.Quantity,
                                    BASE_UOM = "ST",
                                    LAST_INVENTORY_DATE = "00000000",
                                    LAST_INVENTORY_DOCNO = "",
                                    CAP_DATE = "00000000",
                                    COSTCENTER = emailStatuses[i].EmployeeFinal != null && emailStatuses[i].EmployeeFinal.CostCenter != null ? emailStatuses[i].EmployeeFinal.CostCenter.Code : emailStatuses[i].CostCenterFinal.Code,
                                    RESP_CCTR = emailStatuses[i].CostCenterFinal.Code,
                                    INTERN_ORD = "",
                                    PLANT = "RO02",
                                    LOCATION = "",
                                    ROOM = "",
                                    PERSON_NO = emailStatuses[i].EmployeeFinal.InternalCode,
                                    PLATE_NO = asset.AgreementNo != null ? asset.AgreementNo : "",
                                    ZZCLAS = asset.AssetCategory != null ? asset.AssetCategory.Code : string.Empty,
                                    IN_CONSERVATION = "",
                                    PROP_IND = "1",
                                    OPTIMA_ASSET_NO = "",
                                    OPTIMA_ASSET_PARENT_NO = "",
                                    VENDOR_NO = asset.Document.Partner.RegistryNumber,
                                    FROM_DATE = date,
                                    AccMonthId = inventory.AccMonthId.Value,
                                    AssetId = asset.Id,
                                    BudgetManagerId = inventory.BudgetManagerId.Value,
                                    NotSync = true,
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = _context.UserId,
                                    ModifiedAt = DateTime.Now,
                                    ModifiedBy = _context.UserId,
                                    INVOICE = asset.Stock != null && asset.Stock.Invoice != null ? asset.Stock.Invoice : (asset.Document.DocNo1 != null ? asset.Document.DocNo1 : ""),
                                    DOC_YEAR = "00000000",
                                    MAT_DOC = "",
                                    WBS_ELEMENT = asset.BudgetForecast != null && asset.BudgetForecast.BudgetBase != null && asset.BudgetForecast.BudgetBase.Project != null ? asset.BudgetForecast.BudgetBase.Project.Code : ""

                                };


                                _context.Add(assetChangeSAP);
                            }

                            
                            _context.SaveChanges();
                        }
                    }
                }

                return new Model.TransferResult { Success = true, Message = "Transferul a fost salvat cu success!" };
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
            }
        }

		public async Task<Model.TransferResult> Reject(Dto.DocumentUpload documentUpload)
		{
			Model.Asset assetOld = null;
			Model.Asset asset = null;
			Model.AssetOp assetOp = null;
			Model.Inventory inventory = null;
			Model.AppState dstEmployeeAppState = null;
            Model.AppState dstStockEmployeeAppState = null;
            Model.AppState newAppState = null;
			Guid guid = Guid.Empty;

			if (documentUpload.Operations != null)
			{
				dstEmployeeAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
				if (dstEmployeeAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

                dstStockEmployeeAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
                if (dstStockEmployeeAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

                newAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_DECLINED").FirstOrDefaultAsync();
				if (newAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare refuz angajat" };

				inventory = await _context.Set<Model.Inventory>().Where(d => d.Active == true).FirstOrDefaultAsync();
				if (inventory == null) return new Model.TransferResult { Success = false, Message = "Nu exista inventar activ" };

				var date = DateTime.Now.ToString("yyyyMMdd");

				List<Model.EmailStatus> emailStatuses = new List<Model.EmailStatus>();

				foreach (Dto.OperationUpload op in documentUpload.Operations)
				{
					assetOld = await _context.Set<Model.Asset>().Include(a => a.AppState).Where(a => a.Id == op.AssetId && a.IsInTransfer == true).SingleOrDefaultAsync();
					if (assetOld == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intru - un flux de tranfer" };

					if (assetOld.AppState != null && assetOld.AppState.Code == dstEmployeeAppState.Code)
					{
						guid = await _context.Set<Model.EmailStatus>()
						  .Where(a => a.AssetId == op.AssetId && a.IsDeleted == false && a.AppStateId == dstEmployeeAppState.Id).Select(a => a.Guid).FirstOrDefaultAsync();

						emailStatuses = await _context.Set<Model.EmailStatus>()
						  .Include(e => e.EmailType)
						  .Include(c => c.CostCenterFinal)
						  .Include(c => c.EmployeeFinal).ThenInclude(c => c.CostCenter)
						  .Where(a => a.Guid == guid && a.IsDeleted == false && a.AppStateId == dstEmployeeAppState.Id).ToListAsync();
					}

                    if (assetOld.AppState != null && assetOld.AppState.Code == dstStockEmployeeAppState.Code)
                    {
                        guid = await _context.Set<Model.EmailStatus>()
                          .Where(a => a.AssetId == op.AssetId && a.IsDeleted == false && a.AppStateId == dstStockEmployeeAppState.Id).Select(a => a.Guid).FirstOrDefaultAsync();

                        emailStatuses = await _context.Set<Model.EmailStatus>()
                          .Include(e => e.EmailType)
                          .Include(c => c.CostCenterFinal)
                          .Include(c => c.EmployeeFinal).ThenInclude(c => c.CostCenter)
                          .Where(a => a.Guid == guid && a.IsDeleted == false && a.AppStateId == dstStockEmployeeAppState.Id).ToListAsync();
                    }

                    for (int i = 0; i < emailStatuses.Count; i++)
					{
						if (emailStatuses[i] != null && emailStatuses[i].EmailType.Code == "TRANSFER")
						{
							asset = await _context.Set<Model.Asset>()
								.Include(a => a.AppState)
								.Include(c => c.Company)
								.Include(a => a.AssetCategory)
								.Include(a => a.ExpAccount)
								.Include(a => a.Document).ThenInclude(p => p.Partner)
								.Where(a => a.Id == emailStatuses[i].AssetId).SingleAsync();

							assetOp = await _context.Set<Model.AssetOp>().Where(a => a.Id == emailStatuses[i].AssetOpId).SingleAsync();

							asset.ModifiedAt = DateTime.Now;
							asset.ModifiedBy = _context.UserId;
							asset.EmployeeTransferId = null;

                            Model.RequestBudgetForecastMaterial requestBudgetForecastMaterial = null;
							Model.RequestBFMaterialCostCenter requestBFMaterialCost = null;
                            List<Model.RequestBFMaterialCostCenter> requestBFMaterialCostCenters = null;
                            Model.RequestBudgetForecast requestBudgetForecast = null;
							List<Model.RequestBudgetForecastMaterial> requestBudgetForecastMaterials = null;
							Model.AssetAdmMD assetAdmMD = null;
                            Model.Order order = null;
							List<Model.RequestBudgetForecast> requestBudgetForecasts = null;


							if (asset.FirstTransfer)
                            {
								int assetStateId = await _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "STOCK_IT").Select(a => a.Id).SingleAsync();

								order = await _context.Set<Model.Order>()
								   .Where(r => r.Id == asset.OrderId).SingleAsync();

								requestBudgetForecastMaterial = await _context.Set<Model.RequestBudgetForecastMaterial>()
                                    .Where(r => r.Id == asset.ReqBFMaterialId).SingleAsync();

								requestBudgetForecast = await _context.Set<Model.RequestBudgetForecast>()
									.Where(r => r.Id == requestBudgetForecastMaterial.RequestBudgetForecastId).SingleAsync();

								requestBFMaterialCost = await _context.Set<Model.RequestBFMaterialCostCenter>()
									.Where(r => r.Id == asset.ReqBFMCostCenterId).SingleAsync();

								requestBFMaterialCost.IsDeleted = true;
								requestBFMaterialCost.ModifiedAt = DateTime.Now;

								requestBFMaterialCostCenters = await _context.Set<Model.RequestBFMaterialCostCenter>()
								   .Where(r => r.RequestBudgetForecastMaterialId == asset.ReqBFMaterialId && r.IsDeleted == false).ToListAsync();

								var item = from r in requestBFMaterialCostCenters where r.Id == asset.ReqBFMCostCenterId select r;

								item.First().IsDeleted = true;


								int sumQuantity = requestBFMaterialCostCenters.Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == asset.ReqBFMaterialId).Sum(a => a.Quantity);
								decimal sumValue = requestBFMaterialCostCenters.Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == asset.ReqBFMaterialId).Sum(a => a.Value);
								decimal sumValueRon = requestBFMaterialCostCenters.Where(a => a.IsDeleted == false && a.RequestBudgetForecastMaterialId == asset.ReqBFMaterialId).Sum(a => a.ValueRon);

								requestBudgetForecastMaterial.TotalCostCenterQuantity = sumQuantity;
								requestBudgetForecastMaterial.TotalCostCenterValue = sumValue;
								requestBudgetForecastMaterial.TotalCostCenterValueRon = sumValueRon;
								requestBudgetForecastMaterial.Value = sumValue;
								requestBudgetForecastMaterial.ValueRon = sumValueRon;
								requestBudgetForecastMaterial.MaxValue = sumValue;
								requestBudgetForecastMaterial.MaxValueRon = sumValueRon;

								requestBudgetForecastMaterial.Quantity = sumQuantity;
								requestBudgetForecastMaterial.MaxQuantity = sumQuantity;

								_context.Update(requestBudgetForecastMaterial);

	

								requestBudgetForecastMaterials = await _context.Set<Model.RequestBudgetForecastMaterial>()
								   .Where(r => r.RequestBudgetForecastId == requestBudgetForecast.Id && r.IsDeleted == false).ToListAsync();

								var itemBFM = from r in requestBudgetForecastMaterials where r.Id == requestBudgetForecastMaterial.Id select r;

								itemBFM.First().TotalCostCenterQuantity = sumQuantity;
								itemBFM.First().TotalCostCenterValue = sumValue;
								itemBFM.First().TotalCostCenterValueRon = sumValueRon;
								itemBFM.First().Value = sumValue;
								itemBFM.First().ValueRon = sumValueRon;
								itemBFM.First().MaxValue = sumValue;
								itemBFM.First().MaxValueRon = sumValueRon;

								itemBFM.First().Quantity = sumQuantity;
								itemBFM.First().MaxQuantity = sumQuantity;

								int sumQuantityBFM = requestBudgetForecastMaterials.Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecast.Id).Sum(a => a.Quantity);
								decimal sumValueBFM = requestBudgetForecastMaterials.Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecast.Id).Sum(a => a.Value);
								decimal sumValueRonBFM = requestBudgetForecastMaterials.Where(a => a.IsDeleted == false && a.RequestBudgetForecastId == requestBudgetForecast.Id).Sum(a => a.ValueRon);

								requestBudgetForecast.TotalOrderQuantity = sumQuantityBFM;
								requestBudgetForecast.TotalOrderValue = sumValueBFM;
								requestBudgetForecast.TotalOrderValueRon = sumValueRonBFM;
								requestBudgetForecast.Value = sumValueBFM;
								requestBudgetForecast.ValueRon = sumValueRonBFM;
								requestBudgetForecast.MaxValue = sumValueBFM;
								requestBudgetForecast.MaxValueRon = sumValueRonBFM;

								requestBudgetForecast.Quantity = sumQuantityBFM;
								requestBudgetForecast.MaxQuantity = sumQuantityBFM;

								_context.Update(requestBudgetForecast);



								requestBFMaterialCost.MaxValue = sumValue;
								requestBFMaterialCost.MaxValueRon = sumValueRon;
								requestBFMaterialCost.MaxQuantity = sumQuantity;

								_context.Update(requestBFMaterialCost);



                                assetAdmMD = await _context.Set<Model.AssetAdmMD>().Where(a => a.AssetId == asset.Id && a.AccMonthId == inventory.AccMonthId).SingleAsync();
                                assetAdmMD.ProjectId = null;

                                _context.Update(assetAdmMD);


								asset.ReqBFMaterialId = null;
                                asset.ReqBFMCostCenterId = null;
                                asset.BudgetForecastId = null;
                                asset.BudgetBaseId = null;
                                asset.BudgetId = null;
                                asset.ProjectId = null;

                                asset.AssetStateId = assetStateId;

                                for (int r = 0; r < requestBFMaterialCostCenters.Count; r++)
                                {
                                    requestBFMaterialCostCenters[r].MaxValue = sumValue;
									requestBFMaterialCostCenters[r].MaxValueRon = sumValueRon;
									requestBFMaterialCostCenters[r].MaxQuantity = sumQuantity;

                                    _context.Update(requestBFMaterialCostCenters[r]);
								}


								//requestBudgetForecasts = await _context.Set<Model.RequestBudgetForecast>()
								//   .Where(r => r.RequestId == asset.RequestId && r.IsDeleted == false).ToListAsync();

								//var itemRBF = from r in requestBudgetForecasts where r.Id == requestBudgetForecastMaterial.RequestBudgetForecastId select r;

								//itemRBF.First().TotalOrderQuantity = sumQuantityBFM;
								//itemRBF.First().TotalOrderValue = sumValueBFM;
								//itemRBF.First().TotalOrderValueRon = sumValueRonBFM;
								//itemRBF.First().Value = sumValueBFM;
								//itemRBF.First().ValueRon = sumValueRonBFM;
								//itemRBF.First().MaxValue = sumValueBFM;
								//itemRBF.First().MaxValueRon = sumValueRonBFM;

								//itemRBF.First().Quantity = sumQuantityBFM;
								//itemRBF.First().MaxQuantity = sumQuantityBFM;


								//int sumQuantityRBF = requestBudgetForecasts.Where(a => a.IsDeleted == false && a.RequestId == asset.RequestId).Sum(a => a.Quantity);
								//decimal sumValueRBF = requestBudgetForecasts.Where(a => a.IsDeleted == false && a.RequestId == asset.RequestId).Sum(a => a.Value);
								//decimal sumValueRonRBF = requestBudgetForecasts.Where(a => a.IsDeleted == false && a.RequestId == asset.RequestId).Sum(a => a.ValueRon);

        //                        order.Quantity = sumQuantityRBF;
								//order.QuantityUsed = sumQuantityRBF;
								//order.ValueUsed = sumValueRBF;
								//order.ValueUsedRon = sumValueRonRBF;

        //                        _context.Update(order);

							}

							//asset.IsMultipleTransfer = false;
							//asset.TransferMultipleCount = 0;

							//asset.LastTransferReason = String.Empty;
							//asset.LastEmployeeId = null;
							//asset.LastTransferNumber = 0;

							

							assetOp.DstConfAt = DateTime.Now;
							assetOp.DstConfBy = _context.UserId;
							assetOp.ModifiedAt = DateTime.Now;
							assetOp.ModifiedBy = _context.UserId;
							assetOp.IsDeleted = true;

							

							emailStatuses[i].DstEmployeeValidateAt = DateTime.Now;
							emailStatuses[i].DstEmployeeValidateBy = _context.UserId;

							emailStatuses[i].ModifiedAt = DateTime.Now;
							emailStatuses[i].ModifiedBy = _context.UserId;

							

							emailStatuses[i].NotSync = true;
							emailStatuses[i].SyncErrorCount = 0;
							emailStatuses[i].NotCompletedSync = true;
							emailStatuses[i].AppStateId = newAppState.Id;
							emailStatuses[i].Info = documentUpload.Details;

							asset.AppStateId = newAppState.Id;
							asset.IsInTransfer = false;
                            asset.FirstTransfer = false;
							assetOp.AssetOpStateId = newAppState.Id;


							_context.Update(emailStatuses[i]);
							_context.Update(asset);
							_context.Update(assetOp);

						}
					}

					_context.SaveChanges();

					var count = await _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToListAsync();
					var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
					var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
					var countOrdMaterial = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();
					var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
					var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
					var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
					var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
					var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();
					var UpdateAllRequestBudgetForecastMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBudgetForecastMaterials").ToList();
					var UpdateAllRequestBFMaterialCostCenters = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBFMaterialCostCenters").ToList();
				}

				return new Model.TransferResult { Success = true, Message = "Transferul a fost refuzat cu success!" };
			}
			else
			{
				return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
			}
		}
		public async Task<Model.TransferResult> RejectFromStock(Dto.DocumentUpload documentUpload)
		{
			Model.Asset assetOld = null;
			Model.Asset asset = null;
			Model.AssetOp assetOp = null;
			Model.Inventory inventory = null;
			Model.AppState dstEmployeeAppState = null;
			Model.AppState newAppState = null;
			Guid guid = Guid.Empty;

			if (documentUpload.Operations != null)
			{
				dstEmployeeAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
				if (dstEmployeeAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare" };

				newAppState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_DECLINED").FirstOrDefaultAsync();
				if (newAppState == null) return new Model.TransferResult { Success = false, Message = "Nu exista stare refuz angajat" };

				inventory = await _context.Set<Model.Inventory>().Where(d => d.Active == true).FirstOrDefaultAsync();
				if (inventory == null) return new Model.TransferResult { Success = false, Message = "Nu exista inventar activ" };

				var date = DateTime.Now.ToString("yyyyMMdd");

				List<Model.EmailStatus> emailStatuses = new List<Model.EmailStatus>();

				foreach (Dto.OperationUpload op in documentUpload.Operations)
				{
					assetOld = await _context.Set<Model.Asset>().Include(a => a.AppState).Where(a => a.Id == op.AssetId && a.IsInTransfer == true).SingleOrDefaultAsync();
					if (assetOld == null) return new Model.TransferResult { Success = false, Message = $"Numarul de inventar nu se afla intru - un flux de tranfer" };

					if (assetOld.AppState != null && assetOld.AppState.Code == dstEmployeeAppState.Code)
					{
						guid = await _context.Set<Model.EmailStatus>()
						  .Where(a => a.AssetId == op.AssetId && a.IsDeleted == false && a.AppStateId == dstEmployeeAppState.Id).Select(a => a.Guid).FirstOrDefaultAsync();

						emailStatuses = await _context.Set<Model.EmailStatus>()
						  .Include(e => e.EmailType)
						  .Include(c => c.CostCenterFinal)
						  .Include(c => c.EmployeeFinal).ThenInclude(c => c.CostCenter)
						  .Where(a => a.Guid == guid && a.IsDeleted == false && a.AppStateId == dstEmployeeAppState.Id).ToListAsync();
					}

					for (int i = 0; i < emailStatuses.Count; i++)
					{
						if (emailStatuses[i] != null && emailStatuses[i].EmailType.Code == "TRANSFER")
						{
							asset = await _context.Set<Model.Asset>()
								.Include(a => a.AppState)
								.Include(c => c.Company)
								.Include(a => a.AssetCategory)
								.Include(a => a.ExpAccount)
								.Include(a => a.Document).ThenInclude(p => p.Partner)
								.Where(a => a.Id == emailStatuses[i].AssetId).SingleAsync();

							assetOp = await _context.Set<Model.AssetOp>().Where(a => a.Id == emailStatuses[i].AssetOpId).SingleAsync();

							asset.ModifiedAt = DateTime.Now;
							asset.ModifiedBy = _context.UserId;
							asset.EmployeeTransferId = null;
                            asset.AppStateId = newAppState.Id;
							asset.IsInTransfer = false;
							asset.FirstTransfer = false;
                            //asset.EmployeeId = emailStatuses[i].EmployeeIdInitial;

							_context.Update(asset);

							assetOp.DstConfAt = DateTime.Now;
							assetOp.DstConfBy = _context.UserId;
							assetOp.ModifiedAt = DateTime.Now;
							assetOp.ModifiedBy = _context.UserId;
							assetOp.AssetOpStateId = newAppState.Id;

							_context.Update(assetOp);

							emailStatuses[i].DstEmployeeValidateAt = DateTime.Now;
							emailStatuses[i].DstEmployeeValidateBy = _context.UserId;

							emailStatuses[i].ModifiedAt = DateTime.Now;
							emailStatuses[i].ModifiedBy = _context.UserId;

							emailStatuses[i].NotSync = true;
							emailStatuses[i].SyncErrorCount = 0;
							emailStatuses[i].NotCompletedSync = true;
							emailStatuses[i].AppStateId = newAppState.Id;
							emailStatuses[i].Info = documentUpload.Details;
							emailStatuses[i].EmailSend = false;

							_context.Update(emailStatuses[i]);
						}
					}

					_context.SaveChanges();

					var count = await _context.Set<Model.RecordCount>().FromSql("UpdateAllAssets").ToListAsync();
					var countOffer = await _context.Set<Model.RecordCount>().FromSql("UpdateAllOffers").ToListAsync();
					var countOrd = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrders").ToList();
					var countOrdMaterial = _context.Set<Model.RecordCount>().FromSql("UpdateAllOrderMaterials").ToList();
					var countContract = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContracts").ToListAsync();
					var countContractAmount = await _context.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToListAsync();
					var countBudget = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBase").ToListAsync();
					var countBudgetBases = await _context.Set<Model.RecordCount>().FromSql("UpdateAllBudgetBases").ToListAsync();
					var countOfferMaterials2 = _context.Set<Model.RecordCount>().FromSql("UpdateAllOfferMaterials2").ToList();
					var UpdateAllRequestBudgetForecastMaterials = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBudgetForecastMaterials").ToList();
					var UpdateAllRequestBFMaterialCostCenters = _context.Set<Model.RecordCount>().FromSql("UpdateAllRequestBFMaterialCostCenters").ToList();
				}

				return new Model.TransferResult { Success = true, Message = "Transferul a fost salvat cu success!" };
			}
			else
			{
				return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
			}
		}
		public async Task<Model.TransferResult> Save(Dto.DocumentUpload documentUpload)
        {
            Model.Document document = null;
            Model.AssetOp operation = null;
            Model.Asset asset = null;
            Model.Inventory inventory = null;
            Model.EmailStatus emailStatus = null;
            Model.EmailType emailType = null;
            Model.EntityType entityType = null;
            Model.AppState appState = null;
            Model.Employee employee = null;

            document = new Model.Document();

            document.DocumentTypeId = documentUpload.DocumentTypeId;
            document.DocNo1 = documentUpload.DocNo1;
            document.DocNo2 = documentUpload.DocNo2;
            document.DocumentDate = documentUpload.DocumentDate;
            document.Approved = true;
            document.Exported = false;
            document.CreationDate = DateTime.Now;
            document.CreatedAt = DateTime.Now;
            document.RegisterDate = DateTime.Now;
            document.ValidationDate = DateTime.Now;

            string docNo1 = "-";
            document.DocNo1 = docNo1;
            int documentNumber = 0;

            _context.Set<Model.Document>().Add(document);

            if (documentUpload.Operations != null)
            {
                inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).FirstOrDefaultAsync();
                emailType = await _context.Set<Model.EmailType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
                entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();

                

                Guid guid = Guid.NewGuid();
                Guid guidAll = Guid.NewGuid();

                documentNumber = int.Parse(entityType.Name);

                documentNumber++;

                for (int i = 0; i < documentUpload.Operations.Count(); i++)
                {
                    employee = await _context.Set<Model.Employee>().Include(c => c.CostCenter).Where(a => a.Id == documentUpload.Operations[i].EmployeeId).FirstOrDefaultAsync();

                    if(employee == null)
                    {
						employee = await _context.Set<Model.Employee>().Include(c => c.CostCenter).Where(a => a.Id == 3275).SingleAsync();

						appState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
					}
                    else
                    {
						if (employee.InternalCode == "VIRTUAL")
						{
							appState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
						}
						else
						{
							appState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
						}
					}
                   
                    
                    asset = await _context.Set<Model.Asset>()
                        .Include(o => o.Document)
                        .Include(o => o.Order)
                        .Include(o => o.OfferMaterial).ThenInclude(o => o.Offer)
                        .Where(a => a.Id == documentUpload.Operations[i].AssetId)
                        .SingleAsync();

                    asset.IsInTransfer = true;
                    asset.Guid = guid;
                    asset.EmployeeTransferId = documentUpload.Operations[i].EmployeeId;
                    asset.AppStateId = appState.Id;

                    //asset.OrderId = documentUpload.Operations[i].OrderId;

                    operation = new Model.AssetOp
                    {
                        AssetOpStateId = appState.Id,
                        InvStateIdInitial = asset.InvStateId,
                        InvStateIdFinal = asset.InvStateId,
                        AssetStateIdInitial = asset.AssetStateId,
                        AssetStateIdFinal = asset.AssetStateId
                    };

                    operation.AccSystemId = 3;
                    operation.AdministrationIdInitial = asset.AdministrationId;
                    operation.AdministrationIdFinal = asset.AdministrationId;
                    operation.AssetCategoryIdInitial = asset.AssetCategoryId;
                    operation.AssetCategoryIdFinal = asset.AssetCategoryId;
                    operation.AssetId = documentUpload.Operations[i].AssetId;
                    operation.CostCenterIdInitial = asset.CostCenterId;
                    operation.CostCenterIdFinal = documentUpload.Operations[i].CostCenterId;
                    operation.CreatedAt = DateTime.Now;
                    operation.CreatedBy = _context.UserId;
                    operation.DepartmentIdInitial = asset.DepartmentId;
                    operation.DepartmentIdFinal = asset.DepartmentId;
                    operation.DocumentId = document.Id;
                    operation.EmployeeIdInitial = documentUpload.OperationEmpId;
                    operation.EmployeeIdFinal = documentUpload.Operations[i].EmployeeId;
                    operation.IsDeleted = false;
                    operation.ModifiedAt = DateTime.Now;
                    operation.ModifiedBy = _context.UserId;
                    operation.RoomIdInitial = asset.RoomId;
                    operation.RoomIdFinal = asset.RoomId;
                    operation.SrcConfAt = DateTime.Now;
                    operation.SrcConfBy = _context.UserId;
                    operation.AllowLabel = asset.AllowLabel != null ? (bool)asset.AllowLabel : false;
                    operation.AssetTypeIdInitial = asset.AssetTypeId;
                    operation.AssetTypeIdFinal = asset.AssetTypeId;
                    operation.InvName = asset.Name;
                    operation.Quantity = asset.Quantity;
                    operation.SerialNumber = asset.SerialNumber;
                    operation.AssetNatureIdInitial = asset.AssetNatureId;
                    operation.AssetNatureIdFinal = asset.AssetNatureId;
                    operation.BudgetManagerIdInitial = asset.BudgetManagerId;
                    operation.BudgetManagerIdFinal = asset.BudgetManagerId;
                    operation.DimensionIdInitial = asset.DimensionId;
                    operation.DimensionIdFinal = asset.DimensionId;
                    operation.ProjectIdInitial = asset.ProjectId;
                    operation.ProjectIdFinal = asset.ProjectId;
                    operation.IsMinus = false;
                    operation.IsPlus = false;
                    operation.CompanyId = asset.CompanyId;
                    operation.InsuranceCategoryId = asset.InsuranceCategoryId;
                    
                    operation.UomId = asset.UomId;
                    operation.TaxId = asset.TaxId;
                    operation.ValueAdd = documentNumber;
                    operation.Guid = guid;
                    operation.Info = documentUpload.Details;
                    operation.Info2019 = documentUpload.Details;


					emailStatus = new Model.EmailStatus()
                    {
                        AppStateId = appState.Id,
                        AssetId = asset.Id,
                        AssetOp = operation,
                        BudgetBaseId = asset.BudgetBaseId,
                        CompanyId = asset.CompanyId,
                        Completed = false,
                        CostCenterIdFinal = documentUpload.Operations[i].CostCenterId,
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
                        EmployeeIdFinal = employee.Id,
                        EmployeeIdInitial = asset.EmployeeId,
                        ErrorId = null,
                        Exported = false,
                        FinalValidateAt = null,
                        FinalValidateBy = null,
                        Guid = guid,
                        GuidAll = guidAll,
                        Info = documentUpload.Details,
                        IsAccepted = false,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        NotCompletedSync = false,
                        NotDstEmployeeSync = true,
                        NotDstManagerSync = false,
                        NotSrcEmployeeSync = false,
                        NotSrcManagerSync = false,
                        NotSync = false,
                        OfferId = asset.Order != null ? asset.Order.OfferId : null,
                        OrderId = asset.OrderId,
                        PartnerId = asset.Document.PartnerId,
                        RequestId = asset.RequestId,
                        SameEmployee = asset.EmployeeId == employee.Id ? true : false,
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

                    _context.Add(emailStatus);
                    _context.Add(operation);
                    _context.Update(asset);
                    _context.Update(entityType);
                    _context.SaveChanges();
                }

                return new Model.TransferResult { Success = true, Message = "Transferul a fost salvat cu success!" };
            }
			else
			{
                return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
            }
 
        }

        public async Task<Model.TransferResult> WFHSave(Dto.DocumentUpload documentUpload)
        {
            Model.Document document = null;
            Model.AssetOp operation = null;
            Model.Asset asset = null;
            Model.Inventory inventory = null;
            Model.EmailStatus emailStatus = null;
            Model.EmailType emailType = null;
            Model.EntityType entityType = null;
            Model.AppState appState = null;
            Model.AppState wfhState = null;
            Model.Employee employee = null;

            document = new Model.Document();

            document.DocumentTypeId = documentUpload.DocumentTypeId;
            document.DocNo1 = documentUpload.DocNo1;
            document.DocNo2 = documentUpload.DocNo2;
            document.DocumentDate = documentUpload.DocumentDate;
            document.Approved = true;
            document.Exported = false;
            document.CreationDate = DateTime.Now;
            document.CreatedAt = DateTime.Now;
            document.RegisterDate = DateTime.Now;
            document.ValidationDate = DateTime.Now;

            string docNo1 = "-";
            document.DocNo1 = docNo1;
            int documentNumber = 0;

            _context.Set<Model.Document>().Add(document);

            if (documentUpload.Operations != null)
            {
                inventory = await _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).FirstOrDefaultAsync();
                emailType = await _context.Set<Model.EmailType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
                entityType = await _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "TRANSFER").FirstOrDefaultAsync();
                wfhState = await _context.Set<Model.AppState>().Where(r => r.Code == "WFH_TRANSFER_IT").FirstOrDefaultAsync();

                Guid guid = Guid.NewGuid();
                Guid guidAll = Guid.NewGuid();

                documentNumber = int.Parse(entityType.Name);

                documentNumber++;

                for (int i = 0; i < documentUpload.Operations.Count(); i++)
                {
                    employee = await _context.Set<Model.Employee>().Include(c => c.CostCenter).Where(a => a.Id == documentUpload.Operations[i].EmployeeId).FirstOrDefaultAsync();

                    if (employee == null)
                    {
                        employee = await _context.Set<Model.Employee>().Include(c => c.CostCenter).Where(a => a.Id == 3275).SingleAsync();

                        appState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
                    }
                    else
                    {
                        if (employee.InternalCode == "VIRTUAL")
                        {
                            appState = await _context.Set<Model.AppState>().Where(a => a.Code == "STOCK_VALIDATE").FirstOrDefaultAsync();
                        }
                        else
                        {
                            appState = await _context.Set<Model.AppState>().Where(a => a.Code == "FINAL_EMPLOYEE_VALIDATE").FirstOrDefaultAsync();
                        }
                    }


                    asset = await _context.Set<Model.Asset>()
                        .Include(o => o.Document)
                        .Include(o => o.Order)
                        .Include(o => o.OfferMaterial).ThenInclude(o => o.Offer)
                        .Where(a => a.Id == documentUpload.Operations[i].AssetId)
                        .SingleAsync();

                    asset.IsInTransfer = true;
                    asset.Guid = guid;
                    asset.EmployeeTransferId = documentUpload.Operations[i].EmployeeId;
                    asset.AppStateId = appState.Id;
                    asset.WFHStateId = wfhState.Id;
                    asset.IsWFH = asset.InvNo == null && asset.InvNo.StartsWith("WFH") ? true : false;
                    //asset.OrderId = documentUpload.Operations[i].OrderId;

                    operation = new Model.AssetOp
                    {
                        AssetOpStateId = appState.Id,
                        InvStateIdInitial = asset.InvStateId,
                        InvStateIdFinal = asset.InvStateId,
                        AssetStateIdInitial = asset.AssetStateId,
                        AssetStateIdFinal = asset.AssetStateId
                    };

                    operation.AccSystemId = 3;
                    operation.AdministrationIdInitial = asset.AdministrationId;
                    operation.AdministrationIdFinal = asset.AdministrationId;
                    operation.AssetCategoryIdInitial = asset.AssetCategoryId;
                    operation.AssetCategoryIdFinal = asset.AssetCategoryId;
                    operation.AssetId = documentUpload.Operations[i].AssetId;
                    operation.CostCenterIdInitial = asset.CostCenterId;
                    operation.CostCenterIdFinal = documentUpload.Operations[i].CostCenterId;
                    operation.CreatedAt = DateTime.Now;
                    operation.CreatedBy = _context.UserId;
                    operation.DepartmentIdInitial = asset.DepartmentId;
                    operation.DepartmentIdFinal = asset.DepartmentId;
                    operation.DocumentId = document.Id;
                    operation.EmployeeIdInitial = documentUpload.OperationEmpId;
                    operation.EmployeeIdFinal = documentUpload.Operations[i].EmployeeId;
                    operation.IsDeleted = false;
                    operation.ModifiedAt = DateTime.Now;
                    operation.ModifiedBy = _context.UserId;
                    operation.RoomIdInitial = asset.RoomId;
                    operation.RoomIdFinal = asset.RoomId;
                    operation.SrcConfAt = DateTime.Now;
                    operation.SrcConfBy = _context.UserId;
                    operation.AllowLabel = asset.AllowLabel != null ? (bool)asset.AllowLabel : false;
                    operation.AssetTypeIdInitial = asset.AssetTypeId;
                    operation.AssetTypeIdFinal = asset.AssetTypeId;
                    operation.InvName = asset.Name;
                    operation.Quantity = asset.Quantity;
                    operation.SerialNumber = asset.SerialNumber;
                    operation.AssetNatureIdInitial = asset.AssetNatureId;
                    operation.AssetNatureIdFinal = asset.AssetNatureId;
                    operation.BudgetManagerIdInitial = asset.BudgetManagerId;
                    operation.BudgetManagerIdFinal = asset.BudgetManagerId;
                    operation.DimensionIdInitial = asset.DimensionId;
                    operation.DimensionIdFinal = asset.DimensionId;
                    operation.ProjectIdInitial = asset.ProjectId;
                    operation.ProjectIdFinal = asset.ProjectId;
                    operation.IsMinus = false;
                    operation.IsPlus = false;
                    operation.CompanyId = asset.CompanyId;
                    operation.InsuranceCategoryId = asset.InsuranceCategoryId;
                    
                    operation.UomId = asset.UomId;
                    operation.TaxId = asset.TaxId;
                    operation.ValueAdd = documentNumber;
                    operation.Guid = guid;
                    operation.Info = documentUpload.Details;
                    operation.Info2019 = documentUpload.Details;


                    emailStatus = new Model.EmailStatus()
                    {
                        AppStateId = appState.Id,
                        AssetId = asset.Id,
                        AssetOp = operation,
                        BudgetBaseId = asset.BudgetBaseId,
                        CompanyId = asset.CompanyId,
                        Completed = false,
                        CostCenterIdFinal = documentUpload.Operations[i].CostCenterId,
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
                        EmployeeIdFinal = employee.Id,
                        EmployeeIdInitial = asset.EmployeeId,
                        ErrorId = null,
                        Exported = false,
                        FinalValidateAt = null,
                        FinalValidateBy = null,
                        Guid = guid,
                        GuidAll = guidAll,
                        Info = documentUpload.Details,
                        IsAccepted = false,
                        IsDeleted = false,
                        ModifiedAt = DateTime.Now,
                        ModifiedBy = _context.UserId,
                        NotCompletedSync = false,
                        NotDstEmployeeSync = true,
                        NotDstManagerSync = false,
                        NotSrcEmployeeSync = false,
                        NotSrcManagerSync = false,
                        NotSync = false,
                        OfferId = asset.Order != null ? asset.Order.OfferId : null,
                        OrderId = asset.OrderId,
                        PartnerId = asset.Document.PartnerId,
                        RequestId = asset.RequestId,
                        SameEmployee = asset.EmployeeId == employee.Id ? true : false,
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

                    _context.Add(emailStatus);
                    _context.Add(operation);
                    _context.Update(asset);
                    _context.Update(entityType);
                    _context.SaveChanges();
                }

                return new Model.TransferResult { Success = true, Message = "Transferul a fost salvat cu success!" };
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
            }

        }

        public Model.TransferResult SaveStateChange(Dto.DocumentUpload documentUpload)
        {
            Model.Document document = null;
            Model.AssetOp operation = null;
            Model.Asset asset = null;
            Model.Inventory inventory = null;
            Model.EntityType entityType = null;
            Model.AssetState assetState = null;

            document = new Model.Document();

            document.DocumentTypeId = documentUpload.DocumentTypeId;
            document.DocNo1 = documentUpload.DocNo1;
            document.DocNo2 = documentUpload.DocNo2;
            document.DocumentDate = documentUpload.DocumentDate;
            document.Approved = true;
            document.Exported = false;
            document.CreationDate = DateTime.Now;
            document.CreatedAt = DateTime.Now;
            document.RegisterDate = DateTime.Now;
            document.ValidationDate = DateTime.Now;

            string docNo1 = "-";
            document.DocNo1 = docNo1;
            int documentNumber = 0;

            _context.Set<Model.Document>().Add(document);

            if (documentUpload.Operations != null)
            {
                inventory = _context.Set<Model.Inventory>().AsNoTracking().Where(a => a.Active == true).SingleOrDefault();
                entityType = _context.Set<Model.EntityType>().AsNoTracking().Where(a => a.Code == "SOLD").SingleOrDefault();
                


                Guid guid = Guid.NewGuid();
                Guid guidAll = Guid.NewGuid();

                documentNumber = int.Parse(entityType.Name);

                documentNumber++;

                for (int i = 0; i < documentUpload.Operations.Count(); i++)
                {

                    

                    if(documentUpload.Operations[i].AssetAccStateId.Value != 1)
					{
                        assetState = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "SOLD").SingleOrDefault();

                        entityType.Name = documentNumber.ToString();
                        _context.Update(entityType);
                    }
					else
					{
                        assetState = _context.Set<Model.AssetState>().AsNoTracking().Where(a => a.Code == "IN_USE").SingleOrDefault();
                    }


                    asset = _context.Set<Model.Asset>().Where(a => a.Id == documentUpload.Operations[i].AssetId).Single();

                    // asset.IsInTransfer = true;
                    asset.AssetStateId = assetState.Id;
                    // asset.BudgetBaseId = documentUpload.Operations[i].BudgetManagerId != null ? documentUpload.Operations[i].BudgetManagerId : asset.BudgetBaseId;

                    operation = new Model.AssetOp
                    {
                        AssetOpStateId = 3,
                        InvStateIdInitial = asset.InvStateId,
                        InvStateIdFinal = asset.InvStateId,
                        AssetStateIdInitial = asset.AssetStateId,
                        AssetStateIdFinal = assetState.Id
                    };

                    operation.AccSystemId = 3;
                    operation.AdministrationIdInitial = asset.AdministrationId;
                    operation.AdministrationIdFinal = asset.AdministrationId;
                    operation.AssetCategoryIdInitial = asset.AssetCategoryId;
                    operation.AssetCategoryIdFinal = asset.AssetCategoryId;
                    operation.AssetId = documentUpload.Operations[i].AssetId;
                    operation.CostCenterIdInitial = asset.CostCenterId;
                    operation.CostCenterIdFinal = documentUpload.Operations[i].CostCenterId;
                    operation.CreatedAt = DateTime.Now;
                    operation.CreatedBy = _context.UserId;
                    operation.DepartmentIdInitial = asset.DepartmentId;
                    operation.DepartmentIdFinal = asset.DepartmentId;
                    operation.DocumentId = document.Id;
                    operation.EmployeeIdInitial = asset.EmployeeId;
                    operation.EmployeeIdFinal = documentUpload.Operations[i].EmployeeId;
                    operation.IsDeleted = false;
                    operation.ModifiedAt = DateTime.Now;
                    operation.ModifiedBy = _context.UserId;
                    operation.RoomIdInitial = asset.RoomId;
                    operation.RoomIdFinal = asset.RoomId;
                    operation.SrcConfAt = DateTime.Now;
                    operation.SrcConfBy = _context.UserId;
                    operation.AllowLabel = asset.AllowLabel != null ? (bool)asset.AllowLabel : false;
                    operation.AssetTypeIdInitial = asset.AssetTypeId;
                    operation.AssetTypeIdFinal = asset.AssetTypeId;
                    operation.InvName = asset.Name;
                    operation.Quantity = asset.Quantity;
                    operation.SerialNumber = asset.SerialNumber;
                    operation.AssetNatureIdInitial = asset.AssetNatureId;
                    operation.AssetNatureIdFinal = asset.AssetNatureId;
                    operation.BudgetManagerIdInitial = asset.BudgetManagerId;
                    operation.BudgetManagerIdFinal = asset.BudgetManagerId;
                    operation.DimensionIdInitial = asset.DimensionId;
                    operation.DimensionIdFinal = asset.DimensionId;
                    operation.ProjectIdInitial = asset.ProjectId;
                    operation.ProjectIdFinal = asset.ProjectId;
                    operation.IsMinus = false;
                    operation.IsPlus = false;
                    operation.CompanyId = asset.CompanyId;
                    operation.InsuranceCategoryId = asset.InsuranceCategoryId;
                    
                    operation.UomId = asset.UomId;
                    operation.TaxId = asset.TaxId;
                    operation.ValueAdd = documentNumber;

                    



                    _context.Add(operation);
                    _context.Update(asset);
                    
                    _context.SaveChanges();
                }

                return new Model.TransferResult { Success = true, Message = "Modificarea a fost salvata cu succes!" };
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = "Nu exista operatii" };
            }

        }

        public IEnumerable<string> SplitToLines(string stringToSplit, int maximumLineLength)
        {
            var words = stringToSplit.Split(' ').Concat(new[] { "" });
            return
                words
                    .Skip(1)
                    .Aggregate(
                        words.Take(1).ToList(),
                        (a, w) =>
                        {
                            var last = a.Last();
                            while (last.Length > maximumLineLength)
                            {
                                a[a.Count() - 1] = last.Substring(0, maximumLineLength);
                                last = last.Substring(maximumLineLength);
                                a.Add(last);
                            }
                            var test = last + " " + w;
                            if (test.Length > maximumLineLength)
                            {
                                a.Add(w);
                            }
                            else
                            {
                                a[a.Count() - 1] = test;
                            }
                            return a;
                        });
        }
    }
}
