using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetOpSyncMobile
    {
        public int AssetId { get; set; }
        //public int? AdministrationId { get; set; }
        public int? RoomId { get; set; }
        public int? InvStateId { get; set; }
        public string IsDeleted { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public string SerialNumber { get; set; }
		public string Info { get; set; }
	    public string AllowLabel { get; set; }
		public string UserId { get; set; }
		//public int? DimensionId { get; set; }
		//public int? UomId { get; set; }
		//public bool IsMinus { get; set; }
		//public string InfoMinus { get; set; }
		public float? Quantity { get; set; }
		//public string Info2 { get; set; }
		//public int? AssetCategoryId { get; set; }
		//public int? AssetTypeId { get; set; }
		public string InvName { get; set; }
		//public int? CompanyId { get; set; }
		//public int? AssetNatureId { get; set; }
		//public int? InterCompanyId { get; set; }
		//public int? InsuranceCategoryId { get; set; }
		public int? EmployeeId { get; set; }
		public int? CostCenterId { get; set; }
	}
}
