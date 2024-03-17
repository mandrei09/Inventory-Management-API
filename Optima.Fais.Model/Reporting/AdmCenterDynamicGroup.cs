using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
	public class AdmCenterDynamicGroup
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public decimal Fy2019Actual { get; set; }
		public decimal Fy2020Actual { get; set; }
		public decimal Fy2021Actual { get; set; }
		public decimal Fy2022BudgetBase { get; set; }
		public decimal Fy2022BudgetDynamic { get; set; }

		public decimal VarView12MonthActual { get; set; }
		public decimal VarView12MonthBudget { get; set; }

		public decimal ViewYTDBudgetBase { get; set; }
		public decimal ViewYTDBudgetDynamic { get; set; }
		public decimal ViewYTDActual { get; set; }

		public decimal VarYTDActual { get; set; }
		public decimal VarYTDBudget { get; set; }

		public decimal ViewPerMonthBudgetBase { get; set; }
		public decimal ViewPerMonthBudgetDynamic { get; set; }
		public decimal ViewPerMonthActual { get; set; }

		public decimal VarPerMonthActual { get; set; }
		public decimal VarPerMonthBudget { get; set; }
		public int Parent { get; set; }
		public string AdmCenter { get; set; }
		public string Table { get; set; }
	}

}