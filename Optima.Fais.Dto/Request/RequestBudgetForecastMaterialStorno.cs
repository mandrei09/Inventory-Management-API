using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Common
{
    public class RequestBudgetForecastMaterialStorno
    {
        public int Id { get; set; }
        public bool Storno { get; set; }
		public decimal StornoValue { get; set; }
	}
}
