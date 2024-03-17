namespace Optima.Fais.Model
{
    public class AssetDepMD
    {
        public int AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public int AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public decimal CurrentAPC { get; set; }

        public int UsefulLife { get; set; }

        public decimal PosCap { get; set; }

        public int RemLifeInPeriods { get; set; }

        public decimal BkValFYStart { get; set; }

        public int TotLifeInpPeriods { get; set; }

        public decimal AccumulDep { get; set; }

        public decimal DepForYear { get; set; }

        public decimal APCFYStart { get; set; }

        public decimal DepPostCap { get; set; }

        public decimal CurrBkValue { get; set; }

        public int ExpLifeInPeriods { get; set; }

        public decimal DepFYStart { get; set; }

        public decimal Acquisition { get; set; }

        public decimal DepRetirement { get; set; }

        public decimal Retirement { get; set; }

        public decimal DepTransfer { get; set; }

        public decimal Transfer { get; set; }

        public decimal InvestSupport { get; set; }

        public decimal WriteUps { get; set; }
    }
}
