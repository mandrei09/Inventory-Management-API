using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class Asset : Entity
    {
        public Asset()
        {
            ChildAssets = new HashSet<Asset>();
        }

        public int? ParentAssetId { get; set; }

        public virtual Asset ParentAsset { get; set; }

        //public int? AssetInvId { get; set; }

        public virtual AssetInv AssetInv { get; set; }

        public string InvNo { get; set; }

        public string Name { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public bool Validated { get; set; }

        public int? DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? DictionaryItemId { get; set; }

        public virtual DictionaryItem DictionaryItem { get; set; }

        public virtual ICollection<Asset> ChildAssets { get; set; }

        public virtual ICollection<AssetDep> AssetDeps { get; set; }

        public virtual ICollection<AssetDepMD> AssetDepMDs { get; set; }

        public virtual ICollection<AssetAdmMD> AssetAdmMDs { get; set; }

        public float Quantity { get; set; }

        public string SerialNumber { get; set; }

        public string ERPCode { get; set; }

        public decimal ValueInv { get; set; }

        public decimal ValueRem { get; set; }

        public bool? Custody { get; set; }

        //public virtual ICollection<Operation> Operations { get; set; }

        //public virtual AssetAC AssetAC { get; set; }

        //public virtual AssetDep AssetDep { get; set; }

        //public virtual AssetInv AssetInv { get; set; }

        public int? RoomId { get; set; }

        public int? EmployeeId { get; set; }

        public int? CostCenterId { get; set; }

        public int? AssetCategoryId { get; set; }

        public int? DepartmentId { get; set; }

        public int? DimensionId { get; set; }

        public int? AdministrationId { get; set; }

		public int? SubTypeId { get; set; }

		public int? InsuranceCategoryId { get; set; }

		public int? ModelId { get; set; }

		public int? BrandId { get; set; }

		public int? ProjectId { get; set; }

		public int? InterCompanyId { get; set; }

		public int? AssetTypeId { get; set; }

        public int? AssetStateId { get; set; }

        public int? InvStateId { get; set; }

        public int? UomId { get; set; }

        public int? AccountId { get; set; }

        public int? ExpAccountId { get; set; }

        public int? AssetNatureId { get; set; }

        public int? BudgetManagerId { get; set; }

        public int? ArticleId { get; set; }

		public virtual Room Room { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public virtual AssetCategory AssetCategory { get; set; }

        public virtual Department Department { get; set; }

        public virtual Dimension Dimension { get; set; }

        public virtual Administration Administration { get; set; }

		public virtual SubType SubType { get; set; }

		public virtual Model Model { get; set; }

		public virtual Brand Brand { get; set; }

		public virtual Project Project { get; set; }

		public virtual InterCompany InterCompany { get; set; }

		public virtual InsuranceCategory InsuranceCategory { get; set; }

		public virtual AssetType AssetType { get; set; }

        public virtual AssetState AssetState { get; set; }

        public virtual InvState InvState { get; set; }

        public virtual InventoryAsset InventoryAsset { get; set; }

        public virtual Uom Uom { get; set; }

        public virtual Account Account { get; set; }

        public virtual ExpAccount ExpAccount { get; set; }

        public virtual AssetNature AssetNature { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public virtual Article Article { get; set; }

		public string SAPCode { get; set; }

        public string TempReco { get; set; }

        public string TempName { get; set; }

        public bool IsInTransfer { get; set; }

        public DateTime? ReceptionDate { get; set; }

        public DateTime? PODate { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? RemovalDate { get; set; }

        public bool? IsTemp { get; set; }

        public int ImageCount { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsReconcile { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string Info { get; set; }

		public bool IsMinus { get; set; }

		public bool IsPlus { get; set; }

		public string InfoMinus { get; set; }

		public string InfoPlus { get; set; }

		[MaxLength(450)]
		public string CreatedUser { get; set; }

		public ApplicationUser CreatedByUser { get; set; }

		public int NIRNumber { get; set; }

		public DateTime? NIRDate { get; set; }

		public int PIFNumber { get; set; }

		public DateTime? PIFDate { get; set; }

        public int? BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        public int? BudgetBaseId { get; set; }

        public virtual BudgetBase BudgetBase { get; set; }

        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }

        public bool IsDuplicate { get; set; }

        public bool IsPrinted { get; set; }

        public DateTime? PrintDate { get; set; }

        public bool? AllowLabel { get; set; }

		public string SubNo { get; set; }

		public string AgreementNo { get; set; }

		public string Manufacturer { get; set; }

        public int? MaterialId { get; set; }

        public virtual Material Material { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

        public int? ProjectTypeId { get; set; }

        public virtual ProjectType ProjectType { get; set; }

        public int? StockId { get; set; }

        public virtual Stock Stock { get; set; }

        public int? ContractId { get; set; }

        public virtual Contract Contract { get; set; }

        public int? RateId { get; set; }

        public virtual Rate Rate { get; set; }

        public decimal NetAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string HeaderText { get; set; }

        public int? TaxId { get; set; }

        public virtual Tax Tax { get; set; }

		public int? OrderMaterialId { get; set; }

		public virtual OrderMaterial OrderMaterial { get; set; }

		public int? OfferMaterialId { get; set; }

		public virtual OfferMaterial OfferMaterial { get; set; }

        public decimal ReceptionPrice { get; set; }

        public int? AssetRecoStateId { get; set; }

        public virtual AppState AssetRecoState { get; set; }

		public string TempSerialNumber { get; set; }

        public virtual ICollection<CreateAssetSAP> CreateAssetSAP { get; set; }

        public int? CostCenterEmpId { get; set; }

        public virtual CostCenter CostCenterEmp { get; set; }

        public virtual ICollection<AcquisitionAssetSAP> AcquisitionAssetSAP { get; set; }

        public virtual ICollection<AssetChangeSAP> AssetChangeSAP { get; set; }

        public virtual ICollection<AssetInvPlusSAP> AssetInvPlusSAP { get; set; }

        public virtual ICollection<AssetInvMinusSAP> AssetInvMinusSAP { get; set; }

        public virtual ICollection<RetireAssetSAP> RetireAssetSAP { get; set; }

        public virtual ICollection<TransferAssetSAP> TransferAssetSAP { get; set; }

        public virtual ICollection<TransferInStockSAP> TransferInStockSAP { get; set; }

		public bool NotSync { get; set; }

        public int? SubCategoryId { get; set; }

        public virtual SubCategory SubCategory { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }

		public int DepPeriod { get; set; }

        public virtual ICollection<Error> Error { get; set; }

        public decimal NetAmountRon { get; set; }

        public decimal TaxAmountRon { get; set; }

        public decimal TotalAmountRon { get; set; }

        public decimal ValueInvRon { get; set; }

        public decimal ValueRemRon { get; set; }

        public int? EmployeeTransferId { get; set; }

        public virtual Employee EmployeeTransfer { get; set; }

		public Guid Guid { get; set; }

		public bool IsLocked { get; set; }

        public bool IsClone { get; set; }

        public bool IsSAPClone { get; set; }

        public int? BudgetForecastId { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }

        public int? ReqBFMaterialId { get; set; }

        public virtual RequestBudgetForecastMaterial ReqBFMaterial { get; set; }

        public int? ReqBFMCostCenterId { get; set; }

        public virtual RequestBFMaterialCostCenter ReqBFMCostCenter { get; set; }

		public bool Wip { get; set; }

		public bool FirstTransfer { get; set; }

        public string MaterialUnique { get; set; }

		public string PhoneNumber { get; set; }

		public string Imei { get; set; }

		[MaxLength(450)]
		public string TempUserId { get; set; }

		public ApplicationUser TempUser { get; set; }

        public bool InInventory { get; set; }

		public string TempUserName { get; set; }

		public bool Storno { get; set; }

		public decimal StornoValue { get; set; }

		public int StornoQuantity { get; set; }

		public decimal StornoValueRon { get; set; }

		public bool Cassation { get; set; }

		public decimal CassationValue { get; set; }

		public float CassationQuantity { get; set; }

		public decimal CassationValueRon { get; set; }

        public int? WFHStateId { get; set; }

        public virtual AppState WFHState { get; set; }

        public bool IsWFH { get; set; }

        public string InSapValidation { get; set; }
    }
}
