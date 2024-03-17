using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Dto.Reporting
{
	public class InventoryListEmail
	{
		[Key]
		public int AssetId { get; set; }
		public int? AppStateId { get; set; }
		public CodeNameEntity AppState { get; set; }
		public string InvNo { get; set; }
		public string Description { get; set; }
		public DateTime? PurchaseDate { get; set; }
		public decimal ValueDep { get; set; }
		public CodeNameEntity MasterType { get; set; }
		public CodeNameEntity Type { get; set; }
		public CodeNameEntity Brand { get; set; }
		public CodeNameEntity Model { get; set; }
		public string SerialNumber { get; set; }
		public string InternalCode { get; set; }
		public EmployeeResource FullName { get; set; }
		public string Reason { get; set; }
		public bool IsMinus { get; set; }
		public bool IsPlus { get; set; }
		public string InfoMinus { get; set; }
		public string InfoPlus { get; set; }
	}
}
