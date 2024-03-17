using Optima.Fais.Model;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
	public class AssetTypeDashboard
	{
		public AssetTypeGroupBase[] Values { get; set; }
		public AssetTypeGroupBase[] ValueDeps { get; set; }
	}
}
