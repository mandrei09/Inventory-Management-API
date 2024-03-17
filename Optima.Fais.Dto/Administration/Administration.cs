namespace Optima.Fais.Dto
{
    public class Administration
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public CodeNameEntity Division { get; set; }

        public CodeNameEntity CostCenter { get; set; }

        public System.DateTime ModifiedAt { get; set; }

		public bool AssetCount { get; set; }
	}
}
