using System;

namespace Optima.Fais.Model
{
    public class AssetDep
    {
        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public DateTime? UsageStartDate { get; set; }

        //public int? AssetStateIdIn { get; set; }

        //public virtual AssetState AssetStateIn { get; set; }

        public DateTime? UsageEndDate { get; set; }

        public decimal ValueInvIn { get; set; }

        public int DepPeriodIn { get; set; }

        public decimal ValueRemIn { get; set; }

        public int DepPeriodRemIn { get; set; }

        public decimal ValueDepPUIn { get; set; }

        public int DepPeriodMonthIn { get; set; }

        public decimal ValueDepIn { get; set; }

        public decimal ValueDepYTDIn { get; set; }

        public bool? DirectExpense { get; set; }

        public decimal ValueInv { get; set; }

        public int DepPeriod { get; set; }

        public decimal ValueRem { get; set; }

        public int DepPeriodRem { get; set; }

        public decimal ValueDepPU { get; set; }

        public int DepPeriodMonth { get; set; }

        public decimal ValueDep { get; set; }

        public decimal ValueDepYTD { get; set; }

        public decimal ValueRet { get; set; }

        public decimal ValueRetIn { get; set; }

        public decimal ValueTr { get; set; }

        public decimal ValueTrIn { get; set; }

    }
}
