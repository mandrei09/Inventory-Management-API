﻿using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class Category : Entity
	{
		public Category()
		{

		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? CategoryENId { get; set; }

		public virtual CategoryEN CategoryEN { get; set; }

	}
}
