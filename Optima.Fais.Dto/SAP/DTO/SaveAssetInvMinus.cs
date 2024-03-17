using System;

namespace Optima.Fais.Dto
{
    public class SaveAssetInvMinus
    {
		public int InventoryId { get; set; }
		public int AssetId { get; set; }
		public string ItemText { get; set; }
		public string RefDocNo { get; set; }
	}
}
