using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryResultDetail
    {
        public string InvNo { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string CostCenter { get; set; }
        public string Building { get; set; }
        public string Room { get; set; }
        public float Initial { get; set; }
        public float Actual { get; set; }
        public string Uom { get; set; }
        //public float QDiffPlus { get; set; }
        //public float QDiffMinus { get; set; }
        public decimal Value { get; set; }
        //public decimal ValueDiffPlus { get; set; }
        //public decimal ValueDiffMinus { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public decimal ValueDepTotal { get; set; }
        public string Info { get; set; }
        public string Custody { get; set; }
        public string InternalCode { get; set; }
        public string FullName { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int Total { get; set; }
        public int NotScanned { get; set; }
        public int Minus { get; set; }
        public int Plus { get; set; }
        public int Temp { get; set; }
        public int Tranfer { get; set; }
        public string StateNameInitial { get; set; }
        public string StateNameFinal { get; set; }

        public DateTime? EndDate { get; set; }
        public string ERPCode { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public string AssetCategory { get; set; }
        public string AssetCategoryPrefix { get; set; }
        public string AssetType { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingName { get; set; }
        public string RegionName { get; set; }
        public string RegionCode { get; set; }
        public string Producer { get; set; }
        public bool AllowLabel { get; set; }
        public string State { get; set; }
		public int? AppStateId { get; set; }
		public int Key { get; set; }


		//public string Description { get; set; }
		//public string InvNo { get; set; }
		//public string InvNoOld { get; set; }

		//public string InitialSerialNumber { get; set; }
		//public string SerialNumber { get; set; }

		//public int InitialLocationId { get; set; }
		//public string InitialLocationCode { get; set; }
		//public string InitialLocationName { get; set; }

		//public int FinalLocationId { get; set; }
		//public string FinalLocationCode { get; set; }
		//public string FinalLocationName { get; set; }

		//public string InitialCostCenterCode { get; set; }
		//public string InitialCostCenterName { get; set; }

		//public string FinalCostCenterCode { get; set; }
		//public string FinalCostCenterName { get; set; }

		//public string InitialRoomCode { get; set; }
		//public string InitialRoomName { get; set; }

		//public string FinalRoomCode { get; set; }
		//public string FinalRoomName { get; set; }

		//public int InitialEmployeeId { get; set; }
		//public string InitialInternalCode { get; set; }
		//public string InitialFirstName { get; set; }
		//public string InitialLastName { get; set; }

		//public int FinalEmployeeId { get; set; }
		//public string FinalInternalCode { get; set; }
		//public string FinalFirstName { get; set; }
		//public string FinalLastName { get; set; }

		//public string AssetClassCode { get; set; }
		//public string AssetClassName { get; set; }

		//public int InitialQuantity { get; set; }
		//public int ActualQuantity { get; set; }

		//public string MeasureUnit { get; set; }

		//public float UnitValue { get; set; }
		//public float AccountingValue { get; set; }
		//public float InventoryValue { get; set; }
		//public float DepreciationValue { get; set; }
		//public float AquisitionValue { get; set; }
		//public float AcumulatedDepreciationValue { get; set; }
		//public float RemainingAccoutingValue { get; set; }

		//public DateTime PurchaseDate { get; set; }

		//public string Custody { get; set; }

		//public string DepreciationObservation { get; set; }
		//public string AnnulememntObservation { get; set; }
	}
}
