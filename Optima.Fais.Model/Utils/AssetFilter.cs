using System;
using System.Collections.Generic;

namespace Optima.Fais.Model.Utils
{
    public class AssetFilter
    {
        //public string Includes { get; set; }

        public int? AccSystemId { get; set; }
        public int? AccMonthId { get; set; }
        public List<int?> AssetTypeIds { get; set; }
        public List<int?> DivisionIds { get; set; }
        public List<int?> AdministrationIds { get; set; }
        public List<int?> AssetStateIds { get; set; }
        public List<int?> InvStateIds { get; set; }
        public List<int?> AssetClassIds { get; set; }
        public List<int?> AssetCategoryIds { get; set; }
        public List<int?> AssetNatureIds { get; set; }
        public List<int?> CompanyIds { get; set; }
        public List<int?> InterCompanyIds { get; set; }
        public List<int?> InsuranceCategoryIds { get; set; }
        public List<int?> RegionIds { get; set; }
        public List<int?> LocationIds { get; set; }
        public List<int?> RoomIds { get; set; }
        public List<int?> DepartmentIds { get; set; }
        public List<int?> EmployeeIds { get; set; }
        public List<int?> AdmCenterIds { get; set; }
        public List<int?> CostCenterIds { get; set; }
        public List<int?> UomIds { get; set; }
        public List<int?> DimensionIds { get; set; }
        public List<int?> ExpAccountIds { get; set; }
        public List<int?> DictionaryItemIds { get; set; }
        public List<int?> ProjectIds { get; set; }
        public List<int?> BrandIds { get; set; }
        public List<int?> TransferEmployeeIds { get; set; }

        public List<int?> DocumentTypeIds { get; set; }
        public List<int?> PartnerIds { get; set; }
		public List<int?> RequestIds { get; set; }

		public bool? Custody { get; set; }
        public string Filter { get; set; }
        public string FilterDoc { get; set; }
        public bool FilterInv { get; set; }
        public bool FilterName { get; set; }
        public string FilterPurchaseDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromReceptionDate { get; set; }
        public DateTime? ToReceptionDate { get; set; }
        public bool ShowReco { get; set; }
        public bool? ErpCode { get; set; }
        public bool? IsPrinted { get; set; }
        public bool? IsDuplicate { get; set; }
		public bool? IsTemp { get; set; }
		public string UserName { get; set; }
        public string Role { get; set; }
		public int EmployeeId { get; set; }
		public bool ShowValues { get; set; } = false;
        public string Export { get; set; }
		public DateTime? MonthYear { get; set; }
        public int WfhYear { get; set; } = 2023;
		public List<int?> EmpCostCenterIds { get; set; }
		public List<int?> EmpCompanyIds { get; set; }
		public bool ShowAsignedTemp { get; set; } = false;
		public DateTime?[] RangeDates { get; set; }
		//public DocumentFilterIds DocumentFilter { get; set; }
		//public AssetFilterIds AssetFilterIds { get; set; }
	}
}
