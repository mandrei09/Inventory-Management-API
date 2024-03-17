using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Order
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public CodeNameEntity Company { get; set; }

        public ApplicationUser User { get; set; }

        public EmployeeResource Employee { get; set; }

        public MonthEntity AccMonth { get; set; }

        public int? PartnerId { get; set; }

        public CodePartnerEntity Partner { get; set; }

        public int? AppStateId { get; set; }

        public AppState AppState { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public decimal Price { get; set; }

        public decimal PriceRon { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueIniRon { get; set; }

        public decimal ValueFin { get; set; }

        public decimal ValueFinRon { get; set; }

        public decimal ValueUsed { get; set; }

		public decimal ValueUsedRon { get; set; }

		public CodeNameEntity BudgetManager { get; set; }

        public Offer Offer { get; set; }

        public CodeNameEntity Uom { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public string InfoMinus { get; set; }

        public bool IsDeleted { get; set; }

        public CodeNameEntity Division { get; set; }

        public CodeNameEntity ProjectType { get; set; }

		public CodeNameEntity Project { get; set; }

		public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity OrderType { get; set; }

        public Contract Contract { get; set; }

        public decimal PreAmount { get; set; }

		public virtual EmployeeResource EmployeeB1 { get; set; }

		public virtual EmployeeResource EmployeeL1 { get; set; }

        public virtual EmployeeResource EmployeeL2 { get; set; }

        public virtual EmployeeResource EmployeeL3 { get; set; }

        public virtual EmployeeResource EmployeeL4 { get; set; }

        public virtual EmployeeResource EmployeeS1 { get; set; }

        public virtual EmployeeResource EmployeeS2 { get; set; }

        public virtual EmployeeResource EmployeeS3 { get; set; }

		public virtual EmployeeResource Owner { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }
	}
}
