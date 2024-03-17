using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class BudgetMonth : Entity
    {

        public int BudgetId { get; set; }

        [ForeignKey("BudgetId")]
        public virtual Budget Budget { get; set; }

        public int AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int BudgetTypeId { get; set; }

        public virtual BudgetType BudgetType { get; set; }

		public decimal Value { get; set; }

        public decimal ValueDep { get; set; }

        //public decimal ValueTotal { get; set; }

        //public decimal ValueDepTotal { get; set; }

    }
}
