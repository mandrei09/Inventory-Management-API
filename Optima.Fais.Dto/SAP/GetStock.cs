using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class GetStock
	{
		public string Sap_function { get; set; }
		public StockOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<StockData> Data { get; set; }
	}

	public class StockData
	{
		public StockInput I_INPUT { get; set; }
	}

	public class StockInput
	{
		public string Plant { get; set; }
		public string Storage_Location { get; set; }
		public string Category { get; set; }
		public string Material { get; set; }
		public string Batch { get; set; }
	}

	public class StockOptions
	{
		public int Api_call_timeout { get; set; }
	}
}
