using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class DictionaryType : Entity
    {
        public DictionaryType()
        {
            DictionaryItems = new HashSet<DictionaryItem>();
        }


        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<DictionaryItem> DictionaryItems { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? ERPId { get; set; }
	}
}
