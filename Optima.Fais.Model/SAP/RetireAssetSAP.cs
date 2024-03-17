using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Model
{
	public class RetireAssetSAP : Entity
	{
		public string COMPANYCODE { get; set; }
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string VALUEDATE { get; set; }
		public string ITEM_TEXT { get; set; }
		public string FIS_PERIOD { get; set; }
		public string DOC_TYPE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string COMPL_RET { get; set; }
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

		public int AssetId { get; set; }
		[ForeignKey("AssetId")]
		public virtual Asset Asset { get; set; }

		public bool NotSync { get; set; }
		public Guid Guid { get; set; }
		public int SyncErrorCount { get; set; }

		public int? ErrorId { get; set; }
		public virtual Error Error { get; set; }

		public string STORNO { get; set; }

		public string STORNO_DOC { get; set; }

		public string STORNO_DATE { get; set; }

		public string STORNO_REASON { get; set; }

		public bool IsTesting { get; set; }
	}
}
