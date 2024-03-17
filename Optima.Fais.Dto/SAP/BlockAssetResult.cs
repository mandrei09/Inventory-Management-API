using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class BlockAssetResult
	{
		public Meta Meta { get; set; }
		public BlockAssetDataResult Data { get; set; }
	}
}

public class BlockAssetMeta
{
	public int Code { get; set; }
}

public class BlockAssetDataResult
{
	public string Asset { get; set; }
	public string Optima_Asset_No { get; set; }
	public string Optima_Asset_Parent_No { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
	public string SubNumber { get; set; }
}
