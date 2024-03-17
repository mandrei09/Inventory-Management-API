using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class ProjectDynamicGroup
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public decimal Fy2019Act { get; set; }
		public decimal Fy2020Act { get; set; }
		public decimal Fy2021Act { get; set; }
		public decimal Fy2022Budget { get; set; }
		public decimal Fy2022BudgetRem { get; set; }
		public int Parent { get; set; }
		public string Project { get; set; }
		public string Table { get; set; }
	}

}