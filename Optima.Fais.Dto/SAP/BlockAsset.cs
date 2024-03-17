using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class BlockAsset
	{
		public string Sap_function { get; set; }
		public BlockAssetDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<BlockAssetData> Data { get; set; }
	}

	public class BlockAssetData
	{
		public BlockAssetInput I_INPUT { get; set; }
	}

	public class BlockAssetInput
	{
		public string COMPANYCODE { get; set; }
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string BLOCK { get; set; }
		public string OPTIMA_ASSET_NO { get; set; }
		public string OPTIMA_ASSET_PARENT_NO { get; set; }
	}

	public class BlockAssetDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class BlockAssetSAP
	{
		public string COMPANYCODE { get; set; }
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string BLOCK { get; set; }
		public string OPTIMA_ASSET_NO { get; set; }
		public string OPTIMA_ASSET_PARENT_NO { get; set; }
	}
}
