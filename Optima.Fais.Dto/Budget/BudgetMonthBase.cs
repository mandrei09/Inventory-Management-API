using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class BudgetMonthBase
	{
        // public int BudgetId { get; set; }

        public BudgetBase BudgetBase { get; set; }

        //public int AccMonthId { get; set; }

        //public AccMonth AccMonth { get; set; }

        //public int BudgetTypeId { get; set; }

        //public BudgetType BudgetType { get; set; }

        //public decimal Value { get; set; }

        //public decimal ValueDep { get; set; }

		public int Id { get; set; }

        //public decimal ValueTotal { get; set; }

        //public decimal ValueDepTotal { get; set; }

        public CodeNameEntity BudgetType { get; set; }

        public CodeNameEntity BudgetManager { get; set; }

        public CodeNameEntity AppState { get; set; }

        public CodeNameEntity Project { get; set; }

        public MonthEntity AccMonth { get; set; }

        public MonthEntity StartMonth { get; set; }

        public decimal January { get; set; }

        public decimal February { get; set; }

        public decimal March { get; set; }

        public decimal April { get; set; }

        public decimal May { get; set; }

        public decimal June { get; set; }

        public decimal July { get; set; }

        public decimal August { get; set; }

        public decimal September { get; set; }

        public decimal Octomber { get; set; }

        public decimal November { get; set; }

        public decimal December { get; set; }

        public decimal Total { get; set; }

        public decimal DepPeriod { get; set; }

        public decimal DepPeriodRem { get; set; }
    }
}
