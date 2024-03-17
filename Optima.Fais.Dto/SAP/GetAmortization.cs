using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class GetAmortization
	{
		public string Sap_function { get; set; }
		public AmortizationOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<AmortizationData> Data { get; set; }
	}

	public class AmortizationData
	{
		public string I_BUKRS { get; set; }
		public string I_GJAHR { get; set; }
		public string I_AFBLPE { get; set; }
	}

	public class AmortizationOptions
	{
		public int Api_call_timeout { get; set; }
	}
}
