using System;

namespace Optima.Fais.Dto
{
    public class BudgetSave
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
		public decimal DepPeriod { get; set; }
        public decimal DepPeriodRem { get; set; }
        public decimal ValueRem { get; set; }
        public int? EmployeeId { get; set; }
        public int? ProjectId { get; set; }
        public int? CountryId { get; set; }
        public int? ActivityId { get; set; }
        public int? DepartmentId { get; set; }
        public int? AdmCenterId { get; set; }
		public int? RegionId { get; set; }
		public int? DivisionId { get; set; }
        public int? ProjectTypeId { get; set; }
        public int? AssetTypeId { get; set; }
        public int? AppStateId { get; set; }
        public int? StartAccMonthId { get; set; }
        public string Info { get; set; }
        public bool Validated { get; set; }
		public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal Octomber { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
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
        public decimal AprilForecastNew { get; set; }
        public decimal MayForecastNew { get; set; }
        public decimal JuneForecastNew { get; set; }
        public decimal JulyForecastNew { get; set; }
        public decimal AugustForecastNew { get; set; }
        public decimal SeptemberForecastNew { get; set; }
        public decimal OctomberForecastNew { get; set; }
        public decimal NovemberForecastNew { get; set; }
        public decimal DecemberForecastNew { get; set; }
        public decimal JanuaryForecastNew { get; set; }
        public decimal FebruaryForecastNew { get; set; }
        public decimal MarchForecastNew { get; set; }
        public int BudgetForecastId { get; set; }
        // public int BudgetBaseNewId { get; set; }

        //public int? OrderId { get; set; }
        public int? RequestBudgetForecastId { get; set; }
		public int? RequestId { get; set; }
		public string UserId { get; set; }
    }
}
