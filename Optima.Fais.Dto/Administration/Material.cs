using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Material
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
		public CodeNameEntity Account { get; set; }
        public CodeNameEntity ExpAccount { get; set; }
        public CodeNameEntity AssetCategory { get; set; }
        public CodeNameEntity SubCategory { get; set; }
        public CodeNameEntity SubCategoryEN { get; set; }
        public CodeNameEntity SubType { get; set; }
        public string EAN { get; set; }
        public string PartNumber { get; set; }
        public decimal Value { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
