using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class GetPartnersResult
	{
		public MetaPartners Meta { get; set; }
		public DataOutPutPartners Data { get; set; }
		public List<DataOutPutErrorPartners> Errors { get; set; }
	}
}

public class MetaPartners
{
	public int Code { get; set; }
}

public class DataOutPutPartners
{
	public List<OutPutPartners> E_OUTPUT { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}

public class DataOutPutErrorPartners
{
	public string Detail { get; set; }
	public MetaErrorAmortization Meta { get; set; }
}

public class OutPutPartners
{
	public string LIFNR { get; set; }
	public string STCEG { get; set; }
	
}

public class MetaErrorPartners
{
	public string Exception_Class { get; set; }
	public string Original_sap_response { get; set; }
}
