using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Optima.Fais.Dto
{
	public class GetPartners
	{
		public string Sap_function { get; set; }
		public PartnersOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<PartnersData> data { get; set; }
	}

	public class PartnersData
	{
		public PartnersInputs I_INPUT { get; set; }
	}

	public class PartnersOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class PartnersInputs
	{
		public IList<PartnersModel> VENDORS { get; set; }
	}

	public class PartnersModel
	{
		//[JsonProperty(PropertyName = "")]
		public string REGISTRYNUMBER { get; set; }
	}
}
