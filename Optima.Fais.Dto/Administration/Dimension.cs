namespace Optima.Fais.Dto
{
    public class Dimension
    {
        public int Id { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public CodeNameEntity AssetCategory { get; set; }
        public CodeNameEntity AssetType { get; set; }
    }
}
