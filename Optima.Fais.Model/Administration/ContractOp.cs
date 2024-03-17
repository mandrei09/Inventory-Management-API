using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class ContractOp : Entity
    {
        public int ContractId { get; set; }

        public virtual Contract Contract { get; set; }

        public int? AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public int? CommodityId { get; set; }

        public virtual Commodity Commodity { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? ContractDivisionId { get; set; }

        public virtual ContractDivision ContractDivision { get; set; }

        public int? ContractRegionId { get; set; }

        public virtual ContractRegion ContractRegion { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public int? BusinessSystemId { get; set; }

        public virtual BusinessSystem BusinessSystem { get; set; }

        public int? ContractAmountId { get; set; }

        public virtual ContractAmount ContractAmount { get; set; }


        [MaxLength(450)]
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

        public int? ContractStateId { get; set; }

        public virtual AppState ContractState { get; set; }

        public bool Validated { get; set; }

        public bool IsAccepted { get; set; }
    }
}
