using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class BudgetForecast : Entity
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

        public decimal ValueOrder { get; set; }

        public decimal ValueAsset { get; set; }

        public decimal TotalRem { get; set; }

        public decimal DepPeriod { get; set; }

        public decimal DepPeriodRem { get; set; }

        public int? StartMonthId { get; set; }

        public virtual AccMonth StartMonth { get; set; }

        public bool HasChangeApril { get; set; }

        public bool HasChangeMay { get; set; }

        public bool HasChangeJune { get; set; }

        public bool HasChangeJuly { get; set; }

        public bool HasChangeAugust { get; set; }

        public bool HasChangeSeptember { get; set; }

        public bool HasChangeOctomber { get; set; }

        public bool HasChangeNovember { get; set; }

        public bool HasChangeDecember { get; set; }

        public bool HasChangeJanuary { get; set; }

        public bool HasChangeFebruary { get; set; }

        public bool HasChangeMarch { get; set; }

        public decimal ImportValueOrder { get; set; }

		public decimal ValueAssetYTD { get; set; }

		public decimal ValueAssetYTG { get; set; }

        public bool InTransfer { get; set; }

		public decimal ValOrderPending { get; set; }

		public decimal ValOrderApproved { get; set; }

		public decimal ValRequest { get; set; }

	}
}
