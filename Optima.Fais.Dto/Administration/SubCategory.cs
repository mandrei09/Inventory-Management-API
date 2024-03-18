using System;

namespace Optima.Fais.Dto
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public CodeNameEntity CategoryEN { get; set; }
        public CodeNameEntity AssetType { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
