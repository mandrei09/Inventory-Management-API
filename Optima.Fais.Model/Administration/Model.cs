using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class Model : Entity
	{
		public Model()
		{
			
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int AssetCount { get; set; }

		public int? BrandId { get; set; }

		public virtual Brand Brand { get; set; }

        public int SNLength { get; set; }

        public int IMEILength { get; set; }
    }
}
