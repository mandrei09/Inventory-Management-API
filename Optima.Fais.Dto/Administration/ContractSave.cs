using System;

namespace Optima.Fais.Dto
{
    public class ContractSave
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? CompanyId { get; set; }
        public int? DivisionId { get; set; }
        public int? EmployeeId { get; set; }
        public int? AccMonthId { get; set; }
        public int? PartnerId { get; set; }
        public int? UomId { get; set; }
        public decimal ValueIni { get; set; }
        public decimal Price { get; set; }
        public decimal ValueFin { get; set; }
        public float Quantity { get; set; }
        public string Info { get; set; }
        public bool Validated { get; set; }
		public string UserId { get; set; }
		public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
