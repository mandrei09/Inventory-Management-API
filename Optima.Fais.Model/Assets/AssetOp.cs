using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class AssetOp : Entity
    {
        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int? AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public int? AdministrationIdInitial { get; set; }

        public virtual Administration AdministrationInitial { get; set; }

        public int? AdministrationIdFinal { get; set; }

        public virtual Administration AdministrationFinal { get; set; }

        public int? CostCenterIdInitial { get; set; }

        public virtual CostCenter CostCenterInitial { get; set; }

        public int? CostCenterIdFinal { get; set; }

        public virtual CostCenter CostCenterFinal { get; set; }

        public int? AssetStateIdInitial { get; set; }

        public virtual AssetState AssetStateInitial { get; set; }

        public int? AssetStateIdFinal { get; set; }

        public virtual AssetState AssetStateFinal { get; set; }

        public int? DepartmentIdInitial { get; set; }

        public virtual Department DepartmentInitial { get; set; }

        public int? DepartmentIdFinal { get; set; }

        public virtual Department DepartmentFinal { get; set; }

        public decimal? ValueAdd { get; set; }

        public bool? DepUpdate { get; set; }

        public int? EmployeeIdInitial { get; set; }

        public virtual Employee EmployeeInitial { get; set; }

        public int? EmployeeIdFinal { get; set; }

        public virtual Employee EmployeeFinal { get; set; }

        public int? RoomIdInitial { get; set; }

        public virtual Room RoomInitial { get; set; }

        public virtual Room RoomFinal { get; set; }

        public int? RoomIdFinal { get; set; }

        public int? AssetCategoryIdInitial { get; set; }

        public virtual AssetCategory AssetCategoryInitial { get; set; }

        public int? AssetCategoryIdFinal { get; set; }

        public virtual AssetCategory AssetCategoryFinal { get; set; }

        public int? InvStateIdInitial { get; set; }

        public virtual InvState InvStateInitial { get; set; }

        public int? InvStateIdFinal { get; set; }

        public virtual InvState InvStateFinal { get; set; }

        [MaxLength(200)]
        public string Info { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ReleaseConfAt { get; set; }

        [MaxLength(450)]
        public string ReleaseConfBy { get; set; }

        public ApplicationUser ReleaseConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SrcConfAt { get; set; }

        [MaxLength(450)]
        public string SrcConfBy { get; set; }

        public ApplicationUser SrcConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DstConfAt { get; set; }

        [MaxLength(450)]
        public string DstConfBy { get; set; }

        public ApplicationUser DstConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? RegisterConfAt { get; set; }

        [MaxLength(450)]
        public string RegisterConfBy { get; set; }

        public ApplicationUser RegisterConfUser { get; set; }

        public int? AssetOpStateId { get; set; }

        public virtual AppState AssetOpState { get; set; }

        public string InvName { get; set; }

        public bool AllowLabel { get; set; }

        public int? AssetTypeIdInitial { get; set; }

        public virtual AssetType AssetTypeInitial { get; set; }

        public int? AssetTypeIdFinal { get; set; }

        public virtual AssetType AssetTypeFinal { get; set; }

        public string Model { get; set; }

        public string Producer { get; set; }

        public float Quantity { get; set; }

        public int? AssetNatureIdInitial { get; set; }

        public virtual AssetNature AssetNatureInitial { get; set; }

        public int? AssetNatureIdFinal { get; set; }

        public virtual AssetNature AssetNatureFinal { get; set; }

        public string Info2019 { get; set; }

        public int? BudgetManagerIdInitial { get; set; }

        public virtual BudgetManager BudgetManagerInitial { get; set; }

        public int? BudgetManagerIdFinal { get; set; }

        public virtual BudgetManager BudgetManagerFinal { get; set; }

        public string SerialNumber { get; set; }

        public int? ProjectIdInitial { get; set; }

        public virtual Project ProjectInitial { get; set; }

        public int? ProjectIdFinal { get; set; }

        public virtual Project ProjectFinal { get; set; }


        public int? DimensionIdInitial { get; set; }

        public virtual Dimension DimensionInitial { get; set; }

        public int? DimensionIdFinal { get; set; }

        public virtual Dimension DimensionFinal { get; set; }

		public bool IsMinus { get; set; }

		public bool IsPlus { get; set; }

		public string InfoMinus { get; set; }

		public string InfoPlus { get; set; }

		public int? AssetRecoStateId { get; set; }

		public virtual AppState AssetRecoState { get; set; }

		public int? EntityTypeId { get; set; }

		public virtual EntityType EntityType { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }

        public int? InsuranceCategoryId { get; set; }

        public virtual InsuranceCategory InsuranceCategory { get; set; }

        public int? TaxId { get; set; }

        public virtual Tax Tax { get; set; }

		public Guid Guid { get; set; }

	}
}
