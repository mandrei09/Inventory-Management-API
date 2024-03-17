namespace Optima.Fais.Dto
{
    public class DictionaryItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public CodeNameEntity DictionaryType { get; set; }

        public CodeNameEntity AssetCategory { get; set; }

        public System.DateTime ModifiedAt { get; set; }
    }
}
