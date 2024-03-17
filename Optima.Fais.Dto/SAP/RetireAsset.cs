using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class RetireAsset
	{
		public string Sap_function { get; set; }
		public RetireAssetDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<RetireAssetData> Data { get; set; }
	}

	public class RetireAssetData
	{
		public RetireAssetInput I_INPUT { get; set; }
	}

	public class RetireAssetInput
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
		public string STORNO { get; set; }
		public string STORNO_DOC { get; set; }
		public string STORNO_DATE { get; set; }
		public string STORNO_REASON { get; set; }
	}

	public class RetireAssetDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class RetireAssetSAP
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
		public string STORNO { get; set; }
		public string STORNO_DOC { get; set; }
		public string STORNO_DATE { get; set; }
		public string STORNO_REASON { get; set; }

		public int AssetId { get; set; }
	}
}
