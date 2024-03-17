using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class TransferAsset
	{
		public string Sap_function { get; set; }
		public TransferAssetDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<TransferAssetData> Data { get; set; }
	}

	public class TransferAssetData
	{
		public TransferAssetInput I_INPUT { get; set; }
	}

	public class TransferAssetInput
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
	}

	public class TransferAssetDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class TransferAssetSAP
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
	}
}
