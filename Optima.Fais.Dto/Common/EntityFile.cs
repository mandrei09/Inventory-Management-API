namespace Optima.Fais.Dto
{
    public class EntityFile
    {
        public int Id { get; set; }

        public int EntityId { get; set; }

        public int EntityTypeId { get; set; }

		public EntityType EntityType { get; set; }

		public string FileType { get; set; }

        public string StoredAs { get; set; }

        public string Name { get; set; }

        public double Size { get; set; }

        public string Info { get; set; }

        public CodePartnerEntity Partner { get; set; }

		public Request Request { get; set; }

        public RequestBudgetForecast RequestBudgetForecast { get; set; }

        public bool SkipEmail { get; set; }

        public decimal Quantity { get; set; }

		public bool Selected { get; set; }
	}
}
