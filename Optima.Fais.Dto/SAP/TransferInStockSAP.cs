using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class TransferInStockSAP
	{
		public int Id { get; set; }
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

		public virtual BudgetManager BudgetManager { get; set; }
		public virtual AccMonth AccMonth { get; set; }

		public virtual Asset Asset { get; set; }

		public bool NotSync { get; set; }
		public Guid Guid { get; set; }
		public int SyncErrorCount { get; set; }

		public int? ErrorId { get; set; }
		public virtual Error Error { get; set; }

		public string Ref_Doc_No { get; set; }
		public string Header_Text { get; set; }

		public string Storno { get; set; }

		public string Storno_Doc { get; set; }

		public string Storno_Date { get; set; }

		public string Storno_Year { get; set; }

		public string Storno_User { get; set; }
	}
}
