using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class AssetStatusWHDetail
    {
		public Asset Asset { get; set; }
		public ApplicationUser InventoryTeamManager { get; set; }
		public ApplicationUser InventoryResponsable { get; set; }
		public List<Asset> Assets { get; set; }
		public string TeamManagerUserName { get; set; }
	}
}
