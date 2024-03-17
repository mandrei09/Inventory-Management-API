using System;

namespace Optima.Fais.Dto
{
    public class BudgetCorrectionSave
    {
        public decimal AprilForecast { get; set; }
        public decimal MayForecast { get; set; }
        public decimal JuneForecast { get; set; }
        public decimal JulyForecast { get; set; }
        public decimal AugustForecast { get; set; }
        public decimal SeptemberForecast { get; set; }
        public decimal OctomberForecast { get; set; }
        public decimal NovemberForecast { get; set; }
        public decimal DecemberForecast { get; set; }
        public decimal JanuaryForecast { get; set; }
        public decimal FebruaryForecast { get; set; }
        public decimal MarchForecast { get; set; }
        public decimal AprilForecastDstNew { get; set; }
        public decimal MayForecastDstNew { get; set; }
        public decimal JuneForecastDstNew { get; set; }
        public decimal JulyForecastDstNew { get; set; }
        public decimal AugustForecastDstNew { get; set; }
        public decimal SeptemberForecastDstNew { get; set; }
        public decimal OctomberForecastDstNew { get; set; }
        public decimal NovemberForecastDstNew { get; set; }
        public decimal DecemberForecastDstNew { get; set; }
        public decimal JanuaryForecastDstNew { get; set; }
        public decimal FebruaryForecastDstNew { get; set; }
        public decimal MarchForecastDstNew { get; set; }
        public int BudgetForecastId { get; set; }
		public int BudgetForecastDestinationId { get; set; }
		public string UserId { get; set; }
        public int? SrcEmployeeId { get; set; }
		public int? DstEmployeeId { get; set; }
	}
}
