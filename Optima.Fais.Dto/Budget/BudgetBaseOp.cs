using Optima.Fais.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Dto
{
	public class BudgetBaseOp
	{
		public int Id { get; set; }

        public virtual BudgetBase BudgetBase { get; set; }

        public virtual BudgetMonthBase BudgetMonthBase { get; set; }

        public virtual BudgetForecast BudgetForecast { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public virtual Document Document { get; set; }

        public virtual BudgetType BudgetType { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public virtual CodeNameEntity DocumentType { get; set; }

        public decimal JanuaryIni { get; set; }

        public decimal FebruaryIni { get; set; }

        public decimal MarchIni { get; set; }

        public decimal AprilIni { get; set; }

        public decimal MayIni { get; set; }

        public decimal JuneIni { get; set; }

        public decimal JulyIni { get; set; }

        public decimal AugustIni { get; set; }

        public decimal SeptemberIni { get; set; }

        public decimal OctomberIni { get; set; }

        public decimal NovemberIni { get; set; }

        public decimal DecemberIni { get; set; }


        public decimal JanuaryFin { get; set; }

        public decimal FebruaryFin { get; set; }

        public decimal MarchFin { get; set; }

        public decimal AprilFin { get; set; }

        public decimal MayFin { get; set; }

        public decimal JuneFin { get; set; }

        public decimal JulyFin { get; set; }

        public decimal AugustFin { get; set; }

        public decimal SeptemberFin { get; set; }

        public decimal OctomberFin { get; set; }

        public decimal NovemberFin { get; set; }

        public decimal DecemberFin { get; set; }


        [MaxLength(450)]
        public string InfoIni { get; set; }

        [MaxLength(450)]
        public string InfoFin { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ReleaseConfAt { get; set; }

        [MaxLength(450)]
        public string ReleaseConfBy { get; set; }

        public ApplicationUser ReleaseConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SrcConfAt { get; set; }

        [MaxLength(450)]
        public string SrcConfBy { get; set; }

        public ApplicationUser SrcConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DstConfAt { get; set; }

        [MaxLength(450)]
        public string DstConfBy { get; set; }

        public ApplicationUser DstConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? RegisterConfAt { get; set; }

        [MaxLength(450)]
        public string RegisterConfBy { get; set; }

        public ApplicationUser RegisterConfUser { get; set; }

        public virtual AppState BudgetStateInitial { get; set; }

        public Guid Guid { get; set; }

        public int DepPeriodInitial { get; set; }

        public int DepPeriodRemInitial { get; set; }

        public int DepPeriodFinal { get; set; }

        public int DepPeriodRemFinal { get; set; }

        public virtual AppState BudgetStateFinal { get; set; }

		public virtual BudgetForecast BudgetForecastFin { get; set; }

		public int? BudgetForecastId { get; set; }

		public int? BudgetForecastFinId { get; set; }

	}
}
