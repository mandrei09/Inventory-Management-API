using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Model
{
	public class TransferInStockSAP : Entity
	{
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

		public int BudgetManagerId { get; set; }
		public virtual BudgetManager BudgetManager { get; set; }
		public int AccMonthId { get; set; }
		public virtual AccMonth AccMonth { get; set; }
		public int? CreateAssetSAPId { get; set; }
		public virtual CreateAssetSAP CreateAssetSAP { get; set; }
		public int? ErrorId { get; set; }
		public virtual Error Error { get; set; }


		public int AssetStockId { get; set; }
		[ForeignKey("AssetStockId")]
		public virtual Asset AssetStock { get; set; }

		public bool NotSync { get; set; }
		public Guid Guid { get; set; }
		public int SyncErrorCount { get; set; }

		public string Ref_Doc_No { get; set; }
		public string Header_Txt { get; set; }

		public string Storno { get; set; }

		public string Storno_Doc { get; set; }

		public string Storno_Date { get; set; }

		public string Storno_Year { get; set; }

		public string Storno_User { get; set; }

		public bool IsTesting { get; set; }
	}
}
