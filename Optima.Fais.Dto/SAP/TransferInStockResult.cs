using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class TransferInStockResult
	{
		public TransferInMeta Meta { get; set; }
		public TransferInDataOutPut Data { get; set; }
	}
}

public class TransferInMeta
{
	public int Code { get; set; }
}

public class TransferInDataOutPut
{
	public TransferInOutPut E_OutPut { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}

public class TransferInOutPut
{
	public string Mat_Doc { get; set; }
	public string Doc_Year { get; set; }
	
}
