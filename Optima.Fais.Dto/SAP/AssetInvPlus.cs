using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetInvPlus
	{
		public string Sap_function { get; set; }
		public AssetInvPlusDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<AssetInvPlusData> Data { get; set; }
	}

	public class AssetInvPlusData
	{
		public AssetInvPlusInput I_INPUT { get; set; }
	}

	public class AssetInvPlusInput
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
		public decimal AMOUNT { get; set; }
		public string TRANSTYPE { get; set; }
	}

	public class AssetInvPlusDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class AssetInvPlusSAP
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
		public decimal AMOUNT { get; set; }
		public string TRANSTYPE { get; set; }
	}
}
