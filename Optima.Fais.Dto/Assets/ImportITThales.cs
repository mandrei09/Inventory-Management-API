using System;

namespace Optima.Fais.Dto
{
    public class ImportITThales
    {
		public string Company { get; set; }
		public string Description { get; set; }
		public string SN { get; set; }
		public string InvState { get; set; }
		public string Department { get; set; }
		public string ERPCode { get; set; }
		public string Employee { get; set; }
		public string Location { get; set; }
		public string Room { get; set; }
		public string PODate { get; set; }
		public decimal Value { get; set; }
		public string PONumber { get; set; }
		public string AssetNature { get; set; }
		public string SAPCode { get; set; }
		public string Brand { get; set; }
		public int Quantity { get; set; } = 1;
	}
}
