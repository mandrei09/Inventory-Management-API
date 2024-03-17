using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
	public class WFHCheck : Entity
	{

		public int? DictionaryItemId { get; set; }

		public virtual DictionaryItem DictionaryItem { get; set; }

		public int BrandId { get; set; }

		public virtual Brand Brand { get; set; }

		public int? ModelId { get; set; }

		public virtual Model Model { get; set; }

		public string Imei { get; set; }

		public string SerialNumber { get; set; }

		public string InventoryNumber { get; set; }

		public int? BudgetManagerId { get; set; }

		public virtual BudgetManager BudgetManager { get; set; }

		public int? EmployeeId { get; set; }

		public virtual Employee Employee { get; set; }

		public int? AssetId { get; set; }

		public virtual Asset Asset { get; set; }
	}
}
