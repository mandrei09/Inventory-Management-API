using System;

namespace Optima.Fais.Dto
{
    public class AssetSave
    {
        public int Id { get; set; }

        public string InvNo { get; set; }
        public string InvNoOld { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string ERPCode { get; set; }
        public string Info { get; set; }
       
        public string DocNo1 { get; set; }
        public string DocNo2 { get; set; }
        public string Details { get; set; }
        public string Model { get; set; }
      
       
        public decimal ValueInv { get; set; }
		public decimal ValueDep { get; set; }
		public decimal ValueDepPU { get; set; }
		public decimal ValueRem { get; set; }

		public int DepPeriodMonth { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? DocumentId { get; set; }
        public int? AssetCategoryId { get; set; }
        public int? CompanyId { get; set; }
        public int? DictionaryItemId { get; set; }
        public int? InvStateId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoomId { get; set; }
        public int? InsuranceCategoryId { get; set; }
        public int? ModelId { get; set; }
        public int? BrandId { get; set; }
        public int? AssetNatureId { get; set; }
        public int? BudgetManagerId { get; set; }
        public int? OrderId { get; set; }
        public int? BudgetId { get; set; }
        public int? DimensionId { get; set; }
        public int? AssetTypeId { get; set; }
        public int? PartnerId { get; set; }
        public int? AdministrationId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ProjectId { get; set; }
       
        public int? SubTypeId { get; set; }
        public int? CostCenterId { get; set; }
        
        public int? UomId { get; set; }

        public int? AdmCenterId { get; set; }
        public int? RegionId { get; set; }
        public int? DivisionId { get; set; }
        public int? ProjectTypeId { get; set; }

        public float Quantity { get; set; }

        public DateTime? DocumentDate { get; set; }
        public DateTime? PartnerDate { get; set; }
        public DateTime? ReceptionDate { get; set; }
		public DateTime? InvoiceDate { get; set; }
		public DateTime? PODate { get; set; }
		public DateTime? RemovalDate { get; set; }
		
        public string ModelInv { get; set; }
        public string ProducerInv { get; set; }

        public bool Validated { get; set; }
        public bool IsAccepted { get; set; }
    }
}
