using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class BudgetMonth
	{
        public int BudgetId { get; set; }

        public Budget Budget { get; set; }

        public int AccMonthId { get; set; }

        public AccMonth AccMonth { get; set; }

        public int BudgetTypeId { get; set; }

        public BudgetType BudgetType { get; set; }

        public decimal Value { get; set; }

        public decimal ValueDep { get; set; }

		public int Id { get; set; }

		//public decimal ValueTotal { get; set; }

        //public decimal ValueDepTotal { get; set; }
    }
}
