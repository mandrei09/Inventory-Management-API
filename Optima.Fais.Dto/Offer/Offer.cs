using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Offer
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public CodeNameEntity Company { get; set; }

        public CodeNameEntity Division { get; set; }


        [MaxLength(450)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public EmployeeResource Employee { get; set; }

        public MonthEntity AccMonth { get; set; }

        public CodePartnerEntity Partner { get; set; }

        public int? AppStateId { get; set; }

        public AppState AppState { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool Validated { get; set; }

        public float Quantity { get; set; }

        public float QuantityRem { get; set; }

        public float QuantityUsed { get; set; }

        public decimal ValueIni { get; set; }

        public decimal ValueFin { get; set; }

        public CodeNameEntity BudgetManager { get; set; }

        public bool IsAccepted { get; set; }

        public string Info { get; set; }

        public string InfoMinus { get; set; }

        public bool IsDeleted { get; set; }

        public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity ProjectType { get; set; }

        public Request Request { get; set; }

        public ICollection<Dto.OfferMaterial> OfferMaterials { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }
	}
}
