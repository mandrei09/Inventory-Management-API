using System;

namespace Optima.Fais.Dto
{
    public class AssetEditChange
    {
        public int Id { get; set; }
        public int? ExpAccountId { get; set; }
        public int? AssetCategoryId { get; set; }
        public int? PartnerId { get; set; }
        public int? CostCenterId { get; set; }
        public int? EmployeeId { get; set; }    
        public float Quantity { get; set; }
        public string DocNo1 { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public decimal ValueInv { get; set; }
        public int? BudgetForecastId { get; set; }
    }
}
