using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class BudgetMonthBase : Entity
    {

        public int BudgetBaseId { get; set; }
        [ForeignKey("BudgetBaseId")]
        public virtual BudgetBase BudgetBase { get; set; }

        public int BudgetTypeId { get; set; }

        public virtual BudgetType BudgetType { get; set; }

        public int BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public bool IsLast { get; set; }

        public bool IsFirst { get; set; }

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

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

    }
}
