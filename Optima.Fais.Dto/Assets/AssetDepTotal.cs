using System;

namespace Optima.Fais.Dto
{
    public class AssetDepTotal
    {
        public int Count { get; set; }

        public decimal DepFYStart { get; set; }

        public decimal APCFYStart { get; set; }

        public decimal BkValFYStart { get; set; }

        public decimal CurrBkValue { get; set; }

        public decimal CurrentAPC { get; set; }

        public decimal DepTransfer { get; set; }

        public decimal DepRetirement { get; set; }

        public decimal Transfer { get; set; }

        public decimal Retirement { get; set; }

        public decimal Acquisition { get; set; }

        public decimal DepForYear { get; set; }

        public decimal AccumulDep { get; set; }
    }
}
