using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetInvMinusResult
	{
		public Meta Meta { get; set; }
		public AssetInvMinusDataOutPut Data { get; set; }
	}
}

public class AssetInvMinusMeta
{
	public int Code { get; set; }
}

public class AssetInvMinusDataOutPut
{
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}
