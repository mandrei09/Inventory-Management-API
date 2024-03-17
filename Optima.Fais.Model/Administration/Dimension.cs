namespace Optima.Fais.Model
{
    public class Dimension : Entity
    {

        public string Length { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public int? AssetCategoryId { get; set; }

        public AssetCategory AssetCategory { get; set; }

		public int? ERPId { get; set; }

	}
}
