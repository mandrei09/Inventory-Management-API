﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class OrderValidate
    {
        public int BudgetId { get; set; }
		public Guid Guid { get; set; }
		public bool Accepted { get; set; }
        public string Reason { get; set; }
    }
}
