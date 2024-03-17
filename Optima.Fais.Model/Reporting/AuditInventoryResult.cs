using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
	public class AuditInventoryResult
	{
		[Key]
		public int Id { get; set; }

		public DateTime? InventoryStartDate { get; set; }
		public DateTime? InventoryEndDate { get; set; }
		public string Location { get; set; }
		public string Region { get; set; }
		public decimal ValueMinus { get; set; } = 0;
        public decimal ValueInMinus { get; set; } = 0;
        public decimal ValueNotScanned { get; set; } = 0;
		public decimal ValueScanned { get; set; } = 0;
		public int Items { get; set; } = 0;
		public decimal ValueItems { get; set; } = 0;
        public decimal ValueIn { get; set; } = 0;
        public int TransIn { get; set; } = 0;
		public decimal ValueTransIn { get; set; } = 0;
		public int TransOut { get; set; } = 0;
		public decimal ValueTransOut { get; set; } = 0;
		public int NotScanned { get; set; } = 0;
		public int Scanned { get; set; } = 0;
		public int Minus { get; set; } = 0;
		public int Plus { get; set; } = 0;
		public int Cassation { get; set; } = 0;
		public string Member1 { get; set; }
		public string Member2 { get; set; }
		public string Member3 { get; set; }
		public string Member4 { get; set; }
		public string Member5 { get; set; }
		public string Member6 { get; set; }
		public string Member7 { get; set; }
		public string InventoryName { get; set; }


		//public int Temporary { get; set; }
		//public int TranInLocation { get; set; }
		//public decimal TranInLocationValue { get; set; }
		//public int TranBetweenLocations { get; set; }
		//public int TranInAdmCenter { get; set; }
		//public decimal TranInAdmCenterValue { get; set; }
		//public int TranBetweenAdmCenters { get; set; }
		//public decimal TranBetweenAdmCentersValue { get; set; }
		//public int TranBetweenCostCenters { get; set; }
		//public decimal TranBetweenCostCentersValue { get; set; }
		//public string Route { get; set; }
		

		
		
		
		//public decimal PlusValue { get; set; }
		
		//public DateTime? MinDateAdmCenter { get; set; }
		//public DateTime? MaxDateAdmCenter { get; set; }
		//public DateTime? MinDateLocation { get; set; }
		//public DateTime? MaxDateLocation { get; set; }
		
		//public decimal CassationValue { get; set; }
		//public int DiffAdmCenter { get; set; }
		//public int DiffLocation { get; set; }
		//public int AssetCount { get; set; }
		//public int EmployeeCount { get; set; }
		//public int LocationCount { get; set; }
		//public int RoomCount { get; set; }

	}
}
