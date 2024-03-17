using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class TransferInStockSAPDTO
	{
		public int Id { get; set; }
		public string Doc_Date { get; set; }
		public string Pstng_Date { get; set; }
		public string Plant { get; set; }
		public string Storage_Location { get; set; }
		public string Material { get; set; }
		public decimal Quantity { get; set; }
		public string Uom { get; set; }
		public string Batch { get; set; }
		public string Gl_Account { get; set; }
		public string Item_Text { get; set; }
		public string Asset { get; set; }
		public string SubNumber { get; set; }

		public virtual BudgetManager BudgetManager { get; set; }
		public virtual AccMonth AccMonth { get; set; }
		public virtual CreateAssetSAP CreateAssetSAP { get; set; }
		public virtual Error Error { get; set; }
		public bool NotSync { get; set; }
		public int SyncErrorCount { get; set; }
		public Asset AssetStock { get; set; }
	}
}
