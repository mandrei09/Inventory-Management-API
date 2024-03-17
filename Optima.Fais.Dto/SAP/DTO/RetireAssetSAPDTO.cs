using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class RetireAssetSAPDTO
	{
		public int Id { get; set; }
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
		public string STORNO { get; set; }
		public string STORNO_DOC { get; set; }
		public string STORNO_DATE { get; set; }
		public string STORNO_REASON { get; set; }
		public bool NotSync { get; set; }
		public int SyncErrorCount { get; set; }
		public Error Error { get; set; }
		public Asset AssetEntity { get; set; }
	}
}
