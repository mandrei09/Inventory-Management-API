using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class DashboardExport
	{
		[Key]
		public int Id { get; set; }
		public string InvNo { get; set; }
		public string Name { get; set; }
		public string SerialNumber { get; set; }
		public DateTime? PurchaseDate { get; set; }
		public string DepartmentInitial { get; set; }
		public string DivisionInitial { get; set; }
		public string CostCenterInitialCode { get; set; }
		public string CostCenterInitialName { get; set; }
		public string DepartmentFinal { get; set; }
		public string DivisionFinal { get; set; }
		public string CostCenterFinalCode { get; set; }
		public string CostCenterFinalName { get; set; }
		public string AssetType { get; set; }
		public string Type { get; set; }

	}
}