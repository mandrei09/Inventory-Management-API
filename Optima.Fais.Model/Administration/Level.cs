using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
	public partial class Level : Entity
	{
		public Level()
		{
			
		}

		public string Code { get; set; }

		public string Name { get; set; }

		public string NextLevelCode { get; set; }
	}
}
