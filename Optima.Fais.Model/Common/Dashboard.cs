using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class Dashboard : Entity
    {
		public string Name { get; set; }
        public string Code { get; set; }

        public InventoryAsset InventoryAsset { get; set; }
    }
}
