using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetBaseFreezeUI
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

		public CodeNameEntity Project { get; set; }
	}
}
