using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class DeleteAsset
	{
		public string Sap_function { get; set; }
		public DeleteAssetDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<DeleteAssetData> Data { get; set; }
	}

	public class DeleteAssetData
	{
		public DeleteAssetInput I_INPUT { get; set; }
	}

	public class DeleteAssetInput
	{
		public string COMPANYCODE { get; set; }
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string DELETE_IND { get; set; }
		public string OPTIMA_ASSET_NO { get; set; }
		public string OPTIMA_ASSET_PARENT_NO { get; set; }
	}

	public class DeleteAssetDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class DeleteAssetSAP
	{
		public string COMPANYCODE { get; set; }
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string DELETE_IND { get; set; }
		public string OPTIMA_ASSET_NO { get; set; }
		public string OPTIMA_ASSET_PARENT_NO { get; set; }
	}
}
