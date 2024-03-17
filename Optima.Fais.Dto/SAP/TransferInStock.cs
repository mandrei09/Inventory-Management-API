using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Dto
{
	public class TransferInStock
	{
		public string Sap_function { get; set; }
		public TransferInStockOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<TransferInStockData> Data { get; set; }
	}

	public class TransferInStockData
	{
		public TransferInStockInput I_INPUT { get; set; }
	}

	public class TransferInStockInput
	{
		public string Doc_Date { get; set; }
		public string Pstng_Date { get; set; }
		public string Plant { get; set; }
		public string Storage_Location { get; set; }
		public string Material { get; set; }
		public decimal Quantity { get; set; }
		public string Uom { get; set; }
		public string Batch { get; set; }
		public string Gl_Account { get; set; }
		public string Item_Text { get; set; }
		public string Asset { get; set; }
		public string SubNumber { get; set; }

		public string Ref_Doc_No { get; set; }
		public string Header_Txt { get; set; }

		public string Storno { get; set; }

		public string Storno_Doc { get; set; }

		public string Storno_Date { get; set; }

		public string Storno_Year { get; set; }

		public string Storno_User { get; set; }
		//[NotMapped]
		//public int Id { get; set; }
	}

	public class TransferInStockOptions
	{
		public int Api_call_timeout { get; set; }
	}
}
