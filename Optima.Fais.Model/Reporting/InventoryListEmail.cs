using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model.Reporting
{
	public class InventoryListEmail
	{
		[Key]
        public int AssetId { get; set; }
		public string AppState { get; set; }
		public string InvNo { get; set; }
		public string Description { get; set; }
		public DateTime? PurchaseDate { get; set; }
		public decimal ValueDep { get; set; }
		public string MasterType { get; set; }
		public string Type { get; set; }
		public string Brand { get; set; }
		public string Model { get; set; }
		public string SerialNumber { get; set; }
		public string InternalCode { get; set; }
		public string FullName { get; set; }
		public string Reason { get; set; }
	}
}
