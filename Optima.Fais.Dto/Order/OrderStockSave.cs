using Optima.Fais.Dto.Common;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class OrderStockSave
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        // public int? CompanyId { get; set; }
        // public int? ProjectId { get; set; }
        // public int? AdministrationId { get; set; }
        //public int? MasterTypeId { get; set; }
        //public int? TypeId { get; set; }
        //public int? SubTypeId { get; set; }
        //public int? EmployeeId { get; set; }
        //public int? AccMonthId { get; set; }
        //public int? InterCompanyId { get; set; }
        //public int? PartnerId { get; set; }
        //public int? UomId { get; set; }
        //
        //public int? CostCenterId { get; set; }
        //public int? OfferId { get; set; }
        //public int? BudgetId { get; set; }
        //public int? BudgetBaseId { get; set; }
        //public int? BudgetForecastId { get; set; }
        //public int? ContractId { get; set; }
        // public int? RateId { get; set; }
        //public decimal ValueIni { get; set; }
        //public decimal Price { get; set; }
        //public decimal ValueFin { get; set; }
        // public float Quantity { get; set; }
        public string Info { get; set; }
        public bool Validated { get; set; }
		public string UserId { get; set; }
		//public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
		//public decimal SumOfTotalInOtherCurrency { get; set; }
		// public List<OrderMaterialUpdate> OrderMaterialUpdates { get; set; }
        //public List<int> RequestBudgetForecasts { get; set; }
        public List<int> StockMaterialUpdates { get; set; }
        //public decimal PreAmount { get; set; }
        public int? OrderTypeId { get; set; }
		//public bool NeedBudgetAmount { get; set; }
        // public int? StartAccMonthId { get; set; }
		public int? MatrixId { get; set; }
        public int? EmployeeId { get; set; }
        public int? PlantInitialId { get; set; }
        public int? PlantFinalId { get; set; }
        public int? CategoryId { get; set; }
    }
}
