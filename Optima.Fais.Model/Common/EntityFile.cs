using System;

namespace Optima.Fais.Model
{
    public class EntityFile : Entity
    {
        public int? EntityId { get; set; }

        public int EntityTypeId { get; set; }

        public EntityType EntityType { get; set; }

        public string FileType { get; set; }

        public string StoredAs { get; set; }

        public string Name { get; set; }

        public double Size { get; set; }

        public string Info { get; set; }

        public int? RoomId { get; set; }

        public Room Room { get; set; }

		public Guid Guid { get; set; }

        public Guid GuidAll { get; set; }

        public int? CostCenterId { get; set; }

        public CostCenter CostCenter { get; set; }

        public int? PartnerId { get; set; }

        public Partner Partner { get; set; }

        public bool IsEmailSend { get; set; }

        public int? RequestId { get; set; }

        public Request Request { get; set; }

        public int? OrderId { get; set; }

        public Order Order { get; set; }

        public int? RequestBudgetForecastId { get; set; }

        public virtual RequestBudgetForecast RequestBudgetForecast { get; set; }

        public int? ContractId { get; set; }

        public virtual Contract Contract { get; set; }

		//public int? OfferId { get; set; }

		//public virtual Offer Offer { get; set; }

		public bool SkipEmail { get; set; }

		public decimal Quantity { get; set; }

		public int? BudgetForecastId { get; set; }

		public virtual BudgetForecast BudgetForecast { get; set; }

		public int? BudgetBaseOpId { get; set; }

		public virtual BudgetBaseOp BudgetBaseOp { get; set; }

        public DateTime? ModifiedAt { get; set; }
	}
}
