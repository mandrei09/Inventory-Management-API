using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class DictionaryItemSync
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public int? DictionaryTypeId { get; set; }
		public int? AssetCategoryId { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime ModifiedAt { get; set; }
	}
}
