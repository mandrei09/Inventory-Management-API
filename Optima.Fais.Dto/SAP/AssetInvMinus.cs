using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetInvMinus
	{
		public string Sap_function { get; set; }
		public AssetInvMinusDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<AssetInvMinusData> Data { get; set; }
	}

	public class AssetInvMinusData
	{
		public AssetInvMinusInput I_INPUT { get; set; }
	}

	public class AssetInvMinusInput
	{
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string ASVAL_DATE { get; set; }
		public string ITEM_TEXT { get; set; }
		public string DOC_TYPE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string TRANSTYPE { get; set; }
		public string INVENTORY_DIFF { get; set; }
		//public decimal AMOUNT { get; set; }
		
	}

	public class AssetInvMinusDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class AssetInvMinusSAP
	{
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string ASVAL_DATE { get; set; }
		public string ITEM_TEXT { get; set; }
		public string DOC_TYPE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string TRANSTYPE { get; set; }
		public string INVENTORY_DIFF { get; set; }
		//public decimal AMOUNT { get; set; }
	}
}
