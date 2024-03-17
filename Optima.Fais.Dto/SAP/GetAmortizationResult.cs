using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class GetAmortizationResult
	{
		public MetaAmortization Meta { get; set; }
		public DataOutPutAmortization Data { get; set; }
		public List<DataOutPutErrorAmortization> Errors { get; set; }
	}
}

public class MetaAmortization
{
	public int Code { get; set; }
}

public class DataOutPutAmortization
{
	public List<OutPutAmortization> E_FA_AMORTIZATION { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
}

public class DataOutPutErrorAmortization
{
	public string Detail { get; set; }
	public MetaErrorAmortization Meta { get; set; }
}

public class OutPutAmortization
{
	public string ANLN1 { get; set; }
	public string ANLN2 { get; set; }
	public string NDJAR { get; set; }
	public string NDPER { get; set; }
	public string NDABJ { get; set; }
	public decimal APC_FY_START { get; set; }
	public decimal DEP_FY_START { get; set; }
	public decimal ACQUISITION { get; set; }
	public decimal DEP_FY { get; set; }
	public decimal ANBTR_R { get; set; }
	public decimal DEPRET { get; set; }
	public decimal ANBTR_T { get; set; }
	public decimal DEPTRANS { get; set; }
	
}

public class MetaErrorAmortization
{
	public string Exception_Class { get; set; }
	public string Original_sap_response { get; set; }
}
