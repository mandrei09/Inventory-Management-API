using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Dto
{
    public class InventoryAssetWHResource
    {
		public InventoryAssetWHResource()
		{
		}
		public int Id { get; set; }

		public ApplicationUser InventoryTeamManager { get; set; }
		public ApplicationUser InventoryResponsable { get; set; }
		public string UserName { get; set; }
	}
}