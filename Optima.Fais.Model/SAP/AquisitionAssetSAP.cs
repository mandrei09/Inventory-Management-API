using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Model
{
	public class AcquisitionAssetSAP : Entity
	{
		public string STORNO { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string HEADER_TXT { get; set; }
		public string VENDOR_NO { get; set; }
		public string CURRENCY { get; set; }
		public decimal EXCH_RATE { get; set; }
		public decimal TOTAL_AMOUNT { get; set; }


		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string ITEM_TEXT { get; set; }
		public string TAX_CODE { get; set; }
		public decimal NET_AMOUNT { get; set; }
		public decimal TAX_AMOUNT { get; set; }
		public string GL_ACCOUNT { get; set; }
		public string ASVAL_DATE { get; set; }

		public string WBS_ELEMENT { get; set; }
		
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

		public bool IsTesting { get; set; }

		// public virtual AcquisitionAssets AcquisitionAssets { get; set; }
	}
}
