using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Model
{
	public class TransferAssetSAP : Entity
	{
		public string FROM_ASSET { get; set; }
		public string FROM_SUBNUMBER { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string ASVAL_DATE { get; set; }
		public string ITEM_TEXT { get; set; }
		public string TO_ASSET { get; set; }
		public string TO_SUBNUMBER { get; set; }
		public string FIS_PERIOD { get; set; }
		public string DOC_TYPE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string COMPL_TRANSFER { get; set; }
		public decimal AMOUNT { get; set; }
		public string CURRENCY { get; set; }
		public decimal PERCENT { get; set; }
		public decimal QUANTITY { get; set; }
		public string BASE_UOM { get; set; }
		public string PRIOR_YEAR_ACQUISITIONS { get; set; }
		public string CURRENT_YEAR_ACQUISITIONS { get; set; }

		public int BudgetManagerId { get; set; }
		public virtual BudgetManager BudgetManager { get; set; }
		public int AccMonthId { get; set; }
		public virtual AccMonth AccMonth { get; set; }

		//public int ToAssetId { get; set; }
		////[ForeignKey("ToAssetId")]
		//public virtual Asset ToAsset { get; set; }

		//public int FromAssetId { get; set; }
		////[ForeignKey("ToAssetId")]
		//public virtual Asset FromAsset { get; set; }

		//public string FromAsset { get; set; }
		//public string ToAsset { get; set; }

		public int AssetId { get; set; }
		[ForeignKey("AssetId")]
		public virtual Asset Asset { get; set; }

		public bool NotSync { get; set; }
		public Guid Guid { get; set; }
		public int SyncErrorCount { get; set; }

		public int? ErrorId { get; set; }
		public virtual Error Error { get; set; }

		public bool FromClone { get; set; }

		public bool IsTesting { get; set; }
	}
}
