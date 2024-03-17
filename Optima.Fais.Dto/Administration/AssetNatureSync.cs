using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AssetNatureSync
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime ModifiedAt { get; set; }
	}
}
