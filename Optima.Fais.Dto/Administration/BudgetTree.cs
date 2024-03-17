using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class BudgetTree
    {
		
        public ICollection<BudgetMonth> BudgetMonths { get; set; }

        public CodeNameEntity Data { get; set; }

        public ICollection<CodeNameEntity> Children { get; set; }
    }
}
