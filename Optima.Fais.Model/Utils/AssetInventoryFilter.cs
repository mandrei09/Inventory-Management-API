using System.Collections.Generic;

namespace Optima.Fais.Model.Utils
{
    public class AssetInventoryFilter
    {
        //public string Includes { get; set; }

        public int? AccSystemId { get; set; }
        public int? InventoryId { get; set; }
        public List<int?> AssetTypeIds { get; set; }
        public List<int?> AssetStateIds { get; set; }
        public List<int?> AssetClassIds { get; set; }
        public List<int?> AssetCategoryIds { get; set; }

        public List<int?> RoomIdsIni { get; set; }
        public List<int?> LocationIdsIni { get; set; }
        public List<int?> RegionIdsIni { get; set; }

        public List<int?> DepartmentIdsIni { get; set; }
        public List<int?> EmployeeIdsIni { get; set; }

        public List<int?> CostCenterIdsIni { get; set; }
        public List<int?> AdmCenterIdsIni { get; set; }

        public List<int?> RoomIdsFin { get; set; }
        public List<int?> LocationIdsFin { get; set; }
        public List<int?> RegionIdsFin { get; set; }

        public List<int?> DepartmentIdsFin { get; set; }
        public List<int?> EmployeeIdsFin { get; set; }

        public List<int?> CostCenterIdsFin { get; set; }
        public List<int?> AdmCenterIdsFin { get; set; }


        public List<int?> RoomIdsAll { get; set; }
        public List<int?> LocationIdsAll { get; set; }
        public List<int?> RegionIdsAll { get; set; }

        public List<int?> DepartmentIdsAll { get; set; }
        public List<int?> EmployeeIdsAll { get; set; }

        public List<int?> CostCenterIdsAll { get; set; }
        public List<int?> AdmCenterIdsAll { get; set; }

        public List<int?> DocumentTypeIds { get; set; }
        public List<int?> PartnerIds { get; set; }

        public bool? Custody { get; set; }
        public string Filter { get; set; }
        public string ReportType { get; set; }
		public bool? AllowLabel { get; set; }

        public int? AccMonthId { get; set; }
    }
}
