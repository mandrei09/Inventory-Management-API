using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class DictionaryItem : Entity
    {
        public DictionaryItem()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? DictionaryTypeId { get; set; }

        public DictionaryType DictionaryType { get; set; }

        public int? AssetCategoryId { get; set; }

        public virtual AssetCategory AssetCategory { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

        public string Name2 { get; set; }

		public string Name3 { get; set; }

		public int? ERPId { get; set; }

	}
}
