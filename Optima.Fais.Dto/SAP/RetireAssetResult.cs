using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class RetireAssetResult
	{
		public Meta Meta { get; set; }
		public RetireAssetDataOutPut Data { get; set; }
	}
}

public class RetireAssetMeta
{
	public int Code { get; set; }
}

public class RetireAssetDataOutPut
{
	public RetireAssetOutPut E_OutPut { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}

public class RetireAssetOutPut
{
	public string BUKRS { get; set; }
	public string BELNR { get; set; }
	public string GJAHR { get; set; }
}
