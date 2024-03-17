using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetInvPlusResult
	{
		public Meta Meta { get; set; }
		public AssetInvPlusDataOutPut Data { get; set; }
	}
}

public class AssetInvPlusMeta
{
	public int Code { get; set; }
}

public class AssetInvPlusDataOutPut
{
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}
