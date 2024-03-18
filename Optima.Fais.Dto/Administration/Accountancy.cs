using System;

namespace Optima.Fais.Dto
{
    public class Accountancy
    {
        public int Id { get; set; }
        public CodeNameEntity Account { get; set; }
        public CodeNameEntity ExpAccount { get; set; }
        public CodeNameEntity AssetType { get; set; }
        public CodeNameEntity AssetCategory { get; set; }
        public SubCategory SubCategory { get; set; }
        public CodeNameEntity Category { get; set; }
        public decimal Value { get; set; }
    }
}
