using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class OfferOp : Entity
    {
        public int OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public int? AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public int? AccountIdInitial { get; set; }

        public virtual Account AccountInitial { get; set; }

        public int? AccountIdFinal { get; set; }

        public virtual Account AccountFinal { get; set; }

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? AdministrationIdInitial { get; set; }

        public virtual Administration AdministrationInitial { get; set; }

        public int? AdministrationIdFinal { get; set; }

        public virtual Administration AdministrationFinal { get; set; }

        public int? BudgetManagerIdInitial { get; set; }

        public virtual BudgetManager BudgetManagerInitial { get; set; }

        public int? BudgetManagerIdFinal { get; set; }

        public virtual BudgetManager BudgetManagerFinal { get; set; }

        public int? CompanyIdInitial { get; set; }

        public virtual Company CompanyInitial { get; set; }

        public int? CompanyIdFinal { get; set; }

        public virtual Company CompanyFinal { get; set; }

        public int? CostCenterIdInitial { get; set; }

        public virtual CostCenter CostCenterInitial { get; set; }

        public int? CostCenterIdFinal { get; set; }

        public virtual CostCenter CostCenterFinal { get; set; }

        public int? EmployeeIdInitial { get; set; }

        public virtual Employee EmployeeInitial { get; set; }

        public int? EmployeeIdFinal { get; set; }

        public virtual Employee EmployeeFinal { get; set; }

        

        

        public int? PartnerIdInitial { get; set; }

        public virtual Partner PartnerInitial { get; set; }

        public int? PartnerIdFinal { get; set; }

        public virtual Partner PartnerFinal { get; set; }

        public int? ProjectIdInitial { get; set; }

        public virtual Project ProjectInitial { get; set; }

        public int? ProjectIdFinal { get; set; }

        public virtual Project ProjectFinal { get; set; }

        public int? SubTypeIdInitial { get; set; }

        public virtual SubType SubTypeInitial { get; set; }

        public int? SubTypeIdFinal { get; set; }

        public virtual SubType SubTypeFinal { get; set; }

        public decimal ValueIni1 { get; set; }

        public decimal ValueFin1 { get; set; }

        public decimal ValueIni2 { get; set; }

        public decimal ValueFin2 { get; set; }


        [MaxLength(450)]
        public string InfoIni { get; set; }

        [MaxLength(450)]
        public string InfoFin { get; set; }

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

        public float QuantityIni { get; set; }

        public float QuantityFin { get; set; }

        public int? BudgetStateId { get; set; }

        public virtual AppState BudgetState { get; set; }

        public bool Validated { get; set; }

        public bool IsAccepted { get; set; }

        public Guid Guid { get; set; }

        public int? BudgetIdInitial { get; set; }

        public virtual Budget BudgetInitial { get; set; }

        public int? BudgetIdFinal { get; set; }

        public virtual Budget BudgetFinal { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }


        public int? AdmCenterIdInitial { get; set; }

        public virtual AdmCenter AdmCenterInitial { get; set; }

        public int? RegionIdInitial { get; set; }

        public virtual Region RegionInitial { get; set; }

        public int? AssetTypeIdInitial { get; set; }

        public virtual AssetType AssetTypeInitial { get; set; }

        public int? ProjectTypeIdInitial { get; set; }

        public virtual ProjectType ProjectTypeInitial { get; set; }


        public int? AdmCenterIdFinal { get; set; }

        public virtual AdmCenter AdmCenterFinal { get; set; }

        public int? RegionIdFinal { get; set; }

        public virtual Region RegionFinal { get; set; }

        public int? AssetTypeIdFinal { get; set; }

        public virtual AssetType AssetTypeFinal { get; set; }

        public int? ProjectTypeIdFinal { get; set; }

        public virtual ProjectType ProjectTypeFinal { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }

    }
}
