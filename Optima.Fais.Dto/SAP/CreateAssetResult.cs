using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class CreateAssetResult
	{
		public Meta Meta { get; set; }
		public CreateAssetDataResult Data { get; set; }
		public List<Errors> Errors { get; set; }
	}
}

public class CreateAssetMeta
{
	public int Code { get; set; }
}

public class CreateAssetDataResult
{
	public string Asset { get; set; }
	public string Optima_Asset_No { get; set; }
	public string Optima_Asset_Parent_No { get; set; }
	public string Return_Code { get; set; }
	public string Return_Message { get; set; }
	public string SubNumber { get; set; }
}

public class Errors
{
	public string Detail { get; set; }
	public ErrorMeta Meta { get; set; }
}

public class ErrorMeta
{
	public string Exception_class { get; set; }
	public string Original_sap_response { get; set; }
}
