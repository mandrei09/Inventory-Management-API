using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AcquisitionAssetResult
	{
		public AcquisitionAssetMeta Meta { get; set; }
		public AcquisitionAssetDataOutPut Data { get; set; }
	}
}

public class AcquisitionAssetMeta
{
	public int Code { get; set; }
}

public class AcquisitionAssetDataOutPut
{
	public AcquisitionAssetOutPut E_OutPut { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}

public class AcquisitionAssetOutPut
{
	public string BUKRS { get; set; }
	public string BELNR { get; set; }
	public string GJAHR { get; set; }

}