using System;

namespace Optima.Fais.Dto
{
    public class AssetChangeDTO
    {
        public int Id { get; set; }

        
        
        public int? AssetCategoryId { get; set; }
        public int? PartnerId { get; set; }

        
        public int? CostCenterId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DivisionId { get; set; }

        public int? RoomId { get; set; }
        public int? AssetNatureId { get; set; }
        public int? BudgetManagerId { get; set; }
        public int? TypeId { get; set; }

        public int? EmployeeId { get; set; }
        public int? MaterialId { get; set; }
       
        public int? SubTypeId { get; set; }

        public int? AssetClassId { get; set; }
        public int? AdmCenterId { get; set; }
        public int? RegionId { get; set; }
        public int? InsuranceCategoryId { get; set; }

        public int? AssetTypeId { get; set; }
        public int? ProjectId { get; set; }
        public int? OrderId { get; set; }
        public int? DictionaryItemId { get; set; }


        public string InvNo { get; set; }
        public string SubNo { get; set; }
        public float Quantity { get; set; }
        public string ERPCode { get; set; }
        public string License { get; set; }
        public string DocNo1 { get; set; }

        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Agreement { get; set; }
        public string Manufacturer { get; set; }
        public string SAPCode { get; set; }

        public int DepPeriodMonth { get; set; }
        public int DepPeriod { get; set; }
        public int DepPeriodIn { get; set; }
        public int DepPeriodRem { get; set; }

        public decimal ValueInvIn { get; set; }
        public decimal ValueDepIn { get; set; }
        public decimal ValueDepPU { get; set; }
        public decimal ValueDepYTDIn { get; set; }

        public decimal ValueDepYTD { get; set; }
        public decimal ValueRet { get; set; }
        public decimal ValueRetIn { get; set; }
        public decimal ValueDepPUIn { get; set; }


        public decimal ValueTr { get; set; }
		public decimal ValueTrIn { get; set; }
		public decimal ValueRem { get; set; }
		public decimal ValueRemIn { get; set; }


        public decimal InvestSupport { get; set; }
        public decimal WriteUps { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? RemovalDate { get; set; }

        public bool Validated { get; set; }

        public int? TaxId { get; set; }
        public string HeaderText { get; set; }
        public decimal TptalAmount { get; set; }
    }
}
