using System;

namespace Optima.Fais.Dto
{
    public class InventoryAssetSync
	{

		public int Id { get; set; }
		public int InventoryId { get; set; }
		public int AssetId { get; set; }
		public float QInitial { get; set; }
		public float QFinal { get; set; }
		public int? RoomIdInitial { get; set; }
		public int? RoomIdFinal { get; set; }
		public string SerialNumber { get; set; }
		public int? StateIdFinal { get; set; }
		public int? StateIdInitial { get; set; }
		public string Info { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public string ModifiedBy { get; set; }
		public int? CostCenterIdInitial { get; set; }
		public int? CostCenterIdFinal { get; set; }
		public int ImageCount { get; set; }
		public string InfoMinus { get; set; }
		public bool IsMinus { get; set; }
		public bool IsTemp { get; set; }
		public int? AssetRecoStateId { get; set; }
		//public int? DimensionIdInitial { get; set; }
		//public int? UomIdInitial { get; set; }
		public string TempName { get; set; }
		public string TempReco { get; set; }
		public bool NeedLabel { get; set; }
		public DateTime? ScanDate { get; set; }
		public int? AssetTypeId { get; set; }
		public bool InInventory { get; set; }
		public bool IsDeleted { get; set; }
	}
}
